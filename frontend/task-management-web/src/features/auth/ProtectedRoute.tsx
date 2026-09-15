import { Center, Loader, Stack, Text } from '@mantine/core'
import { useEffect, type ReactNode } from 'react'
import { Navigate, useLocation } from 'react-router-dom'

import { useAppDispatch, useAppSelector } from '../../hooks/redux'
import { setCurrentUser } from './authSlice'
import { useGetCurrentUserQuery } from './authApi'

type ProtectedRouteProps = {
  children: ReactNode
}

export function ProtectedRoute({ children }: ProtectedRouteProps) {
  const dispatch = useAppDispatch()
  const location = useLocation()
  const accessToken = useAppSelector((state) => state.auth.accessToken)
  const currentUser = useAppSelector((state) => state.auth.user)
  const { data, isLoading, isFetching, isError } = useGetCurrentUserQuery(undefined, {
    skip: !accessToken,
  })

  useEffect(() => {
    if (data) {
      dispatch(setCurrentUser(data))
    }
  }, [data, dispatch])

  if (!accessToken) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  }

  if (isLoading || isFetching) {
    return (
      <Center mih="100vh">
        <Stack align="center" gap="sm">
          <Loader color="teal" />
          <Text c="dimmed">Verifying your session...</Text>
        </Stack>
      </Center>
    )
  }

  if (isError || !currentUser) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  }

  return <>{children}</>
}