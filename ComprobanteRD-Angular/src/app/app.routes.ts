import { Routes } from '@angular/router';
import { Dashboard } from './components/dashboard/dashboard';
import { Login } from './components/login/login';
import { Voucher } from './components/voucher/voucher';

export const routes: Routes = [
  { path: '', component: Login },
  { path: 'login', component: Login },
  { path: 'dashboard', component: Dashboard },
  { path: 'voucher/:id', component: Voucher },
];
