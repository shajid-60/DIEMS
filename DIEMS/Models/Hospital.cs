using System;

namespace DIEMS.Models
{
    public class Hospital
    {
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public int CapacityBeds { get; set; }
        public int AvailableBeds { get; set; }
        public int CapacityIcu { get; set; }
        public int AvailableIcu { get; set; }
        public int HasSurgery { get; set; }
        public string ContactPhone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int BloodStockOPos { get; set; }
        public int BloodStockONeg { get; set; }
        public int BloodStockAPos { get; set; }
        public int IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class Doctor
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int HospitalId { get; set; }
        public string AvailabilityStatus { get; set; }
        public int IsAvailable { get; set; }
        public string DeploymentLocation { get; set; }
        public int? AssignedDisasterId { get; set; }

        // Joins
        public string HospitalName { get; set; }
        public string DisasterName { get; set; }
    }

    public class Ambulance
    {
        public int AmbulanceId { get; set; }
        public string VehicleNo { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public int HospitalId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Status { get; set; }
        public string AmbulanceType { get; set; }
        public string CurrentLocation { get; set; }
        public DateTime LastUpdated { get; set; }

        // Joins
        public string HospitalName { get; set; }
    }

    public class MedicalRequest
    {
        public int RequestId { get; set; }
        public int HospitalId { get; set; }
        public string RequestType { get; set; }
        public string RequestDetails { get; set; }
        public string Priority { get; set; }
        public string BloodGroup { get; set; }
        public int? DisasterId { get; set; }
        public string Status { get; set; }
        public int RequestedBy { get; set; }
        public DateTime RequestedAt { get; set; }
        public string ResponseNotes { get; set; }

        // Joins
        public string HospitalName { get; set; }
        public string DisasterName { get; set; }
        public string RequestedByName { get; set; }
    }
}
