import { useEffect, useState } from 'react';
import { CSpinner } from '@coreui/react';
import { useAPI } from '../../context/services.tsx';
import { getProfessorCourses } from '../../service/professorCoursesService';
import type { ProfessorCourseDTO } from '../../dto/ProfessorCourseDTO';
import type { Course } from '../faculty/courses/types/Course';
import './MyCourses.css';

interface MyCoursesProps {
    className?: string;
}

export default function MyCourses({ className }: MyCoursesProps) {
    const api = useAPI();
    const [courses, setCourses] = useState<ProfessorCourseDTO[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [selectedCourse, setSelectedCourse] = useState<ProfessorCourseDTO | null>(null);
    const [courseDetails, setCourseDetails] = useState<Course | null>(null);
    const [modalVisible, setModalVisible] = useState<boolean>(false);
    const [modalLoading, setModalLoading] = useState<boolean>(false);

    useEffect(() => {
        void (async () => {
            setLoading(true);
            setError(null);
            try {
                const data = await getProfessorCourses(api);
                setCourses(data);
            } catch (err) {
                console.error('Failed to fetch professor courses', err);
                setError('Failed to load courses');
            } finally {
                setLoading(false);
            }
        })();
    }, [api]);

    const handleViewCourse = async (course: ProfessorCourseDTO) => {
        setSelectedCourse(course);
        setModalVisible(true);
        setModalLoading(true);
        try {
            const details = await api.getCourse(course.id);
            setCourseDetails(details);
        } catch (err) {
            console.error('Failed to fetch course details', err);
        } finally {
            setModalLoading(false);
        }
    };

    const handleCloseModal = () => {
        setModalVisible(false);
        setSelectedCourse(null);
        setCourseDetails(null);
    };

    if (loading) {
        return (
            <div className={`my-courses-section ${className || ''}`}>
                <h2 className="my-courses-title">My Courses</h2>
                <div className="my-courses-loading">
                    <CSpinner />
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className={`my-courses-section ${className || ''}`}>
                <h2 className="my-courses-title">My Courses</h2>
                <div className="my-courses-error">
                    {error}
                </div>
            </div>
        );
    }

    if (courses.length === 0) {
        return (
            <div className={`my-courses-section ${className || ''}`}>
                <h2 className="my-courses-title">My Courses</h2>
                <div className="my-courses-empty">
                    No courses assigned
                </div>
            </div>
        );
    }

    return (
        <div className={`my-courses-section ${className || ''}`}>
            <h2 className="my-courses-title">My Courses</h2>
            <div className="my-courses-grid">
                {courses.map((course) => (
                    <div key={course.id} className="course-card">
                        <div className="course-card-header">
                            <h3 className="course-name">{course.name}</h3>
                            <p className="course-code">{course.code}</p>
                        </div>
                        <div className="course-card-body">
                            <span className="student-count">
                                {course.studentCount} {course.studentCount === 1 ? 'student' : 'students'}
                            </span>
                            <span className={`status-badge ${course.status.toLowerCase()}`}>
                                {course.status}
                            </span>
                        </div>
                        <button
                            className="view-course-btn"
                            onClick={() => handleViewCourse(course)}
                        >
                            View Course
                        </button>
                    </div>
                ))}
            </div>

            {modalVisible && (
                <div className="custom-modal-overlay" onClick={handleCloseModal}>
                    <div className="custom-modal" onClick={(e) => e.stopPropagation()}>
                        <div className="custom-modal-header">
                            <h3>Course Details</h3>
                            <button className="custom-modal-close" onClick={handleCloseModal}>×</button>
                        </div>
                        <div className="custom-modal-body">
                            {modalLoading ? (
                                <div className="text-center p-4">
                                    <CSpinner />
                                </div>
                            ) : courseDetails && selectedCourse ? (
                                <div className="course-modal-content">
                                    <div className="course-modal-row">
                                        <span className="course-modal-label">Name:</span>
                                        <span className="course-modal-value">{courseDetails.name}</span>
                                    </div>
                                    <div className="course-modal-row">
                                        <span className="course-modal-label">Code:</span>
                                        <span className="course-modal-value">{courseDetails.code}</span>
                                    </div>
                                    <div className="course-modal-row">
                                        <span className="course-modal-label">Type:</span>
                                        <span className="course-modal-value">{courseDetails.type}</span>
                                    </div>
                                    <div className="course-modal-row">
                                        <span className="course-modal-label">ECTS:</span>
                                        <span className="course-modal-value">{courseDetails.ects}</span>
                                    </div>
                                    <div className="course-modal-row">
                                        <span className="course-modal-label">Students:</span>
                                        <span className="course-modal-value">{selectedCourse.studentCount}</span>
                                    </div>
                                    <div className="course-modal-row">
                                        <span className="course-modal-label">Status:</span>
                                        <span className={`status-badge ${selectedCourse.status.toLowerCase()}`}>
                                            {selectedCourse.status}
                                        </span>
                                    </div>
                                </div>
                            ) : null}
                        </div>
                        <div className="custom-modal-footer">
                            <button className="custom-modal-btn" onClick={handleCloseModal}>
                                Close
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
