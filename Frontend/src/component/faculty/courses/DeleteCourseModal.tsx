import {
  CModal,
  CModalHeader,
  CModalTitle,
  CModalBody,
  CModalFooter,
  CButton,
} from "@coreui/react";
import type { Course } from "./types/Course";

interface Props {
  visible: boolean;
  course: Course | null;
  onClose: () => void;
  onConfirm: () => void;
}

const DeleteCourseModal = ({ visible, course, onClose, onConfirm }: Props) => {
  return (
    <CModal visible={visible} onClose={onClose}>
      <CModalHeader closeButton>
        <CModalTitle>Confirm Deletion</CModalTitle>
      </CModalHeader>

      <CModalBody>
        {course ? (
          <>
            Are you sure you want to delete course{" "}
            <strong>"{course.name}"</strong>?
          </>
        ) : (
          "Are you sure you want to delete this course?"
        )}
      </CModalBody>

      <CModalFooter className="d-flex justify-content-end gap-2">
        <CButton color="secondary" variant="outline" onClick={onClose}>
          Cancel
        </CButton>

        <CButton color="danger" onClick={onConfirm}>
          Delete
        </CButton>
      </CModalFooter>
    </CModal>
  );
};

export default DeleteCourseModal;