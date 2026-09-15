import { Button, Card, Group, Stack, Text } from '@mantine/core'
import { Link } from 'react-router-dom'

import { PageIntro } from '../components/PageIntro'

export function AccessDeniedPage() {
  return (
    <Stack gap="xl" px="md" py="xl">
      <PageIntro
        eyebrow="Unauthorized"
        title="You do not have access to this area"
        description="Your account is authenticated, but your role does not allow this action or page."
      />

      <Card padding="xl" radius="lg" shadow="sm" withBorder>
        <Text c="dimmed" mb="lg">
          Return to the dashboard or sign in with an account that has the required permission.
        </Text>
        <Group>
          <Button component={Link} to="/" color="teal">
            Back to dashboard
          </Button>
          <Button component={Link} to="/login" variant="light" color="dark">
            Sign in again
          </Button>
        </Group>
      </Card>
    </Stack>
  )
}