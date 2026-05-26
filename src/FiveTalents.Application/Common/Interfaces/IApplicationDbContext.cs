using FiveTalents.Domain.Attendance;
using FiveTalents.Domain.Auth;
using FiveTalents.Domain.Communication;
using FiveTalents.Domain.Events;
using FiveTalents.Domain.Families;
using FiveTalents.Domain.Giving;
using FiveTalents.Domain.Groups;
using FiveTalents.Domain.Members;
using FiveTalents.Domain.Organizations;
using FiveTalents.Domain.Sermons;
using FiveTalents.Domain.Volunteers;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Organization> Organizations { get; }
    DbSet<OrganizationLevel> OrganizationLevels { get; }
    DbSet<OrganizationSettings> OrganizationSettings { get; }
    DbSet<Member> Members { get; }
    DbSet<Family> Families { get; }
    DbSet<FamilyRole> FamilyRoles { get; }
    DbSet<FamilyMember> FamilyMembers { get; }
    DbSet<MemberTag> MemberTags { get; }
    DbSet<ContactType> ContactTypes { get; }
    DbSet<MemberAddress> MemberAddresses { get; }
    DbSet<MemberEmail> MemberEmails { get; }
    DbSet<MemberPhone> MemberPhones { get; }
    DbSet<Group> Groups { get; }
    DbSet<GroupType> GroupTypes { get; }
    DbSet<GroupMember> GroupMembers { get; }
    DbSet<GroupMeeting> GroupMeetings { get; }
    DbSet<AttendanceSession> AttendanceSessions { get; }
    DbSet<AttendanceRecord> AttendanceRecords { get; }
    DbSet<Donation> Donations { get; }
    DbSet<DonationFund> DonationFunds { get; }
    DbSet<DonationPledge> DonationPledges { get; }
    DbSet<DonationBatch> DonationBatches { get; }
    DbSet<ChurchEvent> Events { get; }
    DbSet<EventRegistration> EventRegistrations { get; }
    DbSet<CommunicationTemplate> CommunicationTemplates { get; }
    DbSet<CommunicationLog> CommunicationLogs { get; }
    DbSet<VolunteerOpportunity> VolunteerOpportunities { get; }
    DbSet<VolunteerAssignment> VolunteerAssignments { get; }
    DbSet<Sermon> Sermons { get; }
    DbSet<SermonSeries> SermonSeries { get; }
    DbSet<SermonTag> SermonTags { get; }
    DbSet<UserOrganizationRole> UserOrganizationRoles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
