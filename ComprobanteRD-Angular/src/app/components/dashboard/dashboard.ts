import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { VoucherService } from '../../services/voucher.service';
import { VoucherDTO } from '../../models/voucher';

@Component({
  selector: 'app-dashboard',
  imports: [MatTableModule, MatButtonModule, MatCardModule, CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  ngOnInit(): void {
    this.loadVouchers();
  }
  private voucherService = inject(VoucherService);

  displayedColumns: string[] = ['fecha', 'cliente', 'monto', 'estado', 'acciones'];
  vouchers: VoucherDTO[] = [];
  isLoading = true;

  loadVouchers(): void {
    this.isLoading = true;
    this.voucherService.getVouchers().subscribe({
      next: (data) => {
        this.vouchers = data;
        this.isLoading = false;
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
