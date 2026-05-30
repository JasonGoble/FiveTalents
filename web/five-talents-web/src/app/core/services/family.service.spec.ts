import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { FamilyService } from './family.service';
import { environment } from '../../../environments/environment';

const base = `${environment.apiUrl}/families`;
const rolesBase = `${environment.apiUrl}/family-roles`;

describe('FamilyService', () => {
  let service: FamilyService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(FamilyService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('getRoles sends GET to /family-roles with organizationId', () => {
    service.getRoles(1).subscribe();
    const req = httpMock.expectOne(r => r.url === rolesBase && r.params.get('organizationId') === '1');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getAll sends GET to /families with organizationId', () => {
    service.getAll(2).subscribe();
    const req = httpMock.expectOne(r => r.url === base && r.params.get('organizationId') === '2');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getById sends GET to /families/:id', () => {
    service.getById(5).subscribe();
    httpMock.expectOne(`${base}/5`).flush({});
  });

  it('create sends POST to /families', () => {
    service.create({ name: 'Smith Family', organizationId: 1 } as any).subscribe();
    const req = httpMock.expectOne(base);
    expect(req.request.method).toBe('POST');
    req.flush(null);
  });

  it('update sends PUT to /families/:id', () => {
    service.update(3, { name: 'Updated' } as any).subscribe();
    const req = httpMock.expectOne(`${base}/3`);
    expect(req.request.method).toBe('PUT');
    req.flush(null);
  });

  it('delete sends DELETE to /families/:id', () => {
    service.delete(4).subscribe();
    const req = httpMock.expectOne(`${base}/4`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('getMemberships sends GET to /families/member/:memberId', () => {
    service.getMemberships(10).subscribe();
    httpMock.expectOne(`${base}/member/10`).flush([]);
  });

  it('addMember sends POST to /families/:id/members', () => {
    service.addMember(1, { memberId: 7, roleId: 2, isAdult: true } as any).subscribe();
    const req = httpMock.expectOne(`${base}/1/members`);
    expect(req.request.method).toBe('POST');
    req.flush(null);
  });

  it('updateMemberRole sends PUT to /families/:id/members/:memberId/role', () => {
    service.updateMemberRole(1, 7, { roleId: 3 } as any).subscribe();
    const req = httpMock.expectOne(`${base}/1/members/7/role`);
    expect(req.request.method).toBe('PUT');
    req.flush(null);
  });

  it('removeMember sends DELETE to /families/:id/members/:memberId', () => {
    service.removeMember(1, 7).subscribe();
    const req = httpMock.expectOne(`${base}/1/members/7`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
