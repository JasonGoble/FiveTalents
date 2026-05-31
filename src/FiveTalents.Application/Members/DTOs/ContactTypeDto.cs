namespace FiveTalents.Application.Members.DTOs;

public record ContactTypeDto(int Id, string Name);

public record ContactTypesDto(
    IReadOnlyList<ContactTypeDto> AddressTypes,
    IReadOnlyList<ContactTypeDto> EmailTypes,
    IReadOnlyList<ContactTypeDto> PhoneTypes
);

// Input records used by create/update commands
public record MemberAddressInput(
    int? Id,
    int ContactTypeId,
    bool IsPrimary,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country
);

public record MemberEmailInput(
    int? Id,
    int ContactTypeId,
    bool IsPrimary,
    string Email
);

public record MemberPhoneInput(
    int? Id,
    int ContactTypeId,
    bool IsPrimary,
    string PhoneNumber,
    bool IsMobile
);
