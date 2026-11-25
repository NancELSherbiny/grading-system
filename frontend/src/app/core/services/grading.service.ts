import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { GradingPage, GradingQuery, SaveMarks, SubjectMark } from '../models/grading.model';

/** Calls to /api/grading. */
@Injectable({ providedIn: 'root' })
export class GradingService {
  private readonly http = inject(HttpClient);
  private readonly url = '/api/grading';

  getSubject(subjectId: number, query: GradingQuery): Observable<GradingPage> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize)
      .set('sortBy', query.sortBy)
      .set('sortDirection', query.sortDirection);

    // Only send filters that are set.
    if (query.search) params = params.set('search', query.search);
    if (query.academicYearId) params = params.set('academicYearId', query.academicYearId);

    return this.http.get<GradingPage>(`${this.url}/${subjectId}`, { params });
  }

  saveMarks(subjectId: number, studentId: number, marks: SaveMarks): Observable<SubjectMark> {
    return this.http.put<SubjectMark>(`${this.url}/${subjectId}/students/${studentId}`, marks);
  }
}
