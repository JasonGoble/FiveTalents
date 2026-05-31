using FiveTalents.Domain.Members;

namespace FiveTalents.Infrastructure.Persistence;

internal static class DevSeedData
{
    internal record SeedOrg(string Name, int Level, string? ParentName, string TimeZone = "America/Chicago");

    internal record SeedMember(
        string FirstName, string LastName,
        Gender? Gender, MaritalStatus? MaritalStatus, MemberStatus Status,
        string CampusName,
        DateTime? DateOfBirth = null, DateTime? JoinDate = null);

    internal record SeedFamilySpec(
        string FamilyName, string CampusName,
        (string First, string Last, bool IsAdult)[] Lineup);

    internal static SeedOrg[] Organizations() =>
    [
        new("Grace Network",       1, null),
        new("Eastern Region",      2, "Grace Network"),
        new("Western Region",      2, "Grace Network"),
        new("St. Andrew's Campus", 3, "Eastern Region"),
        new("Cornerstone Campus",  3, "Eastern Region"),
        new("Riverside Campus",    3, "Western Region"),
        new("Hillside Campus",     3, "Western Region"),
    ];

    internal static SeedMember[] Members() =>
    [
        // St. Andrew's — Harrison family (2 adults, 3 children)
        new("Robert",   "Harrison",  Domain.Members.Gender.Male,   Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "St. Andrew's Campus", JoinDate: new(2018, 3, 15)),
        new("Susan",    "Harrison",  Domain.Members.Gender.Female, Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "St. Andrew's Campus", JoinDate: new(2018, 3, 15)),
        new("Emily",    "Harrison",  Domain.Members.Gender.Female, null,                                  MemberStatus.Active,  "St. Andrew's Campus", DateOfBirth: new(2010, 6, 20)),
        new("James",    "Harrison",  Domain.Members.Gender.Male,   null,                                  MemberStatus.Active,  "St. Andrew's Campus", DateOfBirth: new(2013, 9, 5)),
        new("Noah",     "Harrison",  Domain.Members.Gender.Male,   null,                                  MemberStatus.Active,  "St. Andrew's Campus", DateOfBirth: new(2016, 2, 14)),

        // St. Andrew's — Chen family (2 adults, 2 children)
        new("David",    "Chen",      Domain.Members.Gender.Male,   Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "St. Andrew's Campus", JoinDate: new(2020, 9, 1)),
        new("Grace",    "Chen",      Domain.Members.Gender.Female, Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "St. Andrew's Campus", JoinDate: new(2020, 9, 1)),
        new("Lily",     "Chen",      Domain.Members.Gender.Female, null,                                  MemberStatus.Active,  "St. Andrew's Campus", DateOfBirth: new(2014, 4, 12)),
        new("Ethan",    "Chen",      Domain.Members.Gender.Male,   null,                                  MemberStatus.Active,  "St. Andrew's Campus", DateOfBirth: new(2017, 11, 8)),

        // Cornerstone — Martinez (married couple, no kids)
        new("Carlos",   "Martinez",  Domain.Members.Gender.Male,   Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "Cornerstone Campus",  JoinDate: new(2019, 5, 20)),
        new("Maria",    "Martinez",  Domain.Members.Gender.Female, Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "Cornerstone Campus",  JoinDate: new(2019, 5, 20)),

        // Cornerstone — Thompson (single parent, 2 children)
        new("Jennifer", "Thompson",  Domain.Members.Gender.Female, Domain.Members.MaritalStatus.Divorced, MemberStatus.Active,  "Cornerstone Campus",  JoinDate: new(2021, 1, 10)),
        new("Lucas",    "Thompson",  Domain.Members.Gender.Male,   null,                                  MemberStatus.Active,  "Cornerstone Campus",  DateOfBirth: new(2012, 8, 25)),
        new("Olivia",   "Thompson",  Domain.Members.Gender.Female, null,                                  MemberStatus.Active,  "Cornerstone Campus",  DateOfBirth: new(2015, 3, 17)),

        // Cornerstone — Williams (elderly couple)
        new("George",   "Williams",  Domain.Members.Gender.Male,   Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "Cornerstone Campus",  DateOfBirth: new(1948, 7, 4),  JoinDate: new(2005, 6, 1)),
        new("Dorothy",  "Williams",  Domain.Members.Gender.Female, Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "Cornerstone Campus",  DateOfBirth: new(1950, 12, 22), JoinDate: new(2005, 6, 1)),

        // Riverside — Patel (couple with 1 child)
        new("Raj",      "Patel",     Domain.Members.Gender.Male,   Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "Riverside Campus",    JoinDate: new(2022, 4, 3)),
        new("Priya",    "Patel",     Domain.Members.Gender.Female, Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "Riverside Campus",    JoinDate: new(2022, 4, 3)),
        new("Arjun",    "Patel",     Domain.Members.Gender.Male,   null,                                  MemberStatus.Active,  "Riverside Campus",    DateOfBirth: new(2018, 7, 30)),

        // Riverside — Kim (couple with 2 children)
        new("Daniel",   "Kim",       Domain.Members.Gender.Male,   Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "Riverside Campus",    JoinDate: new(2017, 11, 15)),
        new("Hannah",   "Kim",       Domain.Members.Gender.Female, Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "Riverside Campus",    JoinDate: new(2017, 11, 15)),
        new("Nathan",   "Kim",       Domain.Members.Gender.Male,   null,                                  MemberStatus.Active,  "Riverside Campus",    DateOfBirth: new(2011, 2, 9)),
        new("Sophia",   "Kim",       Domain.Members.Gender.Female, null,                                  MemberStatus.Active,  "Riverside Campus",    DateOfBirth: new(2014, 10, 3)),

        // Hillside — Robinson (older married couple)
        new("James",    "Robinson",  Domain.Members.Gender.Male,   Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "Hillside Campus",     DateOfBirth: new(1960, 3, 15),  JoinDate: new(2010, 8, 22)),
        new("Patricia", "Robinson",  Domain.Members.Gender.Female, Domain.Members.MaritalStatus.Married,  MemberStatus.Active,  "Hillside Campus",     DateOfBirth: new(1962, 9, 28),  JoinDate: new(2010, 8, 22)),

        // Hillside — singles & widowed
        new("Michael",  "Anderson",  Domain.Members.Gender.Male,   Domain.Members.MaritalStatus.Single,   MemberStatus.Active,  "Hillside Campus",     DateOfBirth: new(1995, 4, 18),  JoinDate: new(2023, 2, 5)),
        new("Sarah",    "Johnson",   Domain.Members.Gender.Female, Domain.Members.MaritalStatus.Single,   MemberStatus.Active,  "Hillside Campus",     DateOfBirth: new(1997, 8, 11),  JoinDate: new(2022, 10, 30)),
        new("Eleanor",  "Davis",     Domain.Members.Gender.Female, Domain.Members.MaritalStatus.Widowed,  MemberStatus.Active,  "Hillside Campus",     DateOfBirth: new(1945, 5, 6),   JoinDate: new(2008, 3, 10)),

        // Hillside — visitors
        new("Alex",     "Morgan",    Domain.Members.Gender.Male,   Domain.Members.MaritalStatus.Single,   MemberStatus.Visitor, "Hillside Campus"),
        new("Melissa",  "Taylor",    Domain.Members.Gender.Female, Domain.Members.MaritalStatus.Married,  MemberStatus.Visitor, "Hillside Campus"),

        // Hillside — inactive
        new("Thomas",   "Lee",       Domain.Members.Gender.Male,   Domain.Members.MaritalStatus.Single,   MemberStatus.Inactive,"Hillside Campus",     JoinDate: new(2021, 6, 1)),
    ];

    internal static SeedFamilySpec[] Families() =>
    [
        new("Harrison", "St. Andrew's Campus", [("Robert", "Harrison", true), ("Susan", "Harrison", true), ("Emily", "Harrison", false), ("James", "Harrison", false), ("Noah", "Harrison", false)]),
        new("Chen",     "St. Andrew's Campus", [("David", "Chen", true), ("Grace", "Chen", true), ("Lily", "Chen", false), ("Ethan", "Chen", false)]),
        new("Martinez", "Cornerstone Campus",  [("Carlos", "Martinez", true), ("Maria", "Martinez", true)]),
        new("Thompson", "Cornerstone Campus",  [("Jennifer", "Thompson", true), ("Lucas", "Thompson", false), ("Olivia", "Thompson", false)]),
        new("Williams", "Cornerstone Campus",  [("George", "Williams", true), ("Dorothy", "Williams", true)]),
        new("Patel",    "Riverside Campus",    [("Raj", "Patel", true), ("Priya", "Patel", true), ("Arjun", "Patel", false)]),
        new("Kim",      "Riverside Campus",    [("Daniel", "Kim", true), ("Hannah", "Kim", true), ("Nathan", "Kim", false), ("Sophia", "Kim", false)]),
        new("Robinson", "Hillside Campus",     [("James", "Robinson", true), ("Patricia", "Robinson", true)]),
    ];
}
