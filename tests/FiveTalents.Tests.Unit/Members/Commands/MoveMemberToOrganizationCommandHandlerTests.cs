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
        Organization source = new() { Name = "Source Org", Level = 1, IsActive = true };
        Organization target = new() { Name = "Target Org", Level = 1, IsActive = true };
        _db.Organizations.AddRange(source, target);
        await _db.SaveChangesAsync();
        Member member = new() { OrganizationId = source.Id, FirstName = "Alice", LastName = "Smith" };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();
        return (member, target);
    }

    [Fact]
    public async Task Handle_WithValidRequest_UpdatesMemberOrganization()
    {
        var (member, target) = await SeedAsync();

        await _handler.Handle(new MoveMemberToOrganizationCommand(member.Id, target.Id), CancellationToken.None);

        Member? updated = await _db.Members.FindAsync(member.Id);
        updated!.OrganizationId.Should().Be(target.Id);
        updated.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentMember_ThrowsNotFoundException()
    {
        Organization org = new() { Name = "Org", Level = 1, IsActive = true };
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();

        Func<Task> act = async () => await _handler.Handle(new MoveMemberToOrganizationCommand(999, org.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNonExistentOrg_ThrowsNotFoundException()
    {
        Member member = new() { OrganizationId = 1, FirstName = "Bob", LastName = "Jones" };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        Func<Task> act = async () => await _handler.Handle(new MoveMemberToOrganizationCommand(member.Id, 999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithInactiveOrg_ThrowsNotFoundException()
    {
        Member member = new() { OrganizationId = 1, FirstName = "Carol", LastName = "White" };
        Organization inactiveOrg = new() { Name = "Closed Org", Level = 1, IsActive = false };
        _db.Members.Add(member);
        _db.Organizations.Add(inactiveOrg);
        await _db.SaveChangesAsync();

        Func<Task> act = async () => await _handler.Handle(new MoveMemberToOrganizationCommand(member.Id, inactiveOrg.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
