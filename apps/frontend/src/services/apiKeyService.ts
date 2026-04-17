import { request } from './api'

export interface ApiKey {
  id: string
  name: string
  prefix: string
  expiresAt?: string
  lastUsedAt?: string
  deleted: boolean
  deletedAt?: string
}

export interface CreateApiKeyModel {
  name: string
  expiresAt?: string
}

export async function listApiKeys(token: string) {
  return request<ApiKey[]>('/ApiKey', { method: 'GET' }, token)
}

export async function createApiKey(
  token: string,
  payload: CreateApiKeyModel,
) {
  return request<string>(
    '/ApiKey',
    {
      method: 'POST',
      body: JSON.stringify(payload),
    },
    token,
  )
}

export async function revokeApiKey(token: string, id: string) {
  return request<void>(`/ApiKey/${id}`, { method: 'DELETE' }, token)
}
