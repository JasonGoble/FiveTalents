using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Members.DTOs;
using FiveTalents.Domain.Members;

using MediatR;

namespace FiveTalents.Application.Members.Commands;

public record CreateMemberCommand(
    int OrganizationId,
    string FirstName,
    string LastName,
    DateTime? DateOfBirth,
    Gender? Gender,
    MaritalStatus? MaritalStatus,
    DateTime? JoinDate,
    IReadOnlyList<MemberAddressInput>? Addresses,
    IReadOnlyList<MemberEmailInput>? Emails,
    IReadOnlyList<MemberPhoneInput>? Phones
) : IRequest<int>;

public class CreateMemberCommandHandler(IApplicationDbContext db) : IRequestHandler<CreateMemberCommand, int>
{
    public async Task<int> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        Member member = new Member
        {
            OrganizationId = request.OrganizationId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            MaritalStatus = request.MaritalStatus,
            JoinDate = request.JoinDate ?? DateTime.UtcNow,
            Status = MemberStatus.Active
        };

        db.Members.Add(member);
        await db.SaveChangesAsync(cancellationToken);

        ApplyContacts(member.Id, request, db);
        await db.SaveChangesAsync(cancellationToken);

        return member.Id;
    }

    internal static void ApplyContacts(int memberId, CreateMemberCommand req, IApplicationDbContext db)
    {
        foreach (var a in req.Addresses ?? [])
        {
            db.MemberAddresses.Add(new MemberAddress
            {
                MemberId = memberId,
                ContactTypeId = a.ContactTypeId,
                IsPrimary = a.IsPrimary,
                AddressLine1 = a.AddressLine1,
                AddressLine2 = a.AddressLine2,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Country = a.Country
            });
        }

        foreach (var e in req.Emails ?? [])
        {
            db.MemberEmails.Add(new MemberEmail
            {
                MemberId = memberId,
                ContactTypeId = e.ContactTypeId,
                IsPrimary = e.IsPrimary,
                Email = e.Email
            });
        }

        foreach (var p in req.Phones ?? [])
        {
            db.MemberPhones.Add(new MemberPhone
            {
                MemberId = memberId,
                ContactTypeId = p.ContactTypeId,
                IsPrimary = p.IsPrimary,
                PhoneNumber = p.PhoneNumber,
                IsMobile = p.IsMobile
            });
        }
    }
}
