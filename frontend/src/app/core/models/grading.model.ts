import { PagedResult, SortDirection } from './paged-result.model';

/** One row of the Grading page. The mark fields are null until the student has marks in the subject. */
export interface SubjectMark {
  studentId: number;
  studentName: string;
  nationalId: string;
  academicYear: string;
  term1: number | null;
  term2: number | null;
  total: number | null;
  percentage: number | null;
  grade: string | null;
  passed: boolean | null;
}

/** Everything the Grading page shows for one subject. */
export interface GradingPage {
  subjectId: number;
  subject: string;
  students: PagedResult<SubjectMark>;
  succeeded: number;
  failed: number;
}

/** Body for saving a student's marks in a subject. */
export interface SaveMarks {
  term1: number;
  term2: number;
}

/** Subject, search, filter, sort and page options for the Grading page. */
export interface GradingQuery {
  subjectId: number | null;
  search: string;
  academicYearId: number | null;
  sortBy: string;
  sortDirection: SortDirection;
  pageNumber: number;
  pageSize: number;
}
