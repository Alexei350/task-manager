import React, { useState, useEffect } from 'react'
import type { ApiKey } from '../../services/apiKeyService'
import {
  listApiKeys,
  createApiKey,
  revokeApiKey,
} from '../../services/apiKeyService'
import { useAuth } from '../../context/AuthContext'

interface KeyModalProps {
  apiKey: string
  onClose: () => void
}

const KeyDisplayModal: React.FC<KeyModalProps> = ({ apiKey, onClose }) => {
  const [copied, setCopied] = useState(false)

  const handleCopy = () => {
    navigator.clipboard.writeText(apiKey)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }

  return (
    <div
      style={{
        position: 'fixed',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        backgroundColor: 'rgba(0, 0, 0, 0.7)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        zIndex: 9999,
      }}
      onClick={onClose}
    >
      <div
        style={{
          backgroundColor: 'var(--background)',
          color: 'var(--text)',
          borderRadius: '12px',
          padding: '32px',
          maxWidth: '600px',
          width: '90%',
          boxShadow: '0 20px 60px rgba(0, 0, 0, 0.3)',
        }}
        onClick={(e) => e.stopPropagation()}
      >
        <h2 style={{ margin: '0 0 16px 0', fontSize: '1.5rem', color: 'var(--text)' }}>
          ✅ Chave Gerada com Sucesso!
        </h2>
        <p style={{ margin: '0 0 24px 0', fontSize: '0.95rem', color: 'var(--text-secondary)', lineHeight: '1.6' }}>
          <strong style={{ color: '#ef4444' }}>⚠️ Importante:</strong> Esta é a única vez que você verá
          esta chave. Copie-a agora e armazene em um local seguro.
        </p>

        <div
          style={{
            marginBottom: '24px',
            padding: '16px',
            backgroundColor: 'var(--code-bg)',
            border: '2px solid var(--border)',
            borderRadius: '8px',
          }}
        >
          <code
            style={{
              display: 'block',
              fontFamily: 'monospace',
              fontSize: '0.9rem',
              wordBreak: 'break-all',
              color: 'var(--code-text)',
              lineHeight: '1.5',
            }}
          >
            {apiKey}
          </code>
        </div>

        <div style={{ display: 'flex', gap: '12px', justifyContent: 'flex-end' }}>
          <button
            onClick={handleCopy}
            className="primary-btn"
            style={{ minWidth: '120px' }}
          >
            {copied ? '✓ Copiado!' : 'Copiar Chave'}
          </button>
          <button onClick={onClose} className="secondary-btn">
            Fechar
          </button>
        </div>
      </div>
    </div>
  )
}

