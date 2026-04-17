import { useState, type FormEvent } from 'react'
import { GoogleLogin } from '@react-oauth/google'
import { registerUser } from '../services/api'
import { useAuth } from '../context/AuthContext'

type RegisterFormProps = {
  onSwitchToLogin?: () => void
  theme: 'light' | 'dark'
}

export default function RegisterForm({
  onSwitchToLogin,
  theme,
}: RegisterFormProps) {
  const { loginWithGoogle } = useAuth()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setError(null)
    setSuccess(null)
    setLoading(true)

    const payload = {
      name: name.trim(),
      email: email.trim(),
      password,
    }

    try {
      const message = await registerUser(payload)
      setSuccess(message)
      setPassword('')
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : 'Não foi possível concluir o registro.',
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
            <p className="eyebrow">Criar conta</p>
            <h2>Registre-se para começar</h2>
            <p className="muted">
              Preencha os dados abaixo para criar sua conta e começar a gerenciar
              suas tarefas.
            </p>
          </div>
        </div>

        <form className="login-form" onSubmit={handleSubmit}>
          <label className="field">
            <span>Nome completo</span>
            <input
              name="name"
              placeholder="Maria Souza"
              value={name}
              onChange={(event) => setName(event.target.value)}
              required
              autoComplete="name"
            />
          </label>

          <label className="field">
            <span>E-mail</span>
            <input
              type="email"
              name="email"
              placeholder="novo@empresa.com"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              required
              autoComplete="email"
            />
          </label>

          <label className="field">
            <span>Senha</span>
            <input
              type="password"
              name="password"
              placeholder="Crie uma senha forte"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
              autoComplete="new-password"
              minLength={6}
            />
          </label>

          {error ? <p className="feedback error">{error}</p> : null}
          {success ? <p className="feedback success">{success}</p> : null}

          <div className="actions stacked">
            <button type="submit" disabled={loading}>
              {loading ? 'Registrando...' : 'Registrar'}
            </button>
            {onSwitchToLogin ? (
              <button
                type="button"
                className="ghost"
                onClick={onSwitchToLogin}
                disabled={loading}
              >
                Já tenho uma conta
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
              text="signup_with"
              width="100%"
            />
          </div>
        </form>
      </section>
    </div>
  )
}
