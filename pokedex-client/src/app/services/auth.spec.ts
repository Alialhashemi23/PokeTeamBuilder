import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';

import { AuthService } from './auth';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('starts logged out', () => {
    expect(service.currentUser()).toBeNull();
    expect(service.token()).toBeNull();
  });

  it('stores the session on login', () => {
    service.login('ash@example.com', 'pikachu123').subscribe();

    const req = httpMock.expectOne('/api/auth/login');
    expect(req.request.method).toBe('POST');
    req.flush({ token: 'jwt-token', email: 'ash@example.com', expiresAt: '' });

    expect(service.currentUser()).toBe('ash@example.com');
    expect(service.token()).toBe('jwt-token');
  });

  it('clears the session on logout', () => {
    service.register('ash@example.com', 'pikachu123').subscribe();
    httpMock
      .expectOne('/api/auth/register')
      .flush({ token: 'jwt-token', email: 'ash@example.com', expiresAt: '' });

    service.logout();

    expect(service.currentUser()).toBeNull();
    expect(service.token()).toBeNull();
  });
});
