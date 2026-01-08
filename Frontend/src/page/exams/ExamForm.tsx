import { useEffect, useMemo, useState } from 'react';
import { CButton, CCol, CForm, CFormInput, CFormLabel, CFormSelect, CRow } from '@coreui/react';

import type { ExamType } from '../../dto/ExamDTO';

export type ExamFormValues = {
  courseId: string;      // selected course guid
  examDate: string;      // "YYYY-MM-DDTHH:mm" from datetime-local
  regDeadline: string;   // "YYYY-MM-DDTHH:mm" from datetime-local
  location: string;
  examType: ExamType;
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
  const parseDateTimeLocal = (value: string) => {
    if (!value || value.trim().length === 0) return null;

    const [datePart, timePart] = value.split('T');
    if (!datePart || !timePart) return null;

    const [year, month, day] = datePart.split('-').map((n) => Number(n));
    const [hour, minute] = timePart.split(':').map((n) => Number(n));

    if ([year, month, day, hour, minute].some((n) => Number.isNaN(n))) return null;

    const dt = new Date(year, month - 1, day, hour, minute, 0, 0);
    if (Number.isNaN(dt.getTime())) return null;
    return dt;
  };

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
    examDate: '',
    regDeadline: '',
    location: '',
    examType: 'Written',
  });

  type FieldKey = 'courseId' | 'examDate' | 'regDeadline' | 'location' | 'examType';
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

    if (!v.examType || String(v.examType).trim().length === 0) {
      nextErrors.examType = 'Type is required.';
    }

    if (!v.examDate || v.examDate.trim().length === 0) {
      nextErrors.examDate = 'Date & time is required.';
    } else {
      const parsed = parseDateTimeLocal(v.examDate);
      if (!parsed) {
        nextErrors.examDate = 'Date & time is invalid.';
      } else if (parsed.getTime() <= Date.now()) {
        nextErrors.examDate = 'Date & time must be in the future.';
      }
    }

    if (!v.regDeadline || v.regDeadline.trim().length === 0) {
      nextErrors.regDeadline = 'Registration deadline is required.';
    } else {
      const parsed = parseDateTimeLocal(v.regDeadline);
      if (!parsed) {
        nextErrors.regDeadline = 'Registration deadline is invalid.';
      } else if (parsed.getTime() <= Date.now()) {
        nextErrors.regDeadline = 'Registration deadline must be in the future.';
      }
    }

    if (!nextErrors.examDate && !nextErrors.regDeadline) {
      const exam = parseDateTimeLocal(v.examDate)?.getTime();
      const deadline = parseDateTimeLocal(v.regDeadline)?.getTime();
      if (typeof exam === 'number' && typeof deadline === 'number' && deadline >= exam) {
        nextErrors.regDeadline = 'Registration deadline must be before the exam date.';
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
      examDate: initialValues.examDate ?? prev.examDate,
      regDeadline: initialValues.regDeadline ?? prev.regDeadline,
      location: initialValues.location ?? prev.location,
      examType: initialValues.examType ?? prev.examType,
    }));

    setInitialized(true);
  }, [initialized, initialValues]);

  const update = (key: keyof ExamFormValues, value: string) => {
    setValues((prev) => ({ ...prev, [key]: value }));

    if (key === 'courseId' || key === 'examDate' || key === 'regDeadline' || key === 'location' || key === 'examType') {
      setErrors((prev) => ({ ...prev, [key]: undefined }));
    }
  };

  const canSubmit =
    Object.keys(currentValidation).length === 0 && !submitting;

  const getFieldError = (key: FieldKey) => {
    const submitError = errors[key];
    if (submitError) return submitError;

    const value = values[key];
    const shouldShowLive = typeof value === 'string' ? value.trim().length > 0 : Boolean(value);
    if (!shouldShowLive) return undefined;

    return currentValidation[key];
  };

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
          {getFieldError('courseId') && <div className="text-danger small mt-1">{getFieldError('courseId')}</div>}
        </CCol>

        <CCol md={6}>
          <CFormLabel>Date &amp; Time</CFormLabel>
          <CFormInput
            type="datetime-local"
            value={values.examDate}
            onChange={(e) => update('examDate', e.target.value)}
            disabled={submitting}
          />
          {getFieldError('examDate') && <div className="text-danger small mt-1">{getFieldError('examDate')}</div>}
        </CCol>

        <CCol md={6}>
          <CFormLabel>Registration deadline</CFormLabel>
          <CFormInput
            type="datetime-local"
            value={values.regDeadline}
            onChange={(e) => update('regDeadline', e.target.value)}
            disabled={submitting}
          />
          {getFieldError('regDeadline') && <div className="text-danger small mt-1">{getFieldError('regDeadline')}</div>}
        </CCol>

        <CCol md={6}>
          <CFormLabel>Type</CFormLabel>
          <CFormSelect
            value={values.examType}
            onChange={(e) => update('examType', e.target.value)}
            disabled={submitting}
          >
            <option value="Written">Written</option>
            <option value="Oral">Oral</option>
            <option value="Practical">Practical</option>
            <option value="Online">Online</option>
          </CFormSelect>
          {getFieldError('examType') && <div className="text-danger small mt-1">{getFieldError('examType')}</div>}
        </CCol>

        <CCol md={12}>
          <CFormLabel>Location</CFormLabel>
          <CFormInput
            placeholder="e.g., Room 101"
            value={values.location}
            onChange={(e) => update('location', e.target.value)}
            disabled={submitting}
          />
          {getFieldError('location') && <div className="text-danger small mt-1">{getFieldError('location')}</div>}
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
