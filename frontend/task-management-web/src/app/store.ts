import { configureStore } from '@reduxjs/toolkit'

import { authReducer, persistAuthState } from '../features/auth/authSlice'
import '../features/auth/authApi'
import { api } from '../services/api'

export const store = configureStore({
  reducer: {
    auth: authReducer,
    [api.reducerPath]: api.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(api.middleware),
})

store.subscribe(() => {
  persistAuthState(store.getState().auth)
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch