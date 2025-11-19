import { CButton } from "@coreui/react";
import CIcon from "@coreui/icons-react";
import { cilPencil, cilTrash } from "@coreui/icons";

type Props = {
  onEdit: () => void;
  onDelete: () => void;
};

const CourseRowActions = ({ onEdit, onDelete }: Props) => {
  return (
    <>
      <CButton color="link" className="p-1 me-2" onClick={onEdit}>
        <CIcon icon={cilPencil} size="lg" />
      </CButton>

      <CButton color="link" className="p-1" onClick={onDelete}>
        <CIcon icon={cilTrash} size="lg" />
      </CButton>
    </>
  );
};

export default CourseRowActions;
