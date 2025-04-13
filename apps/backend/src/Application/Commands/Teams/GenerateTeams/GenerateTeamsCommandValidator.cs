using FluentValidation;

namespace Application.Commands.Teams.GenerateTeams;

internal sealed class GenerateTeamsCommandValidator : AbstractValidator<GenerateTeamsCommand>
{
    public GenerateTeamsCommandValidator()
    {
        RuleFor(x => x.Roles).NotEmpty();
        RuleFor(x => x.Technologies).NotEmpty();
        RuleFor(x => x.SfiaLevel).InclusiveBetween(1, 7);
        RuleFor(x => x.TeamSize).GreaterThan(0);
        RuleFor(x => x.Weights).NotNull();
        RuleFor(x => x.Weights.SfiaWeight).InclusiveBetween(0, 100);
        RuleFor(x => x.Weights.TechnicalWeight).InclusiveBetween(0, 100);
        RuleFor(x => x.Weights.PsychologicalWeight).InclusiveBetween(0, 100);
        RuleFor(x => x.Weights.ExperienceWeight).InclusiveBetween(0, 100);
        RuleFor(x => x.Weights.LanguageWeight).InclusiveBetween(0, 100);
        RuleFor(x => x.Weights.InterestsWeight).InclusiveBetween(0, 100);
        RuleFor(x => x.Weights.TimezoneWeight).InclusiveBetween(0, 100);
        RuleFor(x => x.Weights)
            .Must(weights =>
            {
                int totalWeight =
                    weights.SfiaWeight
                    + weights.TechnicalWeight
                    + weights.PsychologicalWeight
                    + weights.ExperienceWeight
                    + weights.LanguageWeight
                    + weights.InterestsWeight
                    + weights.TimezoneWeight;
                return totalWeight == 100;
            })
            .WithMessage("Total weight must equal 100%.");
    }
}
