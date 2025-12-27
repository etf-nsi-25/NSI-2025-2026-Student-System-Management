import { useEffect, useState } from "react";
import {
  CAlert,
  CButton,
  CCard,
  CCardBody,
  CForm,
  CFormCheck,
  CFormInput,
  CFormLabel,
  CFormSelect,
  CFormTextarea,
  CSpinner,
} from "@coreui/react";
import { useAPI } from "../../context/services";
import { createIssue } from "../../service/student-support/api";

type SupportRequestFormProps = {
  selectedCategoryId: number | null;
};

export default function SupportRequestForm({
  selectedCategoryId,
}: SupportRequestFormProps) {
  const api = useAPI();
  // --- form state ---
  const [subject, setSubject] = useState("");
  const [description, setDescription] = useState("");
  const [categoryId, setCategoryId] = useState<number | "">("");
  const [acceptedTerms, setAcceptedTerms] = useState(false);

  // --- ui state ---
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  // hardcoded userId (TEMP)
  const USER_ID = "student-demo-001";

  // sync category card -> select
  useEffect(() => {
    if (selectedCategoryId) {
      setCategoryId(selectedCategoryId);
    }
  }, [selectedCategoryId]);

  const isFormValid =
    subject.trim().length > 0 &&
    subject.length <= 200 &&
    description.trim().length > 0 &&
    description.length <= 2000 &&
    categoryId !== "" &&
    acceptedTerms;

  const resetForm = () => {
    setSubject("");
    setDescription("");
    setCategoryId("");
    setAcceptedTerms(false);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!isFormValid || loading) return;

    setLoading(true);
    setError(null);
    setSuccess(false);

    try {
      await createIssue(api, {
        subject,
        description,
        categoryId: Number(categoryId),
        userId: USER_ID,
      });

      setSuccess(true);
      resetForm();
    } catch (err: any) {
      setError(err.message ?? "Unexpected error occurred.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <CCard className="ui-surface-glass-card">
      <CCardBody>
        <h5 className="mb-3">Submit a support request</h5>

        {success && (
          <CAlert color="success" className="ui-alert-success">
            Your request has been successfully submitted.
          </CAlert>
        )}

        {error && (
          <CAlert color="danger" className="ui-alert-error">
            {error}
          </CAlert>
        )}

        <CForm onSubmit={handleSubmit}>
          {/* Subject */}
          <div className="mb-3">
            <CFormLabel>Subject</CFormLabel>
            <CFormInput
              value={subject}
              maxLength={200}
              onChange={(e) => setSubject(e.target.value)}
              placeholder="Short summary of your issue"
              required
            />
          </div>

          {/* Category */}
          <div className="mb-3">
            <CFormLabel>Category</CFormLabel>
            <CFormSelect
              value={categoryId}
              onChange={(e) =>
                setCategoryId(e.target.value ? Number(e.target.value) : "")
              }
              required
            >
              <option value="">Select category</option>
              <option value={1}>Academic support</option>
              <option value={2}>Technical support</option>
              <option value={3}>Administrative support</option>
              <option value={4}>Account &amp; security</option>
            </CFormSelect>
          </div>

          {/* Description */}
          <div className="mb-3">
            <CFormLabel>Description</CFormLabel>
            <CFormTextarea
              rows={5}
              maxLength={2000}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Describe your issue in detail"
              required
            />
          </div>

          {/* Terms */}
          <div className="mb-4">
            <CFormCheck
              id="terms"
              label="I accept the terms and conditions"
              checked={acceptedTerms}
              onChange={(e) => setAcceptedTerms(e.target.checked)}
              required
            />
          </div>

          <CButton
            type="submit"
            className="ui-button-cta"
            disabled={!isFormValid || loading}
          >
            {loading ? <CSpinner size="sm" /> : "Send request"}
          </CButton>
        </CForm>
      </CCardBody>
    </CCard>
  );
}