import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'students', pathMatch: 'full' },
  {
    path: 'students',
    title: 'Students · Grading System',
    loadComponent: () => import('./pages/students/students-page').then((m) => m.StudentsPage),
  },
  { path: '**', redirectTo: 'students' },
];
