import { Component, inject, Input, numberAttribute, OnInit } from '@angular/core';
import { VoucherService } from '../../services/voucher.service';
import { ConfirmVoucherDTO, RejectVoucherDTO, VoucherDTO } from '../../models/voucher';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatAnchor, MatButtonModule } from '@angular/material/button';
import { Location } from '@angular/common';
import { environment } from '../../../environments/environment.development';
import { Router } from '@angular/router';
import { ShowError } from '../../shared/components/show-error/show-error';
import { getErrors } from '../../shared/functions/get-error';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { RejectDialog } from '../../shared/components/reject-dialog/reject-dialog';

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
    ShowError,
    MatDialogModule,
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

  errors: string[] = [];

  private dialog = inject(MatDialog);
  private router = inject(Router);
  private location = inject(Location);
  private voucherService = inject(VoucherService);
  formBuilder = inject(FormBuilder);

  form = this.formBuilder.group({
    amount: [0, { validators: [Validators.required, Validators.min(1)] }],
    bankReferenceNumber: ['', { validators: [Validators.required] }],
  });

  voucher!: VoucherDTO;

  openRejectDialog(): void {
    const dialogRef = this.dialog.open(RejectDialog, {
      width: '450px',
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((rejectVoucherDTO: RejectVoucherDTO | null) => {
      if (rejectVoucherDTO) {
        rejectVoucherDTO: this.declinePayment(rejectVoucherDTO);
      }
    });
  }

  loadVoucher(): void {
    this.voucherService.getVoucherById(this.id).subscribe({
      next: (data) => {
        this.voucher = data;
        this.voucher.imageURL = environment.imageURL + this.voucher.imageURL;
      },
      error: (err) => {},
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }
    const confirmVoucherDTO = this.form.value as ConfirmVoucherDTO;
    this.voucherService.confirmVoucher(this.id, confirmVoucherDTO).subscribe({
      next: () => {
        this.router.navigate([`receipt/${this.voucher.id}`]);
      },
      error: (err) => {
        const error = getErrors(err);
        this.errors = error;
        console.log(this.errors);
        console.log(err);
      },
    });
  }

  declinePayment(rejectVoucherDTO: RejectVoucherDTO): void {
    this.voucherService.rejectVoucher(this.id, rejectVoucherDTO).subscribe({
      next: () => {
        this.router.navigate(['/receipt/', this.id]);
      },
      error: (err) => {
        const error = getErrors(err);
        this.errors = error;
        console.log(this.errors);
        console.log(err);
      },
    });
  }

  goBack(): void {
    this.location.back();
  }

  //Error Handling
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
