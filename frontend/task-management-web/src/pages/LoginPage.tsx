import {
  Alert,
  Button,
  Card,
  PasswordInput,
  Stack,
  Text,
  TextInput,
  Title,
} from '@mantine/core'
import { useState } from 'react'
import { Navigate, useLocation, useNavigate } from 'react-router-dom'

import { setAuthenticatedSession, useLoginMutation } from '../features/auth'
import { useAppDispatch, useAppSelector } from '../hooks/redux'

export function LoginPage() {
  const dispatch = useAppDispatch()
  const navigate = useNavigate()
  const location = useLocation()
  const accessToken = useAppSelector((state) => state.auth.accessToken)
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [login, { isLoading }] = useLoginMutation()

  if (accessToken) {
    return <Navigate to="/" replace />
  }

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setSubmitError(null)

    try {
      const response = await login({ email, password }).unwrap()
      dispatch(setAuthenticatedSession(response))

      const nextPath =
        typeof location.state === 'object' && location.state !== null && 'from' in location.state
          ? String((location.state as { from?: string }).from ?? '/')
          : '/'

      navigate(nextPath, { replace: true })
    } catch (error) {
      if (typeof error === 'object' && error !== null && 'data' in error) {
        const data = error.data as { message?: string; Message?: string }
        setSubmitError(data.message ?? data.Message ?? 'Unable to sign in.')
        return
      }

      setSubmitError('Unable to sign in.')
    }
  }

  return (
    <Stack
      align="center"
      justify="center"
      mih="100vh"
      px="md"
      style={{
        background:
          'linear-gradient(135deg, #0f766e 0%, #164e63 45%, #ecfeff 100%)',
      }}
    >
      <Card maw={460} padding="xl" radius="xl" shadow="xl" w="100%">
        <form onSubmit={handleSubmit}>
          <Stack>
            <Title order={1}>Sign in</Title>
            <Text c="dimmed" size="sm">
              Use your backend account to access dashboards, projects, tasks, and role-protected views.
            </Text>
            {submitError ? <Alert color="red">{submitError}</Alert> : null}
            <TextInput
              label="Email"
              placeholder="name@company.com"
              value={email}
              onChange={(event) => setEmail(event.currentTarget.value)}
              required
            />
            <PasswordInput
              label="Password"
              placeholder="Enter your password"
              value={password}
              onChange={(event) => setPassword(event.currentTarget.value)}
              required
            />
            <Button size="md" type="submit" loading={isLoading}>
              Continue
            </Button>
          </Stack>
        </form>
      </Card>
    </Stack>
  )
}