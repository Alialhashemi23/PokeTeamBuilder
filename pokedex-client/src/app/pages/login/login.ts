import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private auth = inject(AuthService);
  private router = inject(Router);

  readonly mode = signal<'login' | 'register'>('login');
  readonly error = signal<string | null>(null);
  readonly submitting = signal(false);
  email = '';
  password = '';

  toggleMode(): void {
    this.mode.update((m) => (m === 'login' ? 'register' : 'login'));
    this.error.set(null);
  }

  submit(): void {
    if (!this.email.trim() || !this.password) {
      return;
    }

    this.submitting.set(true);
    const request =
      this.mode() === 'login'
        ? this.auth.login(this.email.trim(), this.password)
        : this.auth.register(this.email.trim(), this.password);

    request.subscribe({
      next: () => this.router.navigate(['/teams']),
      error: (err: HttpErrorResponse) => {
        this.error.set(
          typeof err.error === 'string' && err.error
            ? err.error
            : 'Something went wrong. Is the API running?'
        );
        this.submitting.set(false);
      },
    });
  }
}
