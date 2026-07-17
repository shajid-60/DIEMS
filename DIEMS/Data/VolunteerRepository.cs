using System;
using System.Collections.Generic;
using System.Data;
using DIEMS.Models;
using Oracle.ManagedDataAccess.Client;

namespace DIEMS.Data
{
    public class VolunteerRepository
    {
        private readonly OracleDbHelper _db;

        public VolunteerRepository(OracleDbHelper db)
        {
            _db = db;
        }

        public List<Volunteer> GetAllVolunteers()
        {
            return GetFilteredVolunteers("ALL", "LATEST");
        }

        public List<Volunteer> GetFilteredVolunteers(string status, string sort)
        {
            string sql = @"
                SELECT v.*, u.USERNAME, u.EMAIL,
                       (SELECT MAX(TASK_TITLE) KEEP (DENSE_RANK LAST ORDER BY START_DATE) FROM VOLUNTEER_ASSIGNMENTS a WHERE a.VOLUNTEER_ID = v.VOLUNTEER_ID AND a.STATUS = 'Active') AS CURRENT_MISSION
                FROM VOLUNTEERS v
                LEFT JOIN USERS u ON v.USER_ID = u.USER_ID
                WHERE 1=1";

            if (!string.IsNullOrEmpty(status) && status != "ALL")
            {
                sql += " AND v.AVAILABILITY = '" + status.Replace("'", "''") + "'";
            }

            if (sort == "HOURS")
            {
                sql += " ORDER BY v.TOTAL_MISSIONS DESC, v.CREATED_AT DESC";
            }
            else
            {
                sql += " ORDER BY v.CREATED_AT DESC";
            }

            var dt = _db.ExecuteQuery(sql);
            var list = new List<Volunteer>();
            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(MapVolunteer(row));
            }
            return list;
        }

        public Volunteer GetVolunteerById(int id)
        {
            string sql = @"
                SELECT v.*, u.USERNAME, u.EMAIL,
                       (SELECT MAX(TASK_TITLE) KEEP (DENSE_RANK LAST ORDER BY START_DATE) FROM VOLUNTEER_ASSIGNMENTS a WHERE a.VOLUNTEER_ID = v.VOLUNTEER_ID AND a.STATUS = 'Active') AS CURRENT_MISSION
                FROM VOLUNTEERS v
                LEFT JOIN USERS u ON v.USER_ID = u.USER_ID
                WHERE v.VOLUNTEER_ID = :id";

            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", id));
            if (dt.Rows.Count > 0)
            {
                return MapVolunteer(dt.Rows[0]);
            }
            return null;
        }

        public Volunteer GetVolunteerByUserId(int userId)
        {
            string sql = @"
                SELECT v.*, u.USERNAME, u.EMAIL
                FROM VOLUNTEERS v
                JOIN USERS u ON v.USER_ID = u.USER_ID
                WHERE v.USER_ID = :userId";

            var dt = _db.ExecuteQuery(sql, new OracleParameter("userId", userId));
            if (dt.Rows.Count > 0)
            {
                return MapVolunteer(dt.Rows[0]);
            }
            return null;
        }

        public bool InsertVolunteer(Volunteer v)
        {
            string sql = @"
                INSERT INTO VOLUNTEERS (USER_ID, FULL_NAME, PHONE, LANGUAGES, AVAILABILITY, DISTRICT, TOTAL_MISSIONS, EXPERIENCE_YEARS, EMERGENCY_CONTACT)
                VALUES (:userId, :name, :phone, :skills, :status, :district, 0, :hours, :emergency)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("userId", v.UserId == 0 ? (object)DBNull.Value : v.UserId),
                new OracleParameter("name", v.FullName),
                new OracleParameter("phone", v.Phone ?? (object)DBNull.Value),
                new OracleParameter("skills", v.SkillSet ?? (object)DBNull.Value),
                new OracleParameter("status", v.AvailabilityStatus ?? "Available"),
                new OracleParameter("district", v.District ?? (object)DBNull.Value),
                new OracleParameter("hours", v.TotalHoursServed),
                new OracleParameter("emergency", v.EmergencyContact ?? (object)DBNull.Value));

