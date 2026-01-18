import {
  CModal,
  CModalHeader,
  CModalTitle,
  CModalBody,
  CModalFooter,
  CButton,
  CForm,
  CFormInput,
  CRow,
  CCol,
  CFormSelect,
} from "@coreui/react";
import { useState } from "react";
import type { CourseDTO } from "../../../dto/CourseDTO";

type Props = {
  visible: boolean;
  onClose: () => void;
  onCreate: (dto: CourseDTO) => void;
};

const CreateCourseModal = ({ visible, onClose, onCreate }: Props) => {
  const [form, setForm] = useState<CourseDTO>({
    name: "",
    ects: 0,
    code: "",
    type: "mandatory",
    programId: "",
  });

  const update = (key: keyof CourseDTO, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  const handleSubmit = () => {
    if (!form.name.trim()) {
      alert("Name is required.");
      return;
    }

    onCreate(form);
    onClose();
  };

  return (
    <CModal
      visible={visible}
      onClose={onClose}
      alignment="center"
      size="lg"
      className="modal-z-fix"
    >
      <CModalHeader closeButton>
        <CModalTitle>Create Course</CModalTitle>
      </CModalHeader>

      <CModalBody>
        <CForm>
          <CRow className="mb-3">
            <CCol md={6}>
              <CFormInput
                label="Course Name"
                placeholder="Enter name"
                value={form.name}
                onChange={(e) => update("name", e.target.value)}
              />
            </CCol>

            <CCol md={3}>
              <CFormInput
                type="number"
                label="ECTS"
                min={1}
                value={form.ects}
                onChange={(e) => update("ects", Number(e.target.value))}
              />
            </CCol>
          </CRow>

          <CRow className="mb-3">
            <CCol md={4}>
              <CFormInput
                label="Code"
                placeholder="E.g. C045"
                value={form.code}
                onChange={(e) => update("code", e.target.value)}
              />
            </CCol>

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
                label="Program ID"
                placeholder="Enter program id"
                value={form.programId}
                onChange={(e) => update("programId", e.target.value)}
              />
            </CCol>
          </CRow>
        </CForm>
      </CModalBody>

      <CModalFooter className="d-flex justify-content-end gap-2">
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