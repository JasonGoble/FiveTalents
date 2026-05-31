using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Domain.Organizations;

using FluentValidation;

using MediatR;

namespace FiveTalents.Application.Organizations.Commands;

public record CreateOrganizationCommand : IRequest<int>
{
    public string Name { get; init; } = default!;
    public int Level { get; init; }
    public int? ParentOrganizationId { get; init; }
    public string? Description { get; init; }
    public string? Website { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public string? TimeZone { get; init; }
}

public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Level).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Website)
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.Website))
            .WithMessage("Website must be a valid URL.");
    }

    private static bool BeAValidUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}

public class CreateOrganizationCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateOrganizationCommand, int>
{
    public async Task<int> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        Organization org = new Domain.Organizations.Organization
        {
            Name = request.Name,
            Level = request.Level,
            ParentOrganizationId = request.ParentOrganizationId,
            Description = request.Description,
            Website = request.Website,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Address = request.Address,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            TimeZone = request.TimeZone,
            IsActive = true
        };

        db.Organizations.Add(org);
        await db.SaveChangesAsync(cancellationToken);

        db.OrganizationSettings.Add(new Domain.Organizations.OrganizationSettings
        {
            OrganizationId = org.Id,
            Currency = "USD",
            FiscalYearStart = "01-01",
            EnableAttendanceTracking = true
        });
        await db.SaveChangesAsync(cancellationToken);

        return org.Id;
    }
}
