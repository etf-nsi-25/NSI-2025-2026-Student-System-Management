import { useState, useEffect, useRef, useCallback } from "react"
import {
    CContainer,
    CRow,
    CCol,
    CCard,
    CCardBody,
    CButton,
    CModal,
    CModalHeader,
    CModalTitle,
    CModalBody,
    CFormInput,
    CInputGroup,
    CInputGroupText,
    CFormSelect,
    CBadge,
    CAlert,
} from "@coreui/react"
import CIcon from "@coreui/icons-react"
import { cilSearch, cilCheckCircle } from "@coreui/icons"
import "./enrollment.css"
import { useAPI } from "../../context/services"
import { enrollInCourse, getCourses, getTeacherForCourse } from "../../service/enrollment/api"
import type { Course } from "../../component/faculty/courses/types/Course"

export default function EnrollmentPage() {
    const api = useAPI()

    const [courses, setCourses] = useState<Course[]>([])
    const [page, setPage] = useState(1)
    const [hasMore, setHasMore] = useState(true)
    const [isLoading, setIsLoading] = useState(false)

    const [searchQuery, setSearchQuery] = useState("")
    const [selectedSemester, setSelectedSemester] = useState("all")

    const [selectedCourse, setSelectedCourse] = useState<Course | null>(null)
    const [showModal, setShowModal] = useState(false)

    const [successMessage, setSuccessMessage] = useState<string | null>(null)
    const [errorMessage, setErrorMessage] = useState<string | null>(null)
    const [isEnrolling, setIsEnrolling] = useState(false)

    const [teacherName, setTeacherName] = useState<string>("Professor")
    const [teacherLoading, setTeacherLoading] = useState(false)

    const observerRef = useRef<IntersectionObserver | null>(null)
    const loadMoreRef = useRef<HTMLDivElement | null>(null)

    const fetchCourses = useCallback(
        async (pageNum: number, search: string, filter: string) => {
            setIsLoading(true)
            try {
                const data = await getCourses(api, pageNum, 6, search, filter)

                if (pageNum === 1) setCourses(data.courses)
                else setCourses((prev) => [...prev, ...data.courses])

                setHasMore(data.hasMore)
            } catch (error) {
                console.error("Error fetching courses:", error)
            } finally {
                setIsLoading(false)
            }
        },
        [api],
    )

    useEffect(() => {
        setCourses([])
        setPage(1)
        setHasMore(true)
        fetchCourses(1, searchQuery, selectedSemester)
    }, [searchQuery, selectedSemester, fetchCourses])

    useEffect(() => {
        if (isLoading || !hasMore) return

        if (observerRef.current) observerRef.current.disconnect()

        observerRef.current = new IntersectionObserver(
            (entries) => {
                if (entries[0].isIntersecting && hasMore && !isLoading) {
                    setPage((prev) => prev + 1)
                }
            },
            { threshold: 0.1 },
        )

        if (loadMoreRef.current) observerRef.current.observe(loadMoreRef.current)

        return () => {
            if (observerRef.current) observerRef.current.disconnect()
        }
    }, [hasMore, isLoading])

    useEffect(() => {
        if (page > 1) fetchCourses(page, searchQuery, selectedSemester)
    }, [page, fetchCourses, searchQuery, selectedSemester])

    const handleEnrollClick = async (course: Course) => {
        setSelectedCourse(course)
        setShowModal(true)

        setTeacherName("Professor")
        setTeacherLoading(true)

        try {
            const teacher = await getTeacherForCourse(api, course.id)
            setTeacherName(teacher?.fullName || "Professor")
        } catch {
            setTeacherName("Professor")
        } finally {
            setTeacherLoading(false)
        }
    }

    const handleConfirmEnrollment = async () => {
        if (!selectedCourse) return

        setIsEnrolling(true)
        try {
            await enrollInCourse(api, selectedCourse.id)

            setCourses((prevCourses) =>
                prevCourses.map((course) =>
                    course.id === selectedCourse.id ? { ...course, status: "enrolled" as const } : course,
                ),
            )

            setSuccessMessage(`You have successfully enrolled in "${selectedCourse.name}"!`)
            setShowModal(false)
            setTimeout(() => setSuccessMessage(null), 5000)
        } catch (error) {
            console.error("Enrollment error:", error)
            setErrorMessage("Failed to enroll. Please try again.")
            setShowModal(false)
            setTimeout(() => setErrorMessage(null), 5000)
        } finally {
            setIsEnrolling(false)
        }
    }

    return (
        <div className="enrollment-page">
            <CContainer fluid>
                <div className="enrollment-header">
                    <div className="header-content">
                        <h1 className="page-title">Course Enrollment</h1>

                        {successMessage && (
                            <CAlert className="ui-alert ui-alert-success" color="success">
                                <CIcon icon={cilCheckCircle} className="success-icon" />
                                <span className="success-text">{successMessage}</span>
                            </CAlert>
                        )}

                        {errorMessage && (
                            <CAlert color="danger" className="ui-alert ui-alert-error">
                                <span className="error-text">{errorMessage}</span>
                            </CAlert>
                        )}
                    </div>

                    <CRow className="g-3 mb-5">
                        <CCol md={8}>
                            <CInputGroup className="search-input-group">
                                <CInputGroupText className="search-icon-wrapper">
                                    <CIcon icon={cilSearch} />
                                </CInputGroupText>
                                <CFormInput
                                    placeholder="Search for a course"
                                    value={searchQuery}
                                    onChange={(e) => setSearchQuery(e.target.value)}
                                    className="search-input"
                                />
                            </CInputGroup>
                        </CCol>

                        <CCol md={4}>
                            <CFormSelect
                                value={selectedSemester}
                                onChange={(e) => setSelectedSemester(e.target.value)}
                                className="semester-select"
                            >
                                <option value="all">Semester: All</option>
                                <option value="mandatory">Mandatory</option>
                                <option value="elective">Elective</option>
                            </CFormSelect>
                        </CCol>
                    </CRow>
                </div>

                <CRow className="g-5">
                    {courses.map((course) => (
                        <CCol key={course.id} lg={4} md={6}>
                            <CCard
                                className={`course-card ${course.type.toLowerCase() === "mandatory" ? "mandatory-card" : "regular-card"
                                    }`}
                            >
                                <CCardBody className="course-card-body">
                                    <h5 className="course-name">{course.name}</h5>
                                    <p className="course-code">{course.code}</p>
                                    <p className="course-professor">{course.professor}</p>

                                    <div className="course-footer">
                                        <span className="course-ects">{course.ects} ECTS</span>
                                        <CBadge className={`status-badge status-${course.type.toLowerCase()}`}>
                                            {course.type}
                                        </CBadge>
                                    </div>


                                    {course.status === "enrolled" ? (
                                        <CButton className="enroll-button enrolled-button" disabled>
                                            Enrolled
                                        </CButton>
                                    ) : (
                                        <CButton className="enroll-button active-button" onClick={() => handleEnrollClick(course)}>
                                            Enroll
                                        </CButton>
                                    )}
                                </CCardBody>
                            </CCard>
                        </CCol>
                    ))}
                </CRow>

                {hasMore && (
                    <div ref={loadMoreRef} className="load-more-trigger">
                        {isLoading && <div className="loading-text">Loading more courses...</div>}
                    </div>
                )}
            </CContainer>

            <CModal
                visible={showModal}
                onClose={() => setShowModal(false)}
                alignment="center"
                size="lg"
                className="modal-super-high-zindex"
            >
                <CModalHeader className="modal-header-custom">
                    <CModalTitle className="modal-title-custom">Course Details</CModalTitle>
                </CModalHeader>

                <CModalBody className="modal-body-custom">
                    {selectedCourse && (
                        <div className="modal-content-wrapper">
                            <h2 className="modal-course-name">{selectedCourse.name}</h2>
                            <p className="modal-course-code">{selectedCourse.code}</p>

                            <p className="modal-professor">
                                <span className="professor-label">Professor:</span>{" "}
                                {teacherLoading ? "Loading..." : teacherName}
                            </p>
                        </div>
                    )}
                </CModalBody>

                <div className="modal-footer-custom">
                    <CButton className="confirm-button" onClick={handleConfirmEnrollment} disabled={isEnrolling}>
                        Confirm enrollment
                    </CButton>
                </div>
            </CModal>
        </div>
    )
}
