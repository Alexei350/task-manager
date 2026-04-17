import { useState, type FormEvent } from 'react'
import { GoogleLogin } from '@react-oauth/google'
import { useAuth } from '../context/AuthContext'

type LoginFormProps = {
  onSwitchToRegister?: () => void
  theme: 'light' | 'dark'
}

export default function LoginForm({
  onSwitchToRegister,
  theme,
}: LoginFormProps) {
  const { login, loginWithGoogle } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setError(null)
    setLoading(true)

    try {
      await login(email, password)
    } catch (err) {
      setError(
        err instanceof Error ? err.message : 'Não foi possível autenticar.',
      )
    } finally {
      setLoading(false)
    }
  }

  const handleGoogleSuccess = async (credentialResponse: { credential?: string }) => {
    setError(null)
    setLoading(true)
    try {
      if (credentialResponse.credential) {
        await loginWithGoogle(credentialResponse.credential)
      }
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : 'Não foi possível autenticar com Google.',
      )
    } finally {
      setLoading(false)
    }
  }

  const handleGoogleError = () => {
    setError('Falha ao conectar com o Google.')
  }

  return (
    <div className="login-grid">
      <section className="panel">
        <div className="panel-header">
          <div>
            <p className="eyebrow">Bem-vindo</p>
            <h2>Acesse o Task Manager</h2>
            <p className="muted">
              Entre com suas credenciais para gerenciar suas tarefas.
            </p>
          </div>
        </div>

        <form className="login-form" onSubmit={handleSubmit}>
          <label className="field">
            <span>E-mail</span>
            <input
              type="email"
              name="email"
              placeholder="exemplo@empresa.com"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              required
              autoComplete="username"
            />
          </label>

          <label className="field">
            <span>Senha</span>
            <input
              type="password"
              name="password"
              placeholder="••••••••"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
              autoComplete="current-password"
            />
          </label>

          {error ? <p className="feedback error">{error}</p> : null}

          <div className="actions stacked">
            <button type="submit" disabled={loading}>
              {loading ? 'Entrando...' : 'Entrar'}
            </button>
            {onSwitchToRegister ? (
              <button
                type="button"
                className="ghost"
                onClick={onSwitchToRegister}
                disabled={loading}
              >
                Criar uma conta
              </button>
            ) : null}
          </div>

          <div className="divider">
            <span>ou</span>
          </div>

          <div className="google-login-wrapper">
            <GoogleLogin
              onSuccess={handleGoogleSuccess}
              onError={handleGoogleError}
              useOneTap
              theme={theme === 'dark' ? 'filled_black' : 'filled_blue'}
              shape="pill"
              text="signin_with"
              width="100%"
            />
          </div>
        </form>
      </section>
    </div>
  )
}
