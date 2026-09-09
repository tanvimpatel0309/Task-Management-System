import '@mantine/core/styles.css'
import '@mantine/notifications/styles.css'

import { MantineProvider, createTheme } from '@mantine/core'
import { Notifications } from '@mantine/notifications'
import type { ReactNode } from 'react'
import { Provider } from 'react-redux'

import { store } from './store'

const theme = createTheme({
  primaryColor: 'teal',
  fontFamily: 'Segoe UI, sans-serif',
  headings: {
    fontFamily: 'Georgia, serif',
  },
})

type AppProvidersProps = {
  children: ReactNode
}

export function AppProviders({ children }: AppProvidersProps) {
  return (
    <Provider store={store}>
      <MantineProvider theme={theme} defaultColorScheme="light">
        <Notifications position="top-right" />
        {children}
      </MantineProvider>
    </Provider>
  )
}