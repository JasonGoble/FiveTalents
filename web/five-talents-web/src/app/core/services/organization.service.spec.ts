import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { OrganizationService } from './organization.service';
import { environment } from '../../../environments/environment';

const base = `${environment.apiUrl}/organizations`;

describe('OrganizationService', () => {
  let service: OrganizationService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(OrganizationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('getAll sends GET with includeInactive=false by default', () => {
    service.getAll().subscribe();
    const req = httpMock.expectOne(r => r.url === base && r.params.get('includeInactive') === 'false');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getAll passes includeInactive=true when requested', () => {
    service.getAll(true).subscribe();
    const req = httpMock.expectOne(r => r.params.get('includeInactive') === 'true');
    req.flush([]);
  });

  it('getById sends GET to /organizations/:id', () => {
    service.getById(3).subscribe();
    httpMock.expectOne(`${base}/3`).flush({});
  });

  it('getTree sends GET to /organizations/tree', () => {
    service.getTree().subscribe();
    httpMock.expectOne(`${base}/tree`).flush([]);
  });

  it('getLevels sends GET to /organizations/levels', () => {
    service.getLevels().subscribe();
    httpMock.expectOne(`${base}/levels`).flush([]);
  });

  it('create sends POST to /organizations', () => {
    service.create({ name: 'New Org' } as any).subscribe();
    const req = httpMock.expectOne(base);
    expect(req.request.method).toBe('POST');
    req.flush({ id: 10 });
  });

  it('update sends PUT to /organizations/:id', () => {
    service.update({ id: 2, name: 'Updated' } as any).subscribe();
    const req = httpMock.expectOne(`${base}/2`);
    expect(req.request.method).toBe('PUT');
    req.flush(null);
  });

  it('getSettings sends GET to /organizations/:id/settings', () => {
    service.getSettings(1).subscribe();
    httpMock.expectOne(`${base}/1/settings`).flush({});
  });

  it('updateSettings sends PUT to /organizations/:id/settings', () => {
    service.updateSettings(1, {} as any).subscribe();
    const req = httpMock.expectOne(`${base}/1/settings`);
    expect(req.request.method).toBe('PUT');
    req.flush(null);
  });
});
