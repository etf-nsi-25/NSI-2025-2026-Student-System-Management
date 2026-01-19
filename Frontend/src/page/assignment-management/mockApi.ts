interface CreateAssignmentDTO {
  course: string
  name: string
  description: string
  dueDate: Date
  maxPoints: number
}

interface Assignment {
  id: string
  course: string
  name: string
  maxPoints: number
  major: string
  description: string
  dueDate: Date
}

interface PaginationParams {
  query?: string
  page: number
  pageSize: number
}

interface PaginatedResponse<T> {
  data: T[]
  total: number
  page: number
  pageSize: number
  hasMore: boolean
}

// Mock data store - expanded for pagination testing
let mockAssignments: Assignment[] = [
  {
    id: "35009",
    course: "NSI",
    name: "Computer Vision",
    maxPoints: 3,
    major: "Computer Science",
    description: "Introduction to computer vision",
    dueDate: new Date("2024-12-31"),
  },
    {
    id: "35009",
    course: "NSI",
    name: "Computer Vision",
    maxPoints: 3,
    major: "Computer Science",
    description: "Introduction to computer vision",
    dueDate: new Date("2024-12-31"),
  },
    {
    id: "35009",
    course: "NSI",
    name: "Computer Vision",
    maxPoints: 3,
    major: "Computer Science",
    description: "Introduction to computer vision",
    dueDate: new Date("2024-12-31"),
  },
    {
    id: "35009",
    course: "NSI",
    name: "Computer Vision",
    maxPoints: 3,
    major: "Computer Science",
    description: "Introduction to computer vision",
    dueDate: new Date("2024-12-31"),
  },
    {
    id: "35009",
    course: "NSI",
    name: "Computer Vision",
    maxPoints: 3,
    major: "Computer Science",
    description: "Introduction to computer vision",
    dueDate: new Date("2024-12-31"),
  },
    {
    id: "35009",
    course: "NSI",
    name: "Computer Vision",
    maxPoints: 3,
    major: "Computer Science",
    description: "Introduction to computer vision",
    dueDate: new Date("2024-12-31"),
  },
    {
    id: "35009",
    course: "NSI",
    name: "Computer Vision",
    maxPoints: 3,
    major: "Computer Science",
    description: "Introduction to computer vision",
    dueDate: new Date("2024-12-31"),
  },
    {
    id: "35009",
    course: "NSI",
    name: "Computer Vision",
    maxPoints: 3,
    major: "Computer Science",
    description: "Introduction to computer vision",
    dueDate: new Date("2024-12-31"),
  },
  {
    id: "35012",
    course: "NSI",
    name: "Machine Learning",
    maxPoints: 3,
    major: "Computer Science",
    description: "Advanced machine learning techniques",
    dueDate: new Date("2024-12-31"),
  },
  {
    id: "35011",
    course: "AI",
    name: "Deep Learning",
    maxPoints: 3,
    major: "Computer Science",
    description: "Deep learning applications",
    dueDate: new Date("2024-12-31"),
  },
  {
    id: "35010",
    course: "CS",
    name: "Data Structures",
    maxPoints: 3,
    major: "Computer Science",
    description: "Advanced data structures",
    dueDate: new Date("2024-12-31"),
  },
  {
    id: "35008",
    course: "NSI",
    name: "Natural Language Processing",
    maxPoints: 4,
    major: "Computer Science",
    description: "NLP fundamentals",
    dueDate: new Date("2024-12-31"),
  },
  {
    id: "35007",
    course: "AI",
    name: "Robotics",
    maxPoints: 5,
    major: "Computer Science",
    description: "Introduction to robotics",
    dueDate: new Date("2024-12-31"),
  },
  {
    id: "35006",
    course: "CS",
    name: "Algorithms",
    maxPoints: 4,
    major: "Computer Science",
    description: "Algorithm design and analysis",
    dueDate: new Date("2024-12-31"),
  },
  {
    id: "35005",
    course: "NSI",
    name: "Neural Networks",
    maxPoints: 4,
    major: "Computer Science",
    description: "Neural network architectures",
    dueDate: new Date("2024-12-31"),
  },
]

// Simulate API delay
const delay = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms))

const mockAPI = {
  getAssignments: async (params: PaginationParams): Promise<PaginatedResponse<Assignment>> => {
    await delay(300)

    // Filter by query if provided
    let filtered = [...mockAssignments]
    if (params.query) {
      const query = params.query.toLowerCase()
      filtered = filtered.filter(
        (assignment) =>
          assignment.course.toLowerCase().includes(query) || assignment.name.toLowerCase().includes(query),
      )
    }

    // Calculate pagination
    const total = filtered.length
    const startIndex = (params.page - 1) * params.pageSize
    const endIndex = startIndex + params.pageSize
    const data = filtered.slice(startIndex, endIndex)
    const hasMore = endIndex < total

    return {
      data,
      total,
      page: params.page,
      pageSize: params.pageSize,
      hasMore,
    }
  },

  createAssignment: async (dto: CreateAssignmentDTO): Promise<Assignment> => {
    await delay(500)
    const newAssignment: Assignment = {
      id: String(Math.floor(Math.random() * 90000) + 10000),
      course: dto.course,
      name: dto.name,
      maxPoints: dto.maxPoints,
      major: "Computer Science",
      description: dto.description,
      dueDate: dto.dueDate,
    }
    mockAssignments.push(newAssignment)
    return newAssignment
  },

  updateAssignment: async (id: string, dto: CreateAssignmentDTO): Promise<Assignment> => {
    await delay(500)
    const index = mockAssignments.findIndex((a) => a.id === id)
    if (index === -1) {
      throw new Error("Assignment not found")
    }
    mockAssignments[index] = {
      ...mockAssignments[index],
      course: dto.course,
      name: dto.name,
      description: dto.description,
      dueDate: dto.dueDate,
      maxPoints: dto.maxPoints,
    }
    return mockAssignments[index]
  },

  deleteAssignment: async (id: string): Promise<void> => {
    await delay(500)
    mockAssignments = mockAssignments.filter((a) => a.id !== id)
  },
}

export default mockAPI
export type { Assignment, CreateAssignmentDTO, PaginationParams, PaginatedResponse }
