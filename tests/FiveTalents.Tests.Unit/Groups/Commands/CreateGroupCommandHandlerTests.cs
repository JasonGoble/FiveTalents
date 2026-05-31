using FiveTalents.Application.Groups.Commands;
using FiveTalents.Domain.Groups;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

namespace FiveTalents.Tests.Unit.Groups.Commands;

public class CreateGroupCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly CreateGroupCommandHandler _handler;

    public CreateGroupCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new CreateGroupCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    private async Task<GroupType> SeedGroupTypeAsync()
    {
        GroupType gt = new GroupType { OrganizationId = 1, Name = "Small Group" };
        _db.GroupTypes.Add(gt);
        await _db.SaveChangesAsync();
        return gt;
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsPositiveId()
    {
        var gt = await SeedGroupTypeAsync();
        CreateGroupCommand command = new CreateGroupCommand(1, "Monday Night Group", null, gt.Id,
            GroupStatus.Active, null, null, null, null, null, null, null, true, null);

        int id = await _handler.Handle(command, CancellationToken.None);

        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Handle_WithValidCommand_StoresCorrectProperties()
    {
        var gt = await SeedGroupTypeAsync();
        CreateGroupCommand command = new CreateGroupCommand(1, "Bible Study", "Wed evenings", gt.Id,
            GroupStatus.Active, null, null, MeetingFrequency.Weekly, "Wednesday",
            "19:00", "Fellowship Hall", 20, true, null);

        int id = await _handler.Handle(command, CancellationToken.None);

        var group = await _db.Groups.FindAsync(id);
        group.Should().NotBeNull();
        group!.Name.Should().Be("Bible Study");
        group.Description.Should().Be("Wed evenings");
        group.MeetingDay.Should().Be("Wednesday");
        group.MeetingLocation.Should().Be("Fellowship Hall");
        group.MaxCapacity.Should().Be(20);
        group.IsOpenToNewMembers.Should().BeTrue();
        group.Status.Should().Be(GroupStatus.Active);
    }

    [Fact]
    public async Task Handle_WithValidMeetingTime_ParsesTimeOnly()
    {
        var gt = await SeedGroupTypeAsync();
        CreateGroupCommand command = new CreateGroupCommand(1, "Evening Group", null, gt.Id,
            GroupStatus.Active, null, null, null, null, "19:30", null, null, true, null);

        int id = await _handler.Handle(command, CancellationToken.None);

        var group = await _db.Groups.FindAsync(id);
        group!.MeetingTime.Should().Be(new TimeOnly(19, 30));
    }

    [Fact]
    public async Task Handle_WithInvalidMeetingTime_StoresNull()
    {
        var gt = await SeedGroupTypeAsync();
        CreateGroupCommand command = new CreateGroupCommand(1, "Group", null, gt.Id,
            GroupStatus.Active, null, null, null, null, "not-a-time", null, null, true, null);

        int id = await _handler.Handle(command, CancellationToken.None);

        var group = await _db.Groups.FindAsync(id);
        group!.MeetingTime.Should().BeNull();
    }
}
