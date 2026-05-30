import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { MemberService } from './member.service';
import { environment } from '../../../environments/environment';

const base = `${environment.apiUrl}/members`;

describe('MemberService', () => {
  let service: MemberService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(MemberService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('getAll sends GET with organizationId param', () => {
    service.getAll(5).subscribe();
    const req = httpMock.expectOne(r => r.url === base && r.params.get('organizationId') === '5');
    expect(req.request.method).toBe('GET');
    req.flush({ items: [], totalCount: 0 });
  });

  it('getAll includes optional search and status params', () => {
    service.getAll(1, 1, 25, 'John', 'Active' as any).subscribe();
    const req = httpMock.expectOne(r =>
      r.url === base &&
      r.params.get('search') === 'John' &&
      r.params.get('status') === 'Active'
    );
    req.flush({ items: [], totalCount: 0 });
  });

  it('getById sends GET to /members/:id', () => {
    service.getById(42).subscribe();
    const req = httpMock.expectOne(`${base}/42`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('getMyProfile sends GET to /members/me', () => {
    service.getMyProfile().subscribe();
    const req = httpMock.expectOne(`${base}/me`);
    expect(req.request.method).toBe('GET');
    req.flush(null);
  });

  it('create sends POST with member payload', () => {
    const payload: any = { firstName: 'Jane', lastName: 'Doe', organizationId: 1 };
    service.create(payload).subscribe();
    const req = httpMock.expectOne(base);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);
    req.flush({ id: 99 });
  });

  it('update sends PUT to /members/:id', () => {
    service.update(7, { firstName: 'Updated' } as any).subscribe();
    const req = httpMock.expectOne(`${base}/7`);
    expect(req.request.method).toBe('PUT');
    req.flush(null);
  });

  it('delete sends DELETE to /members/:id', () => {
    service.delete(3).subscribe();
    const req = httpMock.expectOne(`${base}/3`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('linkUser sends POST to /members/:id/link-user', () => {
    service.linkUser(10, 'user-abc').subscribe();
    const req = httpMock.expectOne(`${base}/10/link-user`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ userId: 'user-abc' });
    req.flush(null);
  });

  it('unlinkUser sends DELETE to /members/:id/link-user', () => {
    service.unlinkUser(10).subscribe();
    const req = httpMock.expectOne(`${base}/10/link-user`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('invite sends POST to /members/:id/invite', () => {
    service.invite(5, 'https://app.test/accept').subscribe();
    const req = httpMock.expectOne(`${base}/5/invite`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ acceptBaseUrl: 'https://app.test/accept' });
    req.flush(null);
  });

  it('moveOrganization sends PUT to /members/:id/organization', () => {
    service.moveOrganization(2, 9).subscribe();
    const req = httpMock.expectOne(`${base}/2/organization`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ organizationId: 9 });
    req.flush(null);
  });
});
