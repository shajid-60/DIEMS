using System;

namespace DIEMS.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Nid { get; set; }
        public int RoleId { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public string ProfilePic { get; set; }
        public int IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        // Navigation properties / Joined fields
        public string RoleName { get; set; }
    }

    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
