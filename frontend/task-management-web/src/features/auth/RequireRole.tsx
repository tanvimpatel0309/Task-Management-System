import type { ReactNode } from 'react'
import { Navigate, useLocation } from 'react-router-dom'

import { useAppSelector } from '../../hooks/redux'
import type { UserRole } from './authTypes'

type RequireRoleProps = {
  allowedRoles: UserRole[]
  children: ReactNode
}

export function RequireRole({ allowedRoles, children }: RequireRoleProps) {
  const location = useLocation()
  const role = useAppSelector((state) => state.auth.user?.role)

  if (!role) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  }

  if (!allowedRoles.includes(role)) {
    return <Navigate to="/unauthorized" replace />
  }

  return <>{children}</>
}