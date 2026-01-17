"use client"

import { useState, useEffect, useRef, useCallback } from "react"
import { CButton, CModal, CModalHeader, CModalTitle, CModalBody, CModalFooter, CAlert } from "@coreui/react"
import styles from "./AssignmentManagement.module.css"
import type { Assignment, AssignmentDTO } from "../../models/assignments/Assignments.types"
import mockAPI from "./mockApi"
import AssignmentForm from "./AssignmentForm"
//import { useAPI } from "../../context/services" uncomment when BE is ready
import CIcon from "@coreui/icons-react"
import { cilCheckCircle } from "@coreui/icons"

export default function AssignmentManagement() {
  //const api = useAPI(); uncomment when BE is ready

  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [course, setCourse] = useState("")
  const [name, setName] = useState("")
  const [description, setDescription] = useState("")
  const [dueDate, setDueDate] = useState<Date | null>(null)
  const [maxPoints, setMaxPoints] = useState("")

  const [assignments, setAssignments] = useState<Assignment[]>([])
  const [searchQuery, setSearchQuery] = useState("")
  const [page, setPage] = useState(1)
  const [hasMore, setHasMore] = useState(true)
  const [loading, setLoading] = useState(false)
  const pageSize = 10

  // Edit modal state
  const [showEditModal, setShowEditModal] = useState(false)
  const [editingAssignment, setEditingAssignment] = useState<Assignment | null>(null)
  const [editForm, setEditForm] = useState<AssignmentDTO>({
    course: "",
    name: "",
    description: "",
    dueDate: new Date(),
    maxPoints: 0,
  })

  // Delete confirmation modal state
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [deletingAssignmentId, setDeletingAssignmentId] = useState<string | null>(null)

  // Validation errors
  const [errors, setErrors] = useState<Record<string, string>>({})

  const observerTarget = useRef<HTMLDivElement>(null)

  const loadAssignments = useCallback(
    async (pageNum: number, query: string, reset = false) => {
      if (loading) return

      setLoading(true)
      try {

        setErrorMessage(null);
        setSuccessMessage(null);
        // Mock API call - replace with: await api.getAllAssignments(query, pageSize, pageNum)

        const response = await mockAPI.getAssignments({ query, page: pageNum, pageSize })

        setAssignments((prev) => (reset ? response.data : [...prev, ...response.data]))
        setHasMore(response.hasMore)
        setPage(pageNum)
      } catch (error) {
        console.error("Failed to load assignments:", error)
      } finally {
        setLoading(false)
      }
    },
    [loading, pageSize],
  )

  useEffect(() => {
    loadAssignments(1, "", true)
  }, [])

  useEffect(() => {
    const timer = setTimeout(() => {
      setAssignments([])
      setPage(1)
      setHasMore(true)
      loadAssignments(1, searchQuery, true)
    }, 300)

    return () => clearTimeout(timer)
  }, [searchQuery])

  useEffect(() => {
    if (!successMessage && !errorMessage) return;

    const timer = setTimeout(() => {
      setSuccessMessage(null);
      setErrorMessage(null);
    }, 3000); // 3 seconds

    return () => clearTimeout(timer);
  }, [successMessage, errorMessage]);

  useEffect(() => {
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting && hasMore && !loading) {
          loadAssignments(page + 1, searchQuery, false)
        }
      },
      { threshold: 0.1 },
    )

    const currentTarget = observerTarget.current
    if (currentTarget) {
      observer.observe(currentTarget)
    }

    return () => {
      if (currentTarget) {
        observer.unobserve(currentTarget)
      }
    }
  }, [hasMore, loading, page, searchQuery, loadAssignments])

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {}

    if (!course) {
      newErrors.course = "Course is required"
    }
    if (!name.trim()) {
      newErrors.name = "Name is required"
    }
    if (!dueDate) {
      newErrors.dueDate = "Due date is required"
    } else if (dueDate <= new Date()) {
      newErrors.dueDate = "Due date must be in the future"
    }
    if (!maxPoints || Number.parseFloat(maxPoints) <= 0) {
      newErrors.maxPoints = "Max points must be a positive number"
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleCreate = async () => {
    if (!validateForm()) {
      return
    }

    const dto: AssignmentDTO = {
      course,
      name,
      description,
      dueDate: dueDate!,
      maxPoints: Number.parseFloat(maxPoints),
    }

    try {

      setErrorMessage(null);
      setSuccessMessage(null);
      // Mock API call - replace with: await api.createAssignment(dto)
      await mockAPI.createAssignment(dto)

      setAssignments([])
      setPage(1)
      setHasMore(true)
      await loadAssignments(1, searchQuery, true)

      // Reset form
      setCourse("")
      setName("")
      setDescription("")
      setDueDate(null)
      setMaxPoints("")
      setErrors({})
      setSuccessMessage("Assignment created successfully!")
    } catch (error) {
      console.error("Failed to create assignment:", error)
      setErrorMessage("Failed to create assignment")
    }
  }

  const handleEdit = (assignment: Assignment) => {
    setEditingAssignment(assignment)
    setEditForm({
      course: assignment.course,
      name: assignment.name,
      description: assignment.description,
      dueDate: new Date(assignment.dueDate),
      maxPoints: assignment.maxPoints,
    })
    setShowEditModal(true)
  }

  const handleSaveEdit = async () => {
    if (!editingAssignment) return

    try {

      setErrorMessage(null);
      setSuccessMessage(null);
      // Mock API call - replace with: await api.updateAssignment(editingAssignment.id, editForm)
      await mockAPI.updateAssignment(editingAssignment.id, editForm)

      setAssignments([])
      setPage(1)
      setHasMore(true)
      await loadAssignments(1, searchQuery, true)

      // Close modal
      setShowEditModal(false)
      setEditingAssignment(null)

      setSuccessMessage("Assignment updated successfully!")
    } catch (error) {
      console.error("Failed to update assignment:", error)
      setErrorMessage("Failed to update assignment")
    }
  }

  const handleDeleteClick = (id: string) => {
    setDeletingAssignmentId(id)
    setShowDeleteModal(true)
  }

  const handleConfirmDelete = async () => {
    if (!deletingAssignmentId) return

    try {
      setErrorMessage(null);
      setSuccessMessage(null);
      // Mock API call - replace with: await api.deleteAssignment(deletingAssignmentId)
      await mockAPI.deleteAssignment(deletingAssignmentId)
      await loadAssignments(1, searchQuery, true)

      // Close modal
      setShowDeleteModal(false)
      setDeletingAssignmentId(null)

      setSuccessMessage("Assignment deleted successfully!")
    } catch (error) {
      console.error("Failed to delete assignment:", error)
      setErrorMessage("Failed to delete assignment")
    }
  }

  return (
    <div className={styles.assignmentContainer}>
      {successMessage && (
        <CAlert
          color="success"
          className="ui-alert ui-alert-success"
        >
          <CIcon icon={cilCheckCircle} className="success-icon" />
          <span className="success-text">{successMessage}</span>
        </CAlert>
      )}

      {errorMessage && (
        <CAlert
          color="danger"
          className="ui-alert ui-alert-error"
        >
          <span className="error-text">{errorMessage}</span>
        </CAlert>
      )}

      <div className={styles.contentWrapper}>

        <h1 className={styles.pageTitle}>Assignment management</h1>

        {/* Create Assignments Section */}
        <div className={styles.sectionCard}>
          <div className={styles.sectionCardBody}>
            <h2 className={styles.sectionTitle}>Create Assignments</h2>
            <AssignmentForm
              formData={{ course, name, description, dueDate, maxPoints }}
              errors={errors}
              onCourseChange={setCourse}
              onNameChange={setName}
              onDescriptionChange={setDescription}
              onDueDateChange={setDueDate}
              onMaxPointsChange={setMaxPoints}
            />

            <div className={styles.buttonContainer}>
              <button onClick={handleCreate} className={styles.createButton}>
                Create
              </button>
            </div>
          </div>
        </div>

        {/* Edit Assignments Section */}
        <div className={styles.sectionCard}>
          <div className={styles.sectionCardBody}>
            <h2 className={styles.sectionTitle}>Edit Assignments</h2>

            <div className={styles.searchContainer}>
              <span className={styles.searchIcon}>
                <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path
                    d="M9 17A8 8 0 1 0 9 1a8 8 0 0 0 0 16zM19 19l-4.35-4.35"
                    stroke="currentColor"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
              </span>
              <input
                type="text"
                placeholder="Search"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className={styles.searchInput}
              />
            </div>

            <div className={styles.tableContainer}>
              <table className={styles.table}>
                <thead className={styles.tableHeader}>
                  <tr>
                    <th className={styles.tableHeaderCell}>Course</th>
                    <th className={styles.tableHeaderCell}>Name</th>
                    <th className={styles.tableHeaderCell}>Faculty</th>
                    <th className={styles.tableHeaderCell}>Max points</th>
                    <th className={styles.tableHeaderCell}>Major</th>
                    <th className={`${styles.tableHeaderCell} ${styles.actionsCell}`}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {assignments.map((assignment) => (
                    <tr key={assignment.id} className={styles.tableRow}>
                      <td className={styles.tableCell}>{assignment.course}</td>
                      <td className={styles.tableCell}>{assignment.name}</td>
                      <td className={styles.tableCell}>{assignment.faculty}</td>
                      <td className={styles.tableCell}>{assignment.maxPoints}</td>
                      <td className={styles.tableCell}>{assignment.major}</td>
                      <td className={`${styles.tableCell} ${styles.actionsCell}`}>
                        <div className={styles.actionButtonsContainer}>
                          <button
                            onClick={() => handleEdit(assignment)}
                            className={`${styles.actionButton} ${styles.editButton}`}
                            title="Edit"
                            type="button"
                          >
                            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                              <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth={2}
                                d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
                              />
                            </svg>
                          </button>
                          <button
                            onClick={() => handleDeleteClick(assignment.id)}
                            className={`${styles.actionButton} ${styles.deleteButton}`}
                            title="Delete"
                            type="button"
                          >
                            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                              <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth={2}
                                d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                              />
                            </svg>
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
              <div ref={observerTarget} className={styles.scrollTrigger}>
                {loading && <div className={styles.loadingIndicator}>Loading more...</div>}
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Edit Modal */}
      <CModal visible={showEditModal} onClose={() => setShowEditModal(false)} size="lg" alignment="center"
        backdrop={false}
        focus={false}>
        <CModalHeader>
          <CModalTitle>Edit Assignment</CModalTitle>
        </CModalHeader>
        <CModalBody>
          <AssignmentForm
            formData={editForm}
            errors={{}}
            onCourseChange={(value: any) => setEditForm({ ...editForm, course: value })}
            onNameChange={(value: any) => setEditForm({ ...editForm, name: value })}
            onDescriptionChange={(value: any) => setEditForm({ ...editForm, description: value })}
            onDueDateChange={(value: any) => setEditForm({ ...editForm, dueDate: value || new Date() })}
            onMaxPointsChange={(value: any) => setEditForm({ ...editForm, maxPoints: Number.parseFloat(value) || 0 })}
          />
        </CModalBody>
        <CModalFooter>
          <CButton color="secondary" onClick={() => setShowEditModal(false)}>
            Cancel
          </CButton>
          <CButton color="primary" onClick={handleSaveEdit}>
            Save Changes
          </CButton>
        </CModalFooter>
      </CModal>

      {/* Delete Confirmation Modal */}
      <CModal visible={showDeleteModal} onClose={() => setShowDeleteModal(false)} alignment="center"
        backdrop={false}
        focus={false}>
        <CModalHeader>
          <CModalTitle>Confirm Delete</CModalTitle>
        </CModalHeader>
        <CModalBody>Are you sure you want to delete this assignment? This action cannot be undone.</CModalBody>
        <CModalFooter>
          <CButton color="secondary" onClick={() => setShowDeleteModal(false)}>
            Cancel
          </CButton>
          <CButton color="danger" onClick={handleConfirmDelete}>
            Delete
          </CButton>
        </CModalFooter>
      </CModal>
    </div>
  )
}