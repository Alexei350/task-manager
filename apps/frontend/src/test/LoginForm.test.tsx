import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import LoginForm from '../components/LoginForm'
import * as authContext from '../context/AuthContext'

vi.mock('../context/AuthContext')
vi.mock('@react-oauth/google', () => ({
  GoogleLogin: ({ onSuccess }: { onSuccess: (response: { credential?: string }) => void }) => (
    <button onClick={() => onSuccess({ credential: 'fake-google-token' })}>
      Google Login Mock
    </button>
  ),
}))

describe('LoginForm', () => {
  const mockLogin = vi.fn()
  const mockLoginWithGoogle = vi.fn()
  const mockOnSwitchToRegister = vi.fn()

  beforeEach(() => {
    vi.clearAllMocks()
    vi.spyOn(authContext, 'useAuth').mockReturnValue({
      login: mockLogin,
      loginWithGoogle: mockLoginWithGoogle,
      logout: vi.fn(),
      session: null,
      isAuthenticated: false,
    })
  })

  it('renders login form with all fields', () => {
    render(<LoginForm theme="light" />)

    expect(screen.getByLabelText(/e-mail/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/senha/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /entrar/i })).toBeInTheDocument()
  })

  it('submits form with email and password', async () => {
    const user = userEvent.setup()
    mockLogin.mockResolvedValue(undefined)

    render(<LoginForm theme="light" />)

    const emailInput = screen.getByLabelText(/e-mail/i)
    const passwordInput = screen.getByLabelText(/senha/i)
    const submitButton = screen.getByRole('button', { name: /entrar/i })

    await user.type(emailInput, 'test@example.com')
    await user.type(passwordInput, 'password123')
    await user.click(submitButton)

    await waitFor(() => {
      expect(mockLogin).toHaveBeenCalledWith('test@example.com', 'password123')
    })
  })

  it('displays error message when login fails', async () => {
    const user = userEvent.setup()
    mockLogin.mockRejectedValue(new Error('Invalid credentials'))

    render(<LoginForm theme="light" />)

    const emailInput = screen.getByLabelText(/e-mail/i)
    const passwordInput = screen.getByLabelText(/senha/i)
    const submitButton = screen.getByRole('button', { name: /entrar/i })

    await user.type(emailInput, 'test@example.com')
    await user.type(passwordInput, 'wrongpassword')
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText(/invalid credentials/i)).toBeInTheDocument()
    })
  })

  it('disables submit button while loading', async () => {
    const user = userEvent.setup()
    mockLogin.mockImplementation(() => new Promise((resolve) => setTimeout(resolve, 100)))

    render(<LoginForm theme="light" />)

    const emailInput = screen.getByLabelText(/e-mail/i)
    const passwordInput = screen.getByLabelText(/senha/i)
    const submitButton = screen.getByRole('button', { name: /entrar/i })

    await user.type(emailInput, 'test@example.com')
    await user.type(passwordInput, 'password123')
    await user.click(submitButton)

    expect(submitButton).toBeDisabled()

    await waitFor(() => {
      expect(submitButton).not.toBeDisabled()
    })
  })

  it('handles Google login success', async () => {
    const user = userEvent.setup()
    mockLoginWithGoogle.mockResolvedValue(undefined)

    render(<LoginForm theme="light" />)

    const googleButton = screen.getByText('Google Login Mock')
    await user.click(googleButton)

    await waitFor(() => {
      expect(mockLoginWithGoogle).toHaveBeenCalledWith('fake-google-token')
    })
  })

  it('displays error when Google login fails', async () => {
    const user = userEvent.setup()
    mockLoginWithGoogle.mockRejectedValue(new Error('Google authentication failed'))

    render(<LoginForm theme="light" />)

    const googleButton = screen.getByText('Google Login Mock')
    await user.click(googleButton)

    await waitFor(() => {
      expect(screen.getByText(/google authentication failed/i)).toBeInTheDocument()
    })
  })

  it('calls onSwitchToRegister when register link is clicked', async () => {
    const user = userEvent.setup()

    render(<LoginForm theme="light" onSwitchToRegister={mockOnSwitchToRegister} />)

    const registerLink = screen.getByText(/criar uma conta/i)
    await user.click(registerLink)

    expect(mockOnSwitchToRegister).toHaveBeenCalledTimes(1)
  })

  it('clears error message when form is resubmitted', async () => {
    const user = userEvent.setup()
    mockLogin.mockRejectedValueOnce(new Error('First error'))
    mockLogin.mockResolvedValueOnce(undefined)

    render(<LoginForm theme="light" />)

    const emailInput = screen.getByLabelText(/e-mail/i)
    const passwordInput = screen.getByLabelText(/senha/i)
    const submitButton = screen.getByRole('button', { name: /entrar/i })

    // First submission with error
    await user.type(emailInput, 'test@example.com')
    await user.type(passwordInput, 'wrong')
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText(/first error/i)).toBeInTheDocument()
    })

    // Second submission should clear error
    await user.clear(passwordInput)
    await user.type(passwordInput, 'correct')
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.queryByText(/first error/i)).not.toBeInTheDocument()
    })
  })
})