            return rows > 0;
        }

        public bool UpdateVolunteer(Volunteer v)
        {
            string sql = @"
                UPDATE VOLUNTEERS SET 
                    USER_ID = :userId, FULL_NAME = :name, PHONE = :phone, LANGUAGES = :skills, 
                    AVAILABILITY = :status, DISTRICT = :district, TOTAL_MISSIONS = :hours, 
                    EXPERIENCE_YEARS = :hours, EMERGENCY_CONTACT = :emergency
                WHERE VOLUNTEER_ID = :id";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("userId", v.UserId),
                new OracleParameter("name", v.FullName),
                new OracleParameter("phone", v.Phone ?? (object)DBNull.Value),
                new OracleParameter("skills", v.SkillSet ?? (object)DBNull.Value),
                new OracleParameter("status", v.AvailabilityStatus),
                new OracleParameter("district", v.District ?? (object)DBNull.Value),
                new OracleParameter("hours", v.TotalHoursServed),
                new OracleParameter("emergency", v.EmergencyContact ?? (object)DBNull.Value),
                new OracleParameter("id", v.VolunteerId));

            return rows > 0;
        }

        public List<VolunteerAssignment> GetAssignments(int volunteerId)
        {
            var list = new List<VolunteerAssignment>();
            string sql = @"
                SELECT va.*, v.FULL_NAME AS VOLUNTEER_NAME, d.DISASTER_NAME, u.FULL_NAME AS SUPERVISOR_NAME, u.PHONE AS SUPERVISOR_CONTACT
                FROM VOLUNTEER_ASSIGNMENTS va
                JOIN VOLUNTEERS v ON va.VOLUNTEER_ID = v.VOLUNTEER_ID
                JOIN DISASTERS d ON va.DISASTER_ID = d.DISASTER_ID
                LEFT JOIN USERS u ON va.ASSIGNED_BY = u.USER_ID
                WHERE va.VOLUNTEER_ID = :id
                ORDER BY va.START_DATE DESC";

            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", volunteerId));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new VolunteerAssignment
                {
                    AssignmentId = Convert.ToInt32(row["ASSIGNMENT_ID"]),
                    VolunteerId = Convert.ToInt32(row["VOLUNTEER_ID"]),
                    DisasterId = Convert.ToInt32(row["DISASTER_ID"]),
                    TaskName = row["TASK_TITLE"].ToString(),
                    Description = row["TASK_DESCRIPTION"] == DBNull.Value ? null : row["TASK_DESCRIPTION"].ToString(),
                    AssignedDate = Convert.ToDateTime(row["START_DATE"]),
                    Status = row["STATUS"].ToString(),
                    HoursWorked = 0,
                    SupervisorName = row["SUPERVISOR_NAME"] == DBNull.Value ? null : row["SUPERVISOR_NAME"].ToString(),
                    SupervisorContact = row["SUPERVISOR_CONTACT"] == DBNull.Value ? null : row["SUPERVISOR_CONTACT"].ToString(),
                    VolunteerName = row["VOLUNTEER_NAME"].ToString(),
                    DisasterName = row["DISASTER_NAME"].ToString()
                });
            }
            return list;
        }

        public bool InsertAssignment(VolunteerAssignment a, int assignedByUserId)
        {
            string sql = @"
                INSERT INTO VOLUNTEER_ASSIGNMENTS (VOLUNTEER_ID, DISASTER_ID, TASK_TITLE, TASK_DESCRIPTION, START_DATE, STATUS, ASSIGNED_BY)
                VALUES (:volunteerId, :disasterId, :taskName, :taskDesc, SYSTIMESTAMP, 'Active', :assignedBy)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("volunteerId", a.VolunteerId),
                new OracleParameter("disasterId", a.DisasterId),
                new OracleParameter("taskName", a.TaskName),
                new OracleParameter("taskDesc", a.Description ?? (object)DBNull.Value),
                new OracleParameter("assignedBy", assignedByUserId));

            // Mark volunteer as assigned
            _db.ExecuteNonQuery("UPDATE VOLUNTEERS SET AVAILABILITY = 'Assigned' WHERE VOLUNTEER_ID = :id", new OracleParameter("id", a.VolunteerId));

            return rows > 0;
        }

        private Volunteer MapVolunteer(DataRow row)
        {
            return new Volunteer
            {
                VolunteerId = Convert.ToInt32(row["VOLUNTEER_ID"]),
                UserId = row["USER_ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["USER_ID"]),
                FullName = row["FULL_NAME"].ToString(),
                Phone = row["PHONE"] == DBNull.Value ? null : row["PHONE"].ToString(),
                SkillSet = row["LANGUAGES"] == DBNull.Value ? null : row["LANGUAGES"].ToString(),
                AvailabilityStatus = row["AVAILABILITY"].ToString(),
                District = row["DISTRICT"] == DBNull.Value ? null : row["DISTRICT"].ToString(),
                CurrentMission = row.Table.Columns.Contains("CURRENT_MISSION") && row["CURRENT_MISSION"] != DBNull.Value ? row["CURRENT_MISSION"].ToString() : null,
                TotalHoursServed = Convert.ToInt32(row["TOTAL_MISSIONS"]),
                BloodGroup = null,
                EmergencyContact = row["EMERGENCY_CONTACT"] == DBNull.Value ? null : row["EMERGENCY_CONTACT"].ToString(),
                CreatedAt = Convert.ToDateTime(row["CREATED_AT"]),
                Username = row.Table.Columns.Contains("USERNAME") && row["USERNAME"] != DBNull.Value ? row["USERNAME"].ToString() : null,
                Email = row["EMAIL"] == DBNull.Value ? null : row["EMAIL"].ToString()
            };
        }
        
        public bool DeleteVolunteer(int id)
        {
            try
            {
                // First delete related assignments to avoid FK constraint error
                _db.ExecuteNonQuery("DELETE FROM VOLUNTEER_ASSIGNMENTS WHERE VOLUNTEER_ID = :id", new OracleParameter("id", id));
                
                int rows = _db.ExecuteNonQuery("DELETE FROM VOLUNTEERS WHERE VOLUNTEER_ID = :id", new OracleParameter("id", id));
                return rows > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
