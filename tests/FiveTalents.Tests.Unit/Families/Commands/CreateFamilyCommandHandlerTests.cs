using FiveTalents.Application.Families.Commands;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;
using FluentAssertions;

namespace FiveTalents.Tests.Unit.Families.Commands;

public class CreateFamilyCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly CreateFamilyCommandHandler _handler;

    public CreateFamilyCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new CreateFamilyCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsFamilyId()
    {
        var command = new CreateFamilyCommand(OrganizationId: 1, Name: "The Smiths");

        int id = await _handler.Handle(command, CancellationToken.None);

        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Handle_StoresFamilyWithCorrectProperties()
    {
        var command = new CreateFamilyCommand(OrganizationId: 5, Name: "The Joneses");

        int id = await _handler.Handle(command, CancellationToken.None);

        var family = await _db.Families.FindAsync(id);
        family.Should().NotBeNull();
        family!.Name.Should().Be("The Joneses");
        family.OrganizationId.Should().Be(5);
    }

    [Fact]
    public async Task Handle_CreatingMultipleFamilies_EachGetsUniqueId()
    {
        int id1 = await _handler.Handle(new CreateFamilyCommand(1, "Family A"), CancellationToken.None);
        int id2 = await _handler.Handle(new CreateFamilyCommand(1, "Family B"), CancellationToken.None);

        id1.Should().NotBe(id2);
    }
}
