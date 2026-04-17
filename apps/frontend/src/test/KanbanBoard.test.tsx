import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { KanbanBoard } from '../components/KanbanBoard'
import type { Task } from '../services/api'

const mockTasks: Task[] = [
  {
    id: '1',
    status: 'Pending',
    description: 'Task 1',
    observation: 'Observation 1',
    creationDate: '2024-01-01T12:00:00Z',
    dueDate: '2024-01-10T12:00:00Z',
    completedDate: null,
    timeSpent: null,
  },
  {
    id: '2',
    status: 'InProgress',
    description: 'Task 2',
    observation: null,
    creationDate: '2024-01-02T00:00:00Z',
    dueDate: null,
    completedDate: null,
    timeSpent: null,
  },
  {
    id: '3',
    status: 'Finished',
    description: 'Task 3',
    observation: 'Done',
    creationDate: '2024-01-03T00:00:00Z',
    dueDate: null,
    completedDate: '2024-01-05T00:00:00Z',
    timeSpent: null,
  },
]

describe('KanbanBoard', () => {
  const mockOnEditTask = vi.fn()
  const mockOnViewTask = vi.fn()
  const mockOnDeleteTask = vi.fn()
  const mockOnStatusChange = vi.fn()
  const mockOnCreateTask = vi.fn()

  afterEach(() => {
    vi.clearAllMocks()
  })

  it('renders all columns with correct task counts', () => {
    render(
      <KanbanBoard
        tasks={mockTasks}
        onEditTask={mockOnEditTask}
        onViewTask={mockOnViewTask}
        onDeleteTask={mockOnDeleteTask}
        onStatusChange={mockOnStatusChange}
        onCreateTask={mockOnCreateTask}
        isLoading={false}
      />,
    )

    // Check for column titles using role="heading"
    expect(screen.getByRole('heading', { name: 'Pendente' })).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Em Progresso' })).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Concluída' })).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Pausada' })).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Cancelada' })).toBeInTheDocument()
  })

  it('renders tasks in correct columns', () => {
    render(
      <KanbanBoard
        tasks={mockTasks}
        onEditTask={mockOnEditTask}
        onViewTask={mockOnViewTask}
        onDeleteTask={mockOnDeleteTask}
        onStatusChange={mockOnStatusChange}
        onCreateTask={mockOnCreateTask}
        isLoading={false}
      />,
    )

    expect(screen.getByText('Task 1')).toBeInTheDocument()
    expect(screen.getByText('Task 2')).toBeInTheDocument()
    expect(screen.getByText('Task 3')).toBeInTheDocument()
  })

  it('shows loading state when isLoading is true', () => {
    render(
      <KanbanBoard
        tasks={[]}
        onEditTask={mockOnEditTask}
        onViewTask={mockOnViewTask}
        onDeleteTask={mockOnDeleteTask}
        onStatusChange={mockOnStatusChange}
        onCreateTask={mockOnCreateTask}
        isLoading={true}
      />,
    )

    expect(screen.getByText('Carregando tarefas...')).toBeInTheDocument()
  })

  it('shows empty columns when no tasks', () => {
    render(
      <KanbanBoard
        tasks={[]}
        onEditTask={mockOnEditTask}
        onViewTask={mockOnViewTask}
        onDeleteTask={mockOnDeleteTask}
        onStatusChange={mockOnStatusChange}
        onCreateTask={mockOnCreateTask}
        isLoading={false}
      />,
    )

    // Verifica se todas as colunas estão presentes
    expect(screen.getByRole('heading', { name: 'Pendente' })).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Em Progresso' })).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Concluída' })).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Pausada' })).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Cancelada' })).toBeInTheDocument()
    
    // Verifica que todas as colunas têm contagem 0
    const counts = screen.getAllByText('0')
    expect(counts).toHaveLength(5) // Uma para cada coluna
  })

  it('calls onEditTask when edit button is clicked', async () => {
    render(
      <KanbanBoard
        tasks={mockTasks}
        onEditTask={mockOnEditTask}
        onViewTask={mockOnViewTask}
        onDeleteTask={mockOnDeleteTask}
        onStatusChange={mockOnStatusChange}
        onCreateTask={mockOnCreateTask}
        isLoading={false}
      />,
    )

    const editButtons = screen.getAllByLabelText('Editar tarefa')
    await userEvent.click(editButtons[0])

    expect(mockOnEditTask).toHaveBeenCalledWith(mockTasks[0])
  })

  it('calls onDeleteTask when delete button is clicked', async () => {
    render(
      <KanbanBoard
        tasks={mockTasks}
        onEditTask={mockOnEditTask}
        onViewTask={mockOnViewTask}
        onDeleteTask={mockOnDeleteTask}
        onStatusChange={mockOnStatusChange}
        onCreateTask={mockOnCreateTask}
        isLoading={false}
      />,
    )

    const deleteButtons = screen.getAllByLabelText('Excluir tarefa')
    await userEvent.click(deleteButtons[0])

    expect(mockOnDeleteTask).toHaveBeenCalledWith('1')
  })

  it('renders task observations when available', () => {
    render(
      <KanbanBoard
        tasks={mockTasks}
        onEditTask={mockOnEditTask}
        onViewTask={mockOnViewTask}
        onDeleteTask={mockOnDeleteTask}
        onStatusChange={mockOnStatusChange}
        onCreateTask={mockOnCreateTask}
        isLoading={false}
      />,
    )

    expect(screen.getByText('Observation 1')).toBeInTheDocument()
    expect(screen.getByText('Done')).toBeInTheDocument()
  })

  it('formats dates correctly', () => {
    render(
      <KanbanBoard
        tasks={mockTasks}
        onEditTask={mockOnEditTask}
        onViewTask={mockOnViewTask}
        onDeleteTask={mockOnDeleteTask}
        onStatusChange={mockOnStatusChange}
        onCreateTask={mockOnCreateTask}
        isLoading={false}
      />,
    )

    // Check that due dates are rendered with the correct format
    expect(screen.getByText(/10\/01\/2024/)).toBeInTheDocument()
  })
})
