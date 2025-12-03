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

type Course = {
  id: string;
  name: string;
  code: string;
  type: string;
  programId: string;
  ects: number;
};

type Props = {
  visible: boolean;
  course: Course | null;
  onClose: () => void;
  onSave: (updated: Course) => void;
};

const EditCourseModal = ({ visible, course, onClose, onSave }: Props) => {
  const [form, setForm] = useState<Course | null>(null);

  useEffect(() => {
    if (course) setForm(course);
  }, [course]);

  const update = (key: keyof Course, value: any) => {
    if (!form) return;
    setForm({ ...form, [key]: value });
  };

  if (!form) return null;

  return (
    <CModal visible={visible} onClose={onClose} alignment="center" size="lg">
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
                options={[{ label: "ETF UNSA", value: "ETF UNSA" }]}
              />
            </CCol>

            <CCol md={4}>
              <CFormSelect
                label="Code"
                value="C068"
                disabled
                options={[{ label: "Code", value: "Bachelor" }]}
              />
            </CCol>

            <CCol md={4}>
              <CFormSelect
                label="Program"
                value={form.programId}
                onChange={(e) => update("programId", e.target.value)}
                options={[
                  { label: "Computer Science", value: "Computer Science" },
                ]}
              />
            </CCol>
          </CRow>

          <CRow className="mb-3">
            <CCol md={4}>
              <CFormInput
                label="Course"
                value={form.name}
                onChange={(e) => update("name", e.target.value)}
              />
            </CCol>

            <CFormSelect
              label="ECTS"
              value={String(form.ects)}
              onChange={(e) => update("ects", Number(e.target.value))}
              options={[
                { label: "6", value: "6" },
                { label: "5", value: "5" },
              ]}
            />
          </CRow>
          <CRow className="mb-3"></CRow>
        </CForm>
      </CModalBody>

      <CModalFooter>
        <CButton color="secondary" onClick={onClose}>
          Cancel
        </CButton>
        <CButton color="primary" onClick={() => onSave(form)}>
          Done
        </CButton>
      </CModalFooter>
    </CModal>
  );
};

export default EditCourseModal;
