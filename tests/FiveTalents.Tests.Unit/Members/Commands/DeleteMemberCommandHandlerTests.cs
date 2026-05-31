using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Members.Commands;
using FiveTalents.Domain.Members;
using FiveTalents.Infrastructure.Persistence;
using FiveTalents.Tests.Unit.Helpers;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Tests.Unit.Members.Commands;

public class DeleteMemberCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly DeleteMemberCommandHandler _handler;

    public DeleteMemberCommandHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _handler = new DeleteMemberCommandHandler(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Handle_WithExistingMember_SoftDeletesMember()
    {
        Member member = new Member { OrganizationId = 1, FirstName = "Alice", LastName = "Smith" };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        await _handler.Handle(new DeleteMemberCommand(member.Id), CancellationToken.None);

        var deleted = await _db.Members.IgnoreQueryFilters().FirstAsync(m => m.Id == member.Id);
        deleted.IsDeleted.Should().BeTrue();
        deleted.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ThrowsNotFoundException()
    {
        var act = async () => await _handler.Handle(new DeleteMemberCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithAlreadyDeletedMember_ThrowsNotFoundException()
    {
        Member member = new Member { OrganizationId = 1, FirstName = "Bob", LastName = "Jones", IsDeleted = true };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        var act = async () => await _handler.Handle(new DeleteMemberCommand(member.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithExistingMember_DoesNotHardDelete()
    {
        Member member = new Member { OrganizationId = 1, FirstName = "Carol", LastName = "White" };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        await _handler.Handle(new DeleteMemberCommand(member.Id), CancellationToken.None);

        bool stillExists = await _db.Members.IgnoreQueryFilters().AnyAsync(m => m.Id == member.Id);
        stillExists.Should().BeTrue();
    }
}
