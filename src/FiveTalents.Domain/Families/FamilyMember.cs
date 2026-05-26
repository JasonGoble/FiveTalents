using FiveTalents.Domain.Common;
using FiveTalents.Domain.Members;

namespace FiveTalents.Domain.Families;

public class FamilyMember : BaseEntity
{
    public int FamilyId { get; set; }
    public Family Family { get; set; } = default!;

    public int MemberId { get; set; }
    public Member Member { get; set; } = default!;

    public int FamilyRoleId { get; set; }
    public FamilyRole Role { get; set; } = default!;
}
