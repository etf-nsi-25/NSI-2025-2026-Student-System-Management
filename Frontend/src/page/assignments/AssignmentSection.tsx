"use client"

import React, { useEffect, useState } from "react"
import { CCard, CCardBody, CSpinner } from "@coreui/react"
import { AssignmentCard } from "./AssignmentCard"
import { useAPI } from "../../context/services"
import type { Assignment } from "./AssignmentTypes"
import styles from "./Assignments.module.css"

interface AssignmentsSectionProps {
    courseId: string
}

export const AssignmentsSection: React.FC<AssignmentsSectionProps> = ({ courseId }) => {
    const api = useAPI()
    const [assignments, setAssignments] = useState<Assignment[]>([])
    const [loading, setLoading] = useState<boolean>(true)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        let isMounted = true

        const loadData = async () => {
            try {
                setLoading(true)
                setError(null)
                const data = await api.getMyAssignmentsForCourse(courseId)

                if (isMounted) {
                    setAssignments(data)
                }
            } catch (err) {
                if (isMounted) {
                    setError("Failed to load assignments from the server.")
                    console.error("API Error:", err)
                }
            } finally {
                if (isMounted) {
                    setLoading(false)
                }
            }
        }

        if (courseId) {
            loadData()

        }

        return () => {
            isMounted = false
        }
    }, [api, courseId])

    if (loading) {
        return (
            <div className={styles.container}>
                <CCard className={styles.mainCard}>
                    <CCardBody className={styles.loadingBody}>
                        <CSpinner color="primary" variant="grow" />
                        <p className={styles.loadingText}>Fetching Course Assignments...</p>
                    </CCardBody>
                </CCard>
            </div>
        )
    }

    if (error) {
        return (
            <div className={styles.container}>
                <CCard className={styles.mainCard}>
                    <CCardBody className={styles.loadingBody}>
                        <p className="text-danger font-weight-bold">{error}</p>
                        <button
                            className="btn btn-outline-primary btn-sm mt-2"
                            onClick={() => window.location.reload()}
                        >
                            Retry
                        </button>
                    </CCardBody>
                </CCard>
            </div>
        )
    }

    return (
        <div className={styles.container}>
            <CCard className={styles.mainCard}>
                <CCardBody className={styles.mainCardBody}>
                    <div className="d-flex justify-content-between align-items-center mb-4">
                        <h2 className={styles.sectionTitle}>Assignments</h2>
                        <span className="badge bg-info text-white rounded-pill px-3">
                            {assignments.length} Total
                        </span>
                    </div>

                    <div className={styles.assignmentsList}>
                        {assignments.length > 0 ? (
                            assignments.map((item) => (
                                <AssignmentCard
                                    key={item.assignmentId}
                                    assignment={item}
                                    onSubmit={(id) => console.log("Submitting assignment ID:", id)}
                                />
                            ))
                        ) : (
                            <div className={styles.emptyState}>
                                <div className="mb-3 opacity-50">
                                    <svg width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round">
                                        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path>
                                        <polyline points="14 2 14 8 20 8"></polyline>
                                        <line x1="16" y1="13" x2="8" y2="13"></line>
                                        <line x1="16" y1="17" x2="8" y2="17"></line>
                                        <polyline points="10 9 9 9 8 9"></polyline>
                                    </svg>
                                </div>
                                <p className="font-weight-bold text-muted">No assignments found for this course.</p>
                            </div>
                        )}
                    </div>
                </CCardBody>
            </CCard>
        </div>
    )
}