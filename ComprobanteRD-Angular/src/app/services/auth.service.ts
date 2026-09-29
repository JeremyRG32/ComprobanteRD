import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { AuthResponseDTO, LoginDTO } from '../models/auth';
import { Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

interface DecodedToken {
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'?: string;
  name?: string;
  unique_name?: string;
  exp?: number;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private baseURL = environment.apiURL + '/Auth';

  private readonly Token = 'token';
  private readonly Expiration = 'token_expiration';
  private readonly CompanyId = 'company_id';

  isLoggedIn(): boolean {
    const token = this.getToken();
    const expiration = localStorage.getItem(this.Expiration);

    if (!token || !expiration) {
      return false;
    }

    const isExpired = new Date(expiration).getTime() <= Date.now();
    return !isExpired;
  }

  login(credentials: LoginDTO): Observable<AuthResponseDTO> {
    return this.http
      .post<AuthResponseDTO>(`${this.baseURL}/login`, credentials)
      .pipe(tap((response) => this.saveToken(response)));
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  // Decode the token payload
  getDecodedToken(): DecodedToken | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      return jwtDecode<DecodedToken>(token);
    } catch (error) {
      console.error('Invalid token', error);
      return null;
    }
  }

  // Access the name claim specifically
  getUserFullName(): string | null {
    const token = this.getToken();
    console.log('Raw Token:', token);

    const decoded = this.getDecodedToken();
    if (!decoded) return null;

    return (
      decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
      decoded.name ||
      decoded.unique_name ||
      null
    );
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
