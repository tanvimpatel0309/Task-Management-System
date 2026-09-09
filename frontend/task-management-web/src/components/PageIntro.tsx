import { Stack, Text, Title } from '@mantine/core'

type PageIntroProps = {
  eyebrow: string
  title: string
  description: string
}

export function PageIntro({ eyebrow, title, description }: PageIntroProps) {
  return (
    <Stack gap={4}>
      <Text c="teal.7" fw={700} fz="sm" tt="uppercase">
        {eyebrow}
      </Text>
      <Title order={1}>{title}</Title>
      <Text c="dimmed" maw={720}>
        {description}
      </Text>
    </Stack>
  )
}