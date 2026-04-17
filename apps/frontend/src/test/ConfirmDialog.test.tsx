import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { ConfirmDialog } from '../components/ConfirmDialog'

describe('ConfirmDialog', () => {
  const mockOnClose = vi.fn()
  const mockOnConfirm = vi.fn()

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('does not render when isOpen is false', () => {
    render(
      <ConfirmDialog
        isOpen={false}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        title="Test Title"
        message="Test message"
      />
    )

    const dialog = screen.getByRole('dialog', { hidden: true })
    expect(dialog).not.toHaveAttribute('open')
  })

  it('renders dialog when isOpen is true', async () => {
    render(
      <ConfirmDialog
        isOpen={true}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        title="Test Title"
        message="Test message"
      />
    )

    await waitFor(() => {
      const dialog = screen.getByRole('dialog')
      expect(dialog).toHaveAttribute('open')
    })
  })

  it('renders title and message correctly', async () => {
    render(
      <ConfirmDialog
        isOpen={true}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        title="Delete Task"
        message="Are you sure you want to delete this task?"
      />
    )

    await waitFor(() => {
      expect(screen.getByText('Delete Task')).toBeInTheDocument()
      expect(screen.getByText('Are you sure you want to delete this task?')).toBeInTheDocument()
    })
  })

  it('renders custom button texts', async () => {
    render(
      <ConfirmDialog
        isOpen={true}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        title="Confirm"
        message="Proceed?"
        confirmText="Yes"
        cancelText="No"
      />
    )

    await waitFor(() => {
      expect(screen.getByText('Yes')).toBeInTheDocument()
      expect(screen.getByText('No')).toBeInTheDocument()
    })
  })

  it('renders default button texts when not provided', async () => {
    render(
      <ConfirmDialog
        isOpen={true}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        title="Confirm"
        message="Proceed?"
      />
    )

    await waitFor(() => {
      expect(screen.getByText('Confirmar')).toBeInTheDocument()
      expect(screen.getByText('Cancelar')).toBeInTheDocument()
    })
  })

  it('calls onConfirm when confirm button is clicked', async () => {
    const user = userEvent.setup()
    render(
      <ConfirmDialog
        isOpen={true}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        title="Confirm"
        message="Proceed?"
      />
    )

    await waitFor(() => screen.getByText('Confirmar'))
    const confirmButton = screen.getByText('Confirmar')
    await user.click(confirmButton)

    expect(mockOnConfirm).toHaveBeenCalledTimes(1)
  })

  it('calls onClose when cancel button is clicked', async () => {
    const user = userEvent.setup()
    render(
      <ConfirmDialog
        isOpen={true}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        title="Confirm"
        message="Proceed?"
      />
    )

    await waitFor(() => screen.getByText('Cancelar'))
    const cancelButton = screen.getByText('Cancelar')
    await user.click(cancelButton)

    expect(mockOnClose).toHaveBeenCalledTimes(1)
  })

  it('applies danger variant class by default', async () => {
    const { container } = render(
      <ConfirmDialog
        isOpen={true}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        title="Confirm"
        message="Proceed?"
      />
    )

    await waitFor(() => {
      const dialog = container.querySelector('.confirm-dialog-danger')
      expect(dialog).toBeInTheDocument()
    })
  })

  it('applies warning variant class when specified', async () => {
    const { container } = render(
      <ConfirmDialog
        isOpen={true}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        title="Confirm"
        message="Proceed?"
        variant="warning"
      />
    )

    await waitFor(() => {
      const dialog = container.querySelector('.confirm-dialog-warning')
      expect(dialog).toBeInTheDocument()
    })
  })

  it('applies info variant class when specified', async () => {
    const { container } = render(
      <ConfirmDialog
        isOpen={true}
        onClose={mockOnClose}
        onConfirm={mockOnConfirm}
        title="Confirm"
        message="Proceed?"
        variant="info"
      />
    )

    await waitFor(() => {
      const dialog = container.querySelector('.confirm-dialog-info')
      expect(dialog).toBeInTheDocument()
    })
  })
})
