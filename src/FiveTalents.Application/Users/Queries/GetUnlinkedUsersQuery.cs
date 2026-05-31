using FiveTalents.Application.Common.Interfaces;

using MediatR;

namespace FiveTalents.Application.Users.Queries;

public record UserSummaryDto(string Id, string Email, string FullName);

public record GetUnlinkedUsersQuery : IRequest<IReadOnlyList<UserSummaryDto>>;

public class GetUnlinkedUsersQueryHandler(IUserLinkingService userService)
    : IRequestHandler<GetUnlinkedUsersQuery, IReadOnlyList<UserSummaryDto>>
{
    public async Task<IReadOnlyList<UserSummaryDto>> Handle(GetUnlinkedUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userService.GetUnlinkedUsersAsync();
        return users.Select(u => new UserSummaryDto(u.Id, u.Email, u.FullName)).ToList();
    }
}
