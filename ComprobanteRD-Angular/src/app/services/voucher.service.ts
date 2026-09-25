import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { ConfirmVoucherDTO, RejectVoucherDTO, VoucherDTO } from '../models/voucher';
import { Voucher } from '../components/voucher/voucher';
import { TransactionDTO } from '../models/transaction';

@Injectable({
  providedIn: 'root',
})
export class VoucherService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiURL + '/voucher';

  getVouchers(): Observable<VoucherDTO[]> {
    return this.http.get<VoucherDTO[]>(this.baseUrl);
  }

  getVoucherById(id: number): Observable<VoucherDTO> {
    return this.http.get<VoucherDTO>(`${this.baseUrl}/${id}`);
  }

  getTransactions(): Observable<TransactionDTO[]> {
    return this.http.get<TransactionDTO[]>(`${this.baseUrl}/transactions`);
  }

  confirmVoucher(id: number, confirmVoucherDTO: ConfirmVoucherDTO): Observable<any> {
    return this.http.post(`${this.baseUrl}/${id}/confirm`, confirmVoucherDTO);
  }

  rejectVoucher(id: number, rejectVoucherDTO: RejectVoucherDTO): Observable<any> {
    return this.http.post(`${this.baseUrl}/${id}/reject`, rejectVoucherDTO);
  }
}
