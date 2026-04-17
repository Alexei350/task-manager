import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import { ApiTargetPill } from '../components/ApiTargetPill'

describe('ApiTargetPill', () => {
  it('renders the API base URL', () => {
    render(<ApiTargetPill />)
    
    const pillElement = screen.getByText(/API:/i)
    expect(pillElement).toBeInTheDocument()
    expect(pillElement).toHaveClass('pill')
  })

  it('displays the correct API URL format', () => {
    render(<ApiTargetPill />)
    
    const pillElement = screen.getByText(/API:/i)
    expect(pillElement.textContent).toMatch(/^API: /)
  })
})
