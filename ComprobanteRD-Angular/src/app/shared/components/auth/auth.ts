import { Component, inject } from '@angular/core';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-auth',
  imports: [],
  templateUrl: './auth.html',
  styleUrl: './auth.css',
})
export class Auth {
  private authService = inject(AuthService);
}
