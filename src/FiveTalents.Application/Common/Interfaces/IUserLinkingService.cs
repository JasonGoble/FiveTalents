namespace FiveTalents.Application.Common.Interfaces;

public interface IUserLinkingService
{
    public Task<UserLinkInfo?> FindByIdAsync(string userId);
    public Task<UserLinkInfo?> FindByEmailAsync(string email);
    public Task<IReadOnlyList<UserLinkInfo>> GetUnlinkedUsersAsync();
    public Task SetMemberLinkAsync(string userId, int? memberId);
    public Task<string> CreateUserForMemberAsync(string email, string firstName, string lastName, int memberId, int primaryOrgId);
    public Task<string> GenerateInviteTokenAsync(string userId);
}

public record UserLinkInfo(string Id, string Email, string FullName, int? MemberId, bool IsActive);
