using FiveTalents.Application.Members.Commands;
using FiveTalents.Application.Members.DTOs;
using FiveTalents.Domain.Members;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Tests.Unit.Members.Commands;

public class CreateMemberCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly CreateMemberCommandHandler _handler;

    public CreateMemberCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new CreateMemberCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithMinimalCommand_ReturnsMemberId()
    {
        CreateMemberCommand command = new CreateMemberCommand(
            OrganizationId: 1,
            FirstName: "Alice",
            LastName: "Smith",
            DateOfBirth: null,
            Gender: null,
            MaritalStatus: null,
            JoinDate: null,
            Addresses: null,
            Emails: null,
            Phones: null);

        int id = await _handler.Handle(command, CancellationToken.None);

        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Handle_WithValidCommand_StoresMemberWithCorrectProperties()
    {
        DateTime joinDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        CreateMemberCommand command = new CreateMemberCommand(
            OrganizationId: 1,
            FirstName: "Bob",
            LastName: "Jones",
            DateOfBirth: new DateTime(1985, 6, 20, 0, 0, 0, DateTimeKind.Utc),
            Gender: Gender.Male,
            MaritalStatus: MaritalStatus.Married,
            JoinDate: joinDate,
            Addresses: null,
            Emails: null,
            Phones: null);

        int id = await _handler.Handle(command, CancellationToken.None);

        var member = await _db.Members.FindAsync(id);
        member.Should().NotBeNull();
        member!.FirstName.Should().Be("Bob");
        member.LastName.Should().Be("Jones");
        member.Gender.Should().Be(Gender.Male);
        member.MaritalStatus.Should().Be(MaritalStatus.Married);
        member.JoinDate.Should().Be(joinDate);
        member.Status.Should().Be(MemberStatus.Active);
        member.OrganizationId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithNullJoinDate_DefaultsJoinDateToNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        CreateMemberCommand command = new CreateMemberCommand(1, "Carol", "White", null, null, null, null, null, null, null);

        int id = await _handler.Handle(command, CancellationToken.None);

        var member = await _db.Members.FindAsync(id);
        member!.JoinDate.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WithEmails_StoresEmailsForMember()
    {
        List<MemberEmailInput> emails = new List<MemberEmailInput>
        {
            new(Id: null, ContactTypeId: 1, IsPrimary: true, Email: "alice@example.com"),
            new(Id: null, ContactTypeId: 2, IsPrimary: false, Email: "alice.work@example.com")
        };
        CreateMemberCommand command = new CreateMemberCommand(1, "Alice", "Smith", null, null, null, null, null, emails, null);

        int id = await _handler.Handle(command, CancellationToken.None);

        var stored = await _db.MemberEmails.Where(e => e.MemberId == id).ToListAsync();
        stored.Should().HaveCount(2);
        stored.Should().Contain(e => e.Email == "alice@example.com" && e.IsPrimary);
        stored.Should().Contain(e => e.Email == "alice.work@example.com" && !e.IsPrimary);
    }

    [Fact]
    public async Task Handle_WithAllContactTypes_StoresAllContacts()
    {
        List<MemberAddressInput> addresses = new List<MemberAddressInput>
        {
            new(Id: null, ContactTypeId: 1, IsPrimary: true, AddressLine1: "123 Main St",
                AddressLine2: null, City: "Springfield", State: "IL", PostalCode: "62701", Country: "US")
        };
        List<MemberEmailInput> emails = new List<MemberEmailInput> { new(null, 1, true, "bob@example.com") };
        List<MemberPhoneInput> phones = new List<MemberPhoneInput> { new(null, 1, true, "555-1234", false) };
        CreateMemberCommand command = new CreateMemberCommand(1, "Bob", "Jones", null, null, null, null, addresses, emails, phones);

        int id = await _handler.Handle(command, CancellationToken.None);

        (await _db.MemberAddresses.CountAsync(a => a.MemberId == id)).Should().Be(1);
        (await _db.MemberEmails.CountAsync(e => e.MemberId == id)).Should().Be(1);
        (await _db.MemberPhones.CountAsync(p => p.MemberId == id)).Should().Be(1);
    }
}
