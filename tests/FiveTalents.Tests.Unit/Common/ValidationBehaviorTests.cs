using FiveTalents.Application.Common.Behaviors;

using FluentAssertions;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using NSubstitute;

using AppValidationException = FiveTalents.Application.Common.Exceptions.ValidationException;

namespace FiveTalents.Tests.Unit.Common;

public record TestRequest(string Value) : IRequest<int>;

public class ValidationBehaviorTests
{
    private static RequestHandlerDelegate<int> NextReturning(int value)
    {
        var next = Substitute.For<RequestHandlerDelegate<int>>();
        next(Arg.Any<CancellationToken>()).Returns(value);
        return next;
    }

    [Fact]
    public async Task Handle_WithNoValidators_CallsNext()
    {
        ValidationBehavior<TestRequest, int> behavior = new ValidationBehavior<TestRequest, int>([]);
        var next = NextReturning(42);

        int result = await behavior.Handle(new TestRequest("anything"), next, CancellationToken.None);

        result.Should().Be(42);
        await next.Received(1)(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithPassingValidator_CallsNext()
    {
        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        ValidationBehavior<TestRequest, int> behavior = new ValidationBehavior<TestRequest, int>([validator]);
        var next = NextReturning(99);

        int result = await behavior.Handle(new TestRequest("valid"), next, CancellationToken.None);

        result.Should().Be(99);
    }

    [Fact]
    public async Task Handle_WithFailingValidator_ThrowsAppValidationException()
    {
        ValidationFailure failure = new ValidationFailure("Value", "Value is required");
        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([failure]));

        ValidationBehavior<TestRequest, int> behavior = new ValidationBehavior<TestRequest, int>([validator]);

        var act = async () => await behavior.Handle(new TestRequest(""), NextReturning(0), CancellationToken.None);

        var ex = await act.Should().ThrowAsync<AppValidationException>();
        ex.Which.Errors["Value"].Should().Contain("Value is required");
    }

    [Fact]
    public async Task Handle_WithMultipleFailingValidators_AggregatesAllErrors()
    {
        var validator1 = Substitute.For<IValidator<TestRequest>>();
        validator1.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("Value", "Error one")]));

        var validator2 = Substitute.For<IValidator<TestRequest>>();
        validator2.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("Value", "Error two")]));

        ValidationBehavior<TestRequest, int> behavior = new ValidationBehavior<TestRequest, int>([validator1, validator2]);

        var act = async () => await behavior.Handle(new TestRequest(""), NextReturning(0), CancellationToken.None);

        var ex = await act.Should().ThrowAsync<AppValidationException>();
        ex.Which.Errors.Should().ContainKey("Value");
        ex.Which.Errors["Value"].Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithFailingValidator_DoesNotCallNext()
    {
        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("Value", "Required")]));

        ValidationBehavior<TestRequest, int> behavior = new ValidationBehavior<TestRequest, int>([validator]);
        var next = NextReturning(0);

        await Assert.ThrowsAsync<AppValidationException>(
            () => behavior.Handle(new TestRequest(""), next, CancellationToken.None));

        await next.DidNotReceive()(Arg.Any<CancellationToken>());
    }
}
