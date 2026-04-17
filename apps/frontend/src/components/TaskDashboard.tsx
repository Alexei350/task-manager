import { useCallback, useEffect, useMemo, useState } from 'react'
import ReactMarkdown from 'react-markdown'
import { useAuth } from '../context/useAuth'
import {
  createTask,
  deleteTask,
  listTasks,
  updateTask,
  type Task,
  type TaskPayload,
  type TaskStatus,
} from '../services/api'
import { ConfirmDialog } from './ConfirmDialog'
import { KanbanBoard } from './KanbanBoard'
import { Modal } from './Modal'
import { TaskForm } from './TaskForm'
import { TaskViewModal } from './TaskViewModal'
import { ApiKeyManager } from './ApiKey/ApiKeyManager'

const PAGE_SIZE = 100

type TaskDashboardProps = {
  showSettings: boolean
  setShowSettings: (show: boolean) => void
}

export default function TaskDashboard({ showSettings, setShowSettings }: TaskDashboardProps) {
  const { session, logout } = useAuth()
  const [tasks, setTasks] = useState<Task[]>([])
  const [loading, setLoading] = useState(false)
  const [message, setMessage] = useState<string | null>(null)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [editingTask, setEditingTask] = useState<Task | null>(null)
  const [saving, setSaving] = useState(false)
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)
  const [isEditModalOpen, setIsEditModalOpen] = useState(false)
  const [isViewModalOpen, setIsViewModalOpen] = useState(false)
  const [viewingTask, setViewingTask] = useState<Task | null>(null)
  const [viewMode, setViewMode] = useState<'kanban' | 'table'>('kanban')
  const [deleteConfirmation, setDeleteConfirmation] = useState<{
    isOpen: boolean
    taskId: string | null
  }>({ isOpen: false, taskId: null })

  const fetchTasks = useCallback(async () => {
    if (!session) return

    setLoading(true)
    setMessage(null)

    try {
      const response = await listTasks(session.token, {
        page,
        pageSize: PAGE_SIZE,
      })

      setTasks(response.data ?? [])
      setTotalPages(
        response.totalPages ??
          Math.max(
            1,
            Math.ceil((response.totalRecords ?? response.data?.length ?? 1) / PAGE_SIZE),
          ),
      )
    } catch (err) {
      const errorMessage =
        err instanceof Error ? err.message : 'Não foi possível carregar as tarefas.'
      setMessage(errorMessage)

      if (errorMessage.toLowerCase().includes('autoriz')) {
        logout()
      }
    } finally {
      setLoading(false)
    }
  }, [logout, page, session])

  useEffect(() => {
    fetchTasks()
  }, [fetchTasks])

  const handleCreate = useCallback(
    async (payload: TaskPayload) => {
      if (!session) return

      setSaving(true)
      try {
        const response = await createTask(session.token, payload)
        
        // Atualização otimista: adiciona a tarefa localmente ou recarrega a lista
        if (response.data) {
          setTasks((prevTasks) => [response.data!, ...prevTasks])
        } else {
          // Fallback: recarrega a lista se a tarefa não foi retornada
          await fetchTasks()
        }
        
        // Fecha o modal apenas após criar com sucesso
        setIsCreateModalOpen(false)
      } catch (err) {
        const errorMessage =
          err instanceof Error ? err.message : 'Erro ao criar a tarefa.'
        
        setMessage(errorMessage)
        throw err
      } finally {
        setSaving(false)
      }
    },
    [session, fetchTasks],
  )

  const handleUpdate = useCallback(
    async (id: string, payload: TaskPayload) => {
      if (!session) return

      setSaving(true)
      const originalTask = tasks.find((t) => t.id === id)
      
      // Atualização otimista: atualiza localmente antes da resposta
      setTasks((prevTasks) =>
        prevTasks.map((t) =>
          t.id === id
            ? {
                ...t,
                description: payload.description,
                observation: payload.observation,
                status: payload.status,
                dueDate: payload.dueDate,
                timeSpent: payload.timeSpent,
              }
            : t
        )
      )
      
      try {
        await updateTask(session.token, id, payload)
        setEditingTask(null)
        setIsEditModalOpen(false)
      } catch (err) {
        // Reverte em caso de erro
        if (originalTask) {
          setTasks((prevTasks) =>
            prevTasks.map((t) => (t.id === id ? originalTask : t))
          )
        }
        setMessage(
          err instanceof Error ? err.message : 'Erro ao atualizar a tarefa.',
        )
      } finally {
        setSaving(false)
      }
    },
    [session, tasks],
  )

  const handleDelete = useCallback(
    async (taskId: string) => {
      if (!session) return

      setSaving(true)
      const originalTasks = tasks
      
      // Atualização otimista: remove localmente antes da resposta
      setTasks((prevTasks) => prevTasks.filter((t) => t.id !== taskId))
      
      try {
        await deleteTask(session.token, taskId)
        setDeleteConfirmation({ isOpen: false, taskId: null })
      } catch (err) {
        // Reverte em caso de erro
        setTasks(originalTasks)
        setMessage(
          err instanceof Error ? err.message : 'Erro ao remover a tarefa.',
        )
      } finally {
        setSaving(false)
      }
    },
    [session, tasks],
  )

  const openDeleteConfirmation = useCallback((taskId: string) => {
    setDeleteConfirmation({ isOpen: true, taskId })
  }, [])

  const closeDeleteConfirmation = useCallback(() => {
    setDeleteConfirmation({ isOpen: false, taskId: null })
  }, [])

  const confirmDelete = useCallback(() => {
    if (deleteConfirmation.taskId) {
      handleDelete(deleteConfirmation.taskId)
    }
  }, [deleteConfirmation.taskId, handleDelete])

  const handleStatusChange = useCallback(
    async (taskId: string, newStatus: TaskStatus) => {
      if (!session) return

      const task = tasks.find((t) => t.id === taskId)
      if (!task) return

      // Atualização otimista: atualiza o estado local imediatamente
      setTasks((prevTasks) =>
        prevTasks.map((t) =>
          t.id === taskId ? { ...t, status: newStatus } : t
        )
      )

      try {
        await updateTask(session.token, taskId, {
          description: task.description,
          observation: task.observation,
          status: newStatus,
          dueDate: task.dueDate,
          timeSpent: task.timeSpent,
        })
      } catch (err) {
        // Reverte a mudança em caso de erro
        setTasks((prevTasks) =>
          prevTasks.map((t) =>
            t.id === taskId ? { ...t, status: task.status } : t
          )
        )
        setMessage(
          err instanceof Error ? err.message : 'Erro ao atualizar status.',
        )
      }
    },
    [session, tasks],
  )

  const handleEditTask = useCallback((task: Task) => {
    setEditingTask(task)
    setIsEditModalOpen(true)
  }, [])

  const handleViewTask = useCallback((task: Task) => {
    setViewingTask(task)
    setIsViewModalOpen(true)
  }, [])

  const handleViewToEdit = useCallback((task: Task) => {
    setViewingTask(null)
    setIsViewModalOpen(false)
    setEditingTask(task)
    setIsEditModalOpen(true)
  }, [])

  const canGoBack = useMemo(() => page > 1, [page])
  const canGoForward = useMemo(() => page < totalPages, [page, totalPages])

  if (!session) return null

  return (
    <div className="dashboard">
      <section className="panel tasks-panel">
        <div className="panel-header">
          <div>
            <p className="eyebrow">Tarefas</p>
            <h3>Gerenciar Tarefas</h3>
          </div>
          <div className="panel-actions">
            {viewMode === 'table' && (
              <button
                className="primary-btn"
                onClick={() => setIsCreateModalOpen(true)}
                disabled={saving}
              >
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <line x1="12" y1="5" x2="12" y2="19" />
                  <line x1="5" y1="12" x2="19" y2="12" />
                </svg>
                Nova Tarefa
              </button>
            )}
            <div className="view-toggle">
              <button
                className={`view-toggle-btn ${viewMode === 'kanban' ? 'active' : ''}`}
                onClick={() => setViewMode('kanban')}
                aria-label="Visualização Kanban"
              >
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <rect x="3" y="3" width="7" height="7" />
                  <rect x="14" y="3" width="7" height="7" />
                  <rect x="14" y="14" width="7" height="7" />
                  <rect x="3" y="14" width="7" height="7" />
                </svg>
                Kanban
              </button>
              <button
                className={`view-toggle-btn ${viewMode === 'table' ? 'active' : ''}`}
                onClick={() => setViewMode('table')}
                aria-label="Visualização Tabela"
              >
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <line x1="3" y1="6" x2="21" y2="6" />
                  <line x1="3" y1="12" x2="21" y2="12" />
                  <line x1="3" y1="18" x2="21" y2="18" />
                </svg>
                Tabela
              </button>
            </div>
            <button className="icon-btn refresh-btn" onClick={fetchTasks} disabled={loading} title="Atualizar" aria-label="Atualizar">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M21.5 2v6h-6M2.5 22v-6h6M2 11.5a10 10 0 0 1 18.8-4.3M22 12.5a10 10 0 0 1-18.8 4.2" />
              </svg>
            </button>
          </div>
        </div>

        {message ? <p className="feedback">{message}</p> : null}

        {viewMode === 'kanban' ? (
          <KanbanBoard
            tasks={tasks}
            onEditTask={handleEditTask}
            onViewTask={handleViewTask}
            onDeleteTask={openDeleteConfirmation}
            onStatusChange={handleStatusChange}
            onCreateTask={(status) => {
              setEditingTask({ status } as Task)
              setIsCreateModalOpen(true)
            }}
            isLoading={loading}
          />
        ) : (
          <>
            {loading ? (
              <p>Carregando tarefas...</p>
            ) : tasks.length === 0 ? (
              <p className="muted">
                Nenhuma tarefa encontrada para o filtro informado.
              </p>
            ) : (
              <div className="table-wrapper">
                <table>
                  <thead>
                    <tr>
                      <th>Título</th>
                      <th>Status</th>
                      <th>Prazo</th>
                      <th aria-label="Ações" />
                    </tr>
                  </thead>
                  <tbody>
                    {tasks.map((task) => (
                      <tr 
                        key={task.id}
                        onClick={() => handleViewTask(task)}
                        className="clickable-row"
                        role="button"
                        tabIndex={0}
                        onKeyDown={(e) => {
                          if (e.key === 'Enter' || e.key === ' ') {
                            e.preventDefault()
                            handleViewTask(task)
                          }
                        }}
                      >
                        <td>
                          <div className="task-title-cell">
                            <div className="task-title">
                              <ReactMarkdown>{task.description}</ReactMarkdown>
                            </div>
                            {task.observation ? (
                              <div className="muted small task-observation">
                                <ReactMarkdown>{task.observation}</ReactMarkdown>
                              </div>
                            ) : null}
                          </div>
                        </td>
                        <td onClick={(e) => e.stopPropagation()}>
                          <select
                            value={task.status}
                            onChange={(e) =>
                              handleStatusChange(
                                task.id,
                                e.target.value as TaskStatus,
                              )
                            }
                            disabled={saving}
                            className="status-select"
                          >
                            <option value="Pending">Pendente</option>
                            <option value="InProgress">Em Progresso</option>
                            <option value="Finished">Concluída</option>
                            <option value="Paused">Pausada</option>
                            <option value="Cancelled">Cancelada</option>
                          </select>
                        </td>
                        <td>
                          {task.dueDate ? (() => {
                            const dueDate = new Date(task.dueDate)
                            const today = new Date()
                            today.setHours(0, 0, 0, 0)
                            dueDate.setHours(0, 0, 0, 0)
                            
                            const diffTime = dueDate.getTime() - today.getTime()
                            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
                            
                            let className = ''
                            if (diffDays < 0 && task.status !== 'Finished' && task.status !== 'Cancelled') {
                              className = 'date-overdue'
                            } else if (diffDays >= 0 && diffDays <= 3 && task.status !== 'Finished' && task.status !== 'Cancelled') {
                              className = 'date-warning'
                            }
                            
                            return <span className={className}>{dueDate.toLocaleDateString('pt-BR')}</span>
                          })() : '—'}
                        </td>
                        <td onClick={(e) => e.stopPropagation()}>
                          <div className="actions-cell">
                            <button
                              className="icon-btn"
                              onClick={() => handleEditTask(task)}
                              disabled={saving}
                              title="Editar tarefa"
                              aria-label="Editar tarefa"
                            >
                              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                                <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                                <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
                              </svg>
                            </button>
                            <button
                              className="icon-btn danger"
                              onClick={() => openDeleteConfirmation(task.id)}
                              disabled={saving}
                              title="Remover tarefa"
                              aria-label="Remover tarefa"
                            >
                              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                                <path d="M3 6h18" />
                                <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
                                <line x1="10" y1="11" x2="10" y2="17" />
                                <line x1="14" y1="11" x2="14" y2="17" />
                              </svg>
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}

            <div className="pagination">
              <button
                className="ghost"
                onClick={() => canGoBack && setPage((current) => current - 1)}
                disabled={!canGoBack || loading}
              >
                Página anterior
              </button>
              <span>
                Página {page} de {totalPages}
              </span>
              <button
                className="ghost"
                onClick={() =>
                  canGoForward && setPage((current) => current + 1)
                }
                disabled={!canGoForward || loading}
              >
                Próxima página
              </button>
            </div>
          </>
        )}
      </section>

      <Modal
        isOpen={isCreateModalOpen}
        onClose={() => {
          setIsCreateModalOpen(false)
          setEditingTask(null)
        }}
        title="Criar Nova Tarefa"
      >
        <TaskForm
          onSubmit={handleCreate}
          variant="create"
          initialTask={editingTask || undefined}
          onCancel={() => {
            setIsCreateModalOpen(false)
            setEditingTask(null)
          }}
        />
      </Modal>

      <Modal
        isOpen={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false)
          setEditingTask(null)
        }}
        title="Editar Tarefa"
      >
        {editingTask ? (
          <TaskForm
            variant="edit"
            initialTask={editingTask}
            onSubmit={(payload) => handleUpdate(editingTask.id, payload)}
            onCancel={() => {
              setIsEditModalOpen(false)
              setEditingTask(null)
            }}
          />
        ) : null}
      </Modal>

      <ConfirmDialog
        isOpen={deleteConfirmation.isOpen}
        onClose={closeDeleteConfirmation}
        onConfirm={confirmDelete}
        title="Excluir Tarefa"
        message="Tem certeza que deseja excluir esta tarefa? Esta ação não pode ser desfeita."
        confirmText="Excluir"
        cancelText="Cancelar"
        variant="danger"
      />

      <TaskViewModal
        task={viewingTask}
        isOpen={isViewModalOpen}
        onClose={() => {
          setIsViewModalOpen(false)
          setViewingTask(null)
        }}
        onEdit={handleViewToEdit}
      />

      <Modal
        isOpen={showSettings}
        onClose={() => setShowSettings(false)}
        title="Configurações - Chaves de Acesso"
      >
        <ApiKeyManager />
      </Modal>
    </div>
  )
}
