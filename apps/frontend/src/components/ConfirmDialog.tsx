import { useEffect, useRef, useCallback } from 'react'

interface ConfirmDialogProps {
    isOpen: boolean
    onClose: () => void
    onConfirm: () => void
    title: string
    message: string
    confirmText?: string
    cancelText?: string
    variant?: 'danger' | 'warning' | 'info'
}

export function ConfirmDialog({
    isOpen,
    onClose,
    onConfirm,
    title,
    message,
    confirmText = 'Confirmar',
    cancelText = 'Cancelar',
    variant = 'danger',
}: ConfirmDialogProps) {
    const dialogRef = useRef<HTMLDialogElement>(null)
    const confirmButtonRef = useRef<HTMLButtonElement>(null)

    const handleConfirm = useCallback(() => {
        onConfirm()
        onClose()
    }, [onConfirm, onClose])

    useEffect(() => {
        const dialog = dialogRef.current
        if (!dialog) return

        if (isOpen) {
            if (typeof dialog.showModal === 'function') {
                dialog.showModal()
            }
        } else {
            if (typeof dialog.close === 'function') {
                dialog.close()
            }
        }
    }, [isOpen])

    useEffect(() => {
        const dialog = dialogRef.current
        if (!dialog) return

        const handleCancel = (event: Event) => {
            event.preventDefault()
            onClose()
        }

        const handleKeyDown = (event: KeyboardEvent) => {
            if (event.key === 'Enter' && !event.shiftKey && !event.ctrlKey && !event.metaKey) {
                event.preventDefault()
                handleConfirm()
            }
        }

        dialog.addEventListener('cancel', handleCancel)
        dialog.addEventListener('keydown', handleKeyDown)

        return () => {
            dialog.removeEventListener('cancel', handleCancel)
            dialog.removeEventListener('keydown', handleKeyDown)
        }
    }, [onClose, handleConfirm])

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

    // Foca automaticamente no botão de confirmar quando o modal abre
    useEffect(() => {
        if (isOpen) {
            confirmButtonRef.current?.focus()
        }
    }, [isOpen])

    return (
        <dialog
            ref={dialogRef}
            className={`confirm-dialog confirm-dialog-${variant}`}
            onClick={handleBackdropClick}
        >
            <div className="confirm-dialog-content">
                <div className="confirm-dialog-header">
                    <h2>{title}</h2>
                </div>
                <div className="confirm-dialog-body">
                    <p>{message}</p>
                </div>
                <div className="confirm-dialog-actions">
                    <button
                        type="button"
                        className="ghost"
                        onClick={onClose}
                    >
                        {cancelText}
                    </button>
                    <button
                        ref={confirmButtonRef}
                        type="button"
                        className={variant === 'danger' ? 'danger-btn' : 'primary-btn'}
                        onClick={handleConfirm}
                    >
                        {confirmText}
                    </button>
                </div>
            </div>
        </dialog>
    )
}
