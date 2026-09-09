import { createBrowserRouter, RouterProvider } from 'react-router-dom'

import { AppShellLayout } from '../layouts/AppShellLayout'
import { DashboardPage } from '../pages/DashboardPage'
import { LoginPage } from '../pages/LoginPage'
import { ProjectListPage } from '../pages/ProjectListPage'
import { TaskDetailsPage } from '../pages/TaskDetailsPage'
import { TaskListPage } from '../pages/TaskListPage'
import { UserListPage } from '../pages/UserListPage'

const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/',
    element: <AppShellLayout />,
    children: [
      {
        index: true,
        element: <DashboardPage />,
      },
      {
        path: 'users',
        element: <UserListPage />,
      },
      {
        path: 'projects',
        element: <ProjectListPage />,
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