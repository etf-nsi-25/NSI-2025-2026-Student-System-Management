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
  faculty: string;
  ects: number;
  semester: number;
  major: string;
  professor?: string;
  assistant?: string;
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
                label="Faculty"
                value={form.faculty}
                onChange={(e) => update("faculty", e.target.value)}
                options={[{ label: "ETF UNSA", value: "ETF UNSA" }]}
              />
            </CCol>

            <CCol md={4}>
              <CFormSelect
                label="Program"
                value="Bachelor"
                disabled
                options={[{ label: "Bachelor", value: "Bachelor" }]}
              />
            </CCol>

            <CCol md={4}>
              <CFormSelect
                label="Major"
                value={form.major}
                onChange={(e) => update("major", e.target.value)}
                options={[{ label: "Computer Science", value: "Computer Science" }]}
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


            <CFormSelect
                label="Semester"
                value={String(form.semester)}
                onChange={(e) => update("semester", Number(e.target.value))}
                options={[
                    { label: "3", value: "3" },
                    { label: "4", value: "4" },
                    { label: "5", value: "5" },
                ]}
            />
        </CRow>

          <CRow className="mb-3">
            <CCol md={6}>
              <CFormSelect
                label="Professor"
                value={form.professor || ""}
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
                value={form.assistant || ""}
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
