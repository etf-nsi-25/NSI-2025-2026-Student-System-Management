import { useState, useEffect } from "react";
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
import { useAPI } from "../../../context/services";
import type { CourseDTO } from "../../../dto/CourseDTO";

const CourseListPage = () => {
  const api = useAPI();
  const [search, setSearch] = useState("");
  const [courses, setCourses] = useState<Course[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [showCreate, setShowCreate] = useState(false);
  const [editingCourse, setEditingCourse] = useState<Course | null>(null);
  const [showConfirmDelete, setShowConfirmDelete] = useState(false);
  const [courseToDelete, setCourseToDelete] = useState<Course | null>(null);

  const loadCourses = async () => {
    try {
      setLoading(true);
      const data = await api.getAllCourses();
      setCourses(data ?? []);
    } catch (err) {
      setError("Failed to load courses.");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCourses();
  }, []);

  const handleCreate = async (dto: CourseDTO) => {
    await api.createCourse(dto);
    loadCourses();
  };

  const handleSaveEdit = async (id: string, dto: CourseDTO) => {
    try {
      await api.updateCourse(id, dto);
      loadCourses();
      setEditingCourse(null);
    } catch (err) {
      console.error(err);
      alert("Failed to update course.");
    }
  };

  const requestDeleteCourse = (course: Course) => {
    setCourseToDelete(course);
    setShowConfirmDelete(true);
  };

  const handleDeleteConfirmed = async () => {
    if (!courseToDelete) return;

    await api.deleteCourse(courseToDelete.id);
    setShowConfirmDelete(false);
    setCourseToDelete(null);

    loadCourses();
  };

  return (
    <CRow className="mt-4">
      <CCol xs={12}>
        <CCard>
          <CCardBody>
            <h3 className="mb-4">Course Management</h3>

            {error && <p className="text-danger">{error}</p>}
            {loading && <p>Loading...</p>}

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

      {/* CREATE MODAL */}
      <CreateCourseModal
        visible={showCreate}
        onClose={() => setShowCreate(false)}
        onCreate={handleCreate}
      />

      {/* EDIT MODAL */}
      <EditCourseModal
        visible={!!editingCourse}
        course={editingCourse}
        onClose={() => setEditingCourse(null)}
        onSave={handleSaveEdit}
      />

      {/* DELETE MODAL */}
      <CModal
        visible={showConfirmDelete}
        onClose={() => setShowConfirmDelete(false)}
        className="modal-z-fix"
      >
        <CModalHeader closeButton>
          <CModalTitle>Confirm Deletion</CModalTitle>
        </CModalHeader>

        <CModalBody>
          {courseToDelete
            ? `Are you sure you want to delete course "${courseToDelete.name}"?`
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
            Delete
          </CButton>
        </CModalFooter>
      </CModal>
    </CRow>
  );
};

export default CourseListPage;
