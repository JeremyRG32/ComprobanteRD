import { Routes } from '@angular/router';
import { Dashboard } from './components/dashboard/dashboard';
import { Login } from './components/login/login';
import { Voucher } from './components/voucher/voucher';
import { MainLayout } from './components/main-layout/main-layout';

export const routes: Routes = [
  // This route won't have navigation bars
  { path: 'login', component: Login },

  // This routes will have the navigation bars
  {
    path: '',
    component: MainLayout,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: Dashboard },
      { path: 'voucher/:id', component: Voucher },
    ],
  },

  // Fallback si la ruta no existe
  { path: '**', redirectTo: 'login' },
];
