import type { TaskStatus } from '../services/api'

const STATUS_LABELS: Record<TaskStatus, string> = {
  Unknown: 'Indefinido',
  Pending: 'Pendente',
  InProgress: 'Em Progresso',
  Finished: 'Concluída',
  Paused: 'Pausada',
  Cancelled: 'Cancelada',
}

export const STATUS_OPTIONS = Object.entries(STATUS_LABELS).map(
  ([value, label]) => ({
    value: value as TaskStatus,
    label,
  }),
)

export function getStatusLabel(status: TaskStatus) {
  return STATUS_LABELS[status] ?? 'Desconhecido'
}

export function getStatusClass(status: TaskStatus) {
  return `status-${status
    .replace(/([a-z])([A-Z])/g, '$1-$2')
    .toLowerCase()}`
}
