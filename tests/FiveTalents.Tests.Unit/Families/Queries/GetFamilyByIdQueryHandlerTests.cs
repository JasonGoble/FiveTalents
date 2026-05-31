using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Families.Queries;
using FiveTalents.Domain.Families;
using FiveTalents.Domain.Members;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

namespace FiveTalents.Tests.Unit.Families.Queries;

public class GetFamilyByIdQueryHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly GetFamilyByIdQueryHandler _handler;

    public GetFamilyByIdQueryHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new GetFamilyByIdQueryHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithExistingFamily_ReturnsDto()
    {
        Family family = new Family { OrganizationId = 1, Name = "Brown Family" };
        _db.Families.Add(family);
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetFamilyByIdQuery(family.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Brown Family");
        result.OrganizationId.Should().Be(1);
        result.Members.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ThrowsNotFoundException()
    {
        var act = async () => await _handler.Handle(new GetFamilyByIdQuery(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithDeletedFamily_ThrowsNotFoundException()
    {
        Family family = new Family { OrganizationId = 1, Name = "Gone", IsDeleted = true };
        _db.Families.Add(family);
        await _db.SaveChangesAsync();

        var act = async () => await _handler.Handle(new GetFamilyByIdQuery(family.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithMembers_ReturnsMembersOrderedBySortOrder()
    {
        Family family = new Family { OrganizationId = 1, Name = "Carter Family" };
        _db.Families.Add(family);
        Member parent = new Member { OrganizationId = 1, FirstName = "Jim", LastName = "Carter" };
        Member child = new Member { OrganizationId = 1, FirstName = "Tim", LastName = "Carter" };
        _db.Members.AddRange(parent, child);
        FamilyRole headRole = new FamilyRole { OrganizationId = 1, Name = "Head of Household", IsAdult = true, SortOrder = 1 };
        FamilyRole childRole = new FamilyRole { OrganizationId = 1, Name = "Child", IsAdult = false, SortOrder = 3 };
        _db.FamilyRoles.AddRange(headRole, childRole);
        await _db.SaveChangesAsync();
        _db.FamilyMembers.AddRange(
            new FamilyMember { FamilyId = family.Id, MemberId = child.Id, FamilyRoleId = childRole.Id },
            new FamilyMember { FamilyId = family.Id, MemberId = parent.Id, FamilyRoleId = headRole.Id });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetFamilyByIdQuery(family.Id), CancellationToken.None);

        result.Members.Should().HaveCount(2);
        result.Members[0].FullName.Should().Be("Jim Carter");
        result.Members[0].RoleName.Should().Be("Head of Household");
        result.Members[1].FullName.Should().Be("Tim Carter");
        result.Members[1].IsAdult.Should().BeFalse();
    }
}
