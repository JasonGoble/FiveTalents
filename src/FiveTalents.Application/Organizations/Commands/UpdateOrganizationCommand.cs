using FiveTalents.Application.Common.Interfaces;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Organizations.Commands;

public record UpdateOrganizationCommand : IRequest
{
    public int Id { get; init; }
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
    public bool IsActive { get; init; }
}

public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
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

public class UpdateOrganizationCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateOrganizationCommand>
{
    public async Task Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var org = await db.Organizations
            .FirstOrDefaultAsync(o => o.Id == request.Id && !o.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Organization {request.Id} not found.");

        org.Name = request.Name;
        org.Level = request.Level;
        org.ParentOrganizationId = request.ParentOrganizationId;
        org.Description = request.Description;
        org.Website = request.Website;
        org.PhoneNumber = request.PhoneNumber;
        org.Email = request.Email;
        org.Address = request.Address;
        org.City = request.City;
        org.State = request.State;
        org.PostalCode = request.PostalCode;
        org.Country = request.Country;
        org.TimeZone = request.TimeZone;
        org.IsActive = request.IsActive;

        await db.SaveChangesAsync(cancellationToken);
    }
}
