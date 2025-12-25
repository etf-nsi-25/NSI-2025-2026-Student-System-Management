import { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { CAlert, CCard, CCardBody, CCol, CRow } from '@coreui/react';

import { ExamForm, type ExamFormValues } from './ExamForm';
import { createExam } from '../../service/examsApi';
import { courseService } from '../../service/courseService';

type Course = {
  id: string | number;
  name: string;
};

export function CreateExamPage() {
  const navigate = useNavigate();

  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [courses, setCourses] = useState<Course[]>([]);
  const [coursesLoading, setCoursesLoading] = useState(false);
  const [coursesError, setCoursesError] = useState<string | null>(null);

  useEffect(() => {
    const loadCourses = async () => {
      setCoursesLoading(true);
      setCoursesError(null);

      try {
        const data = await courseService.getAll();
        const list = (data ?? []) as Course[];
        setCourses(list);

        if (list.length === 0) {
          setCoursesError('No courses available.');
        }
      } catch (e) {
        console.error('Failed to load courses for CreateExamPage', e);
        setCourses([]);
        setCoursesError('Failed to load courses.');
      } finally {
        setCoursesLoading(false);
      }
    };

    loadCourses();
  }, []);

  const courseNameById = useMemo(() => {
    const map = new Map<string, string>();
    for (const c of courses) map.set(String(c.id), c.name);
    return map;
  }, [courses]);

  const handleCancel = () => navigate('/faculty/exams');

  const handleSubmit = async (values: ExamFormValues) => {
    setSubmitting(true);
    setError(null);

    try {
      // values.courseId is the selected course id from dropdown
      // Always send courseName as real name when we have it (works for mock + real)
      const courseName =
        courseNameById.get(String(values.courseId)) ?? values.courseName ?? values.courseId;

      await createExam({
        courseName,
        dateTime: values.dateTime,
        location: values.location,
      });

      sessionStorage.setItem('exams.toast', 'created');
      navigate('/faculty/exams');
    } catch (e: unknown) {
      console.error('Create exam failed', e);
      setError(e instanceof Error ? e.message : 'Failed to create exam.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <CRow className="mt-4">
      <CCol xs={12}>
        <CCard>
          <CCardBody>
            <h3 className="mb-4">Create exam</h3>

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

            <div style={{ maxWidth: 720, margin: '0 auto' }}>
              <ExamForm
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                submitting={submitting}
                courses={courses}
                coursesLoading={coursesLoading}
              />
            </div>
          </CCardBody>
        </CCard>
      </CCol>
    </CRow>
  );
}