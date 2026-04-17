import { useState, type FormEvent, useEffect, useRef } from 'react'
import type { Task, TaskPayload, TaskStatus } from '../services/api'
import { STATUS_OPTIONS } from '../utils/status'
import { MarkdownEditor } from './MarkdownEditor'

interface TaskFormProps {
  onSubmit: (payload: TaskPayload) => Promise<void>
  onCancel: () => void
  initialTask?: Task
  variant?: 'create' | 'edit'
}

function buildInitialState(task?: Task) {
  return {
    description: task?.description ?? '',
    observation: task?.observation ?? '',
    status: task?.status ?? ('Pending' as TaskStatus),
    dueDate: task?.dueDate ? task.dueDate.slice(0, 10) : '',
  }
}

export function TaskForm({
  onSubmit,
  initialTask,
  variant = 'create',
  onCancel,
}: TaskFormProps) {
  const [form, setForm] = useState(buildInitialState(initialTask))
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const formRef = useRef<HTMLFormElement>(null)
  const titleInputRef = useRef<HTMLInputElement>(null)

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setError(null)
    setSubmitting(true)

    try {
      const payload: TaskPayload = {
        description: form.description.trim(),
        observation: form.observation.trim() || null,
        status: form.status,
        dueDate: form.dueDate ? new Date(form.dueDate).toISOString() : null,
        timeSpent: null,
      }

      await onSubmit(payload)

      if (variant === 'create') {
        setForm(buildInitialState())
      }
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : 'Não foi possível salvar a tarefa.',
      )
    } finally {
      setSubmitting(false)
    }
  }

  // Adiciona suporte para Ctrl+Enter / Cmd+Enter para submeter o formulário
  useEffect(() => {
    const form = formRef.current
    
    const handleKeyDown = (event: KeyboardEvent) => {
      if ((event.ctrlKey || event.metaKey) && event.key === 'Enter') {
        event.preventDefault()
        if (form && form.isConnected) {
          form.requestSubmit()
        }
      }
    }

    document.addEventListener('keydown', handleKeyDown)
    return () => document.removeEventListener('keydown', handleKeyDown)
  }, [])

  // Foca automaticamente no campo título quando o modal abre
  useEffect(() => {
    if (variant === 'create') {
      titleInputRef.current?.focus()
    }
  }, [variant])

  return (
    <form ref={formRef} className="task-form" onSubmit={handleSubmit}>
      <div className="field-grid">
        <label className="field">
          <span>Título</span>
          <input
            ref={titleInputRef}
            required
            name="description"
            value={form.description}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, description: event.target.value }))
            }
            onKeyDown={(event) => {
              if (event.key === 'Enter' && !event.shiftKey && !event.ctrlKey && !event.metaKey) {
                event.preventDefault()
                formRef.current?.requestSubmit()
              }
            }}
            placeholder="Ex: Implementar login"
          />
        </label>

        <label className="field">
          <span>Status</span>
          <select
            name="status"
            value={form.status}
            onChange={(event) =>
              setForm((prev) => ({
                ...prev,
                status: event.target.value as TaskStatus,
              }))
            }
          >
            {STATUS_OPTIONS.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </label>

        <label className="field">
          <span>Prazo</span>
          <input
            type="date"
            name="dueDate"
            value={form.dueDate}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, dueDate: event.target.value }))
            }
          />
        </label>
      </div>

      <label className="field">
        <span>Observações</span>
        <MarkdownEditor
          value={form.observation}
          onChange={(value) =>
            setForm((prev) => ({ ...prev, observation: value }))
          }
          placeholder="Detalhes importantes, links ou critérios de aceite (suporta Markdown)"
        />
      </label>

      {error ? <p className="feedback error">{error}</p> : null}

      <div className="actions">
        <button
          type="button"
          className="ghost"
          onClick={onCancel}
          disabled={submitting}
        >
          Cancelar
        </button>
        <button type="submit" disabled={submitting} title="Ou pressione Ctrl+Enter">
          {submitting
            ? 'Salvando...'
            : variant === 'create'
              ? 'Criar tarefa'
              : 'Salvar alterações'}
        </button>
      </div>
    </form>
  )
}
