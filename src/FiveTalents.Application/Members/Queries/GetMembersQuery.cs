using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Common.Models;
using FiveTalents.Application.Members.DTOs;
using FiveTalents.Domain.Auth;
using FiveTalents.Domain.Members;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Members.Queries;

public record GetMembersQuery(
    int? OrganizationId,
    int PageNumber = 1,
    int PageSize = 25,
    string? Search = null,
    MemberStatus? Status = null,
    bool IncludeChildOrgs = false
) : IRequest<PaginatedResult<MemberSummaryDto>>;

public class GetMembersQueryHandler(
    IApplicationDbContext db,
    IOrganizationHierarchyService hierarchyService,
    ICurrentUserService currentUser
) : IRequestHandler<GetMembersQuery, PaginatedResult<MemberSummaryDto>>
{
    public async Task<PaginatedResult<MemberSummaryDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
    {
        bool isSystemAdmin = currentUser.IsInRole(AppRoles.SystemAdmin);

        if (request.OrganizationId is null && !isSystemAdmin)
        {
            throw new ForbiddenAccessException();
        }

        // null OrganizationId = SystemAdmin "see all" — orgIds left empty means no org filter
        IReadOnlyList<int> orgIds;
        if (request.OrganizationId is null)
        {
            orgIds = [];
        }
        else if (request.IncludeChildOrgs)
        {
            orgIds = await hierarchyService.GetDescendantOrgIdsAsync(request.OrganizationId.Value, cancellationToken);
        }
        else
        {
            orgIds = [request.OrganizationId.Value];
        }

        List<string> explicitMemberUserIds = request.IncludeChildOrgs && request.OrganizationId.HasValue
            ? await db.UserOrganizationRoles
                .Where(r => r.OrganizationId == request.OrganizationId.Value && r.IsActive)
                .Select(r => r.UserId)
                .ToListAsync(cancellationToken)
            : [];

        IQueryable<Member> query = orgIds.Count == 0
            ? db.Members.Where(m => !m.IsDeleted)
            : db.Members.Where(m => orgIds.Contains(m.OrganizationId) && !m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(m =>
                m.FirstName.Contains(request.Search) ||
                m.LastName.Contains(request.Search) ||
                m.Emails.Any(e => e.Email.Contains(request.Search)));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(m => m.Status == request.Status);
        }

        int total = await query.CountAsync(cancellationToken);

        var raw = await query
            .OrderBy(m => m.LastName).ThenBy(m => m.FirstName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new
            {
                m.Id,
                m.FullName,
                m.Status,
                m.OrganizationId,
                m.UserId,
                m.ShareEmailWithNetwork,
                m.SharePhoneWithNetwork,
                PrimaryEmail = m.Emails.Where(e => e.IsPrimary).Select(e => e.Email).FirstOrDefault()
                            ?? m.Emails.OrderBy(e => e.Id).Select(e => e.Email).FirstOrDefault(),
                PrimaryPhone = m.Phones.Where(p => p.IsPrimary).Select(p => p.PhoneNumber).FirstOrDefault()
                            ?? m.Phones.OrderBy(p => p.Id).Select(p => p.PhoneNumber).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        bool showOrgNames = isSystemAdmin || request.IncludeChildOrgs;
        Dictionary<int, string> orgNames = [];
        if (showOrgNames)
        {
            orgNames = orgIds.Count == 0
                ? await db.Organizations.ToDictionaryAsync(o => o.Id, o => o.Name, cancellationToken)
                : await db.Organizations
                    .Where(o => orgIds.Contains(o.Id))
                    .ToDictionaryAsync(o => o.Id, o => o.Name, cancellationToken);
        }

        List<MemberSummaryDto> items = raw.Select(m =>
        {
            bool isDirectOrg = !isSystemAdmin && m.OrganizationId == request.OrganizationId;
            bool hasExplicitRole = m.UserId != null && explicitMemberUserIds.Contains(m.UserId);
            bool showContact = isSystemAdmin || isDirectOrg || hasExplicitRole;

            return new MemberSummaryDto(
                Id: m.Id,
                FullName: m.FullName,
                PrimaryEmail: showContact || m.ShareEmailWithNetwork ? m.PrimaryEmail : null,
                PrimaryPhone: showContact || m.SharePhoneWithNetwork ? m.PrimaryPhone : null,
                Status: m.Status,
                OrganizationId: m.OrganizationId,
                OrgName: showOrgNames ? orgNames.GetValueOrDefault(m.OrganizationId) : null
            );
        }).ToList();

        return new PaginatedResult<MemberSummaryDto>(items, total, request.PageNumber, request.PageSize);
    }
}
