import {
  Alert,
  Badge,
  Button,
  Card,
  Group,
  Loader,
  Modal,
  Select,
  SimpleGrid,
  Stack,
  Table,
  Text,
  TextInput,
  Textarea,
} from '@mantine/core'
import { notifications } from '@mantine/notifications'
import dayjs from 'dayjs'
import { useState } from 'react'

import { PageIntro } from '../components/PageIntro'
import {
  useCreateProjectMutation,
  useDeleteProjectMutation,
  useGetProjectsQuery,
  useUpdateProjectMutation,
  useUpdateProjectStatusMutation,
  type ProjectListItem,
  type ProjectStatus,
} from '../features/projects'

const projectStatusOptions: Array<{ value: ProjectStatus; label: string }> = [
  { value: 'Planning', label: 'Planning' },
  { value: 'Active', label: 'Active' },
  { value: 'Completed', label: 'Completed' },
  { value: 'Archived', label: 'Archived' },
]

type CreateProjectFormState = {
  name: string
  description: string
  startDateUtc: string
  endDateUtc: string
  status: ProjectStatus
}

const defaultCreateProjectForm = (): CreateProjectFormState => ({
  name: '',
  description: '',
  startDateUtc: dayjs().format('YYYY-MM-DD'),
  endDateUtc: '',
  status: 'Planning',
})

function getApiErrorMessage(error: unknown) {
  if (typeof error === 'object' && error !== null && 'data' in error) {
    const data = error.data as { message?: string; Message?: string; errors?: Record<string, string[]>; Errors?: Record<string, string[]> }
    const fieldErrors = Object.values(data.errors ?? data.Errors ?? {}).flat()
    return fieldErrors[0] ?? data.message ?? data.Message ?? 'Request failed.'
  }

  return 'Request failed.'
}

