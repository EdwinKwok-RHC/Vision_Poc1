namespace VisionPlateAPI.DTOs;

public class RatingPlateResponseDto
{
    public string? Manufacturer { get; set; }
    public string? ManufacturerConfidenceRateText { get; set; }

    public string? ModelNumber { get; set; }
    public string? ModelNumberConfidenceRateText { get; set; }

    public string? SerialNumber { get; set; }
    public string? SerialNumberConfidenceRateText { get; set; }

    public string? OverallConfidenceRateText { get; set; }
}
