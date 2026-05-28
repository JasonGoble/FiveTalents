using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Members.Commands;
using FiveTalents.Domain.Members;
using FiveTalents.Domain.Organizations;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;
using FluentAssertions;

namespace FiveTalents.Tests.Unit.Members.Commands;

public class MoveMemberToOrganizationCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly MoveMemberToOrganizationCommandHandler _handler;

    public MoveMemberToOrganizationCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new MoveMemberToOrganizationCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    private async Task<(Member member, Organization org)> SeedAsync()
    {
        var source = new Organization { Name = "Source Org", Level = 1, IsActive = true };
        var target = new Organization { Name = "Target Org", Level = 1, IsActive = true };
        _db.Organizations.AddRange(source, target);
        await _db.SaveChangesAsync();
        var member = new Member { OrganizationId = source.Id, FirstName = "Alice", LastName = "Smith" };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();
        return (member, target);
    }

    [Fact]
    public async Task Handle_WithValidRequest_UpdatesMemberOrganization()
    {
        var (member, target) = await SeedAsync();

        await _handler.Handle(new MoveMemberToOrganizationCommand(member.Id, target.Id), CancellationToken.None);

        var updated = await _db.Members.FindAsync(member.Id);
        updated!.OrganizationId.Should().Be(target.Id);
        updated.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentMember_ThrowsNotFoundException()
    {
        var org = new Organization { Name = "Org", Level = 1, IsActive = true };
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();

        var act = async () => await _handler.Handle(new MoveMemberToOrganizationCommand(999, org.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNonExistentOrg_ThrowsNotFoundException()
    {
        var member = new Member { OrganizationId = 1, FirstName = "Bob", LastName = "Jones" };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        var act = async () => await _handler.Handle(new MoveMemberToOrganizationCommand(member.Id, 999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithInactiveOrg_ThrowsNotFoundException()
    {
        var member = new Member { OrganizationId = 1, FirstName = "Carol", LastName = "White" };
        var inactiveOrg = new Organization { Name = "Closed Org", Level = 1, IsActive = false };
        _db.Members.Add(member);
        _db.Organizations.Add(inactiveOrg);
        await _db.SaveChangesAsync();

        var act = async () => await _handler.Handle(new MoveMemberToOrganizationCommand(member.Id, inactiveOrg.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
