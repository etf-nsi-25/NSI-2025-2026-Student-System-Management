import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { CRow, CCol, CCard, CCardBody } from '@coreui/react';
import type { CourseOverviewDTO } from '../../dto/CourseOverviewDTO';
import { ExamsSection } from '../../component/ExamsSection';
import { AssignmentsSection } from '../../component/AssignmentsSection';
import { useAPI } from '../../context/services'; 
import { useToast } from '../../context/toast';


const CourseOverviewDashboard = () => {
    const { courseId } = useParams<{ courseId: string }>();
    const api = useAPI();
    const { pushToast } = useToast();
    
    const [courseData, setCourseData] = useState<CourseOverviewDTO | null>(null);

    const brandBlue = '#133E87'; 


    useEffect(() => {
        const fetchCourseData = async () => {
            if (!courseId) return;
            try {
                const data = await api.getCourseOverview(courseId);
                setCourseData(data);
            } catch (error) {
          
                pushToast("error", "Connection Error", "Failed to fetch course data.");
            }
        };
        fetchCourseData();
    }, [courseId, api, pushToast]);

  
    const attendance = courseData?.attendance || { present: 0, total: 0, percentage: 0 };
    const progress = courseData?.progress || 0;

    return (
        <div className="p-4">
            
  
            <CRow className="mb-3">
                <CCol md={9}>
                    <h1 className="fw-bold mb-0" style={{ color: brandBlue, fontSize: '2.8rem' }}>
                        {courseData?.name || '[Course Name]'}
                    </h1>
                    <p className="text-primary text-decoration-underline mb-2" style={{ cursor: 'pointer' }}>
                        {courseData?.professor || 'Loading professor...'}
                    </p>
                    <div className="fw-bold text-dark">{courseData?.ects || 0} ECTS</div>
                </CCol>
                
           
                <CCol md={3} className="d-flex justify-content-end align-items-center">
                    <CCard className="border-0 shadow-sm rounded-4 text-center py-3" style={{ minWidth: '160px' }}>
                        <CCardBody>
                            <h5 className="fw-bold mb-1" style={{ color: brandBlue }}>Attendance</h5>
                            <h2 className="fw-bold mb-0" style={{ color: brandBlue }}>
                                {attendance.present}/{attendance.total}
                            </h2>
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>

    
            <CRow className="mb-5">
                <CCol md={8}>
                    <div className="bg-white p-2 rounded-pill shadow-sm d-flex align-items-center" style={{ height: '35px' }}>
                        <div className="flex-grow-1 mx-3" style={{ backgroundColor: '#f0f0f0', height: '10px', borderRadius: '10px' }}>
                            <div style={{ 
                                width: `${progress}%`, 
                                backgroundColor: '#68B99A', 
                                height: '100%', 
                                borderRadius: '10px',
                                transition: 'width 1.5s ease-in-out'
                            }} />
                        </div>
                        <span className="me-3 small fw-bold text-muted">{progress}%</span>
                    </div>
                </CCol>
            </CRow>

      
            <div className="mb-4">
                <ExamsSection 
                    exams={courseData?.exams || []} 
                    brandBlue={brandBlue} 
                    colors={{ danger: '#dc3545' }} 
                />
            </div>

            <div className="mb-4">
                <AssignmentsSection 
                    assignments={courseData?.assignments || []} 
                    brandBlue={brandBlue} 
                />
            </div>
        </div>
    );
};

export default CourseOverviewDashboard;