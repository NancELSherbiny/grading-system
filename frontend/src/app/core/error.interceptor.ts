import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

/**
 * Shows a message at the bottom of the screen when an API call fails.
 * Errors that belong to a form field (like a duplicate National ID) are skipped here:
 * the form shows those under the field itself.
 */
export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const snackBar = inject(MatSnackBar);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      const hasFieldErrors = !!error.error?.errors;
      if (!hasFieldErrors) {
        snackBar.open(errorMessage(error), 'Close', { duration: 5000 });
      }
      return throwError(() => error);
    }),
  );
};

function errorMessage(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return 'Cannot reach the server. Is the API running?';
  }
  // The API sends { title, detail } for every error.
  return error.error?.detail ?? error.error?.title ?? 'Something went wrong. Please try again.';
}
