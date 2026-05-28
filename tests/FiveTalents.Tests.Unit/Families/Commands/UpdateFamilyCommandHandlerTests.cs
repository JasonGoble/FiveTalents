using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Families.Commands;
using FiveTalents.Domain.Families;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;
using FluentAssertions;

namespace FiveTalents.Tests.Unit.Families.Commands;

public class UpdateFamilyCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly UpdateFamilyCommandHandler _handler;

    public UpdateFamilyCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new UpdateFamilyCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithExistingFamily_UpdatesName()
    {
        var family = new Family { OrganizationId = 1, Name = "Old Name" };
        _db.Families.Add(family);
        await _db.SaveChangesAsync();

        await _handler.Handle(new UpdateFamilyCommand(family.Id, "New Name"), CancellationToken.None);

        var updated = await _db.Families.FindAsync(family.Id);
        updated!.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ThrowsNotFoundException()
    {
        var act = async () => await _handler.Handle(new UpdateFamilyCommand(999, "Ghost"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithDeletedFamily_ThrowsNotFoundException()
    {
        var family = new Family { OrganizationId = 1, Name = "Deleted", IsDeleted = true };
        _db.Families.Add(family);
        await _db.SaveChangesAsync();

        var act = async () => await _handler.Handle(new UpdateFamilyCommand(family.Id, "Revived"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
