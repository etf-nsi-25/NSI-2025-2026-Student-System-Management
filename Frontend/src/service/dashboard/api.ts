export async function getDashboardData() {
  await new Promise((resolve) => setTimeout(resolve, 1000));

  return {
    user: { name: "Jane", faculty: "Computer Science" },
    stats: {
      gpa: 8.5,
      enrolledCourses: 6,
      attendanceRate: 92,
      deadlines: { count: 3, period: "This week" },
    },
    courses: [
      {
        id: 1,
        title: "Tehnologije sigurnosti",
        professor: "Prof. Saša Mrdović",
      },
      {
        id: 2,
        title: "Metoda i primjene vještačke inteligencije",
        professor: "Prof. Amila Akagić",
      },
      {
        id: 3,
        title: "Napredni softver inžinjering",
        professor: "Prof. Samir Omanović",
      },
      {
        id: 4,
        title: "Rečunarski sistemi u realnom vremenu",
        professor: "Prof. Ingmar Bešić",
      },
      {
        id: 5,
        title: "Inovacije u projektovanju informacionih sistema",
        professor: "Prof. Almir Karabegović",
      },
    ],
    upcomingTasks: [
      {
        id: 1,
        course: "Tehnologije sigurnosti",
        task: "Criptography task",
        day: "Wednesday",
      },
      {
        id: 2,
        course: "Napredni softver inžinjering",
        task: "Project task",
        day: "Friday",
      },
      {
        id: 3,
        course: "Napredni softver inžinjering",
        task: "Tutorial task",
        day: "Sunday",
      },
    ],
    calendar: {
      currentMonth: "November 2025",
      highlightedDates: [29, 5, 13, 17, 20, 28],
    },
    quickActions: [
      {
        id: 1,
        label: "Submit assignment",
        icon: "assignment",
        color: "purple",
      },
      { id: 2, label: "View transcript", icon: "document", color: "blue" },
      {
        id: 3,
        label: "Download certificate",
        icon: "certificate",
        color: "green",
      },
    ],
  };
}
