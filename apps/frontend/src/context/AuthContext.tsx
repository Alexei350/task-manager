import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  useRef,
  useEffect,
  type PropsWithChildren,
} from 'react'
import {
  login as loginRequest,
  googleLogin as googleLoginRequest,
  setupAuth,
  type TokenReturn,
} from '../services/api'

type Session = TokenReturn

interface AuthContextValue {
  session: Session | null
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<void>
  loginWithGoogle: (token: string) => Promise<void>
  logout: () => void
}

const STORAGE_KEY = 'task-manager:session'

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

export { AuthContext }

function loadStoredSession(): Session | null {
  if (typeof localStorage === 'undefined') return null

  const stored = localStorage.getItem(STORAGE_KEY)
  if (!stored) return null

  try {
    return JSON.parse(stored) as Session
  } catch {
    return null
  }
}

type AuthProviderProps = PropsWithChildren<{
  initialSession?: Session | null
}>

export function AuthProvider({ children, initialSession }: AuthProviderProps) {
  const [session, setSession] = useState<Session | null>(
    initialSession ?? loadStoredSession(),
  )

  const sessionRef = useRef(session)
  useEffect(() => {
    sessionRef.current = session
  }, [session])

  useEffect(() => {
    setupAuth(
      () => sessionRef.current,
      (newSession) => {
        setSession(newSession)
        if (newSession) {
          localStorage.setItem(STORAGE_KEY, JSON.stringify(newSession))
        } else {
          localStorage.removeItem(STORAGE_KEY)
        }
      },
    )
  }, [])

  const logout = useCallback(() => {
    setSession(null)
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem(STORAGE_KEY)
    }
  }, [])

  const login = useCallback(
    async (email: string, password: string) => {
      const result = await loginRequest(email, password)
      setSession(result)

      if (typeof localStorage !== 'undefined') {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(result))
      }
    },
    [],
  )

  const loginWithGoogle = useCallback(
    async (token: string) => {
      const result = await googleLoginRequest(token)
      setSession(result)

      if (typeof localStorage !== 'undefined') {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(result))
      }
    },
    [],
  )

  const value = useMemo<AuthContextValue>(
    () => ({
      session,
      isAuthenticated: Boolean(session?.token),
      login,
      loginWithGoogle,
      logout,
    }),
    [login, loginWithGoogle, logout, session],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used within AuthProvider')
  }

  return context
}
