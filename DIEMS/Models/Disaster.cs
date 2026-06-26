using System;

namespace DIEMS.Models
{
    public class Disaster
    {
        public int DisasterId { get; set; }
        public string DisasterName { get; set; }
        public int TypeId { get; set; }
        public int SeverityLevelId { get; set; }
        public string District { get; set; }
        public string Division { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public int AffectedPopulation { get; set; }
        public int Casualties { get; set; }
        public int Injured { get; set; }
        public int Displaced { get; set; }
        public decimal EstimatedDamage { get; set; }
        public string Description { get; set; }
        public string WeatherConditions { get; set; }
        public int ResponseTeams { get; set; }
        public int? ReportedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Joins
        public string TypeName { get; set; }
        public string TypeIcon { get; set; }
        public string TypeColor { get; set; }
        public string SeverityName { get; set; }
        public string SeverityColor { get; set; }
        public int SeverityCode { get; set; }
        public string ReporterName { get; set; }
    }

    public class DisasterType
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string Icon { get; set; }
        public string ColorCode { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }
    }

    public class SeverityLevel
    {
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public int LevelCode { get; set; }
        public string ColorCode { get; set; }
        public string Description { get; set; }
    }

    public class AffectedArea
    {
        public int AreaId { get; set; }
        public int DisasterId { get; set; }
        public string District { get; set; }
        public string Upazila { get; set; }
        public string UnionName { get; set; }
        public double? AreaKm2 { get; set; }
        public int PopulationAtRisk { get; set; }
        public int IsEvacuated { get; set; }
        public DateTime? EvacuationDate { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Joins
        public string DisasterName { get; set; }
    }
}
