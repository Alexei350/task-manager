import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { AppHeader } from '../components/AppHeader'
import * as authHook from '../context/useAuth'

vi.mock('../context/useAuth')

describe('AppHeader', () => {
  const mockToggleTheme = vi.fn()
  const mockLogout = vi.fn()
  const mockSetShowSettings = vi.fn()

  beforeEach(() => {
    vi.clearAllMocks()
    vi.spyOn(authHook, 'useAuth').mockReturnValue({
      session: {
        token: 'fake-token',
        refreshToken: 'fake-refresh-token',
        user: {
          id: '1',
          name: 'John Doe',
          email: 'john@example.com',
        },
      },
      logout: mockLogout,
      login: vi.fn(),
      loginWithGoogle: vi.fn(),
      isAuthenticated: true,
    })
  })

  it('renders the header with logo and user name', () => {
    render(<AppHeader theme="light" toggleTheme={mockToggleTheme} showSettings={false} setShowSettings={mockSetShowSettings} />)

    expect(screen.getByText('Task Manager')).toBeInTheDocument()
    expect(screen.getByText('Dashboard')).toBeInTheDocument()
    expect(screen.getByText('John Doe')).toBeInTheDocument()
  })

  it('renders light theme icon when theme is light', () => {
    render(<AppHeader theme="light" toggleTheme={mockToggleTheme} showSettings={false} setShowSettings={mockSetShowSettings} />)

    const button = screen.getByRole('button', { name: /alternar tema/i })
    expect(button).toBeInTheDocument()
    expect(button).toHaveAttribute('title', 'Ativar modo escuro')
  })

  it('renders dark theme icon when theme is dark', () => {
    render(<AppHeader theme="dark" toggleTheme={mockToggleTheme} showSettings={false} setShowSettings={mockSetShowSettings} />)

    const button = screen.getByRole('button', { name: /alternar tema/i })
    expect(button).toHaveAttribute('title', 'Ativar modo claro')
  })

  it('calls toggleTheme when theme button is clicked', async () => {
    const user = userEvent.setup()
    render(<AppHeader theme="light" toggleTheme={mockToggleTheme} showSettings={false} setShowSettings={mockSetShowSettings} />)

    const themeButton = screen.getByRole('button', { name: /alternar tema/i })
    await user.click(themeButton)

    expect(mockToggleTheme).toHaveBeenCalledTimes(1)
  })

  it('calls logout when logout button is clicked', async () => {
    const user = userEvent.setup()
    render(<AppHeader theme="light" toggleTheme={mockToggleTheme} showSettings={false} setShowSettings={mockSetShowSettings} />)

    const logoutButton = screen.getByRole('button', { name: /sair/i })
    await user.click(logoutButton)

    expect(mockLogout).toHaveBeenCalledTimes(1)
  })
})
