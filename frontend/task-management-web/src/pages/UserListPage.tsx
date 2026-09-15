import {
  Alert,
  Badge,
  Button,
  Card,
  Group,
  Loader,
  Modal,
  PasswordInput,
  ScrollArea,
  Select,
  SimpleGrid,
  Stack,
  Switch,
  Table,
  Text,
  TextInput,
} from '@mantine/core'
import { notifications } from '@mantine/notifications'
import dayjs from 'dayjs'
import { useState } from 'react'
import { Link } from 'react-router-dom'

import { PageIntro } from '../components/PageIntro'
import type { UserRole } from '../features/auth'
import {
  useActivateUserMutation,
  useCreateUserMutation,
  useDeactivateUserMutation,
  useGetUsersQuery,
  useUpdateUserMutation,
  useUpdateUserRoleMutation,
  type UserListItem,
} from '../features/users'

const roleOptions: Array<{ value: UserRole; label: string }> = [
  { value: 'Admin', label: 'Admin' },
  { value: 'ProjectManager', label: 'Project Manager' },
  { value: 'TeamMember', label: 'Team Member' },
]

type CreateUserFormState = {
  firstName: string
  lastName: string
  email: string
  password: string
  role: UserRole
  isActive: boolean
}

type EditUserFormState = {
  firstName: string
  lastName: string
  email: string
}

const defaultCreateUserForm = (): CreateUserFormState => ({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  role: 'TeamMember',
  isActive: true,
})

function getApiErrorMessage(error: unknown) {
  if (typeof error === 'object' && error !== null && 'data' in error) {
    const data = error.data as { message?: string; Message?: string; errors?: Record<string, string[]>; Errors?: Record<string, string[]> }
    const fieldErrors = Object.values(data.errors ?? data.Errors ?? {}).flat()

    if (fieldErrors.length > 0) {
      return fieldErrors[0]
    }

    return data.message ?? data.Message ?? 'Request failed.'
  }

  return 'Request failed.'
}

