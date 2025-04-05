using FluentValidation;

namespace Application.Commands.Teams.Create;

internal sealed class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.CreatorId).NotEmpty();
        RuleFor(x => x.RequiredTechnologies).NotNull();
        RuleFor(x => x.MemberIds).NotEmpty();
    }
}
