import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { GoogleOAuthProvider } from '@react-oauth/google'
import App from '../App'
import { AuthProvider } from '../context/AuthContext'
import { TaskForm } from '../components/TaskForm'
import type { TaskPayload } from '../services/api'

const createFetchResponse = (payload: unknown, ok = true) =>
  Promise.resolve({
    ok,
    status: ok ? 200 : 400,
    json: () => Promise.resolve(payload),
  } as Response)

describe('Task Manager frontend', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    localStorage.clear()
  })

  it('authenticates and loads tasks after login', async () => {
    const fetchMock = vi.spyOn(window, 'fetch').mockImplementationOnce(() =>
      createFetchResponse({
        success: true,
        data: {
          token: 'token-123',
          refreshToken: 'refresh-123',
          user: { id: '1', name: 'Alexei', email: 'alexei@example.com' },
        },
      }),
    )

    fetchMock.mockImplementationOnce(() =>
      createFetchResponse({
        success: true,
        data: [
          {
            id: 't1',
            status: 'Pending',
            description: 'Configurar Jenkins',
            observation: 'Pipeline do frontend',
            creationDate: '2024-01-10T00:00:00Z',
            dueDate: null,
            completedDate: null,
            timeSpent: null,
          },
        ],
        totalRecords: 1,
        totalPages: 1,
      }),
    )

    render(
      <GoogleOAuthProvider clientId="test-client-id">
        <AuthProvider>
          <App />
        </AuthProvider>
      </GoogleOAuthProvider>,
    )

    await userEvent.type(
      screen.getByLabelText(/e-mail/i),
      'alexei@example.com',
    )
    await userEvent.type(screen.getByLabelText(/senha/i), 'strong-password')
    await userEvent.click(screen.getByRole('button', { name: /entrar/i }))

    await waitFor(() =>
      expect(screen.getAllByText(/gerenciar tarefas/i)[0]).toBeInTheDocument(),
    )

    expect(await screen.findByText('Configurar Jenkins')).toBeInTheDocument()
    expect(fetchMock).toHaveBeenCalledTimes(2)
  })

  it('registers a new user and logs in automatically', async () => {
    const fetchMock = vi.spyOn(window, 'fetch')

    fetchMock.mockImplementationOnce(() =>
      createFetchResponse({
        success: true,
        messages: [{ type: 'Success', message: 'Usuário criado.' }],
      }),
    )

    fetchMock.mockImplementationOnce(() =>
      createFetchResponse({
        success: true,
        data: {
          token: 'token-abc',
          refreshToken: 'refresh-abc',
          user: { id: '2', name: 'Nova Pessoa', email: 'nova@example.com' },
        },
      }),
    )

    fetchMock.mockImplementationOnce(() =>
      createFetchResponse({
        success: true,
        data: [],
        totalRecords: 0,
        totalPages: 1,
      }),
    )

    render(
      <GoogleOAuthProvider clientId="test-client-id">
        <AuthProvider>
          <App />
        </AuthProvider>
      </GoogleOAuthProvider>,
    )

    await userEvent.click(
      screen.getByRole('button', { name: /criar uma conta/i }),
    )

    await userEvent.type(screen.getByLabelText(/nome completo/i), 'Nova Pessoa')
    await userEvent.type(
      screen.getByLabelText(/e-mail/i),
      'nova@example.com',
    )
    await userEvent.type(
      screen.getByLabelText(/senha/i),
      'senha-super-segura',
    )

    await userEvent.click(screen.getByRole('button', { name: /registrar/i }))

    expect(
      await screen.findByText(/Usuário criado\./i),
    ).toBeInTheDocument()
    expect(fetchMock).toHaveBeenCalledTimes(1)
    expect(fetchMock.mock.calls[0]?.[0]).toMatch(/\/User$/)

    await userEvent.click(
      screen.getByRole('button', { name: /já tenho uma conta/i }),
    )

    await userEvent.type(
      screen.getByLabelText(/e-mail/i),
      'nova@example.com',
    )
    await userEvent.type(
      screen.getByLabelText(/senha/i),
      'senha-super-segura',
    )

    await userEvent.click(screen.getByRole('button', { name: /entrar/i }))

    await waitFor(() =>
      expect(screen.getAllByText(/gerenciar tarefas/i)[0]).toBeInTheDocument(),
    )

    expect(fetchMock).toHaveBeenCalledTimes(3)
  })

  it('submits task form with normalized data', async () => {
    const onSubmit =
      vi.fn<(payload: TaskPayload) => Promise<void>>(async () => {})
    const onCancel = vi.fn()

    render(
      <GoogleOAuthProvider clientId="test-client-id">
        <AuthProvider initialSession={null}>
          <TaskForm onSubmit={onSubmit} onCancel={onCancel} variant="create" />
        </AuthProvider>
      </GoogleOAuthProvider>,
    )

    const titleInput = screen.getByLabelText(/título/i)
    const observationTextarea = screen.getByLabelText(/observações/i)
    const statusSelect = screen.getByLabelText(/status/i)
    const dueDateInput = screen.getByLabelText(/prazo/i)

    await userEvent.clear(titleInput)
    await userEvent.type(titleInput, '  Nova tarefa importante ')
    
    await userEvent.clear(observationTextarea)
    await userEvent.type(observationTextarea, ' Ajustar payload ')
    
    await userEvent.selectOptions(statusSelect, 'Finished')
    await userEvent.type(dueDateInput, '2025-01-15')

    await userEvent.click(
      screen.getByRole('button', { name: /criar tarefa/i }),
    )

    await waitFor(() => expect(onSubmit).toHaveBeenCalled())
    const payload = onSubmit.mock.calls[0][0]

    expect(payload.description).toBe('Nova tarefa importante')
    expect(payload.observation).toBe('Ajustar payload')
    expect(payload.status).toBe('Finished')
    expect(payload.dueDate).toMatch(/2025-01-15/)
  })
})
