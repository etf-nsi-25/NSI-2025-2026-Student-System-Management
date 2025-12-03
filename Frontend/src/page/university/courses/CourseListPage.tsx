import { useState } from "react";
import {
  CCard,
  CCardBody,
  CCol,
  CRow,
  CFormInput,
  CButton,
  CModal,
  CModalHeader,
  CModalTitle,
  CModalBody,
  CModalFooter,
} from "@coreui/react";

import type { Course } from "../../../component/faculty/courses/types/Course";
import CourseTable from "../../../component/faculty/courses/CourseTable";
import CreateCourseModal from "../../../component/faculty/courses/CreateCourseModal";
import EditCourseModal from "../../../component/faculty/courses/EditCourseModal";

const CourseListPage = () => {
  const [search, setSearch] = useState("");
  const [showCreate, setShowCreate] = useState(false);
  const [editingCourse, setEditingCourse] = useState<Course | null>(null);

  const [showConfirmDelete, setShowConfirmDelete] = useState(false);
  const [courseToDelete, setCourseToDelete] = useState<Course | null>(null);

  const [courses, setCourses] = useState<Course[]>([
    {
      id: "35009",
      name: "Computer Vision",
      type: "Mandatory",
      code: "C065",
      programId: "2",
      ects: 6,
    },
    {
      id: "35009",
      name: "NASP",
      type: "Mandatory",
      code: "C065",
      programId: "2",
      ects: 6,
    },
  ]);

  const handleCreate = (newCourse: Course) => {
    setCourses((prev) => [...prev, newCourse]);
    setShowCreate(false);
  };

  const handleSaveEdit = (updated: Course) => {
    setCourses((prev) => prev.map((c) => (c.id === updated.id ? updated : c)));
    setEditingCourse(null);
  };

  const requestDeleteCourse = (course: Course) => {
    setCourseToDelete(course);
    setShowConfirmDelete(true);
  };

  const handleDeleteConfirmed = () => {
    if (!courseToDelete) return;

    setCourses((prev) => prev.filter((c) => c.id !== courseToDelete.id));
    setShowConfirmDelete(false);
    setCourseToDelete(null);
  };

  return (
    <CRow className="mt-4">
      <CCol xs={12}>
        <CCard>
          <CCardBody>
            <h3 className="mb-4">Course Management</h3>

            <div className="d-flex justify-content-end mb-4">
              <CButton color="primary" onClick={() => setShowCreate(true)}>
                Create Course
              </CButton>
            </div>

            <div className="mb-3">
              <CFormInput
                placeholder="Search courses..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
              />
            </div>

            <CourseTable
              search={search}
              courses={courses}
              onEdit={(course) => setEditingCourse(course)}
              onDelete={requestDeleteCourse}
            />
          </CCardBody>
        </CCard>
      </CCol>

      <CreateCourseModal
        visible={showCreate}
        onClose={() => setShowCreate(false)}
        onCreate={handleCreate}
      />

      <EditCourseModal
        visible={!!editingCourse}
        course={editingCourse}
        onClose={() => setEditingCourse(null)}
        onSave={handleSaveEdit}
      />

      <CModal
        visible={showConfirmDelete}
        onClose={() => setShowConfirmDelete(false)}
      >
        <CModalHeader closeButton>
          <CModalTitle>Confirm Deletion</CModalTitle>
        </CModalHeader>

        <CModalBody>
          {courseToDelete
            ? `Are you sure you want to delete the course "${courseToDelete.name}"?`
            : "Are you sure you want to delete this course?"}
        </CModalBody>

        <CModalFooter className="d-flex justify-content-end gap-2">
          <CButton
            color="secondary"
            variant="outline"
            onClick={() => setShowConfirmDelete(false)}
          >
            Cancel
          </CButton>

          <CButton color="danger" onClick={handleDeleteConfirmed}>
            Done
          </CButton>
        </CModalFooter>
      </CModal>
    </CRow>
  );
};

export default CourseListPage;