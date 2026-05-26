using FiveTalents.Domain.Common;
using FiveTalents.Domain.Families;

namespace FiveTalents.Domain.Members;

public enum Gender { Male, Female, Other, PreferNotToSay }
public enum MaritalStatus { Single, Married, Divorced, Widowed }
public enum MemberStatus { Active, Inactive, Visitor, Deceased }

public class Member : AuditableEntity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? MiddleName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    public MemberStatus Status { get; set; } = MemberStatus.Active;
    public DateTime? JoinDate { get; set; }
    public DateTime? BaptismDate { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? Occupation { get; set; }
    public string? Employer { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Notes { get; set; }
    public string? UserId { get; set; }
    public bool SharePhoneWithNetwork { get; set; } = false;
    public bool ShareEmailWithNetwork { get; set; } = false;
    public bool ShareAddressWithNetwork { get; set; } = false;
    public ICollection<FamilyMember> Families { get; set; } = [];
    public ICollection<MemberTag> Tags { get; set; } = [];
    public ICollection<MemberAddress> Addresses { get; set; } = [];
    public ICollection<MemberEmail> Emails { get; set; } = [];
    public ICollection<MemberPhone> Phones { get; set; } = [];
}
