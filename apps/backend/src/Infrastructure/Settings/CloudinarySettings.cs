using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Settings;

public sealed class CloudinarySettings
{
    [Required]
    public string CloudName { get; set; } = string.Empty;
    
    [Required]
    public string ApiKey { get; set; } = string.Empty;
    
    [Required]
    public string ApiSecret { get; set; } = string.Empty;
}
