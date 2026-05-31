import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Group, GroupType, CreateGroupRequest, UpdateGroupRequest, GroupStatus } from '../models/group.models';

@Injectable({ providedIn: 'root' })
export class GroupService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/groups`;

  getAll(organizationId: number | null, search?: string, status?: GroupStatus): Observable<Group[]> {
    let params = new HttpParams();
    if (organizationId !== null) params = params.set('organizationId', organizationId);
    if (search) params = params.set('search', search);
    if (status) params = params.set('status', status);
    return this.http.get<Group[]>(this.base, { params });
  }

  getById(id: number): Observable<Group> {
    return this.http.get<Group>(`${this.base}/${id}`);
  }

  getTypes(organizationId: number): Observable<GroupType[]> {
    return this.http.get<GroupType[]>(`${this.base}/types`, {
      params: new HttpParams().set('organizationId', organizationId)
    });
  }

  create(request: CreateGroupRequest): Observable<void> {
    return this.http.post<void>(this.base, request);
  }

  update(id: number, request: UpdateGroupRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
