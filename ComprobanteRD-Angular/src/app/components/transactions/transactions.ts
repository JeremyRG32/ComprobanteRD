import { Component, inject, OnInit } from '@angular/core';
import {
  MatTable,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef,
  MatCell,
  MatCellDef,
  MatTableModule,
} from '@angular/material/table';
import { VoucherService } from '../../services/voucher.service';
import { TransactionDTO, TransactionFilters } from '../../models/transaction';
import { CommonModule, DatePipe } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectModule } from '@angular/material/select';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { filter } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';

interface option {
  value: string;
  viewValue: string;
}

@Component({
  selector: 'app-transactions',
  imports: [
    MatTableModule,
    CommonModule,
    MatIconModule,
    DatePipe,
    MatInputModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatSelectModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
  ],
  templateUrl: './transactions.html',
  styleUrl: './transactions.css',
})
export class Transactions implements OnInit {
  ngOnInit(): void {
    this.loadTransactions();
    this.form.valueChanges.subscribe((valores) => {
      this.applyFilters(this.form.value as TransactionFilters);
    });
  }
  private voucherService = inject(VoucherService);
  private fb = inject(FormBuilder);
  private router = inject(Router);

  form = this.fb.group({
    name: '',
    date: null as Date | null,
    status: 'Todos',
  });

  transactions: TransactionDTO[] = [];
  transactionsFiltered: TransactionDTO[] = [];

  options: option[] = [
    { value: 'Approved', viewValue: 'Aprobado' },
    { value: 'Rejected', viewValue: 'Rechazado' },
    { value: 'Todos', viewValue: 'Todos' },
  ];

  displayedColums = [
    'sentAt',
    'bankReferenceNumber',
    'customerName',
    'amount',
    'status',
    'acciones',
  ];

  applyFilters(filters: TransactionFilters): void {
    let result = this.transactions;

    if (filters.date) {
      result = result.filter((t) => this.isSameDay(filters.date, t.sentAt));
    }
    if (filters.status && filters.status !== 'Todos') {
      result = result.filter((t) => t.status.indexOf(filters.status) !== -1);
    }
    if (filters.name) {
      result = result.filter(
        (t) => t.customerName.toLowerCase().indexOf(filters.name.toLowerCase()) !== -1,
      );
    }

    this.transactionsFiltered = result;
  }

  isSameDay(a: Date | string, b: Date | string): boolean {
    const dateA = a instanceof Date ? a : new Date(a);
    const dateB = b instanceof Date ? b : new Date(b);

    if (isNaN(dateA.getTime()) || isNaN(dateB.getTime())) {
      return false;
    }

    return (
      dateA.getFullYear() === dateB.getFullYear() &&
      dateA.getMonth() === dateB.getMonth() &&
      dateA.getDate() === dateB.getDate()
    );
  }

  loadTransactions() {
    this.voucherService.getTransactions().subscribe({
      next: (data) => {
        this.transactions = data;
        this.transactionsFiltered = this.transactions;
      },
    });
  }

  seeReceipt(id: number) {
    this.router.navigate([`/receipt/${id}`]);
  }
  seeVoucher(id: number) {
    this.router.navigate([`/voucher/${id}`]);
  }
}
