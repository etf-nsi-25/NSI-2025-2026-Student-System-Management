import {
  CTable,
  CTableHead,
  CTableRow,
  CTableHeaderCell,
  CTableBody,
  CTableDataCell,
} from "@coreui/react";

import CourseRowActions from "./CourseRowActions";
import type { Course } from "./types/Course";

type Props = {
  search: string;
  courses: Course[];
  onEdit: (course: Course) => void;
  onDelete: (course: Course) => void;
};

const CourseTable = ({ search, courses, onEdit, onDelete }: Props) => {
  const filtered = courses.filter((c) =>
    c.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <CTable hover responsive>
      <CTableHead color="light">
        <CTableRow>
          <CTableHeaderCell>Name</CTableHeaderCell>
          <CTableHeaderCell>Code</CTableHeaderCell>
          <CTableHeaderCell>Type</CTableHeaderCell>
          <CTableHeaderCell>Program</CTableHeaderCell>
          <CTableHeaderCell>ECTS</CTableHeaderCell>
          <CTableHeaderCell className="text-end">Actions</CTableHeaderCell>
        </CTableRow>
      </CTableHead>

      <CTableBody>
        {filtered.map((course) => (
          <CTableRow key={course.id}>
            <CTableDataCell>{course.name}</CTableDataCell>
            <CTableDataCell>{course.code}</CTableDataCell>
            <CTableDataCell>{course.type}</CTableDataCell>
            <CTableDataCell>{course.programId}</CTableDataCell>
            <CTableDataCell>{course.ects}</CTableDataCell>

            <CTableDataCell className="text-end">
              <CourseRowActions
                onEdit={() => onEdit(course)}
                onDelete={() => onDelete(course)}
              />
            </CTableDataCell>
          </CTableRow>
        ))}
      </CTableBody>
    </CTable>
  );
};

export default CourseTable;