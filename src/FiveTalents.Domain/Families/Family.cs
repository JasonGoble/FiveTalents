using FiveTalents.Domain.Common;

namespace FiveTalents.Domain.Families;

public class Family : AuditableEntity
{
    public string Name { get; set; } = default!;

    public ICollection<FamilyMember> Members { get; set; } = [];
}
