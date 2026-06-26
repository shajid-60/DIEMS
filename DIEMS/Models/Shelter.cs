using System;

namespace DIEMS.Models
{
    public class Shelter
    {
        public int ShelterId { get; set; }
        public string ShelterName { get; set; }
        public string ShelterType { get; set; }
        public string Location { get; set; }
        public string District { get; set; }
        public string Upazila { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string Facilities { get; set; }
        public int HasMedical { get; set; }
        public int HasGenerator { get; set; }
        public int HasWifi { get; set; }
        public int IsActive { get; set; }
        public DateTime OpenedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Joined capacity properties
        public int MaxCapacity { get; set; }
        public int CurrentOccupied { get; set; }
        public int AvailableBeds { get; set; }
        public int ReservedSpots { get; set; }
        public int HasOverflow { get; set; }
        public string OverflowLocation { get; set; }
    }

    public class ShelterResident
    {
        public int SrId { get; set; }
        public int ShelterId { get; set; }
        public int VictimId { get; set; }
        public string BedNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string Status { get; set; }
        public int? CheckedInBy { get; set; }
        public int? CheckedOutBy { get; set; }
        public string Notes { get; set; }

        // Joins
        public string VictimName { get; set; }
        public string ShelterName { get; set; }
    }
}
