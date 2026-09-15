import {
  Alert,
  Badge,
  Button,
  Card,
  Grid,
  Group,
  Loader,
  Modal,
  NumberInput,
  Select,
  Stack,
  Table,
  Text,
  TextInput,
  Textarea,
} from '@mantine/core'
import { notifications } from '@mantine/notifications'
import dayjs from 'dayjs'
import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import { PageIntro } from '../components/PageIntro'
import {
  useAddTaskCommentMutation,
  useAssignTaskMutation,
  useDeleteTaskCommentMutation,
  useDeleteTaskMutation,
  useGetAssignableUsersQuery,
  useGetTaskActivityQuery,
  useGetTaskByIdQuery,
  useGetTaskCommentsQuery,
  useUpdateTaskCommentMutation,
  useUpdateTaskMutation,
  useUpdateTaskPriorityMutation,
  useUpdateTaskProgressMutation,
  useUpdateTaskStatusMutation,
  type CommentItem,
  type TaskPriority,
  type TaskStatus,
} from '../features/tasks'
import { useAppSelector } from '../hooks/redux'

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

function getApiErrorMessage(error: unknown) {
  if (typeof error === 'object' && error !== null && 'data' in error) {
    const data = error.data as { message?: string; Message?: string; errors?: Record<string, string[]>; Errors?: Record<string, string[]> }
    const fieldErrors = Object.values(data.errors ?? data.Errors ?? {}).flat()
    return fieldErrors[0] ?? data.message ?? data.Message ?? 'Request failed.'
  }

  return 'Request failed.'
}

