import {
  AppShell,
  Badge,
  Box,
  Burger,
  Button,
  Group,
  NavLink,
  Stack,
  Text,
  Title,
} from '@mantine/core'
import { useDisclosure } from '@mantine/hooks'
import { Link, Outlet, useLocation, useNavigate } from 'react-router-dom'

import { logout, selectCurrentUser } from '../features/auth'
import { useAppDispatch, useAppSelector } from '../hooks/redux'

const navigation = [
  { label: 'Dashboard', to: '/', roles: ['Admin', 'ProjectManager', 'TeamMember'] },
  { label: 'Users', to: '/users', roles: ['Admin'] },
  { label: 'Projects', to: '/projects', roles: ['Admin', 'ProjectManager'] },
  { label: 'Tasks', to: '/tasks', roles: ['Admin', 'ProjectManager', 'TeamMember'] },
]

export function AppShellLayout() {
  const [opened, { toggle }] = useDisclosure()
  const location = useLocation()
  const navigate = useNavigate()
  const dispatch = useAppDispatch()
  const user = useAppSelector(selectCurrentUser)

  const availableNavigation = navigation.filter((item) =>
    user ? item.roles.includes(user.role) : false,
  )

  const handleLogout = () => {
    dispatch(logout())
    navigate('/login', { replace: true })
  }

  return (
    <AppShell
      header={{ height: 72 }}
      navbar={{ width: 280, breakpoint: 'sm', collapsed: { mobile: !opened } }}
      padding="lg"
    >
      <AppShell.Header>
        <Group h="100%" justify="space-between" px="md">
          <Group>
            <Burger hiddenFrom="sm" opened={opened} onClick={toggle} size="sm" />
            <Box>
              <Text c="teal.7" fw={700} fz="xs" tt="uppercase">
                Auth Foundation
              </Text>
              <Title order={3}>TaskManagement</Title>
            </Box>
          </Group>
          <Group gap="sm">
            <Stack gap={0} align="flex-end">
              <Text fw={600} fz="sm">
                {user ? `${user.firstName} ${user.lastName}` : 'Authenticated user'}
              </Text>
              <Text c="dimmed" fz="xs">
                {user?.email ?? 'No active session'}
              </Text>
            </Stack>
            {user ? <Badge color="teal" variant="light">{user.role}</Badge> : null}
            <Button color="dark" variant="light" onClick={handleLogout}>
              Logout
            </Button>
          </Group>
        </Group>
      </AppShell.Header>

      <AppShell.Navbar p="md">
        <Stack gap="xs">
          {availableNavigation.map((item) => (
            <NavLink
              key={item.to}
              active={location.pathname === item.to}
              component={Link}
              label={item.label}
              to={item.to}
            />
          ))}
        </Stack>
      </AppShell.Navbar>

      <AppShell.Main>
        <Box
          mih="calc(100vh - 104px)"
          style={{
            background:
              'radial-gradient(circle at top left, rgba(18, 184, 134, 0.12), transparent 32%), linear-gradient(180deg, #f6fff9 0%, #f8fafc 58%, #eef7f3 100%)',
            borderRadius: '24px',
            padding: '24px',
          }}
        >
          <Outlet />
        </Box>
      </AppShell.Main>
    </AppShell>
  )
}