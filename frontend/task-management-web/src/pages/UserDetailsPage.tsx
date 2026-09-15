import {
  Alert,
  Badge,
  Button,
  Card,
  Group,
  Loader,
  Select,
  Stack,
  Switch,
  Text,
  TextInput,
} from '@mantine/core'
import { notifications } from '@mantine/notifications'
import dayjs from 'dayjs'
import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'

import type { UserRole } from '../features/auth'
import {
  useActivateUserMutation,
  useDeactivateUserMutation,
  useGetUserByIdQuery,
  useUpdateUserMutation,
  useUpdateUserRoleMutation,
} from '../features/users'

const roleOptions: Array<{ value: UserRole; label: string }> = [
  { value: 'Admin', label: 'Admin' },
  { value: 'ProjectManager', label: 'Project Manager' },
  { value: 'TeamMember', label: 'Team Member' },
]

function getApiErrorMessage(error: unknown) {
  if (typeof error === 'object' && error !== null && 'data' in error) {
    const data = error.data as { message?: string; Message?: string; errors?: Record<string, string[]>; Errors?: Record<string, string[]> }
    const fieldErrors = Object.values(data.errors ?? data.Errors ?? {}).flat()
    return fieldErrors[0] ?? data.message ?? data.Message ?? 'Request failed.'
  }

  return 'Request failed.'
}

export function UserDetailsPage() {
  const { userId } = useParams()
  const { data: user, isLoading, isFetching, isError, refetch } = useGetUserByIdQuery(userId ?? '', {
    skip: !userId,
  })
  const [updateUser, { isLoading: isUpdatingUser }] = useUpdateUserMutation()
  const [updateUserRole, { isLoading: isUpdatingRole }] = useUpdateUserRoleMutation()
  const [activateUser, { isLoading: isActivating }] = useActivateUserMutation()
  const [deactivateUser, { isLoading: isDeactivating }] = useDeactivateUserMutation()
  const [formError, setFormError] = useState<string | null>(null)
  const [formState, setFormState] = useState({ firstName: '', lastName: '', email: '' })

  useEffect(() => {
    if (!user) {
      return
    }

    setFormState({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
    })
  }, [user])

  const isMutating = isUpdatingUser || isUpdatingRole || isActivating || isDeactivating

  const showError = (error: unknown) => {
    const message = getApiErrorMessage(error)
    setFormError(message)
    notifications.show({ color: 'red', message })
  }

  const showSuccess = (message: string) => {
    setFormError(null)
    notifications.show({ color: 'teal', message })
  }

  const handleSave = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    if (!userId) {
      return
    }

    try {
      await updateUser({ userId, body: formState }).unwrap()
      showSuccess('User details updated.')
    } catch (error) {
      showError(error)
    }
  }

  const handleRoleChange = async (value: string | null) => {
    if (!userId || !user || !value || value === user.role) {
      return
    }

    try {
      await updateUserRole({ userId, body: { role: value as UserRole } }).unwrap()
      showSuccess(`User role updated to ${value}.`)
    } catch (error) {
      showError(error)
    }
  }

  const handleActivationToggle = async (checked: boolean) => {
    if (!userId || !user || checked === user.isActive) {
      return
    }

    try {
      if (checked) {
        await activateUser(userId).unwrap()
        showSuccess('User activated.')
      } else {
        await deactivateUser(userId).unwrap()
        showSuccess('User deactivated.')
      }
    } catch (error) {
      showError(error)
    }
  }

  return (
    <Stack gap="xl">
      <Group justify="space-between" align="flex-start">
        <Stack gap={4}>
          <Text c="teal.7" fw={700} fz="xs" tt="uppercase">Users</Text>
          <Text fw={700} fz="2rem">{user ? `${user.firstName} ${user.lastName}` : 'User details'}</Text>
          <Text c="dimmed">Review and update account details, role, and activation status.</Text>
        </Stack>
        <Button component={Link} to="/users" variant="light" color="dark">
          Back to users
        </Button>
      </Group>

      {isLoading || isFetching ? (
        <Card padding="xl" radius="lg" shadow="sm" withBorder>
          <Group justify="center" py="xl">
            <Loader color="teal" />
          </Group>
        </Card>
      ) : isError || !user ? (
        <Alert color="red">
          Unable to load user details right now.
          <Button variant="subtle" color="red" onClick={() => void refetch()}>
            Retry
          </Button>
        </Alert>
      ) : (
        <>
          {formError ? <Alert color="red">{formError}</Alert> : null}

          <Group align="stretch" grow>
            <Card padding="xl" radius="lg" shadow="sm" withBorder>
              <Stack gap="md">
                <Text fw={700}>Account summary</Text>
                <Group>
                  <Badge color={user.isActive ? 'teal' : 'gray'} variant="light">
                    {user.isActive ? 'Active' : 'Inactive'}
                  </Badge>
                  <Badge color="blue" variant="light">{user.role}</Badge>
                </Group>
                <Text c="dimmed">Created {dayjs(user.createdAtUtc).format('DD MMM YYYY HH:mm')}</Text>
                <Text c="dimmed">
                  Last updated {user.updatedAtUtc ? dayjs(user.updatedAtUtc).format('DD MMM YYYY HH:mm') : 'Never'}
                </Text>
                <Switch
                  label="Account is active"
                  checked={user.isActive}
                  disabled={isMutating}
                  onChange={(event) => {
                    void handleActivationToggle(event.currentTarget.checked)
                  }}
                />
                <Select
                  label="Role"
                  data={roleOptions}
                  value={user.role}
                  disabled={isMutating}
                  onChange={(value) => {
                    void handleRoleChange(value)
                  }}
                />
              </Stack>
            </Card>

            <Card padding="xl" radius="lg" shadow="sm" withBorder>
              <form onSubmit={handleSave}>
                <Stack>
                  <Text fw={700}>Profile details</Text>
                  <TextInput
                    label="First name"
                    value={formState.firstName}
                    onChange={(event) => {
                      const { value } = event.currentTarget
                      setFormState((current) => ({ ...current, firstName: value }))
                    }}
                    required
                  />
                  <TextInput
                    label="Last name"
                    value={formState.lastName}
                    onChange={(event) => {
                      const { value } = event.currentTarget
                      setFormState((current) => ({ ...current, lastName: value }))
                    }}
                    required
                  />
                  <TextInput
                    label="Email"
                    type="email"
                    value={formState.email}
                    onChange={(event) => {
                      const { value } = event.currentTarget
                      setFormState((current) => ({ ...current, email: value }))
                    }}
                    required
                  />
                  <Group justify="flex-end">
                    <Button type="submit" color="teal" loading={isUpdatingUser}>
                      Save changes
                    </Button>
                  </Group>
                </Stack>
              </form>
            </Card>
          </Group>
        </>
      )}
    </Stack>
  )
}