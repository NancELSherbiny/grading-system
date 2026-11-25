import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { catchError, of } from 'rxjs';
import { Student } from '../../core/models/student.model';
import { StudentsService } from '../../core/services/students.service';
import { GradeBadge } from '../../shared/grade-badge';

/** A student's grade report: every subject, then the overall result. */
@Component({
  selector: 'app-student-grades-dialog',
  imports: [MatDialogModule, MatTableModule, MatButtonModule, GradeBadge],
  templateUrl: './student-grades-dialog.html',
  styles: `
    table { margin-bottom: 16px; }
    .overall { display: flex; align-items: center; gap: 12px; padding: 12px 16px; border-radius: 8px; background: #f1f4f8; }
  `,
})
export class StudentGradesDialog {
  protected readonly student = inject<Student>(MAT_DIALOG_DATA);
  protected readonly columns = ['subject', 'term1', 'term2', 'total', 'grade'];

  // undefined while loading, null if loading failed (the error interceptor already showed a message).
  protected readonly report = toSignal(
    inject(StudentsService).getGrades(this.student.id).pipe(catchError(() => of(null))),
  );
}
