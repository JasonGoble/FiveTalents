using FluentValidation.Results;

namespace FiveTalents.Application.Common.Exceptions;

public class ValidationException(IEnumerable<ValidationFailure> failures) : Exception("One or more validation failures occurred.")
{
    public IDictionary<string, string[]> Errors { get; } = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
}
