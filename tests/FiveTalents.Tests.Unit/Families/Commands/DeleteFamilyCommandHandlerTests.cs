using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Families.Commands;
using FiveTalents.Domain.Families;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Tests.Unit.Families.Commands;

public class DeleteFamilyCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly DeleteFamilyCommandHandler _handler;

    public DeleteFamilyCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new DeleteFamilyCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithExistingFamily_SetsIsDeleted()
    {
        var family = new Family { OrganizationId = 1, Name = "Smith Family" };
        _db.Families.Add(family);
        await _db.SaveChangesAsync();

        await _handler.Handle(new DeleteFamilyCommand(family.Id), CancellationToken.None);

        var deleted = await _db.Families.IgnoreQueryFilters().FirstAsync(f => f.Id == family.Id);
        deleted.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ThrowsNotFoundException()
    {
        var act = async () => await _handler.Handle(new DeleteFamilyCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithAlreadyDeletedFamily_ThrowsNotFoundException()
    {
        var family = new Family { OrganizationId = 1, Name = "Gone Family", IsDeleted = true };
        _db.Families.Add(family);
        await _db.SaveChangesAsync();

        var act = async () => await _handler.Handle(new DeleteFamilyCommand(family.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
