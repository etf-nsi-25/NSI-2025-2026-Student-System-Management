import { useEffect, useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { CAlert, CCard, CCardBody, CCol, CRow } from '@coreui/react';

import { ExamForm, type ExamFormValues } from './ExamForm';
import { useAPI } from '../../context/services.tsx';

type Course = {
  id: string;
  name: string;
};

function toDateTimeLocal(value: string): string {
  if (!value) return '';

  // Accepts "YYYY-MM-DDTHH:mm" already
  if (value.includes('T')) {
    return value.slice(0, 16);
  }

  // Accepts "YYYY-MM-DD HH:mm" or longer
  const trimmed = value.trim();
  if (trimmed.length >= 16 && trimmed[10] === ' ') {
    return trimmed.slice(0, 10) + 'T' + trimmed.slice(11, 16);
  }

  // Fallback: let the input attempt to render it
  return value;
}

export function EditExamPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const api = useAPI();

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [courses, setCourses] = useState<Course[]>([]);
  const [coursesLoading, setCoursesLoading] = useState(false);
  const [coursesError, setCoursesError] = useState<string | null>(null);

  const [initialValues, setInitialValues] = useState<Partial<ExamFormValues>>({});

  const courseNameById = useMemo(() => {
    const map = new Map<string, string>();
    for (const c of courses) map.set(String(c.id), c.name);
    return map;
  }, [courses]);

  useEffect(() => {
    const load = async () => {
      if (!id) {
        setError('Missing exam id.');
        setLoading(false);
        return;
      }

      setLoading(true);
      setError(null);

      const loadCourses = async (): Promise<Course[]> => {
        setCoursesLoading(true);
        setCoursesError(null);

        try {
          const data = await api.getAllCourses();
          const list = (data ?? []).map((c) => ({ id: c.id, name: c.name })) as Course[];
          if (list.length === 0) {
            setCoursesError('No courses available.');
          }
          return list;
        } catch (e) {
          console.error('Failed to load courses for EditExamPage', e);
          setCoursesError('Failed to load courses.');
          return [];
        } finally {
          setCoursesLoading(false);
        }
      };

      try {
        const [exam, courseList] = await Promise.all([api.getExam(id), loadCourses()]);
        setCourses(courseList);

        const courseId = exam.courseId ? String(exam.courseId) : '';

        setInitialValues({
          courseId,
          examDate: toDateTimeLocal(exam.examDate ?? ''),
          regDeadline: toDateTimeLocal(exam.regDeadline ?? ''),
          location: exam.location,
          examType: exam.examType,
        });
      } catch (e: unknown) {
        console.error('Failed to load exam for edit', e);
        setError(e instanceof Error ? e.message : 'Failed to load exam.');
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [api, id]);

  const handleCancel = () => navigate('/faculty/exams');

  const handleSubmit = async (values: ExamFormValues) => {
    if (!id) return;

    setSubmitting(true);
    setError(null);

    try {
      const courseName = courseNameById.get(String(values.courseId)) ?? '';

      await api.updateExam(id, {
        courseId: values.courseId,
        name: courseName || 'Exam',
        location: values.location,
        examType: values.examType,
        examDate: values.examDate,
        regDeadline: values.regDeadline,
      });

      sessionStorage.setItem('exams.toast', 'updated');
      navigate('/faculty/exams');
    } catch (e: unknown) {
      console.error('Update exam failed', e);
      setError(e instanceof Error ? e.message : 'Failed to update exam.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <CRow className="mt-4">
      <CCol xs={12}>
        <CCard>
          <CCardBody>
            <h3 className="mb-4">Edit exam</h3>

            {coursesError && (
              <CAlert color="warning" className="mb-3">
                {coursesError}
              </CAlert>
            )}

            {error && (
              <CAlert color="danger" className="mb-3" dismissible onClose={() => setError(null)}>
                {error}
              </CAlert>
            )}

            {loading ? (
              <p>Loading...</p>
            ) : (
              <div style={{ maxWidth: 720, margin: '0 auto' }}>
                <ExamForm
                  initialValues={initialValues}
                  onSubmit={handleSubmit}
                  onCancel={handleCancel}
                  submitting={submitting}
                  courses={courses}
                  coursesLoading={coursesLoading}
                />
              </div>
            )}
          </CCardBody>
        </CCard>
      </CCol>
    </CRow>
  );
}
