using FiveTalents.Application.Families.Queries;
using FiveTalents.Domain.Families;
using FiveTalents.Domain.Members;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

namespace FiveTalents.Tests.Unit.Families.Queries;

public class GetFamiliesQueryHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly GetFamiliesQueryHandler _handler;

    public GetFamiliesQueryHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new GetFamiliesQueryHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_ReturnsOnlyFamiliesInRequestedOrg()
    {
        _db.Families.AddRange(
            new Family { OrganizationId = 1, Name = "Smith Family" },
            new Family { OrganizationId = 2, Name = "Jones Family" });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetFamiliesQuery(1), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Smith Family");
    }

    [Fact]
    public async Task Handle_ExcludesSoftDeletedFamilies()
    {
        _db.Families.AddRange(
            new Family { OrganizationId = 1, Name = "Active Family" },
            new Family { OrganizationId = 1, Name = "Deleted Family", IsDeleted = true });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetFamiliesQuery(1), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Active Family");
    }

    [Fact]
    public async Task Handle_IncludesMemberCount()
    {
        Family family = new Family { OrganizationId = 1, Name = "Adams Family" };
        _db.Families.Add(family);
        Member member = new Member { OrganizationId = 1, FirstName = "Alice", LastName = "Adams" };
        _db.Members.Add(member);
        FamilyRole role = new() { OrganizationId = 1, Name = "Head", IsAdult = true, SortOrder = 1 };
        _db.FamilyRoles.Add(role);
        await _db.SaveChangesAsync();
        _db.FamilyMembers.Add(new FamilyMember { FamilyId = family.Id, MemberId = member.Id, FamilyRoleId = role.Id });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetFamiliesQuery(1), CancellationToken.None);

        result[0].MemberCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ReturnsResultsOrderedAlphabetically()
    {
        _db.Families.AddRange(
            new Family { OrganizationId = 1, Name = "Zara Family" },
            new Family { OrganizationId = 1, Name = "Abel Family" });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetFamiliesQuery(1), CancellationToken.None);

        result[0].Name.Should().Be("Abel Family");
        result[1].Name.Should().Be("Zara Family");
    }
}
