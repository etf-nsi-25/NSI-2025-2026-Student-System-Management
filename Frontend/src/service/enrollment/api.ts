import type { API } from '../../api/api.ts';

export async function getCourses(api: API, page = 1, limit = 6, searchQuery = "", filterStatus = "all") {
  try {
    let allCourses = await api.getAllCourses()

    if (searchQuery) {
      const query = searchQuery.toLowerCase()
      allCourses = allCourses.filter(
        (course) => course.name.toLowerCase().includes(query) || course.code.toLowerCase().includes(query),
      )
    }

    if (filterStatus !== "all") {
      allCourses = allCourses.filter((course) => course.type.toLowerCase() === filterStatus.toLowerCase())
    }

    const startIndex = (page - 1) * limit
    const endIndex = startIndex + limit

    const paginatedCourses = allCourses.slice(startIndex, endIndex)
    const hasMore = endIndex < allCourses.length

    return {
      courses: paginatedCourses,
      hasMore,
      total: allCourses.length,
      page,
      limit,
    }
  } catch (error) {
    console.error("Error fetching courses:", error)
    // Return empty result on error
    return {
      courses: [],
      hasMore: false,
      total: 0,
      page,
      limit,
    }
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
