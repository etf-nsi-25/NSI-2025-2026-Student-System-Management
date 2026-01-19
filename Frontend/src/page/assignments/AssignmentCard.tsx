

import type React from "react"
import { CCard, CCardBody } from "@coreui/react"
import type { Assignment } from "./AssignmentTypes"
import { formatDate } from "./AssignmentTypes"
import styles from "./Assignments.module.css"

interface AssignmentCardProps {
    assignment: Assignment
    onSubmit: (id: number) => void
}

export const AssignmentCard: React.FC<AssignmentCardProps> = ({ assignment }) => {
    const { title, description, dueDate, status, submissionDate, points, feedback } = assignment

    const initials = title?.split(" ")[0]?.slice(0, 3) || "Asg"

    const bodyText = (status === "Graded" && feedback) ? feedback : description

    return (
        <CCard className={styles.assignmentCard}>
            <CCardBody className={styles.cardBody}>

                <div className={styles.leftSection}>
                    <div className={styles.avatar}>{initials}</div>
                    <div className={styles.contentWrapper}>
                        <h6 className={styles.title}>{title}</h6>
                        <span className={styles.subtitle}>{bodyText}</span>
                    </div>
                </div>


                <div className={styles.rightSection}>
                    {status === "Pending" && (
                        <>
                            <div className={styles.statusRow}>
                                <span className={styles.statusDot} />
                                <span className={styles.statusText}>Status: Pending</span>
                                <span className={styles.dueDateText}>Due: {formatDate(dueDate)}</span>
                            </div>

                            {/* <CButton color="primary" size="sm" className={styles.submitButton} onClick={() => onSubmit(assignmentId)}>
                                Submit
                            </CButton> */}
                        </>
                    )}

                    {status === "Submitted" && (
                        <div className={styles.statusRow}>
                            <span className={styles.statusDot} />
                            <span className={styles.statusText}>
                                Submitted - {submissionDate ? formatDate(submissionDate) : "Pending review"}
                            </span>
                        </div>
                    )}

                    {status === "Graded" && (
                        <>
                            <div className={styles.statusRow}>
                                <span className={styles.statusDot} />
                                <span className={styles.statusText}>
                                    Marked - Submitted on {submissionDate ? formatDate(submissionDate).split(" ")[0] : "xx.xx.xxxx"}
                                </span>
                            </div>
                            <span className={styles.pointsText}>{points} points</span>
                        </>
                    )}

                    {status === "Missed" && (
                        <>
                            <div className={styles.statusRow}>
                                <span className={styles.statusDot} />
                                <span className={styles.statusText}>Due date {formatDate(dueDate)}</span>
                            </div>
                            <span className={styles.missedText}>Missed</span>
                        </>
                    )}
                </div>
            </CCardBody>
        </CCard>
    )
}
