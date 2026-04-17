import { API_BASE_URL } from '../services/api'

export function ApiTargetPill() {
  return <span className="pill">API: {API_BASE_URL}</span>
}
