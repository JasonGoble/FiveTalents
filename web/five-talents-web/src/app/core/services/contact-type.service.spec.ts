import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { ContactTypeService } from './contact-type.service';
import { environment } from '../../../environments/environment';

describe('ContactTypeService', () => {
  let service: ContactTypeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ContactTypeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('getAll sends GET to /contact-types', () => {
    service.getAll().subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/contact-types`);
    expect(req.request.method).toBe('GET');
    req.flush({ emailTypes: [], phoneTypes: [], addressTypes: [] });
  });
});
