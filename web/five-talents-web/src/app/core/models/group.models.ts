export type GroupStatus = 'Active' | 'Inactive' | 'Forming' | 'Disbanded';
export type MeetingFrequency = 'Weekly' | 'BiWeekly' | 'Monthly' | 'Quarterly' | 'AsNeeded';

export interface GroupType {
  id: number;
  name: string;
  color?: string;
  iconName?: string;
}

export interface Group {
  id: number;
  name: string;
  description?: string;
  groupTypeId: number;
  groupTypeName: string;
  groupTypeColor?: string;
  groupTypeIcon?: string;
  status: GroupStatus;
  memberCount: number;
  leaderMemberId?: number;
  leaderName?: string;
  coLeaderMemberId?: number;
  coLeaderName?: string;
  meetingFrequency?: MeetingFrequency;
  meetingDay?: string;
  meetingTime?: string;
  meetingLocation?: string;
  maxCapacity?: number;
  isOpenToNewMembers: boolean;
  imageUrl?: string;
  organizationId: number;
  orgName?: string;
}

export interface CreateGroupRequest {
  organizationId: number;
  name: string;
  description?: string;
  groupTypeId: number;
  status: GroupStatus;
  leaderMemberId?: number;
  coLeaderMemberId?: number;
  meetingFrequency?: MeetingFrequency;
  meetingDay?: string;
  meetingTime?: string;
  meetingLocation?: string;
  maxCapacity?: number;
  isOpenToNewMembers: boolean;
  imageUrl?: string;
}

export interface UpdateGroupRequest extends CreateGroupRequest {
  id: number;
}
