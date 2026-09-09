import { Card, Stack, Text } from '@mantine/core'
import { useParams } from 'react-router-dom'

import { PageIntro } from '../components/PageIntro'

export function TaskDetailsPage() {
  const { taskId } = useParams()

  return (
    <Stack gap="xl">
      <PageIntro
        eyebrow="Task Details"
        title={`Task ${taskId ?? 'Preview'}`}
        description="Reserved detail view for task metadata, comments, activity, and assignment history fetched from backend query endpoints."
      />
      <Card padding="xl" radius="lg" shadow="sm" withBorder>
        <Text c="dimmed">
          Placeholder for task details, comments, and activity log tabs.
        </Text>
      </Card>
    </Stack>
  )
}