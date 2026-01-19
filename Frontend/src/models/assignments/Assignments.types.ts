export interface AssignmentDTO {
  course: string
  name: string
  description: string
  dueDate: Date
  maxPoints: number
}

export interface Assignment {
  id: string
  course: string
  name: string
  maxPoints: number
  major: string
  description: string
  dueDate: Date
}

export interface AssignmentsPaginated {
    data: Assignment[]
    total: number
    page: number
    pageSize: number
    hasMore: boolean
}