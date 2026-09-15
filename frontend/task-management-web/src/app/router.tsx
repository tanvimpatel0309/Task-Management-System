import { createBrowserRouter, RouterProvider } from 'react-router-dom'

import { ProtectedRoute } from '../features/auth/ProtectedRoute'
import { RequireRole } from '../features/auth/RequireRole'
import { AppShellLayout } from '../layouts/AppShellLayout'
import { AccessDeniedPage } from '../pages/AccessDeniedPage'
import { DashboardPage } from '../pages/DashboardPage'
import { LoginPage } from '../pages/LoginPage'
import { ProjectListPage } from '../pages/ProjectListPage'
import { TaskDetailsPage } from '../pages/TaskDetailsPage'
import { TaskListPage } from '../pages/TaskListPage'
import { UserDetailsPage } from '../pages/UserDetailsPage'
import { UserListPage } from '../pages/UserListPage'

const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/unauthorized',
    element: <AccessDeniedPage />,
  },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <AppShellLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <DashboardPage />,
      },
      {
        path: 'users',
        element: (
          <RequireRole allowedRoles={['Admin']}>
            <UserListPage />
          </RequireRole>
        ),
      },
      {
        path: 'users/:userId',
        element: (
          <RequireRole allowedRoles={['Admin']}>
            <UserDetailsPage />
          </RequireRole>
        ),
      },
      {
        path: 'projects',
        element: (
          <RequireRole allowedRoles={['Admin', 'ProjectManager']}>
            <ProjectListPage />
          </RequireRole>
        ),
      },
      {
        path: 'tasks',
        element: <TaskListPage />,
      },
      {
        path: 'tasks/:taskId',
        element: <TaskDetailsPage />,
      },
    ],
  },
])

export function AppRouter() {
  return <RouterProvider router={router} />
}