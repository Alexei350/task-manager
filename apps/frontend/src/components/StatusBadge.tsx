import type { TaskStatus } from '../services/api'
import { getStatusClass, getStatusLabel } from '../utils/status'

interface StatusBadgeProps {
  status: TaskStatus
}

export function StatusBadge({ status }: StatusBadgeProps) {
  return (
    <span className={`status-badge ${getStatusClass(status)}`}>
      {getStatusLabel(status)}
    </span>
  )
}
