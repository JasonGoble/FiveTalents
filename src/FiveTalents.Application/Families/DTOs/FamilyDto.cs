namespace FiveTalents.Application.Families.DTOs;

public record FamilyRoleDto(int Id, string Name, bool IsAdult, int SortOrder);

public record FamilyMemberDto(
    int MemberId,
    string FullName,
    string? ProfilePhotoUrl,
    int RoleId,
    string RoleName,
    bool IsAdult
);

public record FamilySummaryDto(int Id, string Name, int MemberCount, int OrganizationId, string? OrgName = null);

public record MemberFamilyDto(int FamilyId, string FamilyName, int RoleId, string RoleName, bool IsAdult);

public record FamilyDto(
    int Id,
    string Name,
    int OrganizationId,
    IReadOnlyList<FamilyMemberDto> Members
);
