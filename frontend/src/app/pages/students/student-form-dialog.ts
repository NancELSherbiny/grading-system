import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { SaveStudent, Student } from '../../core/models/student.model';
import { LookupsService } from '../../core/services/lookups.service';
import { StudentsService } from '../../core/services/students.service';
import { showServerErrors } from '../../shared/form-errors';

export interface StudentFormData {
  /** The student to edit. Leave it out to add a new student. */
  student?: Student;
}

/** Add or edit a student. Closes with the saved student, or nothing if cancelled. */
@Component({
  selector: 'app-student-form-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
  templateUrl: './student-form-dialog.html',
})
export class StudentFormDialog {
  private readonly studentsService = inject(StudentsService);
  private readonly dialogRef = inject(MatDialogRef<StudentFormDialog>);
  private readonly student = inject<StudentFormData>(MAT_DIALOG_DATA).student;

  protected readonly isEdit = !!this.student;
  protected readonly lookups = toSignal(inject(LookupsService).getLookups());
  protected readonly saving = signal(false);

  // Same rules as the API's SaveStudentDto, so most mistakes are caught before sending.
  protected readonly form = new FormGroup({
    name: new FormControl(this.student?.name ?? '', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    nationalId: new FormControl(this.student?.nationalId ?? '', {
      nonNullable: true,
      validators: [Validators.required, Validators.pattern(/^[0-9]{14}$/)],
    }),
    academicYearId: new FormControl<number | null>(this.student?.academicYearId ?? null, Validators.required),
  });

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched(); // shows the error under every invalid field
      return;
    }

    const value = this.form.getRawValue();
    const body: SaveStudent = {
      name: value.name.trim(),
      nationalId: value.nationalId.trim(),
      academicYearId: value.academicYearId!,
    };

    this.saving.set(true);
    const request = this.student
      ? this.studentsService.update(this.student.id, body)
      : this.studentsService.create(body);

    request.subscribe({
      next: (saved) => this.dialogRef.close(saved),
      // e.g. 409 "A student with this National ID already exists." -> shown under the National ID field
      error: (error: HttpErrorResponse) => {
        showServerErrors(this.form, error);
        this.saving.set(false);
      },
    });
  }
}
