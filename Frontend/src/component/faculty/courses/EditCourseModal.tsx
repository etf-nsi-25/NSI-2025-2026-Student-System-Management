import {
  CModal,
  CModalHeader,
  CModalTitle,
  CModalBody,
  CModalFooter,
  CButton,
  CForm,
  CFormInput,
  CFormSelect,
  CRow,
  CCol,
} from "@coreui/react";
import { useEffect, useState } from "react";

import type { Course } from "./types/Course";
import type { CourseDTO } from "../../../dto/CourseDTO";

type Props = {
  visible: boolean;
  course: Course | null;
  onClose: () => void;
  onSave: (id: string, dto: CourseDTO) => void;
};

const EditCourseModal = ({ visible, course, onClose, onSave }: Props) => {
  const [form, setForm] = useState<CourseDTO | null>(null);

  useEffect(() => {
    if (course) {
      setForm({
        name: course.name,
        ects: course.ects,
        code: course.code,
        type: course.type.toLowerCase(),
        programId: course.programId ?? ""
      });
    }
  }, [course]);

  const update = (key: keyof CourseDTO, value: any) => {
    if (!form) return;
    setForm({ ...form, [key]: value });
  };

  if (!form || !course) return null;

  return (
    <CModal
      visible={visible}
      onClose={onClose}
      alignment="center"
      size="lg"
      className="modal-z-fix"
    >
      <CModalHeader>
        <CModalTitle>Edit Course</CModalTitle>
      </CModalHeader>

      <CModalBody>
        <CForm>
          <CRow className="mb-3">
            <CCol md={4}>
              <CFormSelect
                label="Type"
                value={form.type}
                onChange={(e) => update("type", e.target.value)}
                options={[
                  { label: "mandatory", value: "mandatory" },
                  { label: "elective", value: "elective" },
                ]}
              />
            </CCol>

            <CCol md={4}>
              <CFormInput
                label="Code"
                value={form.code}
                onChange={(e) => update("code", e.target.value)}
              />
            </CCol>

            <CCol md={4}>
              <CFormInput
                label="Program ID"
                value={form.programId}
                onChange={(e) => update("programId", e.target.value)}
              />
            </CCol>
          </CRow>

          <CRow className="mb-3">
            <CCol md={4}>
              <CFormInput
                label="Course Name"
                value={form.name}
                onChange={(e) => update("name", e.target.value)}
              />
            </CCol>

            <CCol md={4}>
              <CFormSelect
                label="ECTS"
                value={String(form.ects)}
                onChange={(e) => update("ects", Number(e.target.value))}
                options={["6", "5", "4", "3", "2", "1"]}
              />
            </CCol>
          </CRow>
        </CForm>
      </CModalBody>

      <CModalFooter>
        <CButton color="secondary" onClick={onClose}>
          Cancel
        </CButton>
        <CButton color="primary" onClick={() => onSave(course.id, form)}>
          Save
        </CButton>
      </CModalFooter>
    </CModal>
  );
};

export default EditCourseModal;
