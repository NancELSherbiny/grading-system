import { HttpErrorResponse } from '@angular/common/http';
import { FormGroup } from '@angular/forms';

/**
 * Puts the API's field errors under the matching inputs.
 * The API returns { errors: { nationalId: ["A student with this National ID already exists."] } },
 * and the form controls have the same names, so each message lands on the right field.
 */
export function showServerErrors(form: FormGroup, error: HttpErrorResponse): void {
  const errors: Record<string, string[]> = error.error?.errors ?? {};

  for (const [field, messages] of Object.entries(errors)) {
    const control = form.get(field);
    if (control) {
      control.setErrors({ server: messages[0] });
      control.markAsTouched();
    }
  }
}
