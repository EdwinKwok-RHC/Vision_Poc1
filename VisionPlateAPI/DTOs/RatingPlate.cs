namespace VisionPlateAPI.DTOs;


public class RatingPlate
{
    public string Manufacturer { get; set; }
    public double ManufacturerConfidence { get; set; }

    public string ModelNumber { get; set; }
    public double ModelNumberConfidence { get; set; }

    public string SerialNumber { get; set; }
    public double SerialNumberConfidence { get; set; }

    public double OverallConfidence { get; set; }
    //public string Voltage { get; set; }
    //public double VoltageConfidence { get; set; }

    //public string Current { get; set; }
    //public double CurrentConfidence { get; set; }

    //public string Power { get; set; }
    //public double PowerConfidence { get; set; }

    //public string Frequency { get; set; }
    //public double FrequencyConfidence { get; set; }

    //public string BTU { get; set; }
    //public double BTUConfidence { get; set; }

    //public double OverallConfidence { get; set; }

    //// Additional HVAC-specific fields
    //public string RefrigerantType { get; set; }
    //public double RefrigerantTypeConfidence { get; set; }

    //public string ManufacturingDate { get; set; }
    //public double ManufacturingDateConfidence { get; set; }

    //public string EnergyEfficiencyRating { get; set; }
    //public double EnergyEfficiencyRatingConfidence { get; set; }
}


