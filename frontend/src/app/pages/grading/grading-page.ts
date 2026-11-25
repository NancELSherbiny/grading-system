import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
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
import { Subscription, debounceTime, distinctUntilChanged, filter } from 'rxjs';
import { GradingQuery, SubjectMark } from '../../core/models/grading.model';
import { LookupItem } from '../../core/models/lookups.model';
import { GradingService } from '../../core/services/grading.service';
import { LookupsService } from '../../core/services/lookups.service';
import { GradeBadge } from '../../shared/grade-badge';
import { MarksDialog, MarksDialogData } from './marks-dialog';

@Component({
  selector: 'app-grading-page',
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
  templateUrl: './grading-page.html',
  styleUrl: './grading-page.scss',
})
export class GradingPage {
  private readonly gradingService = inject(GradingService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly subjects = signal<LookupItem[]>([]);
  protected readonly academicYears = signal<LookupItem[]>([]);
  protected readonly columns = ['id', 'name', 'academicYear', 'term1', 'term2', 'total', 'grade', 'actions'];

  // What we ask the API for. Every change reloads the page.
  protected readonly query = signal<GradingQuery>({
    subjectId: null,
    search: '',
    academicYearId: null,
    sortBy: 'id',
    sortDirection: 'asc',
    pageNumber: 1,
    pageSize: 10,
  });

  // What the API sent back.
  protected readonly rows = signal<SubjectMark[]>([]);
  protected readonly totalCount = signal(0);
  protected readonly succeeded = signal(0);
  protected readonly failed = signal(0);
  protected readonly loading = signal(false);

  protected readonly searchControl = new FormControl('', { nonNullable: true });
  private loadSubscription?: Subscription;

  constructor() {
    // Fill the dropdowns, then show the first subject.
    inject(LookupsService)
      .getLookups()
      .subscribe((lookups) => {
        this.subjects.set(lookups.subjects);
        this.academicYears.set(lookups.academicYears);
        this.changeQuery({ subjectId: lookups.subjects[0]?.id ?? null });
      });

    // Search 300 ms after the user stops typing, not on every key press.
    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe((search) => this.changeQuery({ search: search.trim(), pageNumber: 1 }));
  }

  /** The selected subject, for the dialog title. */
  protected selectedSubject(): LookupItem | undefined {
    return this.subjects().find((subject) => subject.id === this.query().subjectId);
  }

  // ---------- Subject, filters, sorting and paging ----------

  protected onSubjectChange(subjectId: number): void {
    this.changeQuery({ subjectId, pageNumber: 1 });
  }

  protected onAcademicYearChange(academicYearId: number | null): void {
    this.changeQuery({ academicYearId, pageNumber: 1 });
  }

  protected onSortChange(sort: Sort): void {
    this.changeQuery({
      sortBy: sort.direction ? sort.active : 'id',
      sortDirection: sort.direction || 'asc',
      pageNumber: 1,
    });
  }

  protected onPageChange(event: PageEvent): void {
    this.changeQuery({ pageNumber: event.pageIndex + 1, pageSize: event.pageSize });
  }

  // ---------- Edit marks ----------

  protected openMarksDialog(row: SubjectMark): void {
    const subject = this.selectedSubject();
    if (!subject) return;

    const data: MarksDialogData = { subject, row };
    this.dialog
      .open(MarksDialog, { data, width: '420px', maxWidth: '95vw' })
      .afterClosed()
      .pipe(filter((saved): saved is SubjectMark => !!saved))
      .subscribe((saved) => {
        this.snackBar.open(`${subject.name} marks saved for ${saved.studentName}.`, 'Close', { duration: 3000 });
        this.load(); // also refreshes the succeeded and failed counts
      });
  }

  // ---------- Loading ----------

  private changeQuery(changes: Partial<GradingQuery>): void {
    this.query.update((query) => ({ ...query, ...changes }));
    this.load();
  }

  private load(): void {
    const query = this.query();
    if (query.subjectId === null) return; // subjects haven't loaded yet

    // Cancel a request that's still running, so an older answer can't overwrite a newer one.
    this.loadSubscription?.unsubscribe();
    this.loading.set(true);

    this.loadSubscription = this.gradingService.getSubject(query.subjectId, query).subscribe({
      next: (page) => {
        this.rows.set(page.students.items);
        this.totalCount.set(page.students.totalCount);
        this.succeeded.set(page.succeeded);
        this.failed.set(page.failed);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
