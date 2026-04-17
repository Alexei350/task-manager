import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import RegisterForm from '../components/RegisterForm'
import * as authContext from '../context/AuthContext'
import * as api from '../services/api'

vi.mock('../context/AuthContext')
vi.mock('../services/api')
vi.mock('@react-oauth/google', () => ({
  GoogleLogin: ({ onSuccess }: { onSuccess: (response: { credential?: string }) => void }) => (
    <button onClick={() => onSuccess({ credential: 'fake-google-token' })}>
      Google Login Mock
    </button>
  ),
}))

describe('RegisterForm', () => {
  const mockLoginWithGoogle = vi.fn()
  const mockOnSwitchToLogin = vi.fn()

  beforeEach(() => {
    vi.clearAllMocks()
    vi.spyOn(authContext, 'useAuth').mockReturnValue({
      login: vi.fn(),
      loginWithGoogle: mockLoginWithGoogle,
      logout: vi.fn(),
      session: null,
      isAuthenticated: false,
    })
  })

  it('renders register form with all fields', () => {
    render(<RegisterForm theme="light" />)

    expect(screen.getByLabelText(/nome completo/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/e-mail/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/senha/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /registrar/i })).toBeInTheDocument()
  })

  it('submits form with name, email, and password', async () => {
    const user = userEvent.setup()
    vi.spyOn(api, 'registerUser').mockResolvedValue('Registration successful')

    render(<RegisterForm theme="light" />)

    const nameInput = screen.getByLabelText(/nome completo/i)
    const emailInput = screen.getByLabelText(/e-mail/i)
    const passwordInput = screen.getByLabelText(/senha/i)
    const submitButton = screen.getByRole('button', { name: /registrar/i })

    await user.type(nameInput, 'John Doe')
    await user.type(emailInput, 'john@example.com')
    await user.type(passwordInput, 'password123')
    await user.click(submitButton)

    await waitFor(() => {
      expect(api.registerUser).toHaveBeenCalledWith({
        name: 'John Doe',
        email: 'john@example.com',
        password: 'password123',
      })
    })
  })

  it('displays success message after successful registration', async () => {
    const user = userEvent.setup()
    vi.spyOn(api, 'registerUser').mockResolvedValue('Registration successful')

    render(<RegisterForm theme="light" />)

    const nameInput = screen.getByLabelText(/nome completo/i)
    const emailInput = screen.getByLabelText(/e-mail/i)
    const passwordInput = screen.getByLabelText(/senha/i)
    const submitButton = screen.getByRole('button', { name: /registrar/i })

    await user.type(nameInput, 'John Doe')
    await user.type(emailInput, 'john@example.com')
    await user.type(passwordInput, 'password123')
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText(/registration successful/i)).toBeInTheDocument()
    })
  })

  it('displays error message when registration fails', async () => {
    const user = userEvent.setup()
    vi.spyOn(api, 'registerUser').mockRejectedValue(new Error('Email already exists'))

    render(<RegisterForm theme="light" />)

    const nameInput = screen.getByLabelText(/nome completo/i)
    const emailInput = screen.getByLabelText(/e-mail/i)
    const passwordInput = screen.getByLabelText(/senha/i)
    const submitButton = screen.getByRole('button', { name: /registrar/i })

    await user.type(nameInput, 'John Doe')
    await user.type(emailInput, 'existing@example.com')
    await user.type(passwordInput, 'password123')
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText(/email already exists/i)).toBeInTheDocument()
    })
  })

  it('clears password field after successful registration', async () => {
    const user = userEvent.setup()
    vi.spyOn(api, 'registerUser').mockResolvedValue('Registration successful')

    render(<RegisterForm theme="light" />)

    const nameInput = screen.getByLabelText(/nome completo/i)
    const emailInput = screen.getByLabelText(/e-mail/i)
    const passwordInput = screen.getByLabelText(/senha/i)
    const submitButton = screen.getByRole('button', { name: /registrar/i })

    await user.type(nameInput, 'John Doe')
    await user.type(emailInput, 'john@example.com')
    await user.type(passwordInput, 'password123')
    await user.click(submitButton)

    await waitFor(() => {
      expect(passwordInput).toHaveValue('')
    })
  })

  it('disables submit button while loading', async () => {
    const user = userEvent.setup()
    vi.spyOn(api, 'registerUser').mockImplementation(
      () => new Promise((resolve) => setTimeout(() => resolve('Success'), 100))
    )

    render(<RegisterForm theme="light" />)

    const nameInput = screen.getByLabelText(/nome completo/i)
    const emailInput = screen.getByLabelText(/e-mail/i)
    const passwordInput = screen.getByLabelText(/senha/i)
    const submitButton = screen.getByRole('button', { name: /registrar/i })

    await user.type(nameInput, 'John Doe')
    await user.type(emailInput, 'john@example.com')
    await user.type(passwordInput, 'password123')
    await user.click(submitButton)

    expect(submitButton).toBeDisabled()

    await waitFor(() => {
      expect(submitButton).not.toBeDisabled()
    })
  })

  it('handles Google registration success', async () => {
    const user = userEvent.setup()
    mockLoginWithGoogle.mockResolvedValue(undefined)

    render(<RegisterForm theme="light" />)

    const googleButton = screen.getByText('Google Login Mock')
    await user.click(googleButton)

    await waitFor(() => {
      expect(mockLoginWithGoogle).toHaveBeenCalledWith('fake-google-token')
    })
  })

  it('displays error when Google registration fails', async () => {
    const user = userEvent.setup()
    mockLoginWithGoogle.mockRejectedValue(new Error('Google registration failed'))

    render(<RegisterForm theme="light" />)

    const googleButton = screen.getByText('Google Login Mock')
    await user.click(googleButton)

    await waitFor(() => {
      expect(screen.getByText(/google registration failed/i)).toBeInTheDocument()
    })
  })

  it('calls onSwitchToLogin when login link is clicked', async () => {
    const user = userEvent.setup()

    render(<RegisterForm theme="light" onSwitchToLogin={mockOnSwitchToLogin} />)

    const loginLink = screen.getByText(/já tenho uma conta/i)
    await user.click(loginLink)

    expect(mockOnSwitchToLogin).toHaveBeenCalledTimes(1)
  })

  it('trims whitespace from name and email', async () => {
    const user = userEvent.setup()
    vi.spyOn(api, 'registerUser').mockResolvedValue('Success')

    render(<RegisterForm theme="light" />)

    const nameInput = screen.getByLabelText(/nome completo/i)
    const emailInput = screen.getByLabelText(/e-mail/i)
    const passwordInput = screen.getByLabelText(/senha/i)
    const submitButton = screen.getByRole('button', { name: /registrar/i })

    await user.type(nameInput, '  John Doe  ')
    await user.type(emailInput, '  john@example.com  ')
    await user.type(passwordInput, 'password123')
    await user.click(submitButton)

    await waitFor(() => {
      expect(api.registerUser).toHaveBeenCalledWith({
        name: 'John Doe',
        email: 'john@example.com',
        password: 'password123',
      })
    })
  })
})
