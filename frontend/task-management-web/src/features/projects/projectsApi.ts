import { api } from '../../services/api'
import type {
  CreateProjectRequest,
  ProjectDetails,
  ProjectListItem,
  UpdateProjectRequest,
  UpdateProjectStatusRequest,
} from './projectsTypes'

export const projectsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getProjects: builder.query<ProjectListItem[], void>({
      query: () => '/api/projects',
      providesTags: (result) =>
        result
          ? [
              ...result.map((project) => ({ type: 'Projects' as const, id: project.id })),
              { type: 'Projects' as const, id: 'LIST' },
            ]
          : [{ type: 'Projects' as const, id: 'LIST' }],
    }),
    getProjectById: builder.query<ProjectDetails, string>({
      query: (projectId) => `/api/projects/${projectId}`,
      providesTags: (_result, _error, projectId) => [{ type: 'Projects', id: projectId }],
    }),
    createProject: builder.mutation<ProjectDetails, CreateProjectRequest>({
      query: (body) => ({
        url: '/api/projects',
        method: 'POST',
        body,
      }),
      invalidatesTags: [{ type: 'Projects', id: 'LIST' }],
    }),
    updateProject: builder.mutation<ProjectDetails, { projectId: string; body: UpdateProjectRequest }>({
      query: ({ projectId, body }) => ({
        url: `/api/projects/${projectId}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (_result, _error, { projectId }) => [
        { type: 'Projects', id: projectId },
        { type: 'Projects', id: 'LIST' },
      ],
    }),
    updateProjectStatus: builder.mutation<
      ProjectDetails,
      { projectId: string; body: UpdateProjectStatusRequest }
    >({
      query: ({ projectId, body }) => ({
        url: `/api/projects/${projectId}/status`,
        method: 'PATCH',
        body,
      }),
      invalidatesTags: (_result, _error, { projectId }) => [
        { type: 'Projects', id: projectId },
        { type: 'Projects', id: 'LIST' },
      ],
    }),
    deleteProject: builder.mutation<void, string>({
      query: (projectId) => ({
        url: `/api/projects/${projectId}`,
        method: 'DELETE',
      }),
      invalidatesTags: (_result, _error, projectId) => [
        { type: 'Projects', id: projectId },
        { type: 'Projects', id: 'LIST' },
      ],
    }),
  }),
})

export const {
  useCreateProjectMutation,
  useDeleteProjectMutation,
  useGetProjectByIdQuery,
  useGetProjectsQuery,
  useUpdateProjectMutation,
  useUpdateProjectStatusMutation,
} = projectsApi