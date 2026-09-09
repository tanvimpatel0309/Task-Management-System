import dayjs from 'dayjs'

export function formatDisplayDate(value: string | Date) {
  return dayjs(value).format('DD MMM YYYY')
}