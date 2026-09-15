import { createSlice, type PayloadAction } from '@reduxjs/toolkit'

import type { AuthenticationResponse, AuthState, AuthenticatedUser } from './authTypes'

const AUTH_STORAGE_KEY = 'task-management.auth'

const defaultState: AuthState = {
  accessToken: null,
  expiresAtUtc: null,
  user: null,
}

function loadAuthState(): AuthState {
  if (typeof window === 'undefined') {
    return defaultState
  }

  const rawValue = window.localStorage.getItem(AUTH_STORAGE_KEY)

  if (!rawValue) {
    return defaultState
  }

  try {
    const parsed = JSON.parse(rawValue) as AuthState

    if (!parsed.accessToken || !parsed.user) {
      return defaultState
    }

    return parsed
  } catch {
    return defaultState
  }
}

export function persistAuthState(state: AuthState) {
  if (typeof window === 'undefined') {
    return
  }

  if (!state.accessToken || !state.user) {
    window.localStorage.removeItem(AUTH_STORAGE_KEY)
    return
  }

  window.localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(state))
}

const authSlice = createSlice({
  name: 'auth',
  initialState: loadAuthState(),
  reducers: {
    setAuthenticatedSession: (
      state,
      action: PayloadAction<AuthenticationResponse>,
    ) => {
      state.accessToken = action.payload.accessToken
      state.expiresAtUtc = action.payload.expiresAtUtc
      state.user = action.payload.user
    },
    setCurrentUser: (state, action: PayloadAction<AuthenticatedUser>) => {
      state.user = action.payload
    },
    logout: (state) => {
      state.accessToken = null
      state.expiresAtUtc = null
      state.user = null
    },
  },
})

export const { logout, setAuthenticatedSession, setCurrentUser } = authSlice.actions

export const authReducer = authSlice.reducer

export const selectCurrentUser = (state: { auth: AuthState }) => state.auth.user