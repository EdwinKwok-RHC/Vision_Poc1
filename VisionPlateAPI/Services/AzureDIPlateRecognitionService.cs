using Azure;
using Azure.Core; // Required for RequestContent
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
            // Convert Stream to BinaryData
            BinaryData binaryData = BinaryData.FromStream(imageStream);

            // Create AnalyzeDocumentOptions with custom model ID and BinaryData
            AnalyzeDocumentOptions analyzeDocumentOptions = new AnalyzeDocumentOptions(_customModelId, binaryData);

            // Analyze document using custom model
            Operation<AnalyzeResult> operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, analyzeDocumentOptions);

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
            // Use the correct overload: AnalyzeDocumentAsync(WaitUntil, string, Uri)
            Operation<AnalyzeResult> operation = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                _customModelId,
                new Uri(imageUrl));

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

            if (document.Fields.TryGetValue("Make", out DocumentField manufacturerField) &&
                manufacturerField.FieldType == DocumentFieldType.String)
            {
                ratingPlate.Manufacturer = manufacturerField.ValueString;
                ratingPlate.ManufacturerConfidence = manufacturerField.Confidence ?? 0;
            }

            if (document.Fields.TryGetValue("Model", out DocumentField modelField) &&
                modelField.FieldType == DocumentFieldType.String)
            {
                ratingPlate.ModelNumber = modelField.ValueString;
                ratingPlate.ModelNumberConfidence = modelField.Confidence ?? 0;
            }

            if (document.Fields.TryGetValue("Serial", out DocumentField serialField) &&
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
            //        ratingPlate.Current = currentField.ValueDouble?.ToString();.
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

            //    ratingPlate.BTUConfidence = btuField.Confidence ?? 0;
            //}
            
            ratingPlate.OverallConfidence = document.Confidence;
        }

        return ratingPlate;
    }
}
