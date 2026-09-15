import {
  Alert,
  Badge,
  Button,
  Card,
  Group,
  Loader,
  Modal,
  NumberInput,
  Pagination,
  Select,
  SimpleGrid,
  Stack,
  Switch,
  Table,
  Text,
  Textarea,
  TextInput,
} from '@mantine/core'
import { notifications } from '@mantine/notifications'
import dayjs from 'dayjs'
import { Link } from 'react-router-dom'
import { useState } from 'react'

import { PageIntro } from '../components/PageIntro'
import { useAppSelector } from '../hooks/redux'
import { useGetProjectsQuery } from '../features/projects'
import {
  useCreateTaskMutation,
  useGetAssignableUsersQuery,
  useGetTasksQuery,
  type TaskPriority,
  type TaskStatus,
} from '../features/tasks'

const taskStatusOptions: Array<{ value: TaskStatus; label: string }> = [
  { value: 'Todo', label: 'Todo' },
  { value: 'InProgress', label: 'In progress' },
  { value: 'OnHold', label: 'On hold' },
  { value: 'Completed', label: 'Completed' },
  { value: 'Cancelled', label: 'Cancelled' },
]

const taskPriorityOptions: Array<{ value: TaskPriority; label: string }> = [
  { value: 'Low', label: 'Low' },
  { value: 'Medium', label: 'Medium' },
  { value: 'High', label: 'High' },
  { value: 'Critical', label: 'Critical' },
]

type CreateTaskFormState = {
  title: string
  description: string
  status: TaskStatus
  priority: TaskPriority
  progressPercentage: number
  startDateUtc: string
  dueDateUtc: string
  projectId: string
  assignedUserId: string
}

const defaultCreateTaskForm = (): CreateTaskFormState => ({
  title: '',
  description: '',
  status: 'Todo',
  priority: 'Medium',
  progressPercentage: 0,
  startDateUtc: '',
  dueDateUtc: '',
  projectId: '',
  assignedUserId: '',
})

function getApiErrorMessage(error: unknown) {
  if (typeof error === 'object' && error !== null && 'data' in error) {
    const data = error.data as { message?: string; Message?: string; errors?: Record<string, string[]>; Errors?: Record<string, string[]> }
    const fieldErrors = Object.values(data.errors ?? data.Errors ?? {}).flat()
    return fieldErrors[0] ?? data.message ?? data.Message ?? 'Request failed.'
  }

  return 'Request failed.'
}