export function TaskDetailsPage() {
  const navigate = useNavigate()
  const { taskId } = useParams()
  const currentUser = useAppSelector((state) => state.auth.user)
  const currentRole = currentUser?.role ?? null
  const canManageTask = currentRole === 'Admin' || currentRole === 'ProjectManager'
  const { data: task, isLoading, isFetching, isError, refetch } = useGetTaskByIdQuery(taskId ?? '', {
    skip: !taskId,
  })
  const { data: comments = [] } = useGetTaskCommentsQuery(taskId ?? '', { skip: !taskId })
  const { data: activity = [] } = useGetTaskActivityQuery(taskId ?? '', { skip: !taskId })
  const { data: assignableUsers = [] } = useGetAssignableUsersQuery(undefined, { skip: !canManageTask })
  const [updateTask, { isLoading: isUpdatingTask }] = useUpdateTaskMutation()
  const [updateTaskStatus, { isLoading: isUpdatingStatus }] = useUpdateTaskStatusMutation()
  const [updateTaskPriority, { isLoading: isUpdatingPriority }] = useUpdateTaskPriorityMutation()
  const [updateTaskProgress, { isLoading: isUpdatingProgress }] = useUpdateTaskProgressMutation()
  const [assignTask, { isLoading: isAssigningTask }] = useAssignTaskMutation()
  const [deleteTask, { isLoading: isDeletingTask }] = useDeleteTaskMutation()
  const [addTaskComment, { isLoading: isAddingComment }] = useAddTaskCommentMutation()
  const [updateTaskComment, { isLoading: isUpdatingComment }] = useUpdateTaskCommentMutation()
  const [deleteTaskComment, { isLoading: isDeletingComment }] = useDeleteTaskCommentMutation()
  const [editModalOpen, setEditModalOpen] = useState(false)
  const [commentDraft, setCommentDraft] = useState('')
  const [editingComment, setEditingComment] = useState<CommentItem | null>(null)
  const [editCommentDraft, setEditCommentDraft] = useState('')
  const [formError, setFormError] = useState<string | null>(null)
  const [editForm, setEditForm] = useState({
    title: '',
    description: '',
    startDateUtc: '',
    dueDateUtc: '',
  })

  useEffect(() => {
    if (!task) {
      return
    }

    setEditForm({
      title: task.title,
      description: task.description ?? '',
      startDateUtc: task.startDateUtc ? dayjs(task.startDateUtc).format('YYYY-MM-DD') : '',
      dueDateUtc: task.dueDateUtc ? dayjs(task.dueDateUtc).format('YYYY-MM-DD') : '',
    })
  }, [task])

  const isMutating =
    isUpdatingTask
    || isUpdatingStatus
    || isUpdatingPriority
    || isUpdatingProgress
    || isAssigningTask
    || isDeletingTask
    || isAddingComment
    || isUpdatingComment
    || isDeletingComment

  const assigneeOptions = [
    { value: '', label: 'Unassigned' },
    ...assignableUsers.map((user) => ({
      value: user.id,
      label: `${user.firstName} ${user.lastName} (${user.role})`,
    })),
  ]

  const canEditComment = (comment: CommentItem) => {
    if (!currentUser) {
      return false
    }

    return comment.userId === currentUser.id || currentRole === 'Admin' || currentRole === 'ProjectManager'
  }

  const showError = (error: unknown) => {
    const message = getApiErrorMessage(error)
    setFormError(message)
    notifications.show({ color: 'red', message })
  }

  const showSuccess = (message: string) => {
    setFormError(null)
    notifications.show({ color: 'teal', message })
  }

  const handleTaskUpdate = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    if (!taskId || !task) {
      return
    }

    try {
      await updateTask({
        taskId,
        body: {
          title: editForm.title,
          description: editForm.description.trim() ? editForm.description : null,
          startDateUtc: editForm.startDateUtc ? new Date(editForm.startDateUtc).toISOString() : null,
          dueDateUtc: editForm.dueDateUtc ? new Date(editForm.dueDateUtc).toISOString() : null,
          projectId: task.projectId,
        },
      }).unwrap()
      setEditModalOpen(false)
      showSuccess('Task details updated.')
    } catch (error) {
      showError(error)
    }
  }

  const handleStatusChange = async (value: string | null) => {
    if (!taskId || !task || !value || value === task.status) {
      return
    }

    try {
      await updateTaskStatus({ taskId, body: { status: value as TaskStatus } }).unwrap()
      showSuccess(`Task status updated to ${value}.`)
    } catch (error) {
      showError(error)
    }
  }

  const handlePriorityChange = async (value: string | null) => {
    if (!taskId || !task || !value || value === task.priority) {
      return
    }

    try {
      await updateTaskPriority({ taskId, body: { priority: value as TaskPriority } }).unwrap()
      showSuccess(`Task priority updated to ${value}.`)
    } catch (error) {
      showError(error)
    }
  }

  const handleProgressChange = async (value: number | string) => {
    if (!taskId || !task || typeof value !== 'number' || Number.isNaN(value) || value === task.progressPercentage) {
      return
    }

    try {
      await updateTaskProgress({ taskId, body: { progressPercentage: value } }).unwrap()
      showSuccess(`Task progress updated to ${value}%.`)
    } catch (error) {
      showError(error)
    }
  }

  const handleAssigneeChange = async (value: string | null) => {
    if (!taskId || !task) {
      return
    }

    const assignedUserId = value || null
    if (assignedUserId === task.assignedUserId) {
      return
    }

    try {
      await assignTask({ taskId, body: { assignedUserId } }).unwrap()
      showSuccess(assignedUserId ? 'Task assignee updated.' : 'Task unassigned successfully.')
    } catch (error) {
      showError(error)
    }
  }

  const handleDeleteTask = async () => {
    if (!taskId || !task) {
      return
    }

    const confirmed = window.confirm(`Delete task "${task.title}"?`)
    if (!confirmed) {
      return
    }

    try {
      await deleteTask(taskId).unwrap()
      showSuccess('Task deleted successfully.')
      navigate('/tasks', { replace: true })
    } catch (error) {
      showError(error)
    }
  }

  const handleAddComment = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    if (!taskId || !commentDraft.trim()) {
      return
    }

    try {
      await addTaskComment({ taskId, body: { content: commentDraft } }).unwrap()
      setCommentDraft('')
      showSuccess('Comment added.')
    } catch (error) {
      showError(error)
    }
  }

  const handleCommentUpdate = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    if (!taskId || !editingComment || !editCommentDraft.trim()) {
      return
    }

    try {
      await updateTaskComment({
        taskId,
        commentId: editingComment.id,
        body: { content: editCommentDraft },
      }).unwrap()
      setEditingComment(null)
      setEditCommentDraft('')
      showSuccess('Comment updated.')
    } catch (error) {
      showError(error)
    }
  }

  const handleCommentDelete = async (comment: CommentItem) => {
    if (!taskId) {
      return
    }

    const confirmed = window.confirm('Delete this comment?')
    if (!confirmed) {
      return
    }

    try {
      await deleteTaskComment({ taskId, commentId: comment.id }).unwrap()
      showSuccess('Comment deleted.')
    } catch (error) {
      showError(error)
    }
  }

  return (
    <Stack gap="xl">
      <PageIntro
        eyebrow="Task Details"
        title={task?.title ?? `Task ${taskId ?? 'Preview'}`}
        description="Inspect execution status, ownership, scheduling, and downstream activity counts for an individual task."
      />

      {isLoading || isFetching ? (
        <Card padding="xl" radius="lg" shadow="sm" withBorder>
          <Group justify="center" py="xl">
            <Loader color="teal" />
          </Group>
        </Card>
      ) : isError || !task ? (
        <Alert color="red">
          Unable to load task details right now.
          <Text component="button" onClick={() => void refetch()}>
            Retry
          </Text>
        </Alert>
      ) : (
        <>
          {formError ? <Alert color="red">{formError}</Alert> : null}
          <Grid>
          <Grid.Col span={{ base: 12, lg: 8 }}>
            <Card padding="xl" radius="lg" shadow="sm" withBorder>
              <Stack gap="md">
                <Group justify="space-between" align="flex-start">
                  <Group>
                  <Badge color={task.status === 'Completed' ? 'teal' : 'blue'} variant="light">
                    {task.status}
                  </Badge>
                  <Badge color={task.priority === 'Critical' ? 'red' : task.priority === 'High' ? 'orange' : 'gray'} variant="light">
                    {task.priority}
                  </Badge>
                  {task.isOverdue ? <Badge color="red" variant="filled">Overdue</Badge> : null}
                  </Group>
                  {canManageTask ? (
                    <Group gap="xs">
                      <Button variant="light" color="dark" onClick={() => setEditModalOpen(true)} disabled={isMutating}>
                        Edit task
                      </Button>
                      <Button color="red" variant="light" onClick={() => void handleDeleteTask()} disabled={isMutating}>
                        Delete task
                      </Button>
                    </Group>
                  ) : null}
                </Group>
                <Text>{task.description ?? 'No description provided for this task.'}</Text>
                <SimpleField label="Project" value={task.projectName} />
                <SimpleField label="Assigned to" value={task.assignedUserName || 'Unassigned'} />
                <SimpleField label="Created by" value={task.createdByUserName} />
              </Stack>
            </Card>

            <Card padding="xl" radius="lg" shadow="sm" withBorder mt="xl">
              <Stack gap="md">
                <Text fw={700}>Comments</Text>
                <form onSubmit={handleAddComment}>
                  <Stack>
                    <Textarea
                      label="Add comment"
                      placeholder="Write a task update or note"
                      value={commentDraft}
                      onChange={(event) => setCommentDraft(event.currentTarget.value)}
                      minRows={3}
                      required
                    />
                    <Group justify="flex-end">
                      <Button type="submit" color="teal" loading={isAddingComment}>
                        Post comment
                      </Button>
                    </Group>
                  </Stack>
                </form>

                {comments.length === 0 ? (
                  <Text c="dimmed">No comments yet.</Text>
                ) : (
                  <Stack gap="md">
                    {comments.map((comment) => (
                      <Card key={comment.id} padding="lg" radius="md" withBorder>
                        <Stack gap="xs">
                          <Group justify="space-between" align="flex-start">
                            <Stack gap={2}>
                              <Text fw={600}>{comment.userName}</Text>
                              <Text c="dimmed" fz="xs">
                                {dayjs(comment.createdAtUtc).format('DD MMM YYYY HH:mm')}
                              </Text>
                            </Stack>
                            {canEditComment(comment) ? (
                              <Group gap="xs">
                                <Button
                                  variant="subtle"
                                  color="dark"
                                  onClick={() => {
                                    setEditingComment(comment)
                                    setEditCommentDraft(comment.content)
                                  }}
                                >
                                  Edit
                                </Button>
                                <Button
                                  variant="subtle"
                                  color="red"
                                  onClick={() => {
                                    void handleCommentDelete(comment)
                                  }}
                                >
                                  Delete
                                </Button>
                              </Group>
                            ) : null}
                          </Group>
                          <Text>{comment.content}</Text>
                        </Stack>
                      </Card>
                    ))}
                  </Stack>
                )}
              </Stack>
            </Card>
          </Grid.Col>
          <Grid.Col span={{ base: 12, lg: 4 }}>
            <Stack>
              <Card padding="xl" radius="lg" shadow="sm" withBorder>
                <Stack gap="sm">
                  {canManageTask ? (
                    <>
                      <Select
                        label="Status"
                        data={taskStatusOptions}
                        value={task.status}
                        onChange={(value) => {
                          void handleStatusChange(value)
                        }}
                        disabled={isMutating}
                      />
                      <Select
                        label="Priority"
                        data={taskPriorityOptions}
                        value={task.priority}
                        onChange={(value) => {
                          void handlePriorityChange(value)
                        }}
                        disabled={isMutating}
                      />
                      <NumberInput
                        label="Progress"
                        min={0}
                        max={100}
                        value={task.progressPercentage}
                        onChange={(value) => {
                          void handleProgressChange(value)
                        }}
                        suffix="%"
                        disabled={isMutating}
                      />
                      <Select
                        label="Assignee"
                        data={assigneeOptions}
                        value={task.assignedUserId ?? ''}
                        onChange={(value) => {
                          void handleAssigneeChange(value)
                        }}
                        disabled={isMutating}
                      />
                    </>
                  ) : (
                    <SimpleField label="Progress" value={`${task.progressPercentage}%`} />
                  )}
                  <SimpleField label="Start" value={task.startDateUtc ? dayjs(task.startDateUtc).format('DD MMM YYYY') : 'Not set'} />
                  <SimpleField label="Due" value={task.dueDateUtc ? dayjs(task.dueDateUtc).format('DD MMM YYYY') : 'Not set'} />
                  <SimpleField label="Completed" value={task.completedDateUtc ? dayjs(task.completedDateUtc).format('DD MMM YYYY') : 'Not completed'} />
                </Stack>
              </Card>
              <Card padding="xl" radius="lg" shadow="sm" withBorder>
                <Stack gap="sm">
                  <SimpleField label="Comments" value={String(task.commentCount)} />
                  <SimpleField label="Activity entries" value={String(task.activityCount)} />
                  <SimpleField label="Created" value={dayjs(task.createdAtUtc).format('DD MMM YYYY HH:mm')} />
                  <SimpleField label="Updated" value={task.updatedAtUtc ? dayjs(task.updatedAtUtc).format('DD MMM YYYY HH:mm') : 'Not updated'} />
                </Stack>
              </Card>

              <Card padding="xl" radius="lg" shadow="sm" withBorder>
                <Stack gap="sm">
                  <Text fw={700}>Recent activity</Text>
                  {activity.length === 0 ? (
                    <Text c="dimmed">No activity logged yet.</Text>
                  ) : (
                    <Table verticalSpacing="sm">
                      <Table.Tbody>
                        {activity.slice(0, 6).map((item) => (
                          <Table.Tr key={item.id}>
                            <Table.Td>
                              <Stack gap={2}>
                                <Text fw={600} fz="sm">{item.action}</Text>
                                <Text c="dimmed" fz="xs">{item.details ?? 'No details provided.'}</Text>
                                <Text c="dimmed" fz="xs">
                                  {item.userName} · {dayjs(item.occurredAtUtc).format('DD MMM YYYY HH:mm')}
                                </Text>
                              </Stack>
                            </Table.Td>
                          </Table.Tr>
                        ))}
                      </Table.Tbody>
                    </Table>
                  )}
                </Stack>
              </Card>
            </Stack>
          </Grid.Col>
          </Grid>

          <Modal opened={editModalOpen} onClose={() => setEditModalOpen(false)} title="Edit task" centered>
            <form onSubmit={handleTaskUpdate}>
              <Stack>
                <TextInput
                  label="Title"
                  value={editForm.title}
                  onChange={(event) => {
                    const { value } = event.currentTarget
                    setEditForm((current) => ({ ...current, title: value }))
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
                />
                <TextInput
                  label="Due date"
                  type="date"
                  value={editForm.dueDateUtc}
                  onChange={(event) => {
                    const { value } = event.currentTarget
                    setEditForm((current) => ({ ...current, dueDateUtc: value }))
                  }}
                />
                <Group justify="flex-end">
                  <Button variant="subtle" color="gray" onClick={() => setEditModalOpen(false)}>
                    Cancel
                  </Button>
                  <Button type="submit" color="teal" loading={isUpdatingTask}>
                    Save changes
                  </Button>
                </Group>
              </Stack>
            </form>
          </Modal>

          <Modal
            opened={editingComment !== null}
            onClose={() => {
              setEditingComment(null)
              setEditCommentDraft('')
            }}
            title="Edit comment"
            centered
          >
            <form onSubmit={handleCommentUpdate}>
              <Stack>
                <Textarea
                  label="Comment"
                  value={editCommentDraft}
                  onChange={(event) => setEditCommentDraft(event.currentTarget.value)}
                  minRows={4}
                  required
                />
                <Group justify="flex-end">
                  <Button
                    variant="subtle"
                    color="gray"
                    onClick={() => {
                      setEditingComment(null)
                      setEditCommentDraft('')
                    }}
                  >
                    Cancel
                  </Button>
                  <Button type="submit" color="teal" loading={isUpdatingComment}>
                    Update comment
                  </Button>
                </Group>
              </Stack>
            </form>
          </Modal>
        </>
      )}
    </Stack>
  )
}

type SimpleFieldProps = {
  label: string
  value: string
}

function SimpleField({ label, value }: SimpleFieldProps) {
  return (
    <Stack gap={2}>
      <Text c="dimmed" fz="xs" tt="uppercase">{label}</Text>
      <Text fw={500}>{value}</Text>
    </Stack>
  )
}