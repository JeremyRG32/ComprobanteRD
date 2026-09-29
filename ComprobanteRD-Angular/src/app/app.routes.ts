import { Routes } from '@angular/router';
import { Dashboard } from './components/dashboard/dashboard';
import { Login } from './components/login/login';
import { Voucher } from './components/voucher/voucher';
import { MainLayout } from './components/main-layout/main-layout';
import { Customers } from './components/customers/customers';
import { Transactions } from './components/transactions/transactions';
import { ReceiptPreview } from './components/receipt-preview/receipt-preview';
import { isLoggedGuard } from './shared/guards/is-logged-guard';

export const routes: Routes = [
  // This route won't have navigation bars
  { path: 'login', component: Login },

  // This routes will have the navigation bars
  {
    path: '',
    component: MainLayout,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: Dashboard, canActivate: [isLoggedGuard] },
      { path: 'voucher/:id', component: Voucher, canActivate: [isLoggedGuard] },
      { path: 'customers', component: Customers, canActivate: [isLoggedGuard] },
      { path: 'transaction-history', component: Transactions, canActivate: [isLoggedGuard] },
      { path: 'receipt/:voucherId', component: ReceiptPreview, canActivate: [isLoggedGuard] },
    ],
  },

  // Fallback
  { path: '**', redirectTo: 'login' },
];
