using Domain.Entities.Enums;
using Domain.Entities.Users;
using SharedKernel.Domain;

namespace Domain.Entities.Teams;

public class PsychologistReview : Entity
{
    public Guid TeamId { get; set; }
    public Guid PsychologistId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Team Team { get; set; } = null!;
    public User Psychologist { get; set; } = null!;
}
