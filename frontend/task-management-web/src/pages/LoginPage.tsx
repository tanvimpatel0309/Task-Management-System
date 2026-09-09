import { Button, Card, PasswordInput, Stack, TextInput, Title } from '@mantine/core'

export function LoginPage() {
  return (
    <Stack
      align="center"
      justify="center"
      mih="100vh"
      px="md"
      style={{
        background:
          'linear-gradient(135deg, #0f766e 0%, #164e63 45%, #ecfeff 100%)',
      }}
    >
      <Card maw={460} padding="xl" radius="xl" shadow="xl" w="100%">
        <Stack>
          <Title order={1}>Sign in</Title>
          <TextInput label="Email" placeholder="name@company.com" />
          <PasswordInput label="Password" placeholder="Enter your password" />
          <Button size="md">Continue</Button>
        </Stack>
      </Card>
    </Stack>
  )
}