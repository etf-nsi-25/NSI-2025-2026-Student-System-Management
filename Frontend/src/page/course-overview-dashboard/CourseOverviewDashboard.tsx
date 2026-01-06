import {
  CRow,
  CCol,
  CButton,
  CBadge,
} from '@coreui/react';

const CourseOverviewDashboard = () => {
  const brandBlue = '#133E87'; 
  const pageBackground = '#d7e6f2';
  const attendanceThreshold = 80;

  const colors = {
    success: '#68B99A',
    danger: '#dc3545',  
  };


const getProgressColor = (percent: number) => {
  if (percent >= 75) return '#68B99A'; 
  if (percent >= 40) return '#f9b115'; 
  return '#dc3545';                   
}
  const courseData = {
    name: "[Course Name]",
    professor: "Prof. H S",
    ects: 6,
    progress: 77,
    attendance: { 
      present: 19, 
      total: 22,
      percentage: Math.round((19 / 22) * 100) 
    },
    exams: [
      { id: 1, name: "Exam 1", status: "COMPLETED", date: "Exam date" },
      { id: 2, name: "Exam 2", status: "UPCOMING", date: "Exam date", registerable: true },
      { id: 3, name: "Exam 3", status: "FAILED", date: "Exam date" }
    ],
    assignments: [
      { id: 1, name: "Zadatak 3", desc: "Description", dueDate: "23.11.2025 23:59", status: "todo" },
      { id: 2, name: "Zadatak 2", desc: "Feedback", status: "marked", points: "2 points" }
    ]
  };

  const hasFailedExam = courseData.exams.some(e => e.status === 'FAILED');
  const isAttendanceLow = courseData.attendance.percentage < attendanceThreshold;
  const barColor = getProgressColor(courseData.progress);
const getCourseStatus = () => {

  if (courseData.progress === 100 && !hasFailedExam) {
    return { label: 'Completed', color: 'success', barColor: colors.success };
  }
  
  if (isAttendanceLow || hasFailedExam) {
    return { 
      label: 'At Risk', 
      color: 'danger', 
      barColor: colors.danger 
    };
  }
  return { label: 'In Progress', color: 'info', barColor: colors.success };
};

  const status = getCourseStatus();
  const attendanceColor = isAttendanceLow ? '#dc3545' : '#2eb85c';

  return (
    <div className="p-5" style={{ backgroundColor: pageBackground, minHeight: '100vh' }}>
      
      {/* 1. STATUS INDICATOR */}
      <div className="mb-4 d-flex justify-content-start">
        <CBadge color={status.color} shape="pill" className="px-4 py-2 shadow-sm" style={{ fontSize: '0.9rem' }}>
          STATUS: {status.label.toUpperCase()}
        </CBadge>
      </div>

      {/* 2. HEADER SEKCIJA */}
      <CRow className="mb-4 align-items-start">
        <CCol md={8}>
          <h1 className="fw-bold" style={{ color: brandBlue, fontSize: '2.5rem' }}>{courseData.name}</h1>
          <p className="fw-bold mb-1" style={{ color: '#17a2b8', fontSize: '1.2rem' }}>{courseData.professor}</p>
          <p className="text-muted fw-bold">{courseData.ects} ECTS</p>
        </CCol>
        
        <CCol md={4} className="d-flex justify-content-end">
          <div className="bg-white p-3 rounded-4 shadow-sm text-center border-0" style={{ minWidth: '180px' }}>
            <h5 className="fw-bold" style={{ color: brandBlue }}>Attendance</h5>
            <h2 className="mb-0 fw-bold" style={{ color: attendanceColor, fontSize: '2.5rem' }}>
              {courseData.attendance.present}/{courseData.attendance.total}
            </h2>
            <small className="fw-bold" style={{ color: attendanceColor }}>{courseData.attendance.percentage}%</small>
          </div>
        </CCol>
      </CRow>

   {/* 3. OVERALL PROGRESS SECTION */}
<div className="bg-white p-3 rounded-pill shadow-sm mb-5 d-flex align-items-center" style={{ height: '70px' }}>
  <div className="flex-grow-1 px-4 d-flex align-items-center">
    <div className="w-100" style={{ backgroundColor: '#e9ecef', height: '8px', borderRadius: '10px' }}>
      <div 
        style={{ 
          width: `${courseData.progress}%`, 
          backgroundColor: barColor,
          height: '100%', 
          borderRadius: '10px',
          transition: 'width 1s ease-in-out' 
        }} 
      />
    </div>
    <span className="ms-3 fw-bold text-muted" style={{ fontSize: '0.9rem' }}>
      {courseData.progress}%
    </span>
  </div>
</div>
   

      {/* 4. EXAMS SECTION  */}
      <div className="bg-white p-4 rounded-4 shadow-sm mb-4 border-0">
        <h3 className="mb-4 fw-bold" style={{ color: brandBlue }}>Exams</h3>
        {courseData.exams.map((exam) => (
          <div key={exam.id} className="d-flex align-items-center mb-3 p-3 rounded-3 shadow-sm" style={{ backgroundColor: '#f0f7fa' }}>
            <div className="rounded-3 d-flex align-items-center justify-content-center text-white fw-bold me-3" 
                 style={{ width: '55px', height: '45px', backgroundColor: exam.status === 'FAILED' ? '#dc3545' : brandBlue, fontSize: '1.2rem' }}>
              {exam.status === 'FAILED' ? '!' : '✓'}
            </div>
            <div className="flex-grow-1">
              <h6 className="mb-0 fw-bold" style={{ fontSize: '1.1rem' }}>{exam.name}</h6>
            </div>
            <div className="text-end">
              <div className="small text-muted mb-1 fw-bold">• {exam.date}</div>
              {exam.registerable && (
                <CButton size="sm" className="text-white px-4 shadow-sm" style={{ backgroundColor: '#4b39ef', border: 'none', borderRadius: '6px' }}>
                  Register
                </CButton>
              )}
              {exam.status === 'FAILED' && <CBadge color="danger" className="ms-2">Failed</CBadge>}
              {exam.status === 'COMPLETED' && <CBadge color="success" className="ms-2">Completed</CBadge>}
            </div>
          </div>
        ))}
      </div>

      {/* 5. ASSIGNMENTS SECTION */}
      <div className="bg-white p-4 rounded-4 shadow-sm border-0">
        <h3 className="mb-4 fw-bold" style={{ color: brandBlue }}>Assignments</h3>
        {courseData.assignments.map((task) => (
          <div key={task.id} className="d-flex align-items-center mb-3 p-3 rounded-3 shadow-sm" style={{ backgroundColor: '#f0f7fa' }}>
            <div className="text-white rounded-3 fw-bold me-4 d-flex align-items-center justify-content-center" 
                 style={{ minWidth: '70px', height: '45px', backgroundColor: brandBlue }}>
              Max
            </div>
            <div className="flex-grow-1">
              <h6 className="mb-0 fw-bold" style={{ fontSize: '1.1rem' }}>{task.name}</h6>
              <small className="text-muted fw-semibold">{task.desc}</small>
            </div>
            <div className="text-end small">
              <div className="text-muted mb-1 fw-bold">• {task.status === 'todo' ? `Due ${task.dueDate}` : 'Marked'}</div>
              {task.status === 'todo' && (
                <CButton size="sm" className="text-white px-4 shadow-sm" style={{ backgroundColor: '#4b39ef', border: 'none', borderRadius: '6px' }}>
                  Submit
                </CButton>
              )}
              {task.points && <div className="fw-bold h6 mb-0" style={{ color: brandBlue }}>{task.points}</div>}
            </div>
          </div>
        ))}
      </div>

    </div>
  );
};

export default CourseOverviewDashboard;