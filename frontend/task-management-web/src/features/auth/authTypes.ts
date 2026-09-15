export type UserRole = 'Admin' | 'ProjectManager' | 'TeamMember'

export type AuthenticatedUser = {
  id: string
  firstName: string
  lastName: string
  email: string
  role: UserRole
}

export type AuthenticationResponse = {
  accessToken: string
  tokenType: string
  expiresAtUtc: string
  user: AuthenticatedUser
}

export type LoginRequest = {
  email: string
  password: string
}

export type AuthState = {
  accessToken: string | null
  expiresAtUtc: string | null
  user: AuthenticatedUser | null
}