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
    public DbSet<Organization> Organizations { get; }
    public DbSet<OrganizationLevel> OrganizationLevels { get; }
    public DbSet<OrganizationSettings> OrganizationSettings { get; }
    public DbSet<Member> Members { get; }
    public DbSet<Family> Families { get; }
    public DbSet<FamilyRole> FamilyRoles { get; }
    public DbSet<FamilyMember> FamilyMembers { get; }
    public DbSet<MemberTag> MemberTags { get; }
    public DbSet<ContactType> ContactTypes { get; }
    public DbSet<MemberAddress> MemberAddresses { get; }
    public DbSet<MemberEmail> MemberEmails { get; }
    public DbSet<MemberPhone> MemberPhones { get; }
    public DbSet<Group> Groups { get; }
    public DbSet<GroupType> GroupTypes { get; }
    public DbSet<GroupMember> GroupMembers { get; }
    public DbSet<GroupMeeting> GroupMeetings { get; }
    public DbSet<AttendanceSession> AttendanceSessions { get; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; }
    public DbSet<Donation> Donations { get; }
    public DbSet<DonationFund> DonationFunds { get; }
    public DbSet<DonationPledge> DonationPledges { get; }
    public DbSet<DonationBatch> DonationBatches { get; }
    public DbSet<ChurchEvent> Events { get; }
    public DbSet<EventRegistration> EventRegistrations { get; }
    public DbSet<CommunicationTemplate> CommunicationTemplates { get; }
    public DbSet<CommunicationLog> CommunicationLogs { get; }
    public DbSet<VolunteerOpportunity> VolunteerOpportunities { get; }
    public DbSet<VolunteerAssignment> VolunteerAssignments { get; }
    public DbSet<Sermon> Sermons { get; }
    public DbSet<SermonSeries> SermonSeries { get; }
    public DbSet<SermonTag> SermonTags { get; }
    public DbSet<UserOrganizationRole> UserOrganizationRoles { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