export function TaskListPage() {
  const currentUserRole = useAppSelector((state) => state.auth.user?.role)
  const canManageTasks = currentUserRole === 'Admin' || currentUserRole === 'ProjectManager'
  const [searchTerm, setSearchTerm] = useState('')
  const [projectId, setProjectId] = useState('')
  const [assignedUserId, setAssignedUserId] = useState('')
  const [status, setStatus] = useState<TaskStatus | ''>('')
  const [priority, setPriority] = useState<TaskPriority | ''>('')
  const [overdueOnly, setOverdueOnly] = useState(false)
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [sortBy, setSortBy] = useState<'title' | 'status' | 'priority' | 'dueDateUtc' | 'createdAtUtc' | ''>('dueDateUtc')
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc' | ''>('asc')
  const [createModalOpen, setCreateModalOpen] = useState(false)
  const [pageError, setPageError] = useState<string | null>(null)
  const [createForm, setCreateForm] = useState<CreateTaskFormState>(defaultCreateTaskForm)
  const [createTask, { isLoading: isCreatingTask }] = useCreateTaskMutation()
  const { data: projects = [] } = useGetProjectsQuery(undefined, { skip: !canManageTasks })
  const { data: assignableUsers = [] } = useGetAssignableUsersQuery(undefined, { skip: !canManageTasks })

  const { data, isLoading, isFetching, isError, refetch } = useGetTasksQuery({
    searchTerm: searchTerm || undefined,
    projectId: projectId || undefined,
    assignedUserId: assignedUserId || undefined,
    status,
    priority,
    overdueOnly: overdueOnly || undefined,
    pageNumber,
    pageSize,
    sortBy,
    sortDirection,
  })

  const tasks = data?.items ?? []
  const totalCount = data?.totalCount ?? 0
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize))
  const overdueCount = tasks.filter((task) => task.isOverdue).length
  const completedCount = tasks.filter((task) => task.status === 'Completed').length
  const projectOptions = [{ value: '', label: 'All projects' }, ...projects.map((project) => ({ value: project.id, label: project.name }))]
  const createProjectOptions = projects.map((project) => ({ value: project.id, label: project.name }))
  const assigneeFilterOptions = [{ value: '', label: 'All assignees' }, ...assignableUsers.map((user) => ({ value: user.id, label: `${user.firstName} ${user.lastName}` }))]
  const assigneeCreateOptions = [{ value: '', label: 'Unassigned' }, ...assignableUsers.map((user) => ({ value: user.id, label: `${user.firstName} ${user.lastName}` }))]

  const handleCreateTask = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setPageError(null)

    try {
      await createTask({
        title: createForm.title,
        description: createForm.description.trim() ? createForm.description : null,
        status: createForm.status,
        priority: createForm.priority,
        progressPercentage: createForm.progressPercentage,
        startDateUtc: createForm.startDateUtc ? new Date(createForm.startDateUtc).toISOString() : null,
        dueDateUtc: createForm.dueDateUtc ? new Date(createForm.dueDateUtc).toISOString() : null,
        projectId: createForm.projectId,
        assignedUserId: createForm.assignedUserId || null,
      }).unwrap()

      notifications.show({ color: 'teal', message: 'Task created successfully.' })
      setCreateModalOpen(false)
      setCreateForm(defaultCreateTaskForm())
    } catch (error) {
      const message = getApiErrorMessage(error)
      setPageError(message)
      notifications.show({ color: 'red', message })
    }
  }

  return (
    <Stack gap="xl">
      <PageIntro
        eyebrow="Tasks"
        title="Task execution board"
        description="Review work in progress with live filters, due-date visibility, and direct navigation into task detail records."
      />

      <SimpleGrid cols={{ base: 1, md: 3 }}>
        <Card padding="lg" radius="lg" shadow="sm" withBorder>
          <Text c="dimmed" fz="sm">Results</Text>
          <Text fw={700} fz="2rem">{totalCount}</Text>
        </Card>
        <Card padding="lg" radius="lg" shadow="sm" withBorder>
          <Text c="dimmed" fz="sm">Overdue on page</Text>
          <Text fw={700} fz="2rem">{overdueCount}</Text>
        </Card>
        <Card padding="lg" radius="lg" shadow="sm" withBorder>
          <Text c="dimmed" fz="sm">Completed on page</Text>
          <Text fw={700} fz="2rem">{completedCount}</Text>
        </Card>
      </SimpleGrid>

      <Card padding="xl" radius="lg" shadow="sm" withBorder>
        <Stack gap="lg">
          <Group justify="space-between" align="flex-end">
            <Stack gap={4}>
              <Text fw={700}>Task filters</Text>
              <Text c="dimmed" size="sm">Search existing work and create new tasks when your role allows it.</Text>
            </Stack>
            {canManageTasks ? (
              <Button color="teal" onClick={() => setCreateModalOpen(true)}>
                Create task
              </Button>
            ) : null}
          </Group>

          {pageError ? <Alert color="red">{pageError}</Alert> : null}

          <Group grow align="flex-end">
            <TextInput
              label="Search"
              placeholder="Search title, description, project, or assignee"
              value={searchTerm}
              onChange={(event) => {
                setSearchTerm(event.currentTarget.value)
                setPageNumber(1)
              }}
            />
            <Select
              label="Project"
              data={projectOptions}
              value={projectId}
              onChange={(value) => {
                setProjectId(value ?? '')
                setPageNumber(1)
              }}
            />
            <Select
              label="Assignee"
              data={assigneeFilterOptions}
              value={assignedUserId}
              onChange={(value) => {
                setAssignedUserId(value ?? '')
                setPageNumber(1)
              }}
            />
          </Group>

          <Group grow align="flex-end">
            <Select
              label="Status"
              data={[{ value: '', label: 'All statuses' }, ...taskStatusOptions]}
              value={status}
              onChange={(value) => {
                setStatus((value as TaskStatus | '' | null) ?? '')
                setPageNumber(1)
              }}
            />
            <Select
              label="Priority"
              data={[{ value: '', label: 'All priorities' }, ...taskPriorityOptions]}
              value={priority}
              onChange={(value) => {
                setPriority((value as TaskPriority | '' | null) ?? '')
                setPageNumber(1)
              }}
            />
          </Group>

          <Group grow align="flex-end">
            <Select
              label="Sort by"
              data={[
                { value: 'dueDateUtc', label: 'Due date' },
                { value: 'createdAtUtc', label: 'Created date' },
                { value: 'priority', label: 'Priority' },
                { value: 'status', label: 'Status' },
                { value: 'title', label: 'Title' },
              ]}
              value={sortBy}
              onChange={(value) => setSortBy((value as 'title' | 'status' | 'priority' | 'dueDateUtc' | 'createdAtUtc' | '' | null) ?? '')}
            />
            <Select
              label="Direction"
              data={[
                { value: 'asc', label: 'Ascending' },
                { value: 'desc', label: 'Descending' },
              ]}
              value={sortDirection}
              onChange={(value) => setSortDirection((value as 'asc' | 'desc' | '' | null) ?? '')}
            />
            <Select
              label="Page size"
              data={[
                { value: '10', label: '10' },
                { value: '20', label: '20' },
                { value: '50', label: '50' },
              ]}
              value={String(pageSize)}
              onChange={(value) => {
                setPageSize(Number(value ?? '10'))
                setPageNumber(1)
              }}
            />
          </Group>

          <Switch
            label="Show overdue tasks only"
            checked={overdueOnly}
            onChange={(event) => {
              setOverdueOnly(event.currentTarget.checked)
              setPageNumber(1)
            }}
          />

          {isLoading || isFetching ? (
            <Group justify="center" py="xl">
              <Loader color="teal" />
            </Group>
          ) : isError ? (
            <Alert color="red">
              Unable to load tasks right now.
              <Button variant="subtle" color="red" onClick={() => void refetch()}>
                Retry
              </Button>
            </Alert>
          ) : tasks.length === 0 ? (
            <Card padding="xl" radius="md" withBorder>
              <Text c="dimmed">No tasks match the current query.</Text>
            </Card>
          ) : (
            <Stack gap="lg">
              <Table highlightOnHover verticalSpacing="md">
                <Table.Thead>
                  <Table.Tr>
                    <Table.Th>Task</Table.Th>
                    <Table.Th>Project</Table.Th>
                    <Table.Th>Assignee</Table.Th>
                    <Table.Th>Status</Table.Th>
                    <Table.Th>Priority</Table.Th>
                    <Table.Th>Progress</Table.Th>
                    <Table.Th>Due</Table.Th>
                  </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                  {tasks.map((task) => (
                    <Table.Tr key={task.id}>
                      <Table.Td>
                        <Stack gap={2}>
                          <Text component={Link} to={`/tasks/${task.id}`} fw={600} td="none" c="teal.8">
                            {task.title}
                          </Text>
                          <Text c="dimmed" fz="sm">{task.description ?? 'No description provided.'}</Text>
                        </Stack>
                      </Table.Td>
                      <Table.Td>{task.projectName}</Table.Td>
                      <Table.Td>{task.assignedUserName || 'Unassigned'}</Table.Td>
                      <Table.Td>
                        <Badge color={task.status === 'Completed' ? 'teal' : 'blue'} variant="light">
                          {task.status}
                        </Badge>
                      </Table.Td>
                      <Table.Td>
                        <Badge color={task.priority === 'Critical' ? 'red' : task.priority === 'High' ? 'orange' : 'gray'} variant="light">
                          {task.priority}
                        </Badge>
                      </Table.Td>
                      <Table.Td>{task.progressPercentage}%</Table.Td>
                      <Table.Td>
                        <Text c={task.isOverdue ? 'red' : undefined}>
                          {task.dueDateUtc ? dayjs(task.dueDateUtc).format('DD MMM YYYY') : 'No due date'}
                        </Text>
                      </Table.Td>
                    </Table.Tr>
                  ))}
                </Table.Tbody>
              </Table>

              <Group justify="space-between">
                <Text c="dimmed" size="sm">
                  Page {data?.pageNumber ?? 1} of {totalPages}
                </Text>
                <Pagination value={pageNumber} onChange={setPageNumber} total={totalPages} color="teal" />
              </Group>
            </Stack>
          )}
        </Stack>
      </Card>

      <Modal opened={createModalOpen} onClose={() => setCreateModalOpen(false)} title="Create task" centered>
        <form onSubmit={handleCreateTask}>
          <Stack>
            <TextInput
              label="Title"
              value={createForm.title}
              onChange={(event) => {
                const { value } = event.currentTarget
                setCreateForm((current) => ({ ...current, title: value }))
              }}
              required
            />
            <Textarea
              label="Description"
              value={createForm.description}
              onChange={(event) => {
                const { value } = event.currentTarget
                setCreateForm((current) => ({ ...current, description: value }))
              }}
              minRows={3}
            />
            <Select
              label="Project"
              data={createProjectOptions}
              value={createForm.projectId}
              onChange={(value) => setCreateForm((current) => ({ ...current, projectId: value ?? '' }))}
              required
            />
            <Group grow>
              <Select
                label="Status"
                data={taskStatusOptions}
                value={createForm.status}
                onChange={(value) => setCreateForm((current) => ({ ...current, status: (value as TaskStatus | null) ?? current.status }))}
                allowDeselect={false}
              />
              <Select
                label="Priority"
                data={taskPriorityOptions}
                value={createForm.priority}
                onChange={(value) => setCreateForm((current) => ({ ...current, priority: (value as TaskPriority | null) ?? current.priority }))}
                allowDeselect={false}
              />
            </Group>
            <Group grow>
              <TextInput
                label="Start date"
                type="date"
                value={createForm.startDateUtc}
                onChange={(event) => {
                  const { value } = event.currentTarget
                  setCreateForm((current) => ({ ...current, startDateUtc: value }))
                }}
              />
              <TextInput
                label="Due date"
                type="date"
                value={createForm.dueDateUtc}
                onChange={(event) => {
                  const { value } = event.currentTarget
                  setCreateForm((current) => ({ ...current, dueDateUtc: value }))
                }}
              />
            </Group>
            <Group grow>
              <NumberInput
                label="Initial progress"
                min={0}
                max={100}
                value={createForm.progressPercentage}
                onChange={(value) => setCreateForm((current) => ({ ...current, progressPercentage: typeof value === 'number' && !Number.isNaN(value) ? value : 0 }))}
                suffix="%"
              />
              <Select
                label="Assignee"
                data={assigneeCreateOptions}
                value={createForm.assignedUserId}
                onChange={(value) => setCreateForm((current) => ({ ...current, assignedUserId: value ?? '' }))}
              />
            </Group>
            <Group justify="flex-end">
              <Button variant="subtle" color="gray" onClick={() => setCreateModalOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" color="teal" loading={isCreatingTask}>
                Save task
              </Button>
            </Group>
          </Stack>
        </form>
      </Modal>
    </Stack>
  )
}