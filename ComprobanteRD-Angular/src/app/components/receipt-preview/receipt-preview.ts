import {
  Component,
  ElementRef,
  inject,
  Input,
  numberAttribute,
  OnInit,
  ViewChild,
} from '@angular/core';
import { ReceiptService } from '../../services/receipt.service';
import { ReceiptDto } from '../../models/receipt';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { DatePipe, DecimalPipe, Location } from '@angular/common';
import { PhonePipe } from '../../shared/pipes/phone-pipe';
import { getErrors } from '../../shared/functions/get-error';

@Component({
  selector: 'app-receipt-preview',
  imports: [MatButtonModule, MatIconModule, DatePipe, PhonePipe, DecimalPipe],
  templateUrl: './receipt-preview.html',
  styleUrl: './receipt-preview.css',
})
export class ReceiptPreview implements OnInit {
  ngOnInit(): void {
    this.loadReceiptData();
  }

  @Input({ transform: numberAttribute })
  voucherId!: number;
  private receiptService = inject(ReceiptService);
  private location = inject(Location);

  receiptData!: ReceiptDto;
  errors: string[] = [];

  goBack(): void {
    this.location.back();
  }

  loadReceiptData() {
    this.receiptService.getByVoucher(this.voucherId).subscribe({
      next: (data) => {
        this.receiptData = data;
        if (!this.receiptData.pdfUrl) {
          setTimeout(() => this.saveAndSend());
        }
      },
    });
  }

  @ViewChild('receipt') receiptEl!: ElementRef<HTMLElement>;

  private async generatePdf(): Promise<Blob> {
    const html2pdf = (await import('html2pdf.js')).default;
    return html2pdf()
      .set({
        margin: 0,
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2 },
        jsPDF: { unit: 'mm', format: 'a5', orientation: 'portrait' },
      })
      .from(this.receiptEl.nativeElement)
      .outputPdf('blob');
  }

  async saveAndSend(): Promise<void> {
    const pdf = await this.generatePdf();
    this.receiptService.uploadPdf(this.voucherId, pdf, this.receiptData.receiptNumber).subscribe({
      next: (res) => (this.receiptData.pdfUrl = res.pdfUrl),
      error: (err) => (this.errors = getErrors(err)),
    });
  }
}
