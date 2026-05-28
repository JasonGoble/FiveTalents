using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Groups.Commands;
using FiveTalents.Domain.Groups;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;
using FluentAssertions;

namespace FiveTalents.Tests.Unit.Groups.Commands;

public class UpdateGroupCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly UpdateGroupCommandHandler _handler;

    public UpdateGroupCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new UpdateGroupCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    private async Task<(Group group, GroupType groupType)> SeedGroupAsync()
    {
        var gt = new GroupType { OrganizationId = 1, Name = "Small Group" };
        _db.GroupTypes.Add(gt);
        await _db.SaveChangesAsync();
        var group = new Group { OrganizationId = 1, Name = "Old Name", GroupTypeId = gt.Id, Status = GroupStatus.Active };
        _db.Groups.Add(group);
        await _db.SaveChangesAsync();
        return (group, gt);
    }

    [Fact]
    public async Task Handle_WithExistingGroup_UpdatesProperties()
    {
        var (group, gt) = await SeedGroupAsync();
        var command = new UpdateGroupCommand(group.Id, "New Name", "Updated desc", gt.Id,
            GroupStatus.Inactive, null, null, MeetingFrequency.Monthly, "Friday",
            "18:00", "Room 101", 15, false, null);

        await _handler.Handle(command, CancellationToken.None);

        var updated = await _db.Groups.FindAsync(group.Id);
        updated!.Name.Should().Be("New Name");
        updated.Description.Should().Be("Updated desc");
        updated.Status.Should().Be(GroupStatus.Inactive);
        updated.MeetingDay.Should().Be("Friday");
        updated.MaxCapacity.Should().Be(15);
        updated.IsOpenToNewMembers.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ThrowsNotFoundException()
    {
        var (_, gt) = await SeedGroupAsync();
        var command = new UpdateGroupCommand(999, "Ghost", null, gt.Id, GroupStatus.Active,
            null, null, null, null, null, null, null, true, null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithDeletedGroup_ThrowsNotFoundException()
    {
        var gt = new GroupType { OrganizationId = 1, Name = "Type" };
        _db.GroupTypes.Add(gt);
        var group = new Group { OrganizationId = 1, Name = "Deleted", GroupTypeId = gt.Id, IsDeleted = true };
        _db.Groups.Add(group);
        await _db.SaveChangesAsync();

        var command = new UpdateGroupCommand(group.Id, "Revived", null, gt.Id, GroupStatus.Active,
            null, null, null, null, null, null, null, true, null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
