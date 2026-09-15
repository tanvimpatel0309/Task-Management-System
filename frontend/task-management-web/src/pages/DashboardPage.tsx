import { Card, Grid, SimpleGrid, Stack, Text } from '@mantine/core'

import { PageIntro } from '../components/PageIntro'

const metrics = [
  'Projects in planning',
  'Tasks due soon',
  'High priority work',
  'Team activity snapshots',
]

export function DashboardPage() {
  return (
    <Stack gap="xl">
      <PageIntro
        eyebrow="Dashboard"
        title="Delivery visibility at a glance"
        description="This setup page reserves the future dashboard surface for summary cards, workload insights, and task health metrics powered by read-only query APIs."
      />

      <SimpleGrid cols={{ base: 1, md: 2, xl: 4 }}>
        {metrics.map((metric) => (
          <Card key={metric} padding="lg" radius="lg" shadow="sm" withBorder>
            <Text fw={600}>{metric}</Text>
            <Text c="dimmed" mt="sm" size="sm">
              Placeholder for CQRS query-backed dashboard widgets.
            </Text>
          </Card>
        ))}
      </SimpleGrid>

      <Grid>
        <Grid.Col span={{ base: 12, lg: 7 }}>
          <Card padding="xl" radius="lg" shadow="sm" withBorder>
            <Text fw={700} mb="sm">
              Read model boundary
            </Text>
            <Text c="dimmed">
              Dashboard widgets will consume TaskManagement.Queries endpoints only. No write logic belongs in this frontend setup layer.
            </Text>
          </Card>
        </Grid.Col>
        <Grid.Col span={{ base: 12, lg: 5 }}>
          <Card padding="xl" radius="lg" shadow="sm" withBorder>
            <Text fw={700} mb="sm">
              API contract
            </Text>
            <Text c="dimmed">
              Configure the backend base URL through the frontend environment file before wiring authenticated requests.
            </Text>
          </Card>
        </Grid.Col>
      </Grid>
    </Stack>
  )
}