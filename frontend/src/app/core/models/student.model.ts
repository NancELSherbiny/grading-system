import { SortDirection } from './paged-result.model';

/** A student in the list. Total, percentage and grade are null until every subject has marks. */
export interface Student {
  id: number;
  name: string;
  nationalId: string;
  academicYearId: number;
  academicYear: string;
  total: number | null;
  maxTotal: number;
  percentage: number | null;
  grade: string | null;
}

/** Body for adding or editing a student. */
export interface SaveStudent {
  name: string;
  nationalId: string;
  academicYearId: number;
}

/** Search, filter, sort and page options for the students list. */
export interface StudentQuery {
  search: string;
  academicYearId: number | null;
  grade: string | null;
  sortBy: string;
  sortDirection: SortDirection;
  pageNumber: number;
  pageSize: number;
}

/** A student's grade report. */
export interface StudentGrades {
  studentId: number;
  name: string;
  nationalId: string;
  academicYear: string;
  subjects: SubjectGrade[];
  total: number | null;
  maxTotal: number;
  percentage: number | null;
  grade: string | null;
}

export interface SubjectGrade {
  subjectId: number;
  subject: string;
  term1: number | null;
  term2: number | null;
  total: number | null;
  grade: string | null;
}
