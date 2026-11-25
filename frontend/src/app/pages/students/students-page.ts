import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Subscription, debounceTime, distinctUntilChanged, filter, switchMap } from 'rxjs';
import { Student, StudentQuery } from '../../core/models/student.model';
import { LookupsService } from '../../core/services/lookups.service';
import { StudentsService } from '../../core/services/students.service';
import { ConfirmDialog, ConfirmDialogData } from '../../shared/confirm-dialog';
import { GradeBadge } from '../../shared/grade-badge';
import { StudentFormDialog, StudentFormData } from './student-form-dialog';
import { StudentGradesDialog } from './student-grades-dialog';

@Component({
  selector: 'app-students-page',
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatProgressBarModule,
    GradeBadge,
  ],
  templateUrl: './students-page.html',
  styleUrl: './students-page.scss',
})
export class StudentsPage {
  private readonly studentsService = inject(StudentsService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly lookups = toSignal(inject(LookupsService).getLookups());
  protected readonly grades = ['A', 'B', 'C', 'D', 'F'];
  protected readonly columns = ['id', 'name', 'nationalId', 'academicYear', 'total', 'grade', 'actions'];

  // What we ask the API for. Every change reloads the list.
  protected readonly query = signal<StudentQuery>({
    search: '',
    academicYearId: null,
    grade: null,
    sortBy: 'id',
    sortDirection: 'asc',
    pageNumber: 1,
    pageSize: 10,
  });

  // What the API sent back.
  protected readonly students = signal<Student[]>([]);
  protected readonly totalCount = signal(0);
  protected readonly loading = signal(false);

  protected readonly searchControl = new FormControl('', { nonNullable: true });
  private loadSubscription?: Subscription;

  constructor() {
    // Search 300 ms after the user stops typing, not on every key press.
    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe((search) => this.changeQuery({ search: search.trim(), pageNumber: 1 }));

    this.load();
  }

  // ---------- Filters, sorting and paging ----------

  protected onAcademicYearChange(academicYearId: number | null): void {
    this.changeQuery({ academicYearId, pageNumber: 1 });
  }

  protected onGradeChange(grade: string | null): void {
    this.changeQuery({ grade, pageNumber: 1 });
  }

  protected onSortChange(sort: Sort): void {
    // Clicking a header a third time removes the sort: go back to sorting by id.
    this.changeQuery({
      sortBy: sort.direction ? sort.active : 'id',
      sortDirection: sort.direction || 'asc',
      pageNumber: 1,
    });
  }

  protected onPageChange(event: PageEvent): void {
    this.changeQuery({ pageNumber: event.pageIndex + 1, pageSize: event.pageSize });
  }

  // ---------- Add, edit, delete, grades ----------

  protected openAddDialog(): void {
    this.openFormDialog({}, 'added');
  }

  protected openEditDialog(student: Student): void {
    this.openFormDialog({ student }, 'updated');
  }

  protected openGradesDialog(student: Student): void {
    this.dialog.open(StudentGradesDialog, { data: student, width: '640px', maxWidth: '95vw' });
  }

  protected confirmDelete(student: Student): void {
    const data: ConfirmDialogData = {
      title: 'Delete student?',
      message: `${student.name} and all their marks will be deleted. This can't be undone.`,
      confirmText: 'Delete',
    };

    this.dialog
      .open(ConfirmDialog, { data, width: '420px' })
      .afterClosed()
      .pipe(
        filter((confirmed) => confirmed === true),
        switchMap(() => this.studentsService.delete(student.id)),
      )
      .subscribe(() => {
        this.snackBar.open(`${student.name} was deleted.`, 'Close', { duration: 3000 });

        // If that was the only row on the last page, go back one page.
        const { pageNumber } = this.query();
        const pageIsNowEmpty = this.students().length === 1 && pageNumber > 1;
        this.changeQuery({ pageNumber: pageIsNowEmpty ? pageNumber - 1 : pageNumber });
      });
  }

  private openFormDialog(data: StudentFormData, action: string): void {
    this.dialog
      .open(StudentFormDialog, { data, width: '480px', maxWidth: '95vw' })
      .afterClosed()
      .pipe(filter((saved): saved is Student => !!saved))
      .subscribe((saved) => {
        this.snackBar.open(`${saved.name} was ${action}.`, 'Close', { duration: 3000 });
        this.load();
      });
  }

  // ---------- Loading ----------

  private changeQuery(changes: Partial<StudentQuery>): void {
    this.query.update((query) => ({ ...query, ...changes }));
    this.load();
  }

  private load(): void {
    // Cancel a request that's still running, so an older answer can't overwrite a newer one.
    this.loadSubscription?.unsubscribe();
    this.loading.set(true);

    this.loadSubscription = this.studentsService.getStudents(this.query()).subscribe({
      next: (page) => {
        this.students.set(page.items);
        this.totalCount.set(page.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
