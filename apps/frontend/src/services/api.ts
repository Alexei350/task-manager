export type ReturnMessageType = 'Info' | 'Success' | 'Warning' | 'Error'

export interface ReturnMessage {
  type: ReturnMessageType
  message: string
}

// Backend may return message payloads with either `message` or `Message`
// keys depending on the controller. Keep both until the API is normalized.
type ApiMessagePayload = ReturnMessage & { Message?: string }

export interface ApiResponse<T> {
  success: boolean
  messages?: ReturnMessage[]
  data?: T
  totalRecords?: number
  totalPages?: number
}

export type TaskStatus =
  | 'Unknown'
  | 'Pending'
  | 'InProgress'
  | 'Finished'
  | 'Paused'
  | 'Cancelled'

export interface Task {
  id: string
  status: TaskStatus
  description: string
  observation?: string | null
  creationDate: string
  timeSpent?: string | null
  dueDate?: string | null
  completedDate?: string | null
}

export interface TaskPayload {
  status: TaskStatus
  description: string
  observation?: string | null
  timeSpent?: string | null
  dueDate?: string | null
}

export interface User {
  id: string
  name: string
  email: string
}

export interface TokenReturn {
  token: string
  refreshToken: string
  user: User
}

export interface RegisterUserPayload {
  name: string
  email: string
  password: string
}

const REGISTER_SUCCESS_FALLBACK = 'Cadastrado com sucesso'

let getSession: () => TokenReturn | null = () => null
let setSession: (session: TokenReturn | null) => void = () => {}
let isRefreshing = false
let failedQueue: Array<{
  resolve: (value: unknown) => void
  reject: (reason?: unknown) => void
}> = []

const processQueue = (error: Error | null, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error)
    } else {
      prom.resolve(token)
    }
  })

  failedQueue = []
}

export function setupAuth(
  getFn: () => TokenReturn | null,
  setFn: (session: TokenReturn | null) => void,
) {
  getSession = getFn
  setSession = setFn
}

export const API_BASE_URL = (
  import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:30000'
).replace(/\/$/, '')

function buildUrl(path: string) {
  return `${API_BASE_URL}${path.startsWith('/') ? path : `/${path}`}`
}

async function refreshTokenCall(token: string, refreshToken: string) {
  const response = await fetch(buildUrl('/Authentication/RefreshToken'), {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Accept: 'application/json',
    },
    body: JSON.stringify({ token, refreshToken }),
  })

  const payload = (await response.json().catch(() => null)) as ApiResponse<TokenReturn> | null

  if (!response.ok || !payload?.success || !payload.data) {
    throw new Error('Falha ao atualizar token')
  }

  return payload.data
}

function getPrimaryMessage(messages?: ApiMessagePayload[]) {
  if (!messages?.length) return undefined

  const [first] = messages
  if (typeof first.message === 'string') return first.message
  if (typeof first.Message === 'string') return first.Message

  return undefined
}

function normalizeMessages(
  messages?: Array<ApiMessagePayload | ReturnMessage>,
): ApiMessagePayload[] | undefined {
  if (!Array.isArray(messages)) return undefined

  const normalized = messages.filter(
    (entry): entry is ApiMessagePayload =>
      Boolean(
        entry &&
          typeof entry === 'object' &&
          ('message' in entry || 'Message' in entry),
      ),
  )

  return normalized.length ? normalized : undefined
}

function extractMessage(
  payload: Partial<ApiResponse<unknown>>,
  fallback = 'Não foi possível completar a operação.',
) {
  const message = getPrimaryMessage(normalizeMessages(payload.messages))

  return message ?? fallback
}

export async function request<T>(
  path: string,
  options: RequestInit = {},
  token?: string,
): Promise<ApiResponse<T>> {
  const url = buildUrl(path)
  const headers = {
    'Content-Type': 'application/json',
    Accept: 'application/json',
    ...(options.headers || {}),
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  }

  const response = await fetch(url, {
    ...options,
    headers,
  })

  if (response.status === 401 && token) {
    const session = getSession()

    if (session && session.refreshToken) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({
            resolve: (newToken) => {
              request<T>(path, options, newToken as string)
                .then(resolve)
                .catch(reject)
            },
            reject,
          })
        })
      }

      isRefreshing = true

      try {
        const newSession = await refreshTokenCall(
          session.token,
          session.refreshToken,
        )
        setSession(newSession)
        processQueue(null, newSession.token)

        return request<T>(path, options, newSession.token)
      } catch (err) {
        processQueue(err instanceof Error ? err : new Error('Falha no refresh'))
        setSession(null)
        throw err
      } finally {
        isRefreshing = false
      }
    }
  }

  const payload = (await response.json().catch(() => null)) as
    | ApiResponse<T>
    | null

  if (!payload) {
    throw new Error('Resposta da API inválida.')
  }

  if (!response.ok || payload.success === false) {
    throw new Error(extractMessage(payload))
  }

  return payload
}

export async function login(email: string, password: string) {
  const payload = await request<TokenReturn>(
    '/Authentication/GenerateToken',
    {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    },
  )

  if (!payload.data) {
    throw new Error('Credenciais inválidas.')
  }

  return payload.data
}

export async function googleLogin(token: string) {
  const payload = await request<TokenReturn>(
    '/Authentication/GoogleLogin',
    {
      method: 'POST',
      body: JSON.stringify({ token }),
    },
  )

  if (!payload.data) {
    throw new Error('Falha no login com Google.')
  }

  return payload.data
}

export async function registerUser(payload: RegisterUserPayload) {
  const response = await request<void>('/User', {
    method: 'POST',
    body: JSON.stringify(payload),
  })

  return extractMessage(response, REGISTER_SUCCESS_FALLBACK)
}

export async function loadProfile(token: string) {
  return request<User>('/Me', { method: 'GET' }, token)
}

export interface ListTasksParams {
  page: number
  pageSize: number
  filter?: string
}

export async function listTasks(token: string, params: ListTasksParams) {
  const searchParams = new URLSearchParams({
    Page: String(params.page),
    PageSize: String(params.pageSize),
  })

  if (params.filter) {
    searchParams.append('Filter', params.filter)
  }

  return request<Task[]>(
    `/Task?${searchParams.toString()}`,
    { method: 'GET' },
    token,
  )
}

export async function createTask(token: string, payload: TaskPayload) {
  return request<Task>(
    '/Task',
    {
      method: 'POST',
      body: JSON.stringify(payload),
    },
    token,
  )
}

export async function updateTask(
  token: string,
  id: string,
  payload: TaskPayload,
) {
  return request<Task>(
    '/Task',
    {
      method: 'PUT',
      body: JSON.stringify({ ...payload, id }),
    },
    token,
  )
}

export async function deleteTask(token: string, id: string) {
  return request<void>(`/Task/${id}`, { method: 'DELETE' }, token)
}
