using System;

namespace DIEMS.Models
{
    public class Victim
    {
        public int VictimId { get; set; }
        public string Nid { get; set; }
        public string FullName { get; set; }
        public int? Age { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string EmergencyContact { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public int DisasterId { get; set; }
        public int? ShelterId { get; set; }
        public string MedicalCondition { get; set; }
        public string BloodGroup { get; set; }
        public string Status { get; set; }
        public int? RegisteredBy { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Notes { get; set; }

        // Joins
        public string DisasterName { get; set; }
        public string ShelterName { get; set; }
        public string RegisteredByName { get; set; }
    }

    public class FamilyMember
    {
        public int FmId { get; set; }
        public int VictimId { get; set; }
        public string FullName { get; set; }
        public string Relation { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public int IsSeparated { get; set; }
        public string LastKnownLoc { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }

    public class MissingPerson
    {
        public int MissingId { get; set; }
        public string FullName { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public string LastSeenLocation { get; set; }
        public DateTime? LastSeenDate { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPhone { get; set; }
        public int DisasterId { get; set; }
        public string Status { get; set; }
        public string PhotoUrl { get; set; }
        public int? ReportedBy { get; set; }
        public DateTime ReportedAt { get; set; }
        public string Notes { get; set; }

        // Joins
        public string DisasterName { get; set; }
        public string ReportedByName { get; set; }
    }
}
