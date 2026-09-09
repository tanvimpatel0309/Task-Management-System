import { Card, Stack, Text } from '@mantine/core'

import { PageIntro } from '../components/PageIntro'

export function UserListPage() {
  return (
    <Stack gap="xl">
      <PageIntro
        eyebrow="Users"
        title="Team administration"
        description="Setup placeholder for user management pages that will eventually handle roles, activation, and assignment visibility through backend APIs."
      />
      <Card padding="xl" radius="lg" shadow="sm" withBorder>
        <Text c="dimmed">
          Placeholder for user directory, role filters, and admin actions.
        </Text>
      </Card>
    </Stack>
  )
}