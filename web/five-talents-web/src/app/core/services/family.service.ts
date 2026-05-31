import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Family, FamilyRole, FamilySummary, MemberFamilyMembership,
  CreateFamilyRequest, UpdateFamilyRequest,
  AddFamilyMemberRequest, UpdateMemberRoleRequest,
} from '../models/family.models';

@Injectable({ providedIn: 'root' })
export class FamilyService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/families`;
  private rolesBase = `${environment.apiUrl}/family-roles`;

  getRoles(organizationId: number): Observable<FamilyRole[]> {
    return this.http.get<FamilyRole[]>(this.rolesBase, {
      params: new HttpParams().set('organizationId', organizationId),
    });
  }

  getAll(organizationId: number | null): Observable<FamilySummary[]> {
    const params = organizationId !== null
      ? new HttpParams().set('organizationId', organizationId)
      : new HttpParams();
    return this.http.get<FamilySummary[]>(this.base, { params });
  }

  getById(id: number): Observable<Family> {
    return this.http.get<Family>(`${this.base}/${id}`);
  }

  create(request: CreateFamilyRequest): Observable<void> {
    return this.http.post<void>(this.base, request);
  }

  update(id: number, request: UpdateFamilyRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  getMemberships(memberId: number): Observable<MemberFamilyMembership[]> {
    return this.http.get<MemberFamilyMembership[]>(`${this.base}/member/${memberId}`);
  }

  addMember(familyId: number, request: AddFamilyMemberRequest): Observable<void> {
    return this.http.post<void>(`${this.base}/${familyId}/members`, request);
  }

  updateMemberRole(familyId: number, memberId: number, request: UpdateMemberRoleRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/${familyId}/members/${memberId}/role`, request);
  }

  removeMember(familyId: number, memberId: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${familyId}/members/${memberId}`);
  }
}
