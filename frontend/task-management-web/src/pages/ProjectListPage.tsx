import { Card, Stack, Text } from '@mantine/core'

import { PageIntro } from '../components/PageIntro'

export function ProjectListPage() {
  return (
    <Stack gap="xl">
      <PageIntro
        eyebrow="Projects"
        title="Project workspace"
        description="Initial shell for project planning, ownership, deadlines, and progress tracking. Feature commands and queries will be wired in follow-up branches."
      />
      <Card padding="xl" radius="lg" shadow="sm" withBorder>
        <Text c="dimmed">
          Placeholder for project tables, filters, and create or update flows.
        </Text>
      </Card>
    </Stack>
  )
}