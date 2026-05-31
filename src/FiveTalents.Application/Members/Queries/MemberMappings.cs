using FiveTalents.Application.Members.DTOs;
using FiveTalents.Domain.Members;

namespace FiveTalents.Application.Members.Queries;

internal static class MemberMappings
{
    internal static MemberDto ToDto(Member m, string? orgName) => new(
        Id: m.Id,
        FirstName: m.FirstName,
        LastName: m.LastName,
        DateOfBirth: m.DateOfBirth,
        Gender: m.Gender,
        MaritalStatus: m.MaritalStatus,
        Status: m.Status,
        JoinDate: m.JoinDate,
        ProfilePhotoUrl: m.ProfilePhotoUrl,
        Notes: m.Notes,
        OrganizationId: m.OrganizationId,
        OrgName: orgName,
        UserId: m.UserId,
        IsLinkedToUser: m.UserId != null,
        SharePhoneWithNetwork: m.SharePhoneWithNetwork,
        ShareEmailWithNetwork: m.ShareEmailWithNetwork,
        ShareAddressWithNetwork: m.ShareAddressWithNetwork,
        Addresses: m.Addresses
            .OrderByDescending(a => a.IsPrimary).ThenBy(a => a.Id)
            .Select(a => new MemberAddressDto(a.Id, a.ContactTypeId, a.ContactType.Name, a.IsPrimary,
                a.AddressLine1, a.AddressLine2, a.City, a.State, a.PostalCode, a.Country))
            .ToList(),
        Emails: m.Emails
            .OrderByDescending(e => e.IsPrimary).ThenBy(e => e.Id)
            .Select(e => new MemberEmailDto(e.Id, e.ContactTypeId, e.ContactType.Name, e.IsPrimary, e.Email))
            .ToList(),
        Phones: m.Phones
            .OrderByDescending(p => p.IsPrimary).ThenBy(p => p.Id)
            .Select(p => new MemberPhoneDto(p.Id, p.ContactTypeId, p.ContactType.Name, p.IsPrimary, p.PhoneNumber, p.IsMobile))
            .ToList()
    );
}
