import {
  type BaseQueryFn,
  createApi,
  fetchBaseQuery,
  type FetchArgs,
  type FetchBaseQueryError,
} from '@reduxjs/toolkit/query/react'

import { logout } from '../features/auth/authSlice'
import type { AuthState } from '../features/auth/authTypes'

const rawBaseQuery = fetchBaseQuery({
  baseUrl: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5271',
  prepareHeaders: (headers, { getState }) => {
    const state = getState() as { auth?: AuthState }
    const accessToken = state.auth?.accessToken

    if (accessToken) {
      headers.set('authorization', `Bearer ${accessToken}`)
    }

    return headers
  },
})

const baseQuery: BaseQueryFn<string | FetchArgs, unknown, FetchBaseQueryError> = async (
  args,
  api,
  extraOptions,
) => {
  const result = await rawBaseQuery(args, api, extraOptions)

  if (result.error?.status === 401) {
    api.dispatch(logout())
  }

  return result
}

export const api = createApi({
  reducerPath: 'taskManagementApi',
  baseQuery,
  tagTypes: ['Users', 'Projects', 'Tasks'],
  endpoints: () => ({}),
})