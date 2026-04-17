import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import { StatusBadge } from '../components/StatusBadge'

describe('StatusBadge', () => {
  it('renders Pending status correctly', () => {
    render(<StatusBadge status="Pending" />)
    expect(screen.getByText('Pendente')).toBeInTheDocument()
    expect(screen.getByText('Pendente')).toHaveClass('status-badge', 'status-pending')
  })

  it('renders InProgress status correctly', () => {
    render(<StatusBadge status="InProgress" />)
    expect(screen.getByText('Em Progresso')).toBeInTheDocument()
    expect(screen.getByText('Em Progresso')).toHaveClass('status-badge', 'status-in-progress')
  })

  it('renders Unknown status correctly', () => {
    render(<StatusBadge status="Unknown" />)
    expect(screen.getByText('Indefinido')).toBeInTheDocument()
    expect(screen.getByText('Indefinido')).toHaveClass('status-badge', 'status-unknown')
  })

  it('renders Paused status correctly', () => {
    render(<StatusBadge status="Paused" />)
    expect(screen.getByText('Pausada')).toBeInTheDocument()
    expect(screen.getByText('Pausada')).toHaveClass('status-badge', 'status-paused')
  })

  it('renders Cancelled status correctly', () => {
    render(<StatusBadge status="Cancelled" />)
    expect(screen.getByText('Cancelada')).toBeInTheDocument()
    expect(screen.getByText('Cancelada')).toHaveClass('status-badge', 'status-cancelled')
  })

  it('renders Finished status correctly', () => {
    render(<StatusBadge status="Finished" />)
    expect(screen.getByText('Concluída')).toBeInTheDocument()
    expect(screen.getByText('Concluída')).toHaveClass('status-badge', 'status-finished')
  })
})
