using System.ComponentModel.DataAnnotations;

namespace VisionPlateAPI.Configuration;

public class AzureDIConfiguration
{
    public const string SectionName = "AzureDI";

    [Required]
    public string Endpoint { get; set; } = string.Empty;
    [Required]
    public string ApiKey { get; set; } = string.Empty;
    //[Required]
    public string CustomModelId { get; set; } = string.Empty;
}
