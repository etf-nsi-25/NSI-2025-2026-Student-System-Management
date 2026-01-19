"use client"

import { CForm, CFormSelect, CFormInput, CFormTextarea } from "@coreui/react"
import styles from "./AssignmentManagement.module.css"

interface AssignmentFormData {
  course: string
  name: string
  description: string
  dueDate: Date | null
  maxPoints: string | number
}

interface AssignmentFormProps {
  formData: AssignmentFormData
  errors: Record<string, string>
  onCourseChange: (value: string) => void
  onNameChange: (value: string) => void
  onDescriptionChange: (value: string) => void
  onDueDateChange: (value: Date | null) => void
  onMaxPointsChange: (value: string) => void
}

export default function AssignmentForm({
  formData,
  errors,
  onCourseChange,
  onNameChange,
  onDescriptionChange,
  onDueDateChange,
  onMaxPointsChange,
}: AssignmentFormProps) {
  return (
    <CForm>
      <div className={styles.formGrid}>
        {/* Course Dropdown */}
        <div className={styles.formField}>
          <label className={styles.formLabel}>Course</label>
          <CFormSelect
            value={formData.course}
            onChange={(e) => onCourseChange(e.target.value)}
            className={styles.formSelect}
          >
            <option value="">Select course</option>
            <option value="NSI">NSI</option>
            <option value="ETF">ETF</option>
            <option value="CS101">CS101</option>
          </CFormSelect>
          {errors.course && <span className={styles.errorText}>{errors.course}</span>}
        </div>

        {/* Name Input */}
        <div className={styles.formField}>
          <label className={styles.formLabel}>Name</label>
          <CFormInput
            type="text"
            placeholder="Default input"
            value={formData.name}
            onChange={(e) => onNameChange(e.target.value)}
            className={styles.formInput}
          />
          {errors.name && <span className={styles.errorText}>{errors.name}</span>}
        </div>

        {/* Maximum Points */}
        <div className={styles.formField}>
          <label className={styles.formLabel}>Maximum points</label>
          <CFormInput
            type="number"
            placeholder="Default input"
            value={formData.maxPoints}
            onChange={(e) => onMaxPointsChange(e.target.value)}
            className={styles.formInput}
          />
          {errors.maxPoints && <span className={styles.errorText}>{errors.maxPoints}</span>}
        </div>

        {/* Due Date */}
        <div className={styles.formField}>
          <label className={styles.formLabel}>Due date</label>
          <CFormInput
            type="date"
            value={formData.dueDate ? formData.dueDate.toISOString().split("T")[0] : ""}
            onChange={(e) => onDueDateChange(e.target.value ? new Date(e.target.value) : null)}
            className={styles.formInput}
          />
          {errors.dueDate && <span className={styles.errorText}>{errors.dueDate}</span>}
        </div>

        {/* Description - Full Width */}
        <div className={`${styles.formField} ${styles.formFieldFullWidth}`}>
          <label className={styles.formLabel}>Description</label>
          <CFormTextarea
            placeholder="Default input"
            value={formData.description}
            onChange={(e) => onDescriptionChange(e.target.value)}
            rows={4}
            className={styles.formTextarea}
          />
        </div>
      </div>
    </CForm>
  )
}
