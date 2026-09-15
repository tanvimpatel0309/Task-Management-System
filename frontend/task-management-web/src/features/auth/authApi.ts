import { api } from '../../services/api'
import type { AuthenticatedUser, AuthenticationResponse, LoginRequest } from './authTypes'

export const authApi = api.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<AuthenticationResponse, LoginRequest>({
      query: (body) => ({
        url: '/api/auth/login',
        method: 'POST',
        body,
      }),
    }),
    getCurrentUser: builder.query<AuthenticatedUser, void>({
      query: () => '/api/auth/me',
    }),
  }),
})

export const { useGetCurrentUserQuery, useLoginMutation } = authApi