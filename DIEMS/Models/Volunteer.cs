using System;

namespace DIEMS.Models
{
    public class Volunteer
    {
        public int VolunteerId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string SkillSet { get; set; }
        public string AvailabilityStatus { get; set; }
        public string District { get; set; }
        public string CurrentMission { get; set; }
        public int TotalHoursServed { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyContact { get; set; }
        public DateTime CreatedAt { get; set; }

        // Joins
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class Skill
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string Description { get; set; }
        public string DifficultyLevel { get; set; }
    }

    public class VolunteerAssignment
    {
        public int AssignmentId { get; set; }
        public int VolunteerId { get; set; }
        public int DisasterId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime AssignedDate { get; set; }
        public string Status { get; set; }
        public int HoursWorked { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorContact { get; set; }

        // Joins
        public string VolunteerName { get; set; }
        public string DisasterName { get; set; }
    }
}
