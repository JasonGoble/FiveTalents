using FiveTalents.Application.Organizations.Commands;
using FiveTalents.Domain.Organizations;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

namespace FiveTalents.Tests.Unit.Organizations.Commands;

public class UpdateOrganizationCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly UpdateOrganizationCommandHandler _handler;

    public UpdateOrganizationCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new UpdateOrganizationCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithExistingOrg_UpdatesProperties()
    {
        Organization org = new Organization { Name = "Old Name", Level = 1, IsActive = true };
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();

        UpdateOrganizationCommand command = new UpdateOrganizationCommand
        {
            Id = org.Id,
            Name = "New Name",
            Level = 2,
            IsActive = false,
            Email = "updated@test.org"
        };

        await _handler.Handle(command, CancellationToken.None);

        var updated = await _db.Organizations.FindAsync(org.Id);
        updated!.Name.Should().Be("New Name");
        updated.Level.Should().Be(2);
        updated.IsActive.Should().BeFalse();
        updated.Email.Should().Be("updated@test.org");
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        UpdateOrganizationCommand command = new UpdateOrganizationCommand { Id = 999, Name = "Ghost", Level = 1 };

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithDeletedOrg_ThrowsKeyNotFoundException()
    {
        Organization org = new Organization { Name = "Deleted Org", Level = 1, IsDeleted = true };
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();

        UpdateOrganizationCommand command = new UpdateOrganizationCommand { Id = org.Id, Name = "Revived", Level = 1 };

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
