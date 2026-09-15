import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, inject, Input, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { VoucherService } from '../../services/voucher.service';
import { VoucherDTO } from '../../models/voucher';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  imports: [MatTableModule, MatButtonModule, MatCardModule, CommonModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  constructor() {
    this.loadVouchers();
  }
  private voucherService = inject(VoucherService);
  displayedColumns: string[] = ['sentAt', 'customer', 'amount', 'status', 'accion'];

  vouchers: VoucherDTO[] = [];

  firstLetterUppercase(valor: string) {
    if (!valor) return valor;
    return valor.charAt(0).toUpperCase() + valor.slice(1);
  }

  loadVouchers(): void {
    this.voucherService.getVouchers().subscribe({
      next: (data) => {
        this.vouchers = data;
      },
      error: (err) => {},
    });
  }

  onConfirm(id: number): void {
    this.voucherService.confirmVoucher(id).subscribe({
      next: () => this.loadVouchers(),
      error: (err) => {},
    });
  }
}
