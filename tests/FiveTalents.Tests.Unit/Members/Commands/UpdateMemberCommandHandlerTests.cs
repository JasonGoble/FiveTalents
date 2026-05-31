using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Members.Commands;
using FiveTalents.Application.Members.DTOs;
using FiveTalents.Domain.Members;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Tests.Unit.Members.Commands;

public class UpdateMemberCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly UpdateMemberCommandHandler _handler;

    public UpdateMemberCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new UpdateMemberCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithExistingMember_UpdatesScalarProperties()
    {
        Member member = new Member { OrganizationId = 1, FirstName = "Alice", LastName = "Old", Status = MemberStatus.Active };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        UpdateMemberCommand command = new UpdateMemberCommand(
            Id: member.Id,
            FirstName: "Alice",
            LastName: "New",
            DateOfBirth: new DateTime(1990, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            JoinDate: null,
            Gender: Gender.Female,
            MaritalStatus: MaritalStatus.Single,
            Status: MemberStatus.Inactive,
            Notes: "Updated notes",
            Addresses: null,
            Emails: null,
            Phones: null);

        await _handler.Handle(command, CancellationToken.None);

        var updated = await _db.Members.FindAsync(member.Id);
        updated!.LastName.Should().Be("New");
        updated.Gender.Should().Be(Gender.Female);
        updated.Status.Should().Be(MemberStatus.Inactive);
        updated.Notes.Should().Be("Updated notes");
        updated.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithNewEmails_ReplacesOldEmails()
    {
        Member member = new Member { OrganizationId = 1, FirstName = "Bob", LastName = "Jones" };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();
        _db.MemberEmails.Add(new MemberEmail { MemberId = member.Id, Email = "old@example.com", IsPrimary = true, ContactTypeId = 1 });
        await _db.SaveChangesAsync();

        List<MemberEmailInput> newEmails = new List<MemberEmailInput> { new(null, 1, true, "new@example.com") };
        UpdateMemberCommand command = new UpdateMemberCommand(member.Id, "Bob", "Jones", null, null, null, null,
            MemberStatus.Active, null, null, newEmails, null);

        await _handler.Handle(command, CancellationToken.None);

        var emails = await _db.MemberEmails.Where(e => e.MemberId == member.Id).ToListAsync();
        emails.Should().HaveCount(1);
        emails[0].Email.Should().Be("new@example.com");
    }

    [Fact]
    public async Task Handle_WithNullEmails_ClearsAllEmails()
    {
        Member member = new Member { OrganizationId = 1, FirstName = "Carol", LastName = "White" };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();
        _db.MemberEmails.Add(new MemberEmail { MemberId = member.Id, Email = "carol@example.com", IsPrimary = true, ContactTypeId = 1 });
        await _db.SaveChangesAsync();

        UpdateMemberCommand command = new UpdateMemberCommand(member.Id, "Carol", "White", null, null, null, null,
            MemberStatus.Active, null, null, null, null);

        await _handler.Handle(command, CancellationToken.None);

        var emails = await _db.MemberEmails.Where(e => e.MemberId == member.Id).ToListAsync();
        emails.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ThrowsNotFoundException()
    {
        UpdateMemberCommand command = new UpdateMemberCommand(999, "Ghost", "Member", null, null, null, null,
            MemberStatus.Active, null, null, null, null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
