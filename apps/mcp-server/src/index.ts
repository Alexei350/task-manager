import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import {
  CallToolRequestSchema,
  ListToolsRequestSchema,
} from "@modelcontextprotocol/sdk/types.js";
import axios from "axios";

const server = new Server(
  {
    name: "task-manager-mcp",
    version: "1.0.0",
  },
  {
    capabilities: {
      tools: {},
    },
  }
);

const API_URL = process.env.API_URL || "http://localhost:8000";
const API_KEY = process.env.API_KEY || "";

if (!API_KEY) {
  console.error("ERROR: API_KEY environment variable is required!");
  console.error("Generate an API Key at your Task Manager settings and set it in your Claude config.");
  process.exit(1);
}

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'X-Api-Key': API_KEY,
  },
});

server.setRequestHandler(ListToolsRequestSchema, async () => {
  return {
    tools: [
      {
        name: "list_tasks",
        description: "List tasks with pagination. Returns your personal tasks only.",
        inputSchema: {
          type: "object",
          properties: {
            page: { type: "number", default: 1, description: "Page number (starts at 1)" },
            pageSize: { type: "number", default: 10, description: "Number of tasks per page" },
          },
        },
      },
      {
        name: "get_task",
        description: "Get a specific task by its ID",
        inputSchema: {
          type: "object",
          properties: {
            id: { type: "string", description: "Task ID (GUID format)" },
          },
          required: ["id"],
        },
      },
      {
        name: "create_task",
        description: "Create a new task in your personal task list",
        inputSchema: {
          type: "object",
          properties: {
            status: { 
              type: "number", 
              description: "Task status: 0=Unknown, 1=Pending, 2=InProgress, 3=Finished, 4=Paused, 5=Cancelled",
              default: 1
            },
            description: { type: "string", description: "Task description (required)" },
            observation: { type: "string", description: "Additional observations about the task" },
            timeSpent: { type: "string", description: "Time spent on task in ISO duration format (e.g., PT1H30M for 1 hour 30 minutes)" },
            dueDate: { type: "string", description: "Due date in ISO format (e.g., 2026-01-15T10:00:00Z)" },
          },
          required: ["description"],
        },
      },
      {
        name: "update_task",
        description: "Update an existing task",
        inputSchema: {
          type: "object",
          properties: {
            id: { type: "string", description: "Task ID (GUID format) (required)" },
            status: { 
              type: "number", 
              description: "Task status: 0=Unknown, 1=Pending, 2=InProgress, 3=Finished, 4=Paused, 5=Cancelled (required)",
            },
            description: { type: "string", description: "Task description (required)" },
            observation: { type: "string", description: "Additional observations about the task" },
            timeSpent: { type: "string", description: "Time spent on task in ISO duration format (e.g., PT1H30M)" },
            dueDate: { type: "string", description: "Due date in ISO format (e.g., 2026-01-15T10:00:00Z)" },
          },
          required: ["id", "status", "description"],
        },
      },
      {
        name: "delete_task",
        description: "Delete a task by its ID",
        inputSchema: {
          type: "object",
          properties: {
            id: { type: "string", description: "Task ID (GUID format)" },
          },
          required: ["id"],
        },
      },
    ],
  };
});