export function ProjectListPage() {
  const { data: projects = [], isLoading, isFetching, isError, refetch } = useGetProjectsQuery()
  const [createProject, { isLoading: isCreating }] = useCreateProjectMutation()
  const [updateProject, { isLoading: isUpdatingProject }] = useUpdateProjectMutation()
  const [updateProjectStatus, { isLoading: isUpdatingStatus }] = useUpdateProjectStatusMutation()
  const [deleteProject, { isLoading: isDeletingProject }] = useDeleteProjectMutation()
  const [createModalOpen, setCreateModalOpen] = useState(false)
  const [editingProject, setEditingProject] = useState<ProjectListItem | null>(null)
  const [pageError, setPageError] = useState<string | null>(null)
  const [searchTerm, setSearchTerm] = useState('')
  const [statusFilter, setStatusFilter] = useState<'All' | ProjectStatus>('All')
  const [createForm, setCreateForm] = useState<CreateProjectFormState>(defaultCreateProjectForm)
  const [editForm, setEditForm] = useState<CreateProjectFormState>(defaultCreateProjectForm)

  const filteredProjects = projects.filter((project) => {
    const matchesSearch =
      searchTerm.trim().length === 0
      || `${project.name} ${project.description ?? ''} ${project.createdByUserName}`
        .toLowerCase()
        .includes(searchTerm.trim().toLowerCase())

    return matchesSearch && (statusFilter === 'All' || project.status === statusFilter)
  })

  const activeProjects = projects.filter((project) => project.status === 'Active').length
  const planningProjects = projects.filter((project) => project.status === 'Planning').length
  const totalTasks = projects.reduce((sum, project) => sum + project.taskCount, 0)

  const openEditModal = (project: ProjectListItem) => {
    setEditingProject(project)
    setEditForm({
      name: project.name,
      description: project.description ?? '',
      startDateUtc: dayjs(project.startDateUtc).format('YYYY-MM-DD'),
      endDateUtc: project.endDateUtc ? dayjs(project.endDateUtc).format('YYYY-MM-DD') : '',
      status: project.status,
    })
  }

  const closeEditModal = () => {
    setEditingProject(null)
    setEditForm(defaultCreateProjectForm())
  }

  const handleCreateProject = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setPageError(null)

    try {
      await createProject({
        name: createForm.name,
        description: createForm.description.trim() ? createForm.description : null,
        startDateUtc: new Date(createForm.startDateUtc).toISOString(),
        endDateUtc: createForm.endDateUtc ? new Date(createForm.endDateUtc).toISOString() : null,
        status: createForm.status,
      }).unwrap()

      notifications.show({ color: 'teal', message: 'Project created successfully.' })
      setCreateModalOpen(false)
      setCreateForm(defaultCreateProjectForm())
    } catch (error) {
      const message = getApiErrorMessage(error)
      setPageError(message)
      notifications.show({ color: 'red', message })
    }
  }

  const handleStatusChange = async (project: ProjectListItem, nextStatus: string | null) => {
    if (!nextStatus || nextStatus === project.status) {
      return
    }

    setPageError(null)

    try {
      await updateProjectStatus({
        projectId: project.id,
        body: { status: nextStatus as ProjectStatus },
      }).unwrap()
      notifications.show({ color: 'teal', message: `${project.name} moved to ${nextStatus}.` })
    } catch (error) {
      const message = getApiErrorMessage(error)
      setPageError(message)
      notifications.show({ color: 'red', message })
    }
  }

  const handleUpdateProject = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    if (!editingProject) {
      return
    }

    setPageError(null)

    try {
      await updateProject({
        projectId: editingProject.id,
        body: {
          name: editForm.name,
          description: editForm.description.trim() ? editForm.description : null,
          startDateUtc: new Date(editForm.startDateUtc).toISOString(),
          endDateUtc: editForm.endDateUtc ? new Date(editForm.endDateUtc).toISOString() : null,
        },
      }).unwrap()
      notifications.show({ color: 'teal', message: 'Project updated successfully.' })
      closeEditModal()
    } catch (error) {
      const message = getApiErrorMessage(error)
      setPageError(message)
      notifications.show({ color: 'red', message })
    }
  }

  const handleDeleteProject = async (project: ProjectListItem) => {
    const confirmed = window.confirm(`Delete project "${project.name}"?`)
    if (!confirmed) {
      return
    }

    setPageError(null)

    try {
      await deleteProject(project.id).unwrap()
      notifications.show({ color: 'teal', message: 'Project deleted successfully.' })
    } catch (error) {
      const message = getApiErrorMessage(error)
      setPageError(message)
      notifications.show({ color: 'red', message })
    }
  }

  return (
    <Stack gap="xl">
      <PageIntro
        eyebrow="Projects"
        title="Project workspace"
        description="Track project delivery with status transitions, ownership visibility, and a live view of work volume across initiatives."
      />

      <SimpleGrid cols={{ base: 1, md: 3 }}>
        <Card padding="lg" radius="lg" shadow="sm" withBorder>
          <Text c="dimmed" fz="sm">Active projects</Text>
          <Text fw={700} fz="2rem">{activeProjects}</Text>
        </Card>
        <Card padding="lg" radius="lg" shadow="sm" withBorder>
          <Text c="dimmed" fz="sm">Planning projects</Text>
          <Text fw={700} fz="2rem">{planningProjects}</Text>
        </Card>
        <Card padding="lg" radius="lg" shadow="sm" withBorder>
          <Text c="dimmed" fz="sm">Tracked tasks</Text>
          <Text fw={700} fz="2rem">{totalTasks}</Text>
        </Card>
      </SimpleGrid>

      <Card padding="xl" radius="lg" shadow="sm" withBorder>
        <Stack gap="lg">
          <Group justify="space-between" align="flex-end">
            <Stack gap={4}>
              <Text fw={700}>Project portfolio</Text>
              <Text c="dimmed" size="sm">
                Browse active work, review owners, and adjust project status in place.
              </Text>
            </Stack>
            <Button color="teal" onClick={() => { setPageError(null); setCreateModalOpen(true) }}>
              Create project
            </Button>
          </Group>

          {pageError ? <Alert color="red">{pageError}</Alert> : null}

          <Group grow align="flex-end">
            <TextInput
              label="Search"
              placeholder="Search name, description, or owner"
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.currentTarget.value)}
            />
            <Select
              label="Status"
              data={[{ value: 'All', label: 'All statuses' }, ...projectStatusOptions]}
              value={statusFilter}
              onChange={(value) => setStatusFilter((value as 'All' | ProjectStatus | null) ?? 'All')}
            />
          </Group>

          {isLoading || isFetching ? (
            <Group justify="center" py="xl">
              <Loader color="teal" />
            </Group>
          ) : isError ? (
            <Alert color="red">
              Unable to load projects right now.
              <Button variant="subtle" color="red" onClick={() => void refetch()}>
                Retry
              </Button>
            </Alert>
          ) : filteredProjects.length === 0 ? (
            <Card padding="xl" radius="md" withBorder>
              <Text c="dimmed">No projects match the current search and filters.</Text>
            </Card>
          ) : (
            <Table highlightOnHover verticalSpacing="md">
              <Table.Thead>
                <Table.Tr>
                  <Table.Th>Project</Table.Th>
                  <Table.Th>Owner</Table.Th>
                  <Table.Th>Schedule</Table.Th>
                  <Table.Th>Tasks</Table.Th>
                  <Table.Th>Status</Table.Th>
                </Table.Tr>
              </Table.Thead>
              <Table.Tbody>
                {filteredProjects.map((project) => (
                  <Table.Tr key={project.id}>
                    <Table.Td>
                      <Stack gap={2}>
                        <Text fw={600}>{project.name}</Text>
                        <Text c="dimmed" fz="sm">{project.description ?? 'No description provided.'}</Text>
                      </Stack>
                    </Table.Td>
                    <Table.Td>{project.createdByUserName}</Table.Td>
                    <Table.Td>
                      <Stack gap={2}>
                        <Text fz="sm">Start: {dayjs(project.startDateUtc).format('DD MMM YYYY')}</Text>
                        <Text c="dimmed" fz="sm">
                          End: {project.endDateUtc ? dayjs(project.endDateUtc).format('DD MMM YYYY') : 'Open-ended'}
                        </Text>
                      </Stack>
                    </Table.Td>
                    <Table.Td>
                      <Badge color="blue" variant="light">{project.taskCount} tasks</Badge>
                    </Table.Td>
                    <Table.Td>
                      <Select
                        aria-label={`Status for ${project.name}`}
                        data={projectStatusOptions}
                        value={project.status}
                        disabled={isUpdatingStatus || isUpdatingProject || isDeletingProject}
                        onChange={(value) => {
                          void handleStatusChange(project, value)
                        }}
                      />
                    </Table.Td>
                    <Table.Td>
                      <Group gap="xs">
                        <Button variant="light" color="dark" onClick={() => openEditModal(project)}>
                          Edit
                        </Button>
                        <Button variant="light" color="red" onClick={() => { void handleDeleteProject(project) }}>
                          Delete
                        </Button>
                      </Group>
                    </Table.Td>
                  </Table.Tr>
                ))}
              </Table.Tbody>
            </Table>
          )}
        </Stack>
      </Card>

      <Modal opened={createModalOpen} onClose={() => setCreateModalOpen(false)} title="Create project" centered>
        <form onSubmit={handleCreateProject}>
          <Stack>
            <TextInput
              label="Project name"
              value={createForm.name}
              onChange={(event) => {
                const { value } = event.currentTarget
                setCreateForm((current) => ({ ...current, name: value }))
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
            <TextInput
              label="Start date"
              type="date"
              value={createForm.startDateUtc}
              onChange={(event) => {
                const { value } = event.currentTarget
                setCreateForm((current) => ({ ...current, startDateUtc: value }))
              }}
              required
            />
            <TextInput
              label="End date"
              type="date"
              value={createForm.endDateUtc}
              onChange={(event) => {
                const { value } = event.currentTarget
                setCreateForm((current) => ({ ...current, endDateUtc: value }))
              }}
            />
            <Select
              label="Status"
              data={projectStatusOptions}
              value={createForm.status}
              onChange={(value) => setCreateForm((current) => ({ ...current, status: (value as ProjectStatus | null) ?? current.status }))}
              allowDeselect={false}
            />
            <Group justify="flex-end">
              <Button variant="subtle" color="gray" onClick={() => setCreateModalOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" color="teal" loading={isCreating}>
                Save project
              </Button>
            </Group>
          </Stack>
        </form>
      </Modal>

      <Modal opened={editingProject !== null} onClose={closeEditModal} title={editingProject ? `Edit ${editingProject.name}` : 'Edit project'} centered>
        <form onSubmit={handleUpdateProject}>
          <Stack>
            <TextInput
              label="Project name"
              value={editForm.name}
              onChange={(event) => {
                const { value } = event.currentTarget
                setEditForm((current) => ({ ...current, name: value }))
              }}
              required
            />
            <Textarea
              label="Description"
              value={editForm.description}
              onChange={(event) => {
                const { value } = event.currentTarget
                setEditForm((current) => ({ ...current, description: value }))
              }}
              minRows={3}
            />
            <TextInput
              label="Start date"
              type="date"
              value={editForm.startDateUtc}
              onChange={(event) => {
                const { value } = event.currentTarget
                setEditForm((current) => ({ ...current, startDateUtc: value }))
              }}
              required
            />
            <TextInput
              label="End date"
              type="date"
              value={editForm.endDateUtc}
              onChange={(event) => {
                const { value } = event.currentTarget
                setEditForm((current) => ({ ...current, endDateUtc: value }))
              }}
            />
            <Group justify="flex-end">
              <Button variant="subtle" color="gray" onClick={closeEditModal}>
                Cancel
              </Button>
              <Button type="submit" color="teal" loading={isUpdatingProject}>
                Save changes
              </Button>
            </Group>
          </Stack>
        </form>
      </Modal>
    </Stack>
  )
}