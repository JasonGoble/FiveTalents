using FiveTalents.Application.Organizations.Queries;
using FiveTalents.Domain.Members;
using FiveTalents.Domain.Organizations;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;
using FluentAssertions;

namespace FiveTalents.Tests.Unit.Organizations.Queries;

public class GetOrganizationByIdQueryHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly GetOrganizationByIdQueryHandler _handler;

    public GetOrganizationByIdQueryHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new GetOrganizationByIdQueryHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithExistingId_ReturnsDto()
    {
        var org = new Organization { Name = "St. Andrew's", Level = 2, IsActive = true, Email = "info@standrew.test" };
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetOrganizationByIdQuery(org.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("St. Andrew's");
        result.Email.Should().Be("info@standrew.test");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ReturnsNull()
    {
        var result = await _handler.Handle(new GetOrganizationByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithDeletedOrg_ReturnsNull()
    {
        var org = new Organization { Name = "Gone Org", Level = 1, IsDeleted = true };
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetOrganizationByIdQuery(org.Id), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_IncludesMemberCount()
    {
        var org = new Organization { Name = "Counted Org", Level = 1, IsActive = true };
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();
        _db.Members.Add(new Member { OrganizationId = org.Id, FirstName = "Jane", LastName = "Doe" });
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetOrganizationByIdQuery(org.Id), CancellationToken.None);

        result!.MemberCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithParentOrg_ReturnsParentName()
    {
        var parent = new Organization { Name = "Diocese", Level = 1, IsActive = true };
        _db.Organizations.Add(parent);
        await _db.SaveChangesAsync();
        var child = new Organization { Name = "Parish", Level = 2, IsActive = true, ParentOrganizationId = parent.Id };
        _db.Organizations.Add(child);
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetOrganizationByIdQuery(child.Id), CancellationToken.None);

        result!.ParentName.Should().Be("Diocese");
        result.ParentOrganizationId.Should().Be(parent.Id);
    }
}
