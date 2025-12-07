// src/features/student/pages/dashboard/Dashboard.tsx
import { 
  CRow, 
  CCol, 
  CCard, 
  CCardBody, 
  CCardHeader,
  CFormInput,
  CInputGroup,
  CInputGroupText,
  CButton,
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilSearch, cilArrowRight, cilCalendar } from '@coreui/icons';
import { useAuthContext } from '../../init/auth';
import './Dashborad.css';

export function Dashboard() {
  const { authInfo } = useAuthContext();

  const currentMonth = 'November 2025';
  const calendarDays = [
    { day: 28, inactive: true },
    { day: 29, inactive: true },
    { day: 30, inactive: true },
    { day: 1, inactive: true },
    { day: 2 },
    { day: 3 },
    { day: 4, hasEvent: true, eventColor: 'purple' },
    { day: 5, hasEvent: true, eventColor: 'purple' },
    { day: 6 },
    { day: 7 },
    { day: 8 },
    { day: 9 },
    { day: 10 },
    { day: 11 },
    { day: 12 },
    { day: 13, today: true },
    { day: 14 },
    { day: 15 },
    { day: 16 },
    { day: 17, hasEvent: true, eventColor: 'blue' },
    { day: 18 },
    { day: 19 },
    { day: 20, hasEvent: true, eventColor: 'blue' },
    { day: 21 },
    { day: 22 },
    { day: 23 },
    { day: 24 },
    { day: 25, hasEvent: true, eventColor: 'yellow' },
    { day: 26 },
    { day: 27 },
    { day: 28, hasEvent: true, eventColor: 'green' },
    { day: 29 },
  ];

  return (
    <div className="dashboard-container">
      {/* Header */}
      <div className="dashboard-header">
        <h1 className="welcome-title">Welcome back, {authInfo?.fullName?.split(' ')[0] || 'Jane'}!</h1>
        <CInputGroup className="search-bar">
          <CInputGroupText>
            <CIcon icon={cilSearch} />
          </CInputGroupText>
          <CFormInput placeholder="Search courses, grades ..." />
        </CInputGroup>
      </div>

      {/* Stats Cards */}
      <CRow className="stats-row">
        <CCol xs={12} sm={6} lg={3}>
          <CCard className="stat-card">
            <CCardBody className="text-center">
              <div className="stat-value">8.5</div>
              <div className="stat-label">GAP</div>
            </CCardBody>
          </CCard>
        </CCol>
        <CCol xs={12} sm={6} lg={3}>
          <CCard className="stat-card">
            <CCardBody className="text-center">
              <div className="stat-value">6</div>
              <div className="stat-label">Enrolled courses</div>
            </CCardBody>
          </CCard>
        </CCol>
        <CCol xs={12} sm={6} lg={3}>
          <CCard className="stat-card">
            <CCardBody className="text-center">
              <div className="stat-value">92%</div>
              <div className="stat-label">Attendance rate</div>
            </CCardBody>
          </CCard>
        </CCol>
        <CCol xs={12} sm={6} lg={3}>
          <CCard className="stat-card">
            <CCardBody className="text-center">
              <div className="stat-value">3 deadlines</div>
              <div className="stat-label">This week</div>
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>

      <CRow>
        {/* Main Content */}
        <CCol xs={12} lg={8}>
          {/* My Courses */}
          <CCard className="mb-4 courses-card">
            <CCardHeader>
              <h2 className="section-title">My courses</h2>
            </CCardHeader>
            <CCardBody>
              <CRow>
                <CCol xs={12} md={6} lg={4} className="mb-3">
                  <div className="course-card course-card-yellow">
                    <div className="course-title">Tehnologije sigurnosti</div>
                    <div className="course-professor">Prof. Saša Mrdović</div>
                    <CIcon icon={cilArrowRight} className="course-arrow" />
                  </div>
                </CCol>
                <CCol xs={12} md={6} lg={4} className="mb-3">
                  <div className="course-card course-card-blue">
                    <div className="course-title">Metoda i primjene vještačke inteligencije</div>
                    <div className="course-professor">Prof. Amila Akagić</div>
                    <CIcon icon={cilArrowRight} className="course-arrow" />
                  </div>
                </CCol>
                <CCol xs={12} md={6} lg={4} className="mb-3">
                  <div className="course-card course-card-light-blue">
                    <div className="course-title">Napredni softver inžinjering</div>
                    <div className="course-professor">Prof. Samir Omanović</div>
                    <CIcon icon={cilArrowRight} className="course-arrow" />
                  </div>
                </CCol>
                <CCol xs={12} md={6} lg={4} className="mb-3">
                  <div className="course-card course-card-yellow">
                    <div className="course-title">Računarski sistemi u realnom vremenu</div>
                    <div className="course-professor">Prof. Ingmar Bešić</div>
                    <CIcon icon={cilArrowRight} className="course-arrow" />
                  </div>
                </CCol>
                <CCol xs={12} md={6} lg={4} className="mb-3">
                  <div className="course-card course-card-blue">
                    <div className="course-title">Inovacije u projektovanju i menadžmentu informacionih sistema</div>
                    <div className="course-professor">Prof. Almir Karabegović</div>
                    <CIcon icon={cilArrowRight} className="course-arrow" />
                  </div>
                </CCol>
              </CRow>
            </CCardBody>
          </CCard>

          {/* Upcoming This Week */}
          <CCard className="upcoming-card">
            <CCardHeader>
              <h2 className="section-title">Upcoming this week</h2>
            </CCardHeader>
            <CCardBody>
              <div className="upcoming-item">
                <div className="upcoming-info">
                  <CIcon icon={cilCalendar} className="upcoming-icon" />
                  <div>
                    <div className="upcoming-title">Tehnologije sigurnosti - Criptography task</div>
                    <div className="upcoming-date">Wednesday</div>
                  </div>
                </div>
                <CButton color="primary" size="sm">Submit</CButton>
              </div>
              <div className="upcoming-item">
                <div className="upcoming-info">
                  <CIcon icon={cilCalendar} className="upcoming-icon" />
                  <div>
                    <div className="upcoming-title">Napredni softver inžinjering - Project task</div>
                    <div className="upcoming-date">Friday</div>
                  </div>
                </div>
                <CButton color="primary" size="sm">Submit</CButton>
              </div>
              <div className="upcoming-item">
                <div className="upcoming-info">
                  <CIcon icon={cilCalendar} className="upcoming-icon" />
                  <div>
                    <div className="upcoming-title">Napredni softver inžinjering - Tutorial task</div>
                    <div className="upcoming-date">Sunday</div>
                  </div>
                </div>
                <CButton color="primary" size="sm">Submit</CButton>
              </div>
            </CCardBody>
          </CCard>
        </CCol>

        {/* Sidebar */}
        <CCol xs={12} lg={4}>
          {/* Calendar */}
          <CCard className="mb-4 calendar-card">
            <CCardHeader>
              <div className="calendar-header">
                <span>{currentMonth}</span>
              </div>
            </CCardHeader>
            <CCardBody>
              <div className="calendar-grid">
                <div className="calendar-weekdays">
                  {['S', 'M', 'T', 'W', 'T', 'F', 'S'].map((day, i) => (
                    <div key={i} className="weekday">{day}</div>
                  ))}
                </div>
                <div className="calendar-days">
                  {calendarDays.map((item, i) => (
                    <div 
                      key={i} 
                      className={`calendar-day ${item.inactive ? 'inactive' : ''} ${item.today ? 'today' : ''} ${item.hasEvent ? 'has-event' : ''}`}
                      style={item.hasEvent ? { backgroundColor: `var(--event-${item.eventColor})` } : {}}
                    >
                      {item.day}
                    </div>
                  ))}
                </div>
              </div>
            </CCardBody>
          </CCard>

          {/* Quick Actions */}
          <CCard className="quick-actions-card">
            <CCardHeader>
              <h3 className="section-title">Quick Actions</h3>
            </CCardHeader>
            <CCardBody>
              <div className="quick-action-item">
                <div className="action-icon action-icon-purple">
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="white">
                    <path d="M14 2H6C4.9 2 4 2.9 4 4V20C4 21.1 4.9 22 6 22H18C19.1 22 20 21.1 20 20V8L14 2ZM6 20V4H13V9H18V20H6Z"/>
                  </svg>
                </div>
                <span className="action-text">Submit assignment</span>
              </div>
              <div className="quick-action-item">
                <div className="action-icon action-icon-blue">
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="white">
                    <path d="M14 2H6C4.9 2 4 2.9 4 4V20C4 21.1 4.89 22 6 22H18C19.1 22 20 21.1 20 20V8L14 2ZM16 18H8V16H16V18ZM16 14H8V12H16V14ZM13 9V3.5L18.5 9H13Z"/>
                  </svg>
                </div>
                <span className="action-text">View transcript</span>
              </div>
              <div className="quick-action-item">
                <div className="action-icon action-icon-green">
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="white">
                    <path d="M12 2C6.48 2 2 6.48 2 12C2 17.52 6.48 22 12 22C17.52 22 22 17.52 22 12C22 6.48 17.52 2 12 2ZM12 17C11.45 17 11 16.55 11 16V12C11 11.45 11.45 11 12 11C12.55 11 13 11.45 13 12V16C13 16.55 12.55 17 12 17ZM13 9H11V7H13V9Z"/>
                  </svg>
                </div>
                <span className="action-text">Download certificate</span>
              </div>
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>
    </div>
  );
}