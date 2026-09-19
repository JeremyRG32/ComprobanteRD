import { Component, inject, Input, numberAttribute, OnInit } from '@angular/core';
import { VoucherService } from '../../services/voucher.service';
import { VoucherDTO } from '../../models/voucher';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatAnchor, MatButtonModule } from '@angular/material/button';
import { Location } from '@angular/common';
import { environment } from '../../../environments/environment.development';

@Component({
  selector: 'app-voucher',
  imports: [
    MatTableModule,
    CommonModule,
    MatIconModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatInputModule,
    MatAnchor,
    MatButtonModule,
  ],
  templateUrl: './voucher.html',
  styleUrl: './voucher.css',
})
export class Voucher implements OnInit {
  ngOnInit(): void {
    this.loadVoucher();
  }

  @Input({ transform: numberAttribute })
  id!: number;

  private location = inject(Location);
  private voucherService = inject(VoucherService);
  displayedColumns: string[] = ['sentAt', 'customer', 'amount', 'status', 'accion'];
  formBuilder = inject(FormBuilder);

  form = this.formBuilder.group({
    amount: ['', { validators: [Validators.required, Validators.min(1)] }],
    bankReferenceNumber: ['', { validators: [Validators.required] }],
    customerName: ['', { validators: [Validators.required] }],
    customerPhone: [
      '',
      { validators: [Validators.required, Validators.pattern(/^1(809|829|849)[2-9]\d{6}$/)] },
    ],
  });

  voucher!: VoucherDTO;

  loadVoucher(): void {
    this.voucherService.getVoucherById(this.id).subscribe({
      next: (data) => {
        this.voucher = data;
        this.voucher.imageURL = environment.imageURL + this.voucher.imageURL;
        this.form.controls.customerPhone.setValue(this.voucher.customerPhone);
        this.form.controls.customerName.setValue(this.voucher.customerName);
      },
      error: (err) => {},
    });
  }

  goBack(): void {
    this.location.back();
  }

  //Error Handling
  getPhoneError() {
    let field = this.form.controls.customerPhone;

    if (field.hasError('required')) {
      return 'Este campo es requerido';
    }

    if (field.hasError('pattern')) {
      return 'El numero no es valido';
    }

    return '';
  }
  getNameError() {
    let field = this.form.controls.customerName;

    if (field.hasError('required')) {
      return 'Este campo es requerido';
    }

    return '';
  }
  getReferenceError() {
    let field = this.form.controls.bankReferenceNumber;

    if (field.hasError('required')) {
      return 'Este campo es requerido';
    }

    return '';
  }
  getAmountError() {
    let field = this.form.controls.amount;

    if (field.hasError('required')) {
      return 'Este campo es requerido';
    }

    if (field.hasError('min')) {
      return 'El monto debe ser mayor que 0';
    }

    return '';
  }
}
