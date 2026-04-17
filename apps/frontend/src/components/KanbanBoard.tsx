import { useMemo, useState } from 'react'
import ReactMarkdown from 'react-markdown'
import type { Task, TaskStatus } from '../services/api'

interface KanbanBoardProps {
  tasks: Task[]
  onEditTask: (task: Task) => void
  onViewTask: (task: Task) => void
  onDeleteTask: (taskId: string) => void
  onStatusChange: (taskId: string, newStatus: TaskStatus) => void
  onCreateTask: (status: TaskStatus) => void
  isLoading: boolean
}

interface Column {
  status: TaskStatus
  label: string
  color: string
}

const COLUMNS: Column[] = [
  { status: 'Pending', label: 'Pendente', color: '#fef9c3' },
  { status: 'InProgress', label: 'Em Progresso', color: '#e0f2fe' },
  { status: 'Finished', label: 'Concluída', color: '#dcfce7' },
  { status: 'Paused', label: 'Pausada', color: '#fef3c7' },
  { status: 'Cancelled', label: 'Cancelada', color: '#fee2e2' },
]

function formatDate(value?: string | null) {
  if (!value) return null

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return null

  return date.toLocaleDateString('pt-BR')
}

export function KanbanBoard({
  tasks,
  onEditTask,
  onViewTask,
  onDeleteTask,
  onStatusChange,
  onCreateTask,
  isLoading,
}: KanbanBoardProps) {
  const [draggedTask, setDraggedTask] = useState<Task | null>(null)

  const tasksByStatus = useMemo(() => {
    const grouped = new Map<TaskStatus, Task[]>()

    for (const column of COLUMNS) {
      grouped.set(column.status, [])
    }

    for (const task of tasks) {
      const list = grouped.get(task.status)
      if (list) {
        list.push(task)
      }
    }

    return grouped
  }, [tasks])

  const handleDragStart = (task: Task) => {
    setDraggedTask(task)
  }

  const handleDragOver = (event: React.DragEvent) => {
    event.preventDefault()
  }

  const handleDrop = (event: React.DragEvent, status: TaskStatus) => {
    event.preventDefault()
    if (draggedTask && draggedTask.status !== status) {
      onStatusChange(draggedTask.id, status)
    }
    setDraggedTask(null)
  }

  const handleDragEnd = () => {
    setDraggedTask(null)
  }

  if (isLoading) {
    return <p className="loading-message">Carregando tarefas...</p>
  }

  return (
    <div className="kanban-board">
      {COLUMNS.map((column) => {
        const columnTasks = tasksByStatus.get(column.status) ?? []

        return (
          <div
            key={column.status}
            className="kanban-column"
            onDragOver={handleDragOver}
            onDrop={(e) => handleDrop(e, column.status)}
          >
            <div className="kanban-column-header">
              <div className="kanban-column-title-row">
                <h4 className="kanban-column-title">{column.label}</h4>
                <button
                  type="button"
                  className="kanban-add-btn"
                  onClick={() => onCreateTask(column.status)}
                  aria-label={`Adicionar tarefa ${column.label}`}
                  title={`Adicionar tarefa ${column.label}`}
                >
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <line x1="12" y1="5" x2="12" y2="19" />
                    <line x1="5" y1="12" x2="19" y2="12" />
                  </svg>
                </button>
              </div>
              <span className="kanban-column-count">{columnTasks.length}</span>
            </div>

            <div className="kanban-column-content">
              {columnTasks.map((task) => (
                <div
                  key={task.id}
                  className="kanban-card"
                  draggable
                  onDragStart={() => handleDragStart(task)}
                  onDragEnd={handleDragEnd}
                  onClick={() => onViewTask(task)}
                  role="button"
                  tabIndex={0}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter' || e.key === ' ') {
                      e.preventDefault()
                      onViewTask(task)
                    }
                  }}
                >
                  <div className="kanban-card-header">
                    {task.dueDate ? (() => {
                      const dueDate = new Date(task.dueDate)
                      const today = new Date()
                      today.setHours(0, 0, 0, 0)
                      dueDate.setHours(0, 0, 0, 0)
                      
                      const diffTime = dueDate.getTime() - today.getTime()
                      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
                      
                      let className = 'kanban-card-date'
                      if (diffDays < 0 && task.status !== 'Finished' && task.status !== 'Cancelled') {
                        className += ' date-overdue'
                      } else if (diffDays >= 0 && diffDays <= 3 && task.status !== 'Finished' && task.status !== 'Cancelled') {
                        className += ' date-warning'
                      }

                      return (
                        <span className={className}>
                          📅 {formatDate(task.dueDate)}
                        </span>
                      )
                    })() : null}
                  </div>

                  <h5 className="kanban-card-title">{task.description}</h5>

                  {task.observation ? (
                    <div className="kanban-card-observation">
                      <ReactMarkdown>{task.observation}</ReactMarkdown>
                    </div>
                  ) : null}

                  <div className="kanban-card-footer">
                    <div className="kanban-card-actions">
                      <button
                        type="button"
                        className="kanban-action-btn edit"
                        onClick={(e) => {
                          e.stopPropagation()
                          onEditTask(task)
                        }}
                        aria-label="Editar tarefa"
                        title="Editar tarefa"
                      >
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                          <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                          <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
                        </svg>
                      </button>
                      <button
                        type="button"
                        className="kanban-action-btn delete"
                        onClick={(e) => {
                          e.stopPropagation()
                          onDeleteTask(task.id)
                        }}
                        aria-label="Excluir tarefa"
                        title="Excluir tarefa"
                      >
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                          <path d="M3 6h18" />
                          <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
                          <line x1="10" y1="11" x2="10" y2="17" />
                          <line x1="14" y1="11" x2="14" y2="17" />
                        </svg>
                      </button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )
      })}
    </div>
  )
}
