using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Families.Commands;
using FiveTalents.Domain.Families;
using FiveTalents.Domain.Members;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Tests.Unit.Families.Commands;

public class RemoveFamilyMemberCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly RemoveFamilyMemberCommandHandler _handler;

    public RemoveFamilyMemberCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new RemoveFamilyMemberCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    private async Task<(Family family, Member member)> SeedWithMembershipAsync()
    {
        var family = new Family { OrganizationId = 1, Name = "Smith Family" };
        var member = new Member { OrganizationId = 1, FirstName = "Alice", LastName = "Smith" };
        var role = new FamilyRole { OrganizationId = 1, Name = "Spouse", IsAdult = true, SortOrder = 2 };
        _db.Families.Add(family);
        _db.Members.Add(member);
        _db.FamilyRoles.Add(role);
        await _db.SaveChangesAsync();
        _db.FamilyMembers.Add(new FamilyMember { FamilyId = family.Id, MemberId = member.Id, FamilyRoleId = role.Id });
        await _db.SaveChangesAsync();
        return (family, member);
    }

    [Fact]
    public async Task Handle_WithExistingMembership_RemovesMember()
    {
        var (family, member) = await SeedWithMembershipAsync();

        await _handler.Handle(new RemoveFamilyMemberCommand(family.Id, member.Id), CancellationToken.None);

        var exists = await _db.FamilyMembers
            .AnyAsync(fm => fm.FamilyId == family.Id && fm.MemberId == member.Id);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithNonExistentMembership_ThrowsNotFoundException()
    {
        var (family, _) = await SeedWithMembershipAsync();

        var act = async () => await _handler.Handle(new RemoveFamilyMemberCommand(family.Id, 999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
