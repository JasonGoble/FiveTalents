using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Members.DTOs;
using FiveTalents.Domain.Members;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Members.Commands;

public record UpdateMemberCommand(
    int Id,
    string FirstName,
    string LastName,
    DateTime? DateOfBirth,
    DateTime? JoinDate,
    Gender? Gender,
    MaritalStatus? MaritalStatus,
    MemberStatus Status,
    string? Notes,
    IReadOnlyList<MemberAddressInput>? Addresses,
    IReadOnlyList<MemberEmailInput>? Emails,
    IReadOnlyList<MemberPhoneInput>? Phones
) : IRequest;

public class UpdateMemberCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdateMemberCommand>
{
    public async Task Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await db.Members.FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Member), request.Id);

        member.FirstName = request.FirstName;
        member.LastName = request.LastName;
        member.DateOfBirth = request.DateOfBirth;
        member.JoinDate = request.JoinDate;
        member.Gender = request.Gender;
        member.MaritalStatus = request.MaritalStatus;
        member.Status = request.Status;
        member.Notes = request.Notes;
        member.UpdatedAt = DateTime.UtcNow;

        // Full-replace contacts
        var oldAddresses = await db.MemberAddresses.Where(a => a.MemberId == request.Id).ToListAsync(cancellationToken);
        db.MemberAddresses.RemoveRange(oldAddresses);

        var oldEmails = await db.MemberEmails.Where(e => e.MemberId == request.Id).ToListAsync(cancellationToken);
        db.MemberEmails.RemoveRange(oldEmails);

        var oldPhones = await db.MemberPhones.Where(p => p.MemberId == request.Id).ToListAsync(cancellationToken);
        db.MemberPhones.RemoveRange(oldPhones);

        foreach (var a in request.Addresses ?? [])
        {
            db.MemberAddresses.Add(new MemberAddress
            {
                MemberId = request.Id,
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

        foreach (var e in request.Emails ?? [])
        {
            db.MemberEmails.Add(new MemberEmail
            {
                MemberId = request.Id,
                ContactTypeId = e.ContactTypeId,
                IsPrimary = e.IsPrimary,
                Email = e.Email
            });
        }

        foreach (var p in request.Phones ?? [])
        {
            db.MemberPhones.Add(new MemberPhone
            {
                MemberId = request.Id,
                ContactTypeId = p.ContactTypeId,
                IsPrimary = p.IsPrimary,
                PhoneNumber = p.PhoneNumber,
                IsMobile = p.IsMobile
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