// Helper function to format backend response
function formatResponse(response: any): string {
  const data = response.data;
  
  // Build formatted response
  let result = "";
  
  // Add success/failure indicator
  if (data.success !== undefined) {
    result += data.success ? "✓ Success\n\n" : "✗ Failed\n\n";
  }
  
  // Add messages if present
  if (data.messages && Array.isArray(data.messages) && data.messages.length > 0) {
    result += "Messages:\n";
    data.messages.forEach((msg: any) => {
      const typeLabels: Record<number, string> = {
        0: "ℹ Info",
        1: "✓ Success", 
        2: "⚠ Warning",
        3: "✗ Error"
      };
      const typeLabel = typeLabels[msg.type as number] || "Message";
      result += `  ${typeLabel}: ${msg.message}\n`;
    });
    result += "\n";
  }
  
  // Add data if present
  if (data.data) {
    result += "Data:\n";
    if (Array.isArray(data.data)) {
      result += `  Total items: ${data.data.length}\n`;
      data.data.forEach((item: any, index: number) => {
        result += `  [${index + 1}] ${formatTaskItem(item)}\n`;
      });
    } else {
      result += formatTaskItem(data.data);
    }
    result += "\n";
  }
  
  // Add pagination info if present
  if (data.page !== undefined) {
    result += `Pagination:\n`;
    result += `  Page: ${data.page} of ${data.totalPages}\n`;
    result += `  Total items: ${data.totalItems}\n`;
    result += `  Page size: ${data.pageSize}\n`;
  }
  
  return result || JSON.stringify(data, null, 2);
}

function formatTaskItem(task: any): string {
  const statusLabels: Record<number, string> = {
    0: "Unknown",
    1: "Pending",
    2: "InProgress",
    3: "Finished",
    4: "Paused",
    5: "Cancelled"
  };
  
  let result = `ID: ${task.id}\n`;
  result += `    Description: ${task.description}\n`;
  result += `    Status: ${statusLabels[task.status as number] || task.status}\n`;
  if (task.observation) result += `    Observation: ${task.observation}\n`;
  if (task.creationDate) result += `    Created: ${new Date(task.creationDate).toLocaleString()}\n`;
  if (task.dueDate) result += `    Due Date: ${new Date(task.dueDate).toLocaleString()}\n`;
  if (task.completedDate) result += `    Completed: ${new Date(task.completedDate).toLocaleString()}\n`;
  if (task.timeSpent) result += `    Time Spent: ${task.timeSpent}\n`;
  
  return result;
}

server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;

  try {
    if (name === "list_tasks") {
      const { page = 1, pageSize = 10 } = args as any;
      const response = await api.get("/Task", {
        params: { page, pageSize },
      });
      return {
        content: [
          {
            type: "text",
            text: formatResponse(response),
          },
        ],
      };
    }

    if (name === "get_task") {
      const { id } = args as any;
      const response = await api.get(`/Task/${id}`);
      return {
        content: [
          {
            type: "text",
            text: formatResponse(response),
          },
        ],
      };
    }

    if (name === "create_task") {
      const { status = 1, description, observation, timeSpent, dueDate } = args as any;
      const response = await api.post("/Task", {
        status,
        description,
        observation,
        timeSpent,
        dueDate,
      });
      return {
        content: [
          {
            type: "text",
            text: formatResponse(response),
          },
        ],
      };
    }

    if (name === "update_task") {
      const { id, status, description, observation, timeSpent, dueDate } = args as any;
      const response = await api.put("/Task", {
        id,
        status,
        description,
        observation,
        timeSpent,
        dueDate,
      });
      return {
        content: [
          {
            type: "text",
            text: formatResponse(response),
          },
        ],
      };
    }

    if (name === "delete_task") {
      const { id } = args as any;
      const response = await api.delete(`/Task/${id}`);
      return {
        content: [
          {
            type: "text",
            text: formatResponse(response),
          },
        ],
      };
    }

    throw new Error(`Unknown tool: ${name}`);
  } catch (error: any) {
    const errorMessage = error.response?.data?.message || error.message;
    const errorData = error.response?.data;
    
    let errorText = "✗ Error occurred\n\n";
    
    if (errorData) {
      errorText += formatResponse({ data: errorData });
    } else {
      errorText += `Error: ${errorMessage}`;
    }
    
    return {
      content: [{ type: "text", text: errorText }],
      isError: true,
    };
  }
});

async function main() {
  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error("Task Manager MCP Server running on stdio");
}

main().catch((error) => {
  console.error("Fatal error in main():", error);
  process.exit(1);
});
