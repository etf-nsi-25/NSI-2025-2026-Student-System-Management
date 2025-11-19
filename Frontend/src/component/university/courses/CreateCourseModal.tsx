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
import { useState } from "react";
import type { Course } from "./types/Course";

type Props = {
  visible: boolean;
  onClose: () => void;
  onCreate: (course: Course) => void;
};

const CreateCourseModal = ({ visible, onClose, onCreate }: Props) => {
  const [form, setForm] = useState({
    faculty: "ETF",
    program: "Bachelor",
    major: "Computer Science",
    name: "",               
    ects: "6",
    semester: "3",
    professor: "",
    assistant: "",
  });

  const update = (key: string, value: string) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  const handleSubmit = () => {
    if (!form.name.trim()) {
      alert("Course name is required.");
      return;
    }

    const newCourse: Course = {
      id: Date.now().toString(),          
      name: form.name,
      faculty: form.faculty,
      ects: Number(form.ects),            
      semester: Number(form.semester),    
      major: form.major,
    };

    onCreate(newCourse);  
    onClose();             
  };

  return (
    <CModal visible={visible} onClose={onClose} alignment="center" size="lg">
      <CModalHeader closeButton>
        <CModalTitle>Create Course</CModalTitle>
      </CModalHeader>

      <CModalBody>
        <CForm>
          <CRow className="mb-3">
            <CCol md={4}>
              <CFormSelect
                label="Faculty"
                value={form.faculty}
                onChange={(e) => update("faculty", e.target.value)}
                options={["ETF"]}
              />
            </CCol>

            <CCol md={4}>
              <CFormSelect
                label="Program"
                value={form.program}
                onChange={(e) => update("program", e.target.value)}
                options={["Bachelor"]}
              />
            </CCol>

            <CCol md={4}>
              <CFormSelect
                label="Major"
                value={form.major}
                onChange={(e) => update("major", e.target.value)}
                options={["Computer Science"]}
              />
            </CCol>
          </CRow>

          <CRow className="mb-3">
            <CCol md={4}>
              <CFormInput
                label="Course Name"
                placeholder="Enter course name"
                value={form.name}
                onChange={(e) => update("name", e.target.value)}
              />
            </CCol>

            <CCol md={4}>
              <CFormSelect
                label="ECTS"
                value={form.ects}
                onChange={(e) => update("ects", e.target.value)}
                options={["6", "5", "4"]}
              />
            </CCol>

            <CCol md={4}>
              <CFormSelect
                label="Semester"
                value={form.semester}
                onChange={(e) => update("semester", e.target.value)}
                options={["1", "2", "3", "4", "5", "6", "7", "8"]}
              />
            </CCol>
          </CRow>

          <CRow className="mb-3">
            <CCol md={6}>
              <CFormSelect
                label="Professor"
                value={form.professor}
                onChange={(e) => update("professor", e.target.value)}
                options={[
                  { label: "Choose a professor", value: "" },
                  { label: "Prof. John", value: "Prof. John" },
                  { label: "Prof. Anne", value: "Prof. Anne" },
                ]}
              />
            </CCol>

            <CCol md={6}>
              <CFormSelect
                label="Assistant"
                value={form.assistant}
                onChange={(e) => update("assistant", e.target.value)}
                options={[
                  { label: "Choose an assistant", value: "" },
                  { label: "Sarah Alba", value: "Sarah Alba" },
                  { label: "Michael Lee", value: "Michael Lee" },
                ]}
              />
            </CCol>
          </CRow>
        </CForm>
      </CModalBody>

      <CModalFooter>
        <CButton color="secondary" variant="outline" onClick={onClose}>
          Cancel
        </CButton>
        <CButton color="primary" onClick={handleSubmit}>
          Create
        </CButton>
      </CModalFooter>
    </CModal>
  );
};

export default CreateCourseModal;