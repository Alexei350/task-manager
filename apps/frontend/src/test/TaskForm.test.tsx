import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { TaskForm } from '../components/TaskForm'
import type { Task } from '../services/api'

describe('TaskForm', () => {
  const mockOnSubmit = vi.fn()
  const mockOnCancel = vi.fn()

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('renders create form with empty fields', () => {
    render(
      <TaskForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
        variant="create"
      />
    )

    expect(screen.getByLabelText(/título/i)).toHaveValue('')
    expect(screen.getByLabelText(/status/i)).toHaveValue('Pending')
    expect(screen.getByRole('button', { name: /criar tarefa/i })).toBeInTheDocument()
  })

  it('renders edit form with initial task data', () => {
    const task: Task = {
      id: '1',
      description: 'Test task',
      observation: 'Test observation',
      status: 'InProgress',
      dueDate: '2024-12-31T00:00:00Z',
      creationDate: '2024-01-01T00:00:00Z',
      timeSpent: null,
      completedDate: null,
    }

    render(
      <TaskForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
        initialTask={task}
        variant="edit"
      />
    )

    expect(screen.getByLabelText(/título/i)).toHaveValue('Test task')
    expect(screen.getByLabelText(/observações/i)).toHaveValue('Test observation')
    expect(screen.getByLabelText(/status/i)).toHaveValue('InProgress')
    expect(screen.getByRole('button', { name: /salvar alterações/i })).toBeInTheDocument()
  })

  it('submits form with valid data', async () => {
    const user = userEvent.setup()
    mockOnSubmit.mockResolvedValue(undefined)

    render(
      <TaskForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
        variant="create"
      />
    )

    const descriptionInput = screen.getByLabelText(/título/i)
    const observationInput = screen.getByLabelText(/observações/i)
    const statusSelect = screen.getByLabelText(/status/i)
    const submitButton = screen.getByRole('button', { name: /criar tarefa/i })

    await user.type(descriptionInput, 'New task description')
    await user.type(observationInput, 'Some observation')
    await user.selectOptions(statusSelect, 'InProgress')
    await user.click(submitButton)

    await waitFor(() => {
      expect(mockOnSubmit).toHaveBeenCalledWith({
        description: 'New task description',
        observation: 'Some observation',
        status: 'InProgress',
        dueDate: null,
        timeSpent: null,
      })
    })
  })

  it('shows error message when submission fails', async () => {
    const user = userEvent.setup()
    mockOnSubmit.mockRejectedValue(new Error('Failed to create task'))

    render(
      <TaskForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
        variant="create"
      />
    )

    const descriptionInput = screen.getByLabelText(/título/i)
    const submitButton = screen.getByRole('button', { name: /criar tarefa/i })

    await user.type(descriptionInput, 'New task')
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText(/failed to create task/i)).toBeInTheDocument()
    })
  })

  it('calls onCancel when cancel button is clicked', async () => {
    const user = userEvent.setup()

    render(
      <TaskForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
        variant="create"
      />
    )

    const cancelButton = screen.getByRole('button', { name: /cancelar/i })
    await user.click(cancelButton)

    expect(mockOnCancel).toHaveBeenCalledTimes(1)
  })

  it('disables submit button while submitting', async () => {
    const user = userEvent.setup()
    mockOnSubmit.mockImplementation(() => new Promise((resolve) => setTimeout(resolve, 100)))

    render(
      <TaskForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
        variant="create"
      />
    )

    const descriptionInput = screen.getByLabelText(/título/i)
    const submitButton = screen.getByRole('button', { name: /criar tarefa/i })

    await user.type(descriptionInput, 'New task')
    await user.click(submitButton)

    expect(submitButton).toBeDisabled()
    
    await waitFor(() => {
      expect(submitButton).not.toBeDisabled()
    })
  })

  it('clears form after successful create submission', async () => {
    const user = userEvent.setup()
    mockOnSubmit.mockResolvedValue(undefined)

    render(
      <TaskForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
        variant="create"
      />
    )

    const descriptionInput = screen.getByLabelText(/título/i)
    const submitButton = screen.getByRole('button', { name: /criar tarefa/i })

    await user.type(descriptionInput, 'New task')
    await user.click(submitButton)

    await waitFor(() => {
      expect(descriptionInput).toHaveValue('')
    })
  })

  it('trims whitespace from description and observation', async () => {
    const user = userEvent.setup()
    mockOnSubmit.mockResolvedValue(undefined)

    render(
      <TaskForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
        variant="create"
      />
    )

    const descriptionInput = screen.getByLabelText(/título/i)
    const observationInput = screen.getByLabelText(/observações/i)
    const submitButton = screen.getByRole('button', { name: /criar tarefa/i })

    await user.type(descriptionInput, '  Task with spaces  ')
    await user.type(observationInput, '  Observation with spaces  ')
    await user.click(submitButton)

    await waitFor(() => {
      expect(mockOnSubmit).toHaveBeenCalledWith(
        expect.objectContaining({
          description: 'Task with spaces',
          observation: 'Observation with spaces',
        })
      )
    })
  })

  it('converts empty observation to null', async () => {
    const user = userEvent.setup()
    mockOnSubmit.mockResolvedValue(undefined)

    render(
      <TaskForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
        variant="create"
      />
    )

    const descriptionInput = screen.getByLabelText(/título/i)
    const submitButton = screen.getByRole('button', { name: /criar tarefa/i })

    await user.type(descriptionInput, 'Task without observation')
    await user.click(submitButton)

    await waitFor(() => {
      expect(mockOnSubmit).toHaveBeenCalledWith(
        expect.objectContaining({
          observation: null,
        })
      )
    })
  })
})
