export interface FamilyRole {
  id: number;
  name: string;
  isAdult: boolean;
  sortOrder: number;
}

export interface FamilyMember {
  memberId: number;
  fullName: string;
  profilePhotoUrl?: string;
  roleId: number;
  roleName: string;
  isAdult: boolean;
}

export interface FamilySummary {
  id: number;
  name: string;
  memberCount: number;
  organizationId: number;
  orgName?: string;
}

export interface Family {
  id: number;
  name: string;
  organizationId: number;
  members: FamilyMember[];
}

export interface CreateFamilyRequest {
  organizationId: number;
  name: string;
}

export interface UpdateFamilyRequest {
  id: number;
  name: string;
}

export interface AddFamilyMemberRequest {
  memberId: number;
  roleId: number;
}

export interface UpdateMemberRoleRequest {
  roleId: number;
}

export interface MemberFamilyMembership {
  familyId: number;
  familyName: string;
  roleId: number;
  roleName: string;
  isAdult: boolean;
}