export function UserListPage() {
  const { data: users = [], isLoading, isFetching, isError, refetch } = useGetUsersQuery()
  const [createUser, { isLoading: isCreating }] = useCreateUserMutation()
  const [updateUser, { isLoading: isUpdatingUser }] = useUpdateUserMutation()
  const [updateUserRole, { isLoading: isUpdatingRole }] = useUpdateUserRoleMutation()
  const [activateUser, { isLoading: isActivating }] = useActivateUserMutation()
  const [deactivateUser, { isLoading: isDeactivating }] = useDeactivateUserMutation()
  const [searchTerm, setSearchTerm] = useState('')
  const [roleFilter, setRoleFilter] = useState<'All' | UserRole>('All')
  const [statusFilter, setStatusFilter] = useState<'All' | 'Active' | 'Inactive'>('All')
  const [createModalOpen, setCreateModalOpen] = useState(false)
  const [editingUser, setEditingUser] = useState<UserListItem | null>(null)
  const [pageError, setPageError] = useState<string | null>(null)
  const [createForm, setCreateForm] = useState<CreateUserFormState>(defaultCreateUserForm)
  const [editForm, setEditForm] = useState<EditUserFormState>({
    firstName: '',
    lastName: '',
    email: '',
  })

  const isMutating = isCreating || isUpdatingUser || isUpdatingRole || isActivating || isDeactivating

  const filteredUsers = users.filter((user) => {
    const matchesSearch =
      searchTerm.trim().length === 0
      || `${user.firstName} ${user.lastName} ${user.email}`
        .toLowerCase()
        .includes(searchTerm.trim().toLowerCase())
    const matchesRole = roleFilter === 'All' || user.role === roleFilter
    const matchesStatus =
      statusFilter === 'All'
      || (statusFilter === 'Active' && user.isActive)
      || (statusFilter === 'Inactive' && !user.isActive)

    return matchesSearch && matchesRole && matchesStatus
  })

  const totalUsers = users.length
  const activeUsers = users.filter((user) => user.isActive).length
  const adminUsers = users.filter((user) => user.role === 'Admin').length
  const projectManagers = users.filter((user) => user.role === 'ProjectManager').length

  const resetCreateForm = () => {
    setCreateForm(defaultCreateUserForm())
  }

  const openEditModal = (user: UserListItem) => {
    setEditingUser(user)
    setEditForm({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
    })
    setPageError(null)
  }

  const closeEditModal = () => {
    setEditingUser(null)
    setEditForm({
      firstName: '',
      lastName: '',
      email: '',
    })
  }

  const handleCreateUser = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setPageError(null)

    try {
      await createUser(createForm).unwrap()
      notifications.show({
        color: 'teal',
        message: 'User created successfully.',
      })
      setCreateModalOpen(false)
      resetCreateForm()
    } catch (error) {
      const message = getApiErrorMessage(error)
      setPageError(message)
      notifications.show({
        color: 'red',
        message,
      })
    }
  }

  const handleUpdateUser = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    if (!editingUser) {
      return
    }

    setPageError(null)

    try {
      await updateUser({ userId: editingUser.id, body: editForm }).unwrap()
      notifications.show({
        color: 'teal',
        message: 'User details updated.',
      })
      closeEditModal()
    } catch (error) {
      const message = getApiErrorMessage(error)
      setPageError(message)
      notifications.show({
        color: 'red',
        message,
      })
    }
  }

  const handleRoleChange = async (user: UserListItem, role: string | null) => {
    if (!role || role === user.role) {
      return
    }

    setPageError(null)

    try {
      await updateUserRole({ userId: user.id, body: { role: role as UserRole } }).unwrap()
      notifications.show({
        color: 'teal',
        message: `${user.firstName} ${user.lastName} is now ${role}.`,
      })
    } catch (error) {
      const message = getApiErrorMessage(error)
      setPageError(message)
      notifications.show({
        color: 'red',
        message,
      })
    }
  }

  const handleActivationToggle = async (user: UserListItem) => {
    setPageError(null)

    try {
      if (user.isActive) {
        await deactivateUser(user.id).unwrap()
      } else {
        await activateUser(user.id).unwrap()
      }

      notifications.show({
        color: 'teal',
        message: `${user.firstName} ${user.lastName} has been ${user.isActive ? 'deactivated' : 'activated'}.`,
      })
    } catch (error) {
      const message = getApiErrorMessage(error)
      setPageError(message)
      notifications.show({
        color: 'red',
        message,
      })
    }
  }

  return (
    <Stack gap="xl">
      <PageIntro
        eyebrow="Users"
        title="Team administration"
        description="Manage team access with admin-only controls for onboarding, profile updates, role changes, and account activation."
      />

      <SimpleGrid cols={{ base: 1, md: 2, xl: 4 }}>
        <Card padding="lg" radius="lg" shadow="sm" withBorder>
          <Text c="dimmed" fz="sm">Total users</Text>
          <Text fw={700} fz="2rem">{totalUsers}</Text>
        </Card>
        <Card padding="lg" radius="lg" shadow="sm" withBorder>
          <Text c="dimmed" fz="sm">Active accounts</Text>
          <Text fw={700} fz="2rem">{activeUsers}</Text>
        </Card>
        <Card padding="lg" radius="lg" shadow="sm" withBorder>
          <Text c="dimmed" fz="sm">Administrators</Text>
          <Text fw={700} fz="2rem">{adminUsers}</Text>
        </Card>
        <Card padding="lg" radius="lg" shadow="sm" withBorder>
          <Text c="dimmed" fz="sm">Project managers</Text>
          <Text fw={700} fz="2rem">{projectManagers}</Text>
        </Card>
      </SimpleGrid>

      <Card padding="xl" radius="lg" shadow="sm" withBorder>
        <Stack gap="lg">
          <Group justify="space-between" align="flex-end">
            <Stack gap={4}>
              <Text fw={700}>Directory controls</Text>
              <Text c="dimmed" size="sm">
                Filter the directory and perform the core admin actions already supported by the backend.
              </Text>
            </Stack>
            <Button color="teal" onClick={() => { setPageError(null); setCreateModalOpen(true); }}>
              Create user
            </Button>
          </Group>

          {pageError ? <Alert color="red">{pageError}</Alert> : null}

          <Group grow align="flex-end">
            <TextInput
              label="Search"
              placeholder="Search by name or email"
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.currentTarget.value)}
            />
            <Select
              label="Role"
              data={[{ value: 'All', label: 'All roles' }, ...roleOptions]}
              value={roleFilter}
              onChange={(value) => setRoleFilter((value as 'All' | UserRole | null) ?? 'All')}
            />
            <Select
              label="Status"
              data={[
                { value: 'All', label: 'All statuses' },
                { value: 'Active', label: 'Active' },
                { value: 'Inactive', label: 'Inactive' },
              ]}
              value={statusFilter}
              onChange={(value) => setStatusFilter((value as 'All' | 'Active' | 'Inactive' | null) ?? 'All')}
            />
          </Group>

          {isLoading || isFetching ? (
            <Group justify="center" py="xl">
              <Loader color="teal" />
            </Group>
          ) : isError ? (
            <Alert color="red">
              Unable to load users right now.
              <Button variant="subtle" color="red" onClick={() => void refetch()}>
                Retry
              </Button>
            </Alert>
          ) : filteredUsers.length === 0 ? (
            <Card padding="xl" radius="md" withBorder>
              <Text c="dimmed">No users match the current search and filters.</Text>
            </Card>
          ) : (
            <ScrollArea>
              <Table highlightOnHover verticalSpacing="md">
                <Table.Thead>
                  <Table.Tr>
                    <Table.Th>Name</Table.Th>
                    <Table.Th>Email</Table.Th>
                    <Table.Th>Role</Table.Th>
                    <Table.Th>Status</Table.Th>
                    <Table.Th>Created</Table.Th>
                    <Table.Th>Actions</Table.Th>
                  </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                  {filteredUsers.map((user) => (
                    <Table.Tr key={user.id}>
                      <Table.Td>
                        <Stack gap={2}>
                          <Text component={Link} to={`/users/${user.id}`} fw={600} td="none" c="teal.8">
                            {user.firstName} {user.lastName}
                          </Text>
                          <Text c="dimmed" fz="xs">{user.id}</Text>
                        </Stack>
                      </Table.Td>
                      <Table.Td>{user.email}</Table.Td>
                      <Table.Td>
                        <Select
                          aria-label={`Role for ${user.firstName} ${user.lastName}`}
                          data={roleOptions}
                          value={user.role}
                          disabled={isMutating}
                          onChange={(value) => {
                            void handleRoleChange(user, value)
                          }}
                        />
                      </Table.Td>
                      <Table.Td>
                        <Badge color={user.isActive ? 'teal' : 'gray'} variant="light">
                          {user.isActive ? 'Active' : 'Inactive'}
                        </Badge>
                      </Table.Td>
                      <Table.Td>{dayjs(user.createdAtUtc).format('DD MMM YYYY')}</Table.Td>
                      <Table.Td>
                        <Group gap="xs">
                          <Button component={Link} to={`/users/${user.id}`} variant="light" color="teal" disabled={isMutating}>
                            View
                          </Button>
                          <Button variant="light" color="dark" onClick={() => openEditModal(user)} disabled={isMutating}>
                            Edit
                          </Button>
                          <Button
                            variant="light"
                            color={user.isActive ? 'red' : 'teal'}
                            onClick={() => {
                              void handleActivationToggle(user)
                            }}
                            disabled={isMutating}
                          >
                            {user.isActive ? 'Deactivate' : 'Activate'}
                          </Button>
                        </Group>
                      </Table.Td>
                    </Table.Tr>
                  ))}
                </Table.Tbody>
              </Table>
            </ScrollArea>
          )}
        </Stack>
      </Card>

      <Modal
        opened={createModalOpen}
        onClose={() => {
          setCreateModalOpen(false)
          resetCreateForm()
        }}
        title="Create user"
        centered
      >
        <form onSubmit={handleCreateUser}>
          <Stack>
            <TextInput
              label="First name"
              value={createForm.firstName}
              onChange={(event) => {
                const { value } = event.currentTarget
                setCreateForm((current) => ({ ...current, firstName: value }))
              }}
              required
            />
            <TextInput
              label="Last name"
              value={createForm.lastName}
              onChange={(event) => {
                const { value } = event.currentTarget
                setCreateForm((current) => ({ ...current, lastName: value }))
              }}
              required
            />
            <TextInput
              label="Email"
              type="email"
              value={createForm.email}
              onChange={(event) => {
                const { value } = event.currentTarget
                setCreateForm((current) => ({ ...current, email: value }))
              }}
              required
            />
            <PasswordInput
              label="Temporary password"
              value={createForm.password}
              onChange={(event) => {
                const { value } = event.currentTarget
                setCreateForm((current) => ({ ...current, password: value }))
              }}
              required
            />
            <Select
              label="Role"
              data={roleOptions}
              value={createForm.role}
              onChange={(value) => setCreateForm((current) => ({ ...current, role: (value as UserRole | null) ?? current.role }))}
              allowDeselect={false}
            />
            <Switch
              label="Account is active"
              checked={createForm.isActive}
              onChange={(event) => {
                const { checked } = event.currentTarget
                setCreateForm((current) => ({ ...current, isActive: checked }))
              }}
            />
            <Group justify="flex-end">
              <Button variant="subtle" color="gray" onClick={() => setCreateModalOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" color="teal" loading={isCreating}>
                Save user
              </Button>
            </Group>
          </Stack>
        </form>
      </Modal>

      <Modal
        opened={editingUser !== null}
        onClose={closeEditModal}
        title={editingUser ? `Edit ${editingUser.firstName} ${editingUser.lastName}` : 'Edit user'}
        centered
      >
        <form onSubmit={handleUpdateUser}>
          <Stack>
            <TextInput
              label="First name"
              value={editForm.firstName}
              onChange={(event) => {
                const { value } = event.currentTarget
                setEditForm((current) => ({ ...current, firstName: value }))
              }}
              required
            />
            <TextInput
              label="Last name"
              value={editForm.lastName}
              onChange={(event) => {
                const { value } = event.currentTarget
                setEditForm((current) => ({ ...current, lastName: value }))
              }}
              required
            />
            <TextInput
              label="Email"
              type="email"
              value={editForm.email}
              onChange={(event) => {
                const { value } = event.currentTarget
                setEditForm((current) => ({ ...current, email: value }))
              }}
              required
            />
            <Group justify="flex-end">
              <Button variant="subtle" color="gray" onClick={closeEditModal}>
                Cancel
              </Button>
              <Button type="submit" color="teal" loading={isUpdatingUser}>
                Update user
              </Button>
            </Group>
          </Stack>
        </form>
      </Modal>
    </Stack>
  )
}