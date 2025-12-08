import type { Course } from "../../models/enrollment/Enrollment.types"

//Remove when backend part is done
const DUMMY_COURSES: Course[] = [
  {
    id: "1",
    name: "Algorithms and Data Structures",
    code: "ADSII",
    professor: "Prof. H S",
    ects: 6,
    status: "required",
  },
  {
    id: "2",
    name: "Algorithms and Data Structures",
    code: "ADSII",
    professor: "Prof. H S",
    ects: 6,
    status: "elective",
  },
  {
    id: "3",
    name: "Algorithms and Data Structures",
    code: "ADSII",
    professor: "Prof. H S",
    ects: 6,
    status: "elective",
  },
  {
    id: "4",
    name: "Algorithms and Data Structures",
    code: "ADSII",
    professor: "Prof. H S",
    ects: 6,
    status: "required",
  },
  {
    id: "5",
    name: "Algorithms and Data Structures",
    code: "ADSII",
    professor: "Prof. H S",
    ects: 6,
    status: "required",
  },
  {
    id: "6",
    name: "Algorithms and Data Structures",
    code: "ADSII",
    professor: "Prof. H S",
    ects: 6,
    status: "required",
  },
  {
    id: "7",
    name: "Database Systems",
    code: "DBSYS",
    professor: "Prof. M K",
    ects: 5,
    status: "required",
  },
  {
    id: "8",
    name: "Web Development",
    code: "WEBDEV",
    professor: "Prof. A L",
    ects: 4,
    status: "elective",
  },
  {
    id: "9",
    name: "Machine Learning",
    code: "ML101",
    professor: "Prof. R T",
    ects: 6,
    status: "elective",
  },
  {
    id: "10",
    name: "Software Engineering",
    code: "SE202",
    professor: "Prof. J D",
    ects: 5,
    status: "required",
  },
  {
    id: "11",
    name: "Computer Networks",
    code: "CN301",
    professor: "Prof. S B",
    ects: 5,
    status: "elective",
  },
  {
    id: "12",
    name: "Operating Systems",
    code: "OS401",
    professor: "Prof. K P",
    ects: 6,
    status: "required",
  },
  {
    id: "13",
    name: "Artificial Intelligence",
    code: "AI501",
    professor: "Prof. N Q",
    ects: 6,
    status: "elective",
  },
  {
    id: "14",
    name: "Cybersecurity",
    code: "CS601",
    professor: "Prof. L W",
    ects: 5,
    status: "required",
  },
  {
    id: "15",
    name: "Mobile App Development",
    code: "MAD701",
    professor: "Prof. E V",
    ects: 4,
    status: "elective",
  },
  {
    id: "16",
    name: "Cloud Computing",
    code: "CC801",
    professor: "Prof. O F",
    ects: 5,
    status: "elective",
  },
  {
    id: "17",
    name: "Data Science",
    code: "DS901",
    professor: "Prof. I G",
    ects: 6,
    status: "required",
  },
  {
    id: "18",
    name: "Blockchain Technology",
    code: "BT101",
    professor: "Prof. U H",
    ects: 4,
    status: "elective",
  },
]

export async function getCourses(page = 1, limit = 6, searchQuery = "", filterStatus = "all") {
  await new Promise((resolve) => setTimeout(resolve, 300))
  let filteredCourses = DUMMY_COURSES

  if (searchQuery) {
    const query = searchQuery.toLowerCase()
    filteredCourses = filteredCourses.filter(
      (course) =>
        course.name.toLowerCase().includes(query) ||
        course.code.toLowerCase().includes(query) ||
        course.professor.toLowerCase().includes(query),
    )
  }

  if (filterStatus !== "all") {
    filteredCourses = filteredCourses.filter((course) => course.status === filterStatus)
  }

  const startIndex = (page - 1) * limit
  const endIndex = startIndex + limit

  const paginatedCourses = filteredCourses.slice(startIndex, endIndex)
  const hasMore = endIndex < filteredCourses.length

  return {
    courses: paginatedCourses,
    hasMore,
    total: filteredCourses.length,
    page,
    limit,
  }
}

export async function enrollInCourse(courseId: string, courseName: string, courseCode: string) {
  await new Promise((resolve) => setTimeout(resolve, 500))

  return {
    success: true,
    message: "Successfully enrolled",
    data: {
      courseId,
      courseName,
      courseCode,
      enrolledAt: new Date().toISOString(),
    },
  }
}