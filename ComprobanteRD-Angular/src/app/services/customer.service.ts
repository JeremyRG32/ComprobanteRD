import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CustomerDirectoryDTO } from '../models/customer';

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  private http = inject(HttpClient);
  url = environment.apiURL + '/customer';

  getCustomer(): Observable<CustomerDirectoryDTO[]> {
    return this.http.get<CustomerDirectoryDTO[]>(this.url);
  }
}
