using FluentValidation;

namespace Application.Queries.Teams.GetById;

internal sealed class GetTeamByIdQueryValidator : AbstractValidator<GetTeamByIdQuery>
{
    public GetTeamByIdQueryValidator()
    {
        RuleFor(x => x.TeamId).NotEmpty();
    }
}
