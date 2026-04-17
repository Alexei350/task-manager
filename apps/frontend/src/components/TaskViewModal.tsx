import ReactMarkdown from 'react-markdown'
import type { Task } from '../services/api'
import { Modal } from './Modal'
import { StatusBadge } from './StatusBadge'

interface TaskViewModalProps {
  task: Task | null
  isOpen: boolean
  onClose: () => void
  onEdit: (task: Task) => void
}

function formatDate(value?: string | null) {
  if (!value) return null

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return null

  return date.toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: 'long',
    year: 'numeric',
  })
}

function formatDateTime(value?: string | null) {
  if (!value) return null

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return null

  return date.toLocaleString('pt-BR', {
    day: '2-digit',
    month: 'long',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

export function TaskViewModal({
  task,
  isOpen,
  onClose,
  onEdit,
}: TaskViewModalProps) {
  if (!task) return null

  const handleEdit = () => {
    onEdit(task)
    onClose()
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Detalhes da Tarefa">
      <div className="task-view">
        <div className="task-view-header">
          <div className="task-view-status">
            <StatusBadge status={task.status} />
          </div>
        </div>

        <div className="task-view-section">
          <h3 className="task-view-label">Descrição</h3>
          <div className="task-view-content task-view-description">
            <ReactMarkdown>{task.description}</ReactMarkdown>
          </div>
        </div>

        {task.observation ? (
          <div className="task-view-section">
            <h3 className="task-view-label">Observações</h3>
            <div className="task-view-content task-view-observation">
              <ReactMarkdown>{task.observation}</ReactMarkdown>
            </div>
          </div>
        ) : null}

        <div className="task-view-info-grid">
          {task.dueDate ? (
            <div className="task-view-info-item">
              <span className="task-view-info-label">Prazo</span>
              <span className="task-view-info-value">
                {formatDate(task.dueDate)}
              </span>
            </div>
          ) : null}

          <div className="task-view-info-item">
            <span className="task-view-info-label">Criada em</span>
            <span className="task-view-info-value">
              {formatDateTime(task.creationDate)}
            </span>
          </div>

          {task.completedDate ? (
            <div className="task-view-info-item">
              <span className="task-view-info-label">Concluída em</span>
              <span className="task-view-info-value">
                {formatDateTime(task.completedDate)}
              </span>
            </div>
          ) : null}
        </div>

        <div className="task-view-actions">
          <button type="button" className="ghost" onClick={onClose}>
            Fechar
          </button>
          <button type="button" className="primary-btn" onClick={handleEdit}>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
              <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
            </svg>
            Editar Tarefa
          </button>
        </div>
      </div>
    </Modal>
  )
}
