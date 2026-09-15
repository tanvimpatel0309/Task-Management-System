export type ProjectStatus = 'Planning' | 'Active' | 'Completed' | 'Archived'

export type ProjectListItem = {
  id: string
  name: string
  description: string | null
  status: ProjectStatus
  startDateUtc: string
  endDateUtc: string | null
  createdByUserId: string
  createdByUserName: string
  taskCount: number
  createdAtUtc: string
  updatedAtUtc: string | null
}

export type ProjectDetails = ProjectListItem & {
  canAcceptTasks: boolean
}

export type CreateProjectRequest = {
  name: string
  description: string | null
  startDateUtc: string
  endDateUtc: string | null
  status: ProjectStatus
}

export type UpdateProjectRequest = {
  name: string
  description: string | null
  startDateUtc: string
  endDateUtc: string | null
}

export type UpdateProjectStatusRequest = {
  status: ProjectStatus
}