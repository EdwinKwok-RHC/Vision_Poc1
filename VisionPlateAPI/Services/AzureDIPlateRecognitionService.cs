using Azure;
using Azure.AI.DocumentIntelligence;
using VisionPlateAPI.DTOs;

namespace VisionPlateAPI.Services;

public class AzureDIPlateRecognitionService : IAiVisionRecognitionService
{
    private readonly DocumentIntelligenceClient _client;
    private readonly string _customModelId;

    public AzureDIPlateRecognitionService(string endpoint, string apiKey, string customModelId = "HVAC")
    {
        var credential = new AzureKeyCredential(apiKey);
        _client = new DocumentIntelligenceClient(new Uri(endpoint), credential);
        _customModelId = customModelId;
    }

    public async Task<RatingPlate> ExtractRatingPlateInfoAsync(Stream imageStream)
    {
        try
        {
            // Analyze document using custom model
            var content = new AnalyzeDocumentContent()
            {
                Base64Source = BinaryData.FromStream(imageStream)
            };

            Operation<AnalyzeResult> operation = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                _customModelId,
                content);

            AnalyzeResult result = operation.Value;

            return ExtractRatingPlateFromResult(result);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to extract rating plate information: {ex.Message}", ex);
        }
    }

    public async Task<RatingPlate> ExtractRatingPlateInfoAsync(byte[] imageBytes)
    {
        using var stream = new MemoryStream(imageBytes);
        return await ExtractRatingPlateInfoAsync(stream);
    }

    public async Task<RatingPlate> ExtractRatingPlateInfoAsync(string imageUrl)
    {
        try
        {
            var content = new AnalyzeDocumentContent()
            {
                UrlSource = new Uri(imageUrl)
            };

            Operation<AnalyzeResult> operation = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                _customModelId,
                content);

            AnalyzeResult result = operation.Value;

            return ExtractRatingPlateFromResult(result);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to extract rating plate information from URL: {ex.Message}", ex);
        }
    }

    private RatingPlate ExtractRatingPlateFromResult(AnalyzeResult result)
    {
        var ratingPlate = new RatingPlate();

        if (result.Documents?.Count > 0)
        {
            var document = result.Documents[0];

            // Extract fields based on your custom model's trained fields
            // Adjust field names according to your HVAC model's schema

            if (document.Fields.TryGetValue("Manufacturer", out DocumentField manufacturerField) &&
                manufacturerField.FieldType == DocumentFieldType.String)
            {
                ratingPlate.Manufacturer = manufacturerField.ValueString;
                ratingPlate.ManufacturerConfidence = manufacturerField.Confidence ?? 0;
            }

            if (document.Fields.TryGetValue("ModelNumber", out DocumentField modelField) &&
                modelField.FieldType == DocumentFieldType.String)
            {
                ratingPlate.ModelNumber = modelField.ValueString;
                ratingPlate.ModelNumberConfidence = modelField.Confidence ?? 0;
            }

            if (document.Fields.TryGetValue("SerialNumber", out DocumentField serialField) &&
                serialField.FieldType == DocumentFieldType.String)
            {
                ratingPlate.SerialNumber = serialField.ValueString;
                ratingPlate.SerialNumberConfidence = serialField.Confidence ?? 0;
            }

            //if (document.Fields.TryGetValue("Voltage", out DocumentField voltageField))
            //{
            //    if (voltageField.FieldType == DocumentFieldType.String)
            //    {
            //        ratingPlate.Voltage = voltageField.ValueString;
            //    }
            //    else if (voltageField.FieldType == DocumentFieldType.Double)
            //    {
            //        ratingPlate.Voltage = voltageField.ValueDouble?.ToString();
            //    }
            //    ratingPlate.VoltageConfidence = voltageField.Confidence ?? 0;
            //}

            //if (document.Fields.TryGetValue("Current", out DocumentField currentField))
            //{
            //    if (currentField.FieldType == DocumentFieldType.String)
            //    {
            //        ratingPlate.Current = currentField.ValueString;
            //    }
            //    else if (currentField.FieldType == DocumentFieldType.Double)
            //    {
            //        ratingPlate.Current = currentField.ValueDouble?.ToString();
            //    }
            //    ratingPlate.CurrentConfidence = currentField.Confidence ?? 0;
            //}

            //if (document.Fields.TryGetValue("Power", out DocumentField powerField))
            //{
            //    if (powerField.FieldType == DocumentFieldType.String)
            //    {
            //        ratingPlate.Power = powerField.ValueString;
            //    }
            //    else if (powerField.FieldType == DocumentFieldType.Double)
            //    {
            //        ratingPlate.Power = powerField.ValueDouble?.ToString();
            //    }
            //    ratingPlate.PowerConfidence = powerField.Confidence ?? 0;
            //}

            //if (document.Fields.TryGetValue("Frequency", out DocumentField frequencyField))
            //{
            //    if (frequencyField.FieldType == DocumentFieldType.String)
            //    {
            //        ratingPlate.Frequency = frequencyField.ValueString;
            //    }
            //    else if (frequencyField.FieldType == DocumentFieldType.Double)
            //    {
            //        ratingPlate.Frequency = frequencyField.ValueDouble?.ToString();
            //    }
            //    ratingPlate.FrequencyConfidence = frequencyField.Confidence ?? 0;
            //}

            //if (document.Fields.TryGetValue("BTU", out DocumentField btuField))
            //{
            //    if (btuField.FieldType == DocumentFieldType.String)
            //    {
            //        ratingPlate.BTU = btuField.ValueString;
            //    }
            //    else if (btuField.FieldType == DocumentFieldType.Double)
            //    {
            //        ratingPlate.BTU = btuField.ValueDouble?.ToString();
            //    }

            // Overall document confidence
            // Replace this line:
            // ratingPlate.OverallConfidence = document.Confidence ?? 0;
            // With this line:
            ratingPlate.OverallConfidence = document.Confidence;
            //    ratingPlate.BTUConfidence = btuField.Confidence ?? 0;
            //}


        }

        return ratingPlate;
    }
}
