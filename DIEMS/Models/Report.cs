using System;

namespace DIEMS.Models
{
    public class IncidentReport
    {
        public int ReportId { get; set; }
        public int? DisasterId { get; set; }
        public string ReporterName { get; set; }
        public string ReporterPhone { get; set; }
        public string IncidentType { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string District { get; set; }
        public string Upazila { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string SeverityLevel { get; set; }
        public string Status { get; set; }
        public DateTime ReportedAt { get; set; }
        public int? AssignedTo { get; set; }
        public string ResolutionNotes { get; set; }

        // Joins
        public string DisasterName { get; set; }
        public string AssignedToName { get; set; }
    }

    public class ReportAttachment
    {
        public int AttachmentId { get; set; }
        public int ReportId { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public int? FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class AuditLog
    {
        public int LogId { get; set; }
        public string TableName { get; set; }
        public int? RecordId { get; set; }
        public string Operation { get; set; }
        public string ColumnName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string IpAddress { get; set; }
        public string SessionId { get; set; }
        public string Module { get; set; }
        public string Notes { get; set; }
    }
}
