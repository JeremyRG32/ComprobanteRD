import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ReceiptDto } from '../models/receipt';

@Injectable({
  providedIn: 'root',
})
export class ReceiptService {
  private http = inject(HttpClient);
  private url = environment.apiURL + '/receipt';

  getByVoucher(voucherId: number): Observable<ReceiptDto> {
    return this.http.get<ReceiptDto>(`${this.url}/${voucherId}`);
  }

  uploadPdf(voucherId: number, pdf: Blob, receiptNumber: string) {
    const form = new FormData();
    form.append('file', pdf, `${receiptNumber}.pdf`);
    return this.http.post<{ pdfUrl: string }>(`${this.url}/${voucherId}/pdf`, form);
  }
}
