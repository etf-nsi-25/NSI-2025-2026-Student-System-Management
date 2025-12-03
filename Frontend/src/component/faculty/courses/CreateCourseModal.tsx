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
    name: "",   
    code: "string",
    type: "string",     
    programId: "2",
    ects: "6",           
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
      ects: Number(form.ects),
      code: form.code,
      type: form.type,
      programId: form.programId
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
                label="Type"
                value={form.type}
                onChange={(e) => update("type", e.target.value)}
                options={["ETF"]}
              />
            </CCol>

            <CCol md={4}>
              <CFormSelect
                label="Code"
                value={form.code}
                onChange={(e) => update("code", e.target.value)}
                options={["C0585"]}
              />
            </CCol>

            <CCol md={4}>
              <CFormSelect
                label="Program"
                value={form.programId}
                onChange={(e) => update("programId", e.target.value)}
                options={["2"]}
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
          </CRow>

          <CRow className="mb-3">
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