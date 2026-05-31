using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Groups.Commands;
using FiveTalents.Domain.Groups;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Tests.Unit.Groups.Commands;

public class DeleteGroupCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly DeleteGroupCommandHandler _handler;

    public DeleteGroupCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new DeleteGroupCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    private async Task<Group> SeedGroupAsync()
    {
        GroupType gt = new GroupType { OrganizationId = 1, Name = "Type" };
        _db.GroupTypes.Add(gt);
        Group group = new Group { OrganizationId = 1, Name = "Active Group", GroupTypeId = gt.Id };
        _db.Groups.Add(group);
        await _db.SaveChangesAsync();
        return group;
    }

    [Fact]
    public async Task Handle_WithExistingGroup_SetsIsDeleted()
    {
        var group = await SeedGroupAsync();

        await _handler.Handle(new DeleteGroupCommand(group.Id), CancellationToken.None);

        var deleted = await _db.Groups.IgnoreQueryFilters().FirstAsync(g => g.Id == group.Id);
        deleted.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ThrowsNotFoundException()
    {
        var act = async () => await _handler.Handle(new DeleteGroupCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithAlreadyDeletedGroup_ThrowsNotFoundException()
    {
        GroupType gt = new GroupType { OrganizationId = 1, Name = "Type" };
        _db.GroupTypes.Add(gt);
        Group group = new Group { OrganizationId = 1, Name = "Gone", GroupTypeId = gt.Id, IsDeleted = true };
        _db.Groups.Add(group);
        await _db.SaveChangesAsync();

        var act = async () => await _handler.Handle(new DeleteGroupCommand(group.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
