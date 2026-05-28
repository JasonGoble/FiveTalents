using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Families.Commands;
using FiveTalents.Domain.Families;
using FiveTalents.Domain.Members;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Tests.Unit.Families.Commands;

public class AddFamilyMemberCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly AddFamilyMemberCommandHandler _handler;

    public AddFamilyMemberCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new AddFamilyMemberCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    private async Task<(Family family, Member member, FamilyRole role)> SeedAsync()
    {
        var family = new Family { OrganizationId = 1, Name = "Jones Family" };
        var member = new Member { OrganizationId = 1, FirstName = "Bob", LastName = "Jones" };
        var role = new FamilyRole { OrganizationId = 1, Name = "Head of Household", IsAdult = true, SortOrder = 1 };
        _db.Families.Add(family);
        _db.Members.Add(member);
        _db.FamilyRoles.Add(role);
        await _db.SaveChangesAsync();
        return (family, member, role);
    }

    [Fact]
    public async Task Handle_WithValidRequest_AddsMemberToFamily()
    {
        var (family, member, role) = await SeedAsync();

        await _handler.Handle(new AddFamilyMemberCommand(family.Id, member.Id, role.Id), CancellationToken.None);

        var membership = await _db.FamilyMembers
            .FirstOrDefaultAsync(fm => fm.FamilyId == family.Id && fm.MemberId == member.Id);
        membership.Should().NotBeNull();
        membership!.FamilyRoleId.Should().Be(role.Id);
    }

    [Fact]
    public async Task Handle_WithNonExistentFamily_ThrowsNotFoundException()
    {
        var (_, member, role) = await SeedAsync();

        var act = async () => await _handler.Handle(new AddFamilyMemberCommand(999, member.Id, role.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithDuplicateMember_ThrowsInvalidOperationException()
    {
        var (family, member, role) = await SeedAsync();
        _db.FamilyMembers.Add(new FamilyMember { FamilyId = family.Id, MemberId = member.Id, FamilyRoleId = role.Id });
        await _db.SaveChangesAsync();

        var act = async () => await _handler.Handle(new AddFamilyMemberCommand(family.Id, member.Id, role.Id), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already in this family*");
    }
}
