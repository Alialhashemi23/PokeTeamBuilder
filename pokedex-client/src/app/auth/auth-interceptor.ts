import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth';

/**
 * Attaches the JWT to outgoing API requests and logs out on 401s
 * (expired/invalid token) so the user lands back on the login page.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.token();

  const request = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(request).pipe(
    catchError((err) => {
      if (err.status === 401 && token && !req.url.includes('/api/auth/')) {
        auth.logout();
      }
      return throwError(() => err);
    })
  );
};
