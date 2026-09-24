import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { RejectVoucherDTO } from '../../../models/voucher';
import { TextFieldModule } from '@angular/cdk/text-field';

@Component({
  selector: 'app-reject-dialog',
  imports: [
    TextFieldModule,
    FormsModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule,
  ],
  templateUrl: './reject-dialog.html',
  styleUrl: './reject-dialog.css',
})
export class RejectDialog {
  private dialogRef = inject(MatDialogRef<RejectDialog>);
  rejectVoucherDTO: RejectVoucherDTO = { reason: '' };

  onConfirm(): void {
    if (this.rejectVoucherDTO.reason.trim()) {
      // Close and return the object
      this.rejectVoucherDTO.reason = this.rejectVoucherDTO.reason.trim();
      this.dialogRef.close(this.rejectVoucherDTO);
    }
  }
  onCancel(): void {
    // Close and return nothing
    this.dialogRef.close(null);
  }
}
