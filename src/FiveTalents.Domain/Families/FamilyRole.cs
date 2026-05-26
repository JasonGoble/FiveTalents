using FiveTalents.Domain.Common;

namespace FiveTalents.Domain.Families;

public class FamilyRole : AuditableEntity
{
    public string Name { get; set; } = default!;
    public bool IsAdult { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<FamilyMember> FamilyMembers { get; set; } = [];
}
