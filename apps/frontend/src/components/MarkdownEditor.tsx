import { useMemo } from 'react'
import SimpleMDE from 'react-simplemde-editor'
import type { Options } from 'easymde'
import 'easymde/dist/easymde.min.css'

interface MarkdownEditorProps {
  value: string
  onChange: (value: string) => void
  placeholder?: string
}

export function MarkdownEditor({ value, onChange, placeholder }: MarkdownEditorProps) {
  const options = useMemo<Options>(() => {
    return {
      spellChecker: false,
      placeholder: placeholder || 'Digite aqui...',
      status: false,
      toolbar: [
        'bold',
        'italic',
        'strikethrough',
        '|',
        'heading-1',
        'heading-2',
        'heading-3',
        '|',
        'quote',
        'unordered-list',
        'ordered-list',
        '|',
        'link',
        'image',
        '|',
        'preview',
        'side-by-side',
        'fullscreen',
        '|',
        'guide',
      ],
      minHeight: '200px',
      maxHeight: '400px',
      autofocus: false,
      hideIcons: ['guide'],
      showIcons: ['code', 'table'],
    }
  }, [placeholder])

  return (
    <SimpleMDE
      value={value}
      onChange={onChange}
      options={options}
    />
  )
}
