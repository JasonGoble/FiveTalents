using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Members.Queries;
using FiveTalents.Domain.Members;
using FiveTalents.Domain.Organizations;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

namespace FiveTalents.Tests.Unit.Members.Queries;

public class GetMemberByIdQueryHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly GetMemberByIdQueryHandler _handler;

    public GetMemberByIdQueryHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new GetMemberByIdQueryHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithExistingMember_ReturnsDto()
    {
        Member member = new Member
        {
            OrganizationId = 1,
            FirstName = "Alice",
            LastName = "Smith",
            Gender = Gender.Female,
            Status = MemberStatus.Active
        };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetMemberByIdQuery(member.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.FirstName.Should().Be("Alice");
        result.LastName.Should().Be("Smith");
        result.Gender.Should().Be(Gender.Female);
        result.Status.Should().Be(MemberStatus.Active);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ThrowsNotFoundException()
    {
        var act = async () => await _handler.Handle(new GetMemberByIdQuery(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithDeletedMember_ThrowsNotFoundException()
    {
        Member member = new Member { OrganizationId = 1, FirstName = "Bob", LastName = "Gone", IsDeleted = true };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        var act = async () => await _handler.Handle(new GetMemberByIdQuery(member.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_IncludesOrgName()
    {
        Organization org = new Organization { Name = "Grace Parish", Level = 1, IsActive = true };
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();
        Member member = new Member { OrganizationId = org.Id, FirstName = "Carol", LastName = "White" };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetMemberByIdQuery(member.Id), CancellationToken.None);

        result.OrgName.Should().Be("Grace Parish");
    }

    [Fact]
    public async Task Handle_WithEmails_ReturnsEmailsInDto()
    {
        Member member = new Member { OrganizationId = 1, FirstName = "Dave", LastName = "Lee" };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();
        ContactType contactType = new ContactType { Category = ContactTypeCategory.Email, Name = "Personal", SortOrder = 1 };
        _db.ContactTypes.Add(contactType);
        await _db.SaveChangesAsync();
        _db.MemberEmails.Add(new MemberEmail
        {
            MemberId = member.Id,
            Email = "dave@example.com",
            IsPrimary = true,
            ContactTypeId = contactType.Id
        });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetMemberByIdQuery(member.Id), CancellationToken.None);

        result.Emails.Should().HaveCount(1);
        result.Emails[0].Email.Should().Be("dave@example.com");
        result.Emails[0].IsPrimary.Should().BeTrue();
    }
}
