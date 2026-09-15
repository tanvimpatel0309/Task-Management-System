import type { UserRole } from '../auth'

export type UserListItem = {
  id: string
  firstName: string
  lastName: string
  email: string
  role: UserRole
  isActive: boolean
  createdAtUtc: string
}

export type UserDetails = UserListItem & {
  updatedAtUtc: string | null
}

export type CreateUserRequest = {
  firstName: string
  lastName: string
  email: string
  password: string
  role: UserRole
  isActive: boolean
}

export type UpdateUserRequest = {
  firstName: string
  lastName: string
  email: string
}

export type UpdateUserRoleRequest = {
  role: UserRole
}