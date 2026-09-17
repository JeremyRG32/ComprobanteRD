import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { LoginDTO } from '../../models/auth';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { getIdentityErrors } from '../../shared/functions/get-error';
import { ShowError } from '../../shared/components/show-error/show-error';

@Component({
  selector: 'app-login',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatButtonModule,
    MatInputModule,
    MatIconModule,
    ShowError,
  ],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private authService = inject(AuthService);
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);
  hidePassword = true;

  error!: string;

  // Form Creation
  form = this.formBuilder.group({
    email: ['', { validators: [Validators.required, Validators.email] }],
    password: ['', { validators: Validators.required }],
  });

  // Error handling
  getEmailErrors(): string {
    let field = this.form.controls.email;

    if (field.hasError('required')) {
      return 'Este campo es requerido';
    }

    if (field.hasError('email')) {
      return 'El email no es valido';
    }

    return '';
  }

  getPasswordErrors(): string {
    let field = this.form.controls.password;

    if (field.hasError('required')) {
      return 'Este campo es requerido';
    }

    return '';
  }

  // Login Method
  onSubmit(): void {
    if (!this.form.valid) {
      return;
    }

    const loginDTO = this.form.value as LoginDTO;
    this.authService.login(loginDTO).subscribe({
      next: (response) => {
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        const errors = getIdentityErrors(err);
        this.error = errors;
        console.log(err);
      },
    });
  }
}
