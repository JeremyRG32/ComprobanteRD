import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { AuthResponseDTO, LoginDTO } from '../models/auth';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private baseURL = environment.apiURL + '/Auth';

  private readonly Token = 'token';
  private readonly Expiration = 'token_expiration';
  private readonly CompanyId = 'company_id';

  login(credentials: LoginDTO): Observable<AuthResponseDTO> {
    return this.http
      .post<AuthResponseDTO>(`${this.baseURL}/login`, credentials)
      .pipe(tap((response) => this.saveToken(response)));
  }

  getToken(): string | null {
    return localStorage.getItem('Token');
  }

  logout(): void {
    localStorage.removeItem(this.Token);
    localStorage.removeItem(this.Expiration);
    localStorage.removeItem(this.CompanyId);
  }
  saveToken(response: AuthResponseDTO) {
    localStorage.setItem(this.Token, response.token);
    localStorage.setItem(this.Expiration, response.expiration.toString());
    localStorage.setItem(this.CompanyId, response.companyId.toString());
  }
}