export const ApiKeyManager: React.FC = () => {
  const { session } = useAuth()
  const [keys, setKeys] = useState<ApiKey[]>([])
  const [loading, setLoading] = useState(false)
  const [newKeyName, setNewKeyName] = useState('')
  const [createdKey, setCreatedKey] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  const loadKeys = async () => {
    if (!session?.token) return
    setLoading(true)
    try {
      const response = await listApiKeys(session.token)
      if (response.success && response.data) {
        setKeys(response.data)
      }
    } catch {
      setError('Falha ao carregar chaves de acesso')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadKeys()
  }, [])

  const handleCreate = async () => {
    if (!newKeyName.trim()) {
      setError('Por favor, informe um nome para a chave')
      return
    }
    if (!session?.token) return
    setError(null)
    setLoading(true)
    try {
      const response = await createApiKey(session.token, { name: newKeyName })
      if (response.success && response.data) {
        setCreatedKey(response.data)
        setNewKeyName('')
        loadKeys()
      } else {
        setError('Falha ao criar chave de acesso')
      }
    } catch {
      setError('Falha ao criar chave de acesso')
    } finally {
      setLoading(false)
    }
  }

  const handleRevoke = async (id: string) => {
    if (!confirm('Tem certeza que deseja revogar esta chave? Esta ação não pode ser desfeita.')) return
    if (!session?.token) return
    setLoading(true)
    try {
      await revokeApiKey(session.token, id)
      loadKeys()
    } catch {
      setError('Falha ao revogar chave de acesso')
    } finally {
      setLoading(false)
    }
  }

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault()
      handleCreate()
    }
  }

  return (
    <div className="api-key-manager">
      {createdKey && <KeyDisplayModal apiKey={createdKey} onClose={() => setCreatedKey(null)} />}

      {error && (
        <div className="feedback error" style={{ marginBottom: '16px' }}>
          {error}
        </div>
      )}

      <div className="field" style={{ marginBottom: '24px' }}>
        <label htmlFor="keyName" style={{ color: 'var(--text)' }}>Nome da Chave</label>
        <div style={{ display: 'flex', gap: '8px' }}>
          <input
            id="keyName"
            type="text"
            value={newKeyName}
            onChange={(e) => setNewKeyName(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Ex: CI/CD, Integração Mobile, API Externa"
            disabled={loading}
            style={{ flex: 1 }}
          />
          <button
            onClick={handleCreate}
            disabled={loading || !newKeyName.trim()}
            className="primary-btn"
          >
            {loading ? 'Gerando...' : 'Gerar Chave'}
          </button>
        </div>
      </div>

      {createdKey && (
        <div
          style={{
            marginBottom: '24px',
            padding: '16px',
            background: '#f0fdf4',
            border: '1px solid #86efac',
            borderRadius: '12px',
          }}
        >
          <h4 style={{ margin: '0 0 8px 0', color: '#166534', fontSize: '1rem', fontWeight: 600 }}>
            ✅ Nova Chave Gerada com Sucesso
          </h4>
          <p style={{ margin: '0 0 12px 0', fontSize: '0.9rem', color: '#15803d' }}>
            <strong>Importante:</strong> Copie esta chave agora. Ela não será exibida novamente!
          </p>
          <div style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
            <code
              style={{
                flex: 1,
                padding: '12px',
                background: '#fff',
                border: '1px solid #d1d5db',
                borderRadius: '8px',
                fontFamily: 'monospace',
                fontSize: '0.9rem',
                wordBreak: 'break-all',
              }}
            >
              {createdKey}
            </code>
            <button
              onClick={() => {
                navigator.clipboard.writeText(createdKey)
                alert('Chave copiada para área de transferência!')
              }}
              className="primary-btn"
              style={{ whiteSpace: 'nowrap' }}
            >
              Copiar
            </button>
          </div>
          <button
            onClick={() => setCreatedKey(null)}
            style={{
              marginTop: '12px',
              padding: '8px 12px',
              background: 'transparent',
              border: 'none',
              color: '#64748b',
              cursor: 'pointer',
              fontSize: '0.9rem',
            }}
          >
            Fechar
          </button>
        </div>
      )}

      <div style={{ overflowX: 'auto' }}>
        <table className="data-table" style={{ width: '100%', minWidth: '600px' }}>
          <thead>
            <tr>
              <th>Nome</th>
              <th>Prefixo</th>
              <th>Expira em</th>
              <th>Último uso</th>
              <th>Ação</th>
            </tr>
          </thead>
          <tbody>
            {keys.map((key) => (
              <tr key={key.id}>
                <td>{key.name}</td>
                <td>
                  <code style={{ fontSize: '0.85rem', color: '#64748b' }}>{key.prefix}</code>
                </td>
                <td>
                  {key.expiresAt
                    ? new Date(key.expiresAt).toLocaleDateString('pt-BR')
                    : 'Nunca'}
                </td>
                <td>
                  {key.lastUsedAt
                    ? new Date(key.lastUsedAt).toLocaleDateString('pt-BR')
                    : 'Nunca'}
                </td>
                <td>
                  <button
                    onClick={() => handleRevoke(key.id)}
                    className="danger-btn"
                    disabled={loading}
                  >
                    Revogar
                  </button>
                </td>
              </tr>
            ))}
            {keys.length === 0 && (
              <tr>
                <td colSpan={5} style={{ textAlign: 'center', padding: '24px', color: '#94a3b8' }}>
                  Nenhuma chave de acesso encontrada
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}
