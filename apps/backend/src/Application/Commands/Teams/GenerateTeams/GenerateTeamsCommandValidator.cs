using FluentValidation;

namespace Application.Commands.Teams.GenerateTeams;

internal sealed class GenerateTeamsCommandValidator : AbstractValidator<GenerateTeamsCommand>
{
    public GenerateTeamsCommandValidator()
    {
        RuleFor(x => x.Roles).NotEmpty();
        RuleFor(x => x.Technologies).NotEmpty();
        RuleFor(x => x.SfiaLevel).InclusiveBetween(1, 7);
        RuleFor(x => x.CriteriaWeights).NotEmpty();
        RuleFor(x => x.CriteriaWeights["technical"] + x.CriteriaWeights["psychological"] + x.CriteriaWeights["interests"])
            .Equal(100).WithMessage("The sum of weights must be 100");
    }
}
