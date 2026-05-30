import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideRouter, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { AuthResponse, CurrentUser } from '../models/api.models';
import { environment } from '../../../environments/environment';

const mockUser: CurrentUser = {
  id: 'user-1',
  email: 'admin@test.com',
  fullName: 'Admin User',
  primaryOrganizationId: 1,
  isSystemAdmin: false,
};

const mockAuthResponse: AuthResponse = { token: 'test-token', user: mockUser };

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
      ],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  describe('initial state', () => {
    it('is unauthenticated when localStorage is empty', () => {
      expect(service.isAuthenticated()).toBe(false);
      expect(service.currentUser()).toBeNull();
    });

    it('restores session from localStorage on init', () => {
      localStorage.setItem('token', 'stored-token');
      localStorage.setItem('user', JSON.stringify(mockUser));
      TestBed.resetTestingModule();
      TestBed.configureTestingModule({
        providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
      });
      const freshService = TestBed.inject(AuthService);
      expect(freshService.isAuthenticated()).toBe(true);
      expect(freshService.currentUser()?.email).toBe(mockUser.email);
    });
  });

  describe('login', () => {
    it('posts to /auth/login', () => {
      service.login('admin@test.com', 'pass').subscribe();
      const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ email: 'admin@test.com', password: 'pass' });
      req.flush(mockAuthResponse);
    });

    it('stores token and user in localStorage after login', () => {
      service.login('admin@test.com', 'pass').subscribe();
      httpMock.expectOne(`${environment.apiUrl}/auth/login`).flush(mockAuthResponse);
      expect(localStorage.getItem('token')).toBe('test-token');
      expect(JSON.parse(localStorage.getItem('user')!)).toEqual(mockUser);
    });

    it('updates signals after login', () => {
      service.login('admin@test.com', 'pass').subscribe();
      httpMock.expectOne(`${environment.apiUrl}/auth/login`).flush(mockAuthResponse);
      expect(service.isAuthenticated()).toBe(true);
      expect(service.currentUser()).toEqual(mockUser);
    });
  });

  describe('setupAccount', () => {
    it('posts to /auth/setup-account and stores session', () => {
      service.setupAccount('admin@test.com', 'invite-token', 'NewPass1!').subscribe();
      const req = httpMock.expectOne(`${environment.apiUrl}/auth/setup-account`);
      expect(req.request.method).toBe('POST');
      req.flush(mockAuthResponse);
      expect(service.isAuthenticated()).toBe(true);
    });
  });

  describe('logout', () => {
    it('clears localStorage and resets signals', () => {
      const router = TestBed.inject(Router);
      vi.spyOn(router, 'navigate').mockResolvedValue(true);

      service.isAuthenticated.set(true);
      service.currentUser.set(mockUser);
      localStorage.setItem('token', 'test-token');

      service.logout();

      expect(service.isAuthenticated()).toBe(false);
      expect(service.currentUser()).toBeNull();
      expect(localStorage.getItem('token')).toBeNull();
      expect(localStorage.getItem('user')).toBeNull();
    });
  });

  describe('getToken', () => {
    it('returns null when no token stored', () => {
      expect(service.getToken()).toBeNull();
    });

    it('returns token from localStorage', () => {
      localStorage.setItem('token', 'my-token');
      expect(service.getToken()).toBe('my-token');
    });
  });
});
