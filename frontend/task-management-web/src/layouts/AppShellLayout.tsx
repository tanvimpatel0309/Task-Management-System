import {
  AppShell,
  Box,
  Burger,
  Group,
  NavLink,
  Stack,
  Text,
  Title,
} from '@mantine/core'
import { useDisclosure } from '@mantine/hooks'
import { Link, Outlet, useLocation } from 'react-router-dom'

const navigation = [
  { label: 'Dashboard', to: '/' },
  { label: 'Users', to: '/users' },
  { label: 'Projects', to: '/projects' },
  { label: 'Tasks', to: '/tasks' },
]

export function AppShellLayout() {
  const [opened, { toggle }] = useDisclosure()
  const location = useLocation()

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
                Monorepo Setup
              </Text>
              <Title order={3}>TaskManagement</Title>
            </Box>
          </Group>
          <Text c="dimmed" fz="sm">
            React frontend for the Task Management platform
          </Text>
        </Group>
      </AppShell.Header>

      <AppShell.Navbar p="md">
        <Stack gap="xs">
          {navigation.map((item) => (
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