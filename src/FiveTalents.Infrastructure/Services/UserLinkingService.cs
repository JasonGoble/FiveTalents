using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Infrastructure.Services;

public class UserLinkingService(UserManager<ApplicationUser> userManager) : IUserLinkingService
{
    public async Task<UserLinkInfo?> FindByIdAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user == null ? null : new UserLinkInfo(user.Id, user.Email!, user.FullName, user.MemberId, user.IsActive);
    }

    public async Task<UserLinkInfo?> FindByEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user == null ? null : new UserLinkInfo(user.Id, user.Email!, user.FullName, user.MemberId, user.IsActive);
    }

    public async Task<IReadOnlyList<UserLinkInfo>> GetUnlinkedUsersAsync()
    {
        return await userManager.Users
            .Where(u => u.MemberId == null && u.IsActive)
            .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
            .Select(u => new UserLinkInfo(u.Id, u.Email!, u.FullName, null, true))
            .ToListAsync();
    }

    public async Task SetMemberLinkAsync(string userId, int? memberId)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException($"User {userId} not found.");
        user.MemberId = memberId;
        await userManager.UpdateAsync(user);
    }

    public async Task<string> CreateUserForMemberAsync(string email, string firstName, string lastName, int memberId, int primaryOrgId)
    {
        ApplicationUser user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            MemberId = memberId,
            PrimaryOrganizationId = primaryOrgId,
            IsActive = false
        };

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Could not create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return user.Id;
    }

    public async Task<string> GenerateInviteTokenAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException($"User {userId} not found.");
        return await userManager.GeneratePasswordResetTokenAsync(user);
    }
}
