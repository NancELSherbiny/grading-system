import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult } from '../models/paged-result.model';
import { SaveStudent, Student, StudentGrades, StudentQuery } from '../models/student.model';

/** Calls to /api/students. */
@Injectable({ providedIn: 'root' })
export class StudentsService {
  private readonly http = inject(HttpClient);
  private readonly url = '/api/students';

  getStudents(query: StudentQuery): Observable<PagedResult<Student>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize)
      .set('sortBy', query.sortBy)
      .set('sortDirection', query.sortDirection);

    // Only send filters that are set.
    if (query.search) params = params.set('search', query.search);
    if (query.academicYearId) params = params.set('academicYearId', query.academicYearId);
    if (query.grade) params = params.set('grade', query.grade);

    return this.http.get<PagedResult<Student>>(this.url, { params });
  }

  getGrades(id: number): Observable<StudentGrades> {
    return this.http.get<StudentGrades>(`${this.url}/${id}/grades`);
  }

  create(student: SaveStudent): Observable<Student> {
    return this.http.post<Student>(this.url, student);
  }

  update(id: number, student: SaveStudent): Observable<Student> {
    return this.http.put<Student>(`${this.url}/${id}`, student);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
