import type { ProjectListItem } from '../projects'

export type TaskStatus = 'Todo' | 'InProgress' | 'OnHold' | 'Completed' | 'Cancelled'

export type TaskPriority = 'Low' | 'Medium' | 'High' | 'Critical'

export type UserSummary = {
  id: string
  firstName: string
  lastName: string
  email: string
  role: 'Admin' | 'ProjectManager' | 'TeamMember'
  isActive: boolean
  createdAtUtc: string
}

export type CommentItem = {
  id: string
  content: string
  taskItemId: string
  userId: string
  userName: string
  createdAtUtc: string
  updatedAtUtc: string | null
}

export type ActivityItem = {
  id: string
  action: string
  details: string | null
  oldValue: string | null
  newValue: string | null
  occurredAtUtc: string
  userId: string
  userName: string
}

export type TaskListItem = {
  id: string
  title: string
  description: string | null
  status: TaskStatus
  priority: TaskPriority
  progressPercentage: number
  dueDateUtc: string | null
  projectId: string
  projectName: string
  assignedUserId: string | null
  assignedUserName: string
  isOverdue: boolean
  createdAtUtc: string
  updatedAtUtc: string | null
}

export type TaskDetails = TaskListItem & {
  startDateUtc: string | null
  completedDateUtc: string | null
  createdByUserId: string
  createdByUserName: string
  commentCount: number
  activityCount: number
}

export type PagedResult<T> = {
  items: T[]
  pageNumber: number
  pageSize: number
  totalCount: number
}

export type GetTasksQueryParams = {
  searchTerm?: string
  projectId?: string
  assignedUserId?: string
  status?: TaskStatus | ''
  priority?: TaskPriority | ''
  overdueOnly?: boolean
  pageNumber?: number
  pageSize?: number
  sortBy?: 'title' | 'status' | 'priority' | 'dueDateUtc' | 'createdAtUtc' | ''
  sortDirection?: 'asc' | 'desc' | ''
}

export type UpdateTaskRequest = {
  title: string
  description: string | null
  startDateUtc: string | null
  dueDateUtc: string | null
  projectId: string
}

export type UpdateTaskStatusRequest = {
  status: TaskStatus
}

export type UpdateTaskPriorityRequest = {
  priority: TaskPriority
}

export type UpdateTaskProgressRequest = {
  progressPercentage: number
}

export type AssignTaskRequest = {
  assignedUserId: string | null
}

export type AddCommentRequest = {
  content: string
}

export type UpdateCommentRequest = {
  content: string
}

export type CreateTaskRequest = {
  title: string
  description: string | null
  status: TaskStatus
  priority: TaskPriority
  progressPercentage: number
  startDateUtc: string | null
  dueDateUtc: string | null
  projectId: string
  assignedUserId: string | null
}

export type CreateTaskFormDependencies = {
  projects: ProjectListItem[]
  users: UserSummary[]
}