namespace VisionPlateAPI.Services;

using System.IO;
using System.Threading.Tasks;
using VisionPlateAPI.DTOs;

public interface IAiVisionRecognitionService
{
    Task<RatingPlate> ExtractRatingPlateInfoAsync(Stream imageStream);
    Task<RatingPlate> ExtractRatingPlateInfoAsync(byte[] imageBytes);
    Task<RatingPlate> ExtractRatingPlateInfoAsync(string imageUrl);
}

