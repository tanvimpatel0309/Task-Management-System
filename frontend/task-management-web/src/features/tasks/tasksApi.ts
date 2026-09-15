import { api } from '../../services/api'
import type {
  ActivityItem,
  AddCommentRequest,
  AssignTaskRequest,
  CommentItem,
  CreateTaskRequest,
  GetTasksQueryParams,
  PagedResult,
  TaskDetails,
  TaskListItem,
  UpdateCommentRequest,
  UpdateTaskPriorityRequest,
  UpdateTaskProgressRequest,
  UpdateTaskRequest,
  UpdateTaskStatusRequest,
  UserSummary,
} from './tasksTypes'

export const tasksApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getTasks: builder.query<PagedResult<TaskListItem>, GetTasksQueryParams | void>({
      query: (params) =>
        params
          ? {
              url: '/api/tasks',
              params,
            }
          : '/api/tasks',
      providesTags: (result) =>
        result
          ? [
              ...result.items.map((task) => ({ type: 'Tasks' as const, id: task.id })),
              { type: 'Tasks' as const, id: 'LIST' },
            ]
          : [{ type: 'Tasks' as const, id: 'LIST' }],
    }),
    getTaskById: builder.query<TaskDetails, string>({
      query: (taskId) => `/api/tasks/${taskId}`,
      providesTags: (_result, _error, taskId) => [{ type: 'Tasks', id: taskId }],
    }),
    createTask: builder.mutation<TaskDetails, CreateTaskRequest>({
      query: (body) => ({
        url: '/api/tasks',
        method: 'POST',
        body,
      }),
      invalidatesTags: [{ type: 'Tasks', id: 'LIST' }],
    }),
    updateTask: builder.mutation<TaskDetails, { taskId: string; body: UpdateTaskRequest }>({
      query: ({ taskId, body }) => ({
        url: `/api/tasks/${taskId}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (_result, _error, { taskId }) => [
        { type: 'Tasks', id: taskId },
        { type: 'Tasks', id: 'LIST' },
      ],
    }),
    updateTaskStatus: builder.mutation<TaskDetails, { taskId: string; body: UpdateTaskStatusRequest }>({
      query: ({ taskId, body }) => ({
        url: `/api/tasks/${taskId}/status`,
        method: 'PATCH',
        body,
      }),
      invalidatesTags: (_result, _error, { taskId }) => [
        { type: 'Tasks', id: taskId },
        { type: 'Tasks', id: 'LIST' },
      ],
    }),
    updateTaskPriority: builder.mutation<TaskDetails, { taskId: string; body: UpdateTaskPriorityRequest }>({
      query: ({ taskId, body }) => ({
        url: `/api/tasks/${taskId}/priority`,
        method: 'PATCH',
        body,
      }),
      invalidatesTags: (_result, _error, { taskId }) => [
        { type: 'Tasks', id: taskId },
        { type: 'Tasks', id: 'LIST' },
      ],
    }),
    updateTaskProgress: builder.mutation<TaskDetails, { taskId: string; body: UpdateTaskProgressRequest }>({
      query: ({ taskId, body }) => ({
        url: `/api/tasks/${taskId}/progress`,
        method: 'PATCH',
        body,
      }),
      invalidatesTags: (_result, _error, { taskId }) => [
        { type: 'Tasks', id: taskId },
        { type: 'Tasks', id: 'LIST' },
      ],
    }),
    assignTask: builder.mutation<TaskDetails, { taskId: string; body: AssignTaskRequest }>({
      query: ({ taskId, body }) => ({
        url: `/api/tasks/${taskId}/assignee`,
        method: 'PATCH',
        body,
      }),
      invalidatesTags: (_result, _error, { taskId }) => [
        { type: 'Tasks', id: taskId },
        { type: 'Tasks', id: 'LIST' },
      ],
    }),
    deleteTask: builder.mutation<void, string>({
      query: (taskId) => ({
        url: `/api/tasks/${taskId}`,
        method: 'DELETE',
      }),
      invalidatesTags: (_result, _error, taskId) => [
        { type: 'Tasks', id: taskId },
        { type: 'Tasks', id: 'LIST' },
      ],
    }),
    getTaskComments: builder.query<CommentItem[], string>({
      query: (taskId) => `/api/tasks/${taskId}/comments`,
      providesTags: (_result, _error, taskId) => [{ type: 'Tasks', id: `COMMENTS-${taskId}` }],
    }),
    addTaskComment: builder.mutation<CommentItem, { taskId: string; body: AddCommentRequest }>({
      query: ({ taskId, body }) => ({
        url: `/api/tasks/${taskId}/comments`,
        method: 'POST',
        body,
      }),
      invalidatesTags: (_result, _error, { taskId }) => [
        { type: 'Tasks', id: taskId },
        { type: 'Tasks', id: `COMMENTS-${taskId}` },
      ],
    }),
    updateTaskComment: builder.mutation<CommentItem, { commentId: string; body: UpdateCommentRequest; taskId: string }>({
      query: ({ commentId, body }) => ({
        url: `/api/tasks/comments/${commentId}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (_result, _error, { taskId }) => [
        { type: 'Tasks', id: taskId },
        { type: 'Tasks', id: `COMMENTS-${taskId}` },
      ],
    }),
    deleteTaskComment: builder.mutation<void, { commentId: string; taskId: string }>({
      query: ({ commentId }) => ({
        url: `/api/tasks/comments/${commentId}`,
        method: 'DELETE',
      }),
      invalidatesTags: (_result, _error, { taskId }) => [
        { type: 'Tasks', id: taskId },
        { type: 'Tasks', id: `COMMENTS-${taskId}` },
      ],
    }),
    getTaskActivity: builder.query<ActivityItem[], string>({
      query: (taskId) => `/api/tasks/${taskId}/activity`,
      providesTags: (_result, _error, taskId) => [{ type: 'Tasks', id: `ACTIVITY-${taskId}` }],
    }),
    getAssignableUsers: builder.query<UserSummary[], void>({
      query: () => '/api/tasks/assignable-users',
      providesTags: [{ type: 'Tasks', id: 'ASSIGNABLE-USERS' }],
    }),
  }),
})

export const {
  useAddTaskCommentMutation,
  useAssignTaskMutation,
  useCreateTaskMutation,
  useDeleteTaskCommentMutation,
  useDeleteTaskMutation,
  useGetAssignableUsersQuery,
  useGetTaskActivityQuery,
  useGetTaskByIdQuery,
  useGetTaskCommentsQuery,
  useGetTasksQuery,
  useUpdateTaskCommentMutation,
  useUpdateTaskMutation,
  useUpdateTaskPriorityMutation,
  useUpdateTaskProgressMutation,
  useUpdateTaskStatusMutation,
} = tasksApi