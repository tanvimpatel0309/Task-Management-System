import { api } from '../../services/api'
import type {
  CreateUserRequest,
  UpdateUserRequest,
  UpdateUserRoleRequest,
  UserDetails,
  UserListItem,
} from './usersTypes'

export const usersApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getUsers: builder.query<UserListItem[], void>({
      query: () => '/api/users',
      providesTags: (result) =>
        result
          ? [
              ...result.map((user) => ({ type: 'Users' as const, id: user.id })),
              { type: 'Users' as const, id: 'LIST' },
            ]
          : [{ type: 'Users' as const, id: 'LIST' }],
    }),
    getUserById: builder.query<UserDetails, string>({
      query: (userId) => `/api/users/${userId}`,
      providesTags: (_result, _error, userId) => [{ type: 'Users', id: userId }],
    }),
    createUser: builder.mutation<UserDetails, CreateUserRequest>({
      query: (body) => ({
        url: '/api/users',
        method: 'POST',
        body,
      }),
      invalidatesTags: [{ type: 'Users', id: 'LIST' }],
    }),
    updateUser: builder.mutation<UserDetails, { userId: string; body: UpdateUserRequest }>({
      query: ({ userId, body }) => ({
        url: `/api/users/${userId}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (_result, _error, { userId }) => [
        { type: 'Users', id: userId },
        { type: 'Users', id: 'LIST' },
      ],
    }),
    updateUserRole: builder.mutation<UserDetails, { userId: string; body: UpdateUserRoleRequest }>({
      query: ({ userId, body }) => ({
        url: `/api/users/${userId}/role`,
        method: 'PATCH',
        body,
      }),
      invalidatesTags: (_result, _error, { userId }) => [
        { type: 'Users', id: userId },
        { type: 'Users', id: 'LIST' },
      ],
    }),
    activateUser: builder.mutation<UserDetails, string>({
      query: (userId) => ({
        url: `/api/users/${userId}/activate`,
        method: 'PATCH',
      }),
      invalidatesTags: (_result, _error, userId) => [
        { type: 'Users', id: userId },
        { type: 'Users', id: 'LIST' },
      ],
    }),
    deactivateUser: builder.mutation<UserDetails, string>({
      query: (userId) => ({
        url: `/api/users/${userId}/deactivate`,
        method: 'PATCH',
      }),
      invalidatesTags: (_result, _error, userId) => [
        { type: 'Users', id: userId },
        { type: 'Users', id: 'LIST' },
      ],
    }),
  }),
})

export const {
  useActivateUserMutation,
  useCreateUserMutation,
  useDeactivateUserMutation,
  useGetUserByIdQuery,
  useGetUsersQuery,
  useUpdateUserMutation,
  useUpdateUserRoleMutation,
} = usersApi