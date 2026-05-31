import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { PaginatedResult } from '../models/api.models';
import { CreateMemberRequest, Member, MemberStatus, MemberSummary, UpdateMemberRequest } from '../models/member.models';

@Injectable({ providedIn: 'root' })
export class MemberService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/members`;

  getAll(organizationId: number | null, page = 1, pageSize = 25, search?: string, status?: MemberStatus, includeChildOrgs = false) {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('includeChildOrgs', includeChildOrgs);
    if (organizationId !== null) params = params.set('organizationId', organizationId);
    if (search) params = params.set('search', search);
    if (status !== undefined) params = params.set('status', status);
    return this.http.get<PaginatedResult<MemberSummary>>(this.baseUrl, { params });
  }

  getById(id: number) {
    return this.http.get<Member>(`${this.baseUrl}/${id}`);
  }

  getMyProfile() {
    return this.http.get<Member | null>(`${this.baseUrl}/me`);
  }

  create(member: CreateMemberRequest) {
    return this.http.post<{ id: number }>(this.baseUrl, member);
  }

  update(id: number, member: UpdateMemberRequest | Partial<Member>) {
    return this.http.put(`${this.baseUrl}/${id}`, { id, ...member });
  }

  delete(id: number) {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  linkUser(memberId: number, userId: string) {
    return this.http.post(`${this.baseUrl}/${memberId}/link-user`, { userId });
  }

  unlinkUser(memberId: number) {
    return this.http.delete(`${this.baseUrl}/${memberId}/link-user`);
  }

  invite(memberId: number, acceptBaseUrl: string) {
    return this.http.post(`${this.baseUrl}/${memberId}/invite`, { acceptBaseUrl });
  }

  moveOrganization(memberId: number, organizationId: number) {
    return this.http.put(`${this.baseUrl}/${memberId}/organization`, { organizationId });
  }
}
