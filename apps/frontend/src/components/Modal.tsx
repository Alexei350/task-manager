import { useEffect, useRef, type PropsWithChildren } from 'react'

interface ModalProps {
  isOpen: boolean
  onClose: () => void
  title: string
}

export function Modal({
  isOpen,
  onClose,
  title,
  children,
}: PropsWithChildren<ModalProps>) {
  const dialogRef = useRef<HTMLDialogElement>(null)

  useEffect(() => {
    const dialog = dialogRef.current
    if (!dialog) return

    if (isOpen) {
      dialog.showModal()
    } else {
      dialog.close()
    }
  }, [isOpen])

  useEffect(() => {
    const dialog = dialogRef.current
    if (!dialog) return

    const handleCancel = (event: Event) => {
      event.preventDefault()
      onClose()
    }

    dialog.addEventListener('cancel', handleCancel)

    return () => {
      dialog.removeEventListener('cancel', handleCancel)
    }
  }, [onClose])

  const handleBackdropClick = (event: React.MouseEvent<HTMLDialogElement>) => {
    const dialog = dialogRef.current
    if (!dialog) return

    const rect = dialog.getBoundingClientRect()
    const isInDialog =
      rect.top <= event.clientY &&
      event.clientY <= rect.top + rect.height &&
      rect.left <= event.clientX &&
      event.clientX <= rect.left + rect.width

    if (!isInDialog) {
      onClose()
    }
  }

  if (!isOpen) return null

  return (
    <dialog 
      ref={dialogRef} 
      className="modal" 
      onClick={handleBackdropClick}
    >
      <div className="modal-content">
        <div className="modal-header">
          <h3 className="modal-title">{title}</h3>
          <button
            type="button"
            className="modal-close"
            onClick={onClose}
            aria-label="Fechar"
          >
            ✕
          </button>
        </div>
        <div className="modal-body">{children}</div>
      </div>
    </dialog>
  )
}
