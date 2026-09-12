import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { RejectVoucherDTO, VoucherDTO } from '../models/voucher';

@Injectable({
  providedIn: 'root',
})
export class VoucherService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiURL + '/voucher';

  getVouchers(): Observable<VoucherDTO[]> {
    return this.http.get<VoucherDTO[]>(this.baseUrl);
  }

  confirmVoucher(id: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/${id}/confirm`, {});
  }

  rejectVoucher(id: number, rejectVoucherDTO: RejectVoucherDTO): Observable<any> {
    return this.http.post(`${this.baseUrl}/${id}/reject`, rejectVoucherDTO);
  }
}
