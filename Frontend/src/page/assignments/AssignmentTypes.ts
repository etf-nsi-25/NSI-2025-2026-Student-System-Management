export interface Assignment {
  assignmentId: number
  title: string
  description: string
  dueDate: string
  maxPoints: number
  status: "Pending" | "Submitted" | "Graded" | "Missed"
  submissionDate: string | null
  grade: number | null
  points: number | null
  feedback: string | null
}

export function formatDate(dateString: string): string {
  const date = new Date(dateString)
  const day = date.getDate().toString().padStart(2, "0")
  const month = (date.getMonth() + 1).toString().padStart(2, "0")
  const year = date.getFullYear()
  const hours = date.getHours().toString().padStart(2, "0")
  const minutes = date.getMinutes().toString().padStart(2, "0")
  return `${day}.${month}.${year} ${hours}:${minutes}`
}
