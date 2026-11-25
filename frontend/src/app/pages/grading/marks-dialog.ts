import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { SubjectMark } from '../../core/models/grading.model';
import { LookupItem } from '../../core/models/lookups.model';
import { GradingService } from '../../core/services/grading.service';
import { showServerErrors } from '../../shared/form-errors';

export interface MarksDialogData {
  subject: LookupItem;
  row: SubjectMark;
}

/** Highest mark for one term. The API and the database enforce the same limit. */
export const MAX_TERM_MARK = 30;

// Same rules as the API's SaveMarksDto: required, whole number, 0 to 30.
const termValidators = [
  Validators.required,
  Validators.min(0),
  Validators.max(MAX_TERM_MARK),
  Validators.pattern(/^[0-9]+$/), // no decimals or minus sign
];

/** Enter or change one student's Term 1 and Term 2 marks in one subject. */
@Component({
  selector: 'app-marks-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './marks-dialog.html',
  styles: `
    .terms { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; padding-top: 8px; }
    .total { display: flex; justify-content: space-between; padding: 12px 16px; border-radius: 8px; background: #f1f4f8; }
  `,
})
export class MarksDialog {
  private readonly gradingService = inject(GradingService);
  private readonly dialogRef = inject(MatDialogRef<MarksDialog>);

  protected readonly data = inject<MarksDialogData>(MAT_DIALOG_DATA);
  protected readonly maxTermMark = MAX_TERM_MARK;
  protected readonly saving = signal(false);

  protected readonly form = new FormGroup({
    term1: new FormControl<number | null>(this.data.row.term1, termValidators),
    term2: new FormControl<number | null>(this.data.row.term2, termValidators),
  });

  /** Live total while typing. Counts only valid marks. */
  protected total(): number {
    const { term1, term2 } = this.form.controls;
    return (term1.valid ? term1.value! : 0) + (term2.valid ? term2.value! : 0);
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched(); // shows the error under every invalid field
      return;
    }

    const { term1, term2 } = this.form.getRawValue();
    this.saving.set(true);

    this.gradingService
      .saveMarks(this.data.subject.id, this.data.row.studentId, { term1: term1!, term2: term2! })
      .subscribe({
        next: (saved) => this.dialogRef.close(saved),
        error: (error: HttpErrorResponse) => {
          showServerErrors(this.form, error);
          this.saving.set(false);
        },
      });
  }
}
