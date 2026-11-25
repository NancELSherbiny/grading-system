import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'students', pathMatch: 'full' },
  {
    path: 'students',
    title: 'Students · Grading System',
    loadComponent: () => import('./pages/students/students-page').then((m) => m.StudentsPage),
  },
  {
    path: 'grading',
    title: 'Grading · Grading System',
    loadComponent: () => import('./pages/grading/grading-page').then((m) => m.GradingPage),
  },
  { path: '**', redirectTo: 'students' },
];
