import { CIcon } from "@coreui/icons-react";
import { cilInfo } from "@coreui/icons";

type Props = {
  title: string;
  description: string;
};

export function ExamEmptyState({ title, description }: Props) {
  return (
    <div className="d-flex flex-column align-items-center gap-3 py-5">
      <CIcon icon={cilInfo} size="xl" style={{ opacity: 0.5, color: '#64748b' }} />
      <h4 className="ui-heading-md">{title}</h4>
      <p className="ui-text-muted text-center" style={{ maxWidth: 420 }}>
        {description}
      </p>
    </div>
  );
}
