import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { CustomerDirectoryDTO } from '../../models/customer';
import { CustomerService } from '../../services/customer.service';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { PhonePipe } from '../../shared/pipes/phone-pipe';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-customers',
  imports: [CommonModule, MatTableModule, MatButtonModule, PhonePipe, RouterLink],
  templateUrl: './customers.html',
  styleUrl: './customers.css',
})
export class Customers {
  constructor() {
    this.loadCustomer();
  }
  private customerService = inject(CustomerService);
  transactionVolume = 0;
  customerCount = 0;

  customers: CustomerDirectoryDTO[] = [];

  displayedColums = ['customerName', 'phoneNumber', 'totalTransactions', 'amount', 'historial'];

  loadCustomer(): void {
    this.customerService.getCustomer().subscribe({
      next: (data) => {
        this.customers = data;
        this.loadCards();
      },
      error: (err) => {},
    });
  }

  loadCards() {
    this.transactionVolume = this.customers.reduce((acc, item) => acc + item.amount, 0);
    this.customerCount = this.customers.length;
  }
}
