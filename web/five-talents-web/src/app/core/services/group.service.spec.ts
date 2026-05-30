import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { GroupService } from './group.service';
import { environment } from '../../../environments/environment';

const base = `${environment.apiUrl}/groups`;

describe('GroupService', () => {
  let service: GroupService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(GroupService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('getAll sends GET with organizationId', () => {
    service.getAll(1).subscribe();
    const req = httpMock.expectOne(r => r.url === base && r.params.get('organizationId') === '1');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getAll includes optional search and status params', () => {
    service.getAll(1, 'choir', 'Active' as any).subscribe();
    const req = httpMock.expectOne(r =>
      r.params.get('search') === 'choir' && r.params.get('status') === 'Active'
    );
    req.flush([]);
  });

  it('getById sends GET to /groups/:id', () => {
    service.getById(9).subscribe();
    httpMock.expectOne(`${base}/9`).flush({});
  });

  it('getTypes sends GET to /groups/types with organizationId', () => {
    service.getTypes(1).subscribe();
    const req = httpMock.expectOne(r => r.url === `${base}/types` && r.params.get('organizationId') === '1');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('create sends POST to /groups', () => {
    service.create({ name: 'Choir', organizationId: 1 } as any).subscribe();
    const req = httpMock.expectOne(base);
    expect(req.request.method).toBe('POST');
    req.flush(null);
  });

  it('update sends PUT to /groups/:id', () => {
    service.update(3, { name: 'Updated Choir' } as any).subscribe();
    const req = httpMock.expectOne(`${base}/3`);
    expect(req.request.method).toBe('PUT');
    req.flush(null);
  });

  it('delete sends DELETE to /groups/:id', () => {
    service.delete(3).subscribe();
    httpMock.expectOne(`${base}/3`).flush(null);
  });
});
