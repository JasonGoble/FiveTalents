using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Members.Queries;
using FiveTalents.Domain.Members;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;
using FluentAssertions;
using NSubstitute;

namespace FiveTalents.Tests.Unit.Members.Queries;

public class GetMembersQueryHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly IOrganizationHierarchyService _hierarchy;
    private readonly GetMembersQueryHandler _handler;

    public GetMembersQueryHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _hierarchy = Substitute.For<IOrganizationHierarchyService>();
        _handler = new GetMembersQueryHandler(_db, _hierarchy);
    }

    public void Dispose() => _db.Dispose();

    private async Task SeedMembersAsync(params Member[] members)
    {
        _db.Members.AddRange(members);
        await _db.SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ReturnsOnlyMembersInRequestedOrg()
    {
        await SeedMembersAsync(
            new Member { OrganizationId = 1, FirstName = "Alice", LastName = "Smith" },
            new Member { OrganizationId = 2, FirstName = "Bob", LastName = "Jones" });

        var result = await _handler.Handle(new GetMembersQuery(OrganizationId: 1), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].FullName.Should().Be("Alice Smith");
    }

    [Fact]
    public async Task Handle_ExcludesSoftDeletedMembers()
    {
        await SeedMembersAsync(
            new Member { OrganizationId = 1, FirstName = "Active", LastName = "Member" },
            new Member { OrganizationId = 1, FirstName = "Deleted", LastName = "Member", IsDeleted = true });

        var result = await _handler.Handle(new GetMembersQuery(OrganizationId: 1), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].FullName.Should().Be("Active Member");
    }

    [Fact]
    public async Task Handle_WithSearchTerm_FiltersOnFirstAndLastName()
    {
        await SeedMembersAsync(
            new Member { OrganizationId = 1, FirstName = "Alice", LastName = "Smith" },
            new Member { OrganizationId = 1, FirstName = "Bob", LastName = "Jones" });

        var result = await _handler.Handle(new GetMembersQuery(OrganizationId: 1, Search: "Jones"), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].FullName.Should().Be("Bob Jones");
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ReturnsOnlyMatchingStatus()
    {
        await SeedMembersAsync(
            new Member { OrganizationId = 1, FirstName = "Active", LastName = "One", Status = MemberStatus.Active },
            new Member { OrganizationId = 1, FirstName = "Inactive", LastName = "One", Status = MemberStatus.Inactive });

        var result = await _handler.Handle(
            new GetMembersQuery(OrganizationId: 1, Status: MemberStatus.Inactive), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].FullName.Should().Be("Inactive One");
    }

    [Fact]
    public async Task Handle_ReturnsCorrectPaginationMetadata()
    {
        await SeedMembersAsync(
            new Member { OrganizationId = 1, FirstName = "A", LastName = "One" },
            new Member { OrganizationId = 1, FirstName = "B", LastName = "Two" },
            new Member { OrganizationId = 1, FirstName = "C", LastName = "Three" });

        var result = await _handler.Handle(
            new GetMembersQuery(OrganizationId: 1, PageNumber: 1, PageSize: 2), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ReturnsItemsOrderedByLastNameThenFirstName()
    {
        await SeedMembersAsync(
            new Member { OrganizationId = 1, FirstName = "Bob", LastName = "Smith" },
            new Member { OrganizationId = 1, FirstName = "Alice", LastName = "Smith" },
            new Member { OrganizationId = 1, FirstName = "Carol", LastName = "Adams" });

        var result = await _handler.Handle(new GetMembersQuery(OrganizationId: 1), CancellationToken.None);

        result.Items[0].FullName.Should().Be("Carol Adams");
        result.Items[1].FullName.Should().Be("Alice Smith");
        result.Items[2].FullName.Should().Be("Bob Smith");
    }
}
