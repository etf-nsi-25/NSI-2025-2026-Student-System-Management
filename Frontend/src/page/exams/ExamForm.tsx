import { useEffect, useMemo, useState } from 'react';
import { CButton, CCol, CForm, CFormInput, CFormLabel, CFormSelect, CRow } from '@coreui/react';

export type ExamFormValues = {
  courseId: string;    // selected course id
  courseName?: string; // optional (kept for compatibility)
  dateTime: string;    // "YYYY-MM-DDTHH:mm" from datetime-local
  location: string;
};

type Course = {
  id: string | number;
  name: string;
};

type Props = {
  onSubmit: (values: ExamFormValues) => Promise<void> | void;
  onCancel: () => void;
  submitting?: boolean;

  initialValues?: Partial<ExamFormValues>;

  courses?: Course[];
  coursesLoading?: boolean;
};

export function ExamForm({
  onSubmit,
  onCancel,
  submitting = false,
  initialValues,
  courses = [],
  coursesLoading = false,
}: Props) {
  const courseOptions = useMemo(
    () =>
      courses.map((c) => ({
        value: String(c.id),
        label: c.name,
      })),
    [courses],
  );

  const [values, setValues] = useState<ExamFormValues>({
    courseId: '',
    dateTime: '',
    location: '',
  });

  type FieldKey = 'courseId' | 'dateTime' | 'location';
  const [errors, setErrors] = useState<Partial<Record<FieldKey, string>>>({});
  const [initialized, setInitialized] = useState(false);

  const validate = (v: ExamFormValues) => {
    const nextErrors: Partial<Record<FieldKey, string>> = {};

    if (!v.courseId || v.courseId.trim().length === 0) {
      nextErrors.courseId = 'Course is required.';
    }

    if (!v.location || v.location.trim().length === 0) {
      nextErrors.location = 'Location is required.';
    }

    if (!v.dateTime || v.dateTime.trim().length === 0) {
      nextErrors.dateTime = 'Date & time is required.';
    } else {
      const parsed = new Date(v.dateTime);
      if (Number.isNaN(parsed.getTime())) {
        nextErrors.dateTime = 'Date & time is invalid.';
      } else if (parsed.getTime() <= Date.now()) {
        nextErrors.dateTime = 'Date & time must be in the future.';
      }
    }

    return nextErrors;
  };

  const currentValidation = validate(values);

  // if courses loaded and nothing selected, keep it empty (force user choice)
  useEffect(() => {
    // no auto-select: user must pick (safer)
  }, [courses]);

  useEffect(() => {
    if (initialized) return;
    if (!initialValues) return;

    setValues((prev) => ({
      ...prev,
      ...initialValues,
      courseId: initialValues.courseId ?? prev.courseId,
      dateTime: initialValues.dateTime ?? prev.dateTime,
      location: initialValues.location ?? prev.location,
      courseName: initialValues.courseName ?? prev.courseName,
    }));

    setInitialized(true);
  }, [initialized, initialValues]);

  const update = (key: keyof ExamFormValues, value: string) => {
    setValues((prev) => ({ ...prev, [key]: value }));

    if (key === 'courseId' || key === 'dateTime' || key === 'location') {
      setErrors((prev) => ({ ...prev, [key]: undefined }));
    }
  };

  const canSubmit =
    Object.keys(currentValidation).length === 0 && !submitting;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const nextErrors = validate(values);
    setErrors(nextErrors);
    if (Object.keys(nextErrors).length > 0) return;

    onSubmit(values);
  };

  return (
    <CForm id="exam-create-form" onSubmit={handleSubmit}>
      <CRow className="g-4">
        <CCol md={6}>
          <CFormLabel>Course</CFormLabel>
          <CFormSelect
            value={values.courseId}
            onChange={(e) => update('courseId', e.target.value)}
            disabled={coursesLoading || submitting}
          >
            <option value="">
              {coursesLoading ? 'Loading courses…' : 'Select course'}
            </option>
            {courseOptions.map((o) => (
              <option key={o.value} value={o.value}>
                {o.label}
              </option>
            ))}
          </CFormSelect>
          {errors.courseId && <div className="text-danger small mt-1">{errors.courseId}</div>}
        </CCol>

        <CCol md={6}>
          <CFormLabel>Date &amp; Time</CFormLabel>
          <CFormInput
            type="datetime-local"
            value={values.dateTime}
            onChange={(e) => update('dateTime', e.target.value)}
            disabled={submitting}
          />
          {errors.dateTime && <div className="text-danger small mt-1">{errors.dateTime}</div>}
        </CCol>

        <CCol md={12}>
          <CFormLabel>Location</CFormLabel>
          <CFormInput
            placeholder="e.g., Room 101"
            value={values.location}
            onChange={(e) => update('location', e.target.value)}
            disabled={submitting}
          />
          {errors.location && <div className="text-danger small mt-1">{errors.location}</div>}
        </CCol>

        <CCol xs={12} className="d-flex justify-content-center gap-2 mt-2">
          <CButton
            type="button"
            color="secondary"
            variant="outline"
            onClick={onCancel}
            disabled={submitting}
          >
            Cancel
          </CButton>

          <CButton type="submit" color="primary" disabled={!canSubmit}>
            {submitting ? 'Saving…' : 'Save'}
          </CButton>
        </CCol>
      </CRow>
    </CForm>
  );
}
