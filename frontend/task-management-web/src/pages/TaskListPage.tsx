import { Card, SimpleGrid, Stack, Text } from '@mantine/core'

import { PageIntro } from '../components/PageIntro'

export function TaskListPage() {
  return (
    <Stack gap="xl">
      <PageIntro
        eyebrow="Tasks"
        title="Task execution board"
        description="Initial surface for task listing, filtering, assignment, and status tracking driven by CQRS commands and queries in the backend."
      />
      <SimpleGrid cols={{ base: 1, md: 2 }}>
        <Card padding="xl" radius="lg" shadow="sm" withBorder>
          <Text fw={700}>Command workflows</Text>
          <Text c="dimmed" mt="sm">
            Create, update, assign, reprioritize, and progress operations will call dedicated write endpoints.
          </Text>
        </Card>
        <Card padding="xl" radius="lg" shadow="sm" withBorder>
          <Text fw={700}>Query workflows</Text>
          <Text c="dimmed" mt="sm">
            Search, filter, sort, and pagination will be powered by read-only task query APIs.
          </Text>
        </Card>
      </SimpleGrid>
    </Stack>
  )
}