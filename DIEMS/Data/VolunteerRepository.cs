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
            var list = new List<Volunteer>();
            
            using (var conn = _db.GetConnection())
            using (var cmd = new OracleCommand("GET_FILTERED_VOLUNTEERS", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var pCursor = new OracleParameter("v_cursor", OracleDbType.RefCursor);
                pCursor.Direction = System.Data.ParameterDirection.ReturnValue;
                cmd.Parameters.Add(pCursor);

                var pStatus = new OracleParameter("p_status", OracleDbType.Varchar2);
                pStatus.Value = string.IsNullOrEmpty(status) ? "ALL" : status;
                cmd.Parameters.Add(pStatus);

                var pSort = new OracleParameter("p_sort", OracleDbType.Varchar2);
                pSort.Value = string.IsNullOrEmpty(sort) ? "LATEST" : sort;
                cmd.Parameters.Add(pSort);
                
                using (var reader = cmd.ExecuteReader())
                {
                    var dt = new System.Data.DataTable();
                    dt.Load(reader);
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        list.Add(MapVolunteer(row));
                    }
                }
            }
            return list;
        }

        public Volunteer GetVolunteerById(int id)
        {
            string sql = @"
                SELECT v.*, u.USERNAME, u.EMAIL
                FROM VOLUNTEERS v
                JOIN USERS u ON v.USER_ID = u.USER_ID
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
                new OracleParameter("userId", v.UserId),
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
                SELECT va.*, v.FULL_NAME AS VOLUNTEER_NAME, d.DISASTER_NAME
                FROM VOLUNTEER_ASSIGNMENTS va
                JOIN VOLUNTEERS v ON va.VOLUNTEER_ID = v.VOLUNTEER_ID
                JOIN DISASTERS d ON va.DISASTER_ID = d.DISASTER_ID
                WHERE va.VOLUNTEER_ID = :id
                ORDER BY va.ASSIGNED_DATE DESC";

            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", volunteerId));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new VolunteerAssignment
                {
                    AssignmentId = Convert.ToInt32(row["ASSIGNMENT_ID"]),
                    VolunteerId = Convert.ToInt32(row["VOLUNTEER_ID"]),
                    DisasterId = Convert.ToInt32(row["DISASTER_ID"]),
                    TaskName = row["TASK_NAME"].ToString(),
                    Description = row["DESCRIPTION"] == DBNull.Value ? null : row["DESCRIPTION"].ToString(),
                    AssignedDate = Convert.ToDateTime(row["ASSIGNED_DATE"]),
                    Status = row["STATUS"].ToString(),
                    HoursWorked = Convert.ToInt32(row["HOURS_WORKED"]),
                    SupervisorName = row["SUPERVISOR_NAME"] == DBNull.Value ? null : row["SUPERVISOR_NAME"].ToString(),
                    SupervisorContact = row["SUPERVISOR_CONTACT"] == DBNull.Value ? null : row["SUPERVISOR_CONTACT"].ToString(),
                    VolunteerName = row["VOLUNTEER_NAME"].ToString(),
                    DisasterName = row["DISASTER_NAME"].ToString()
                });
            }
            return list;
        }

        public bool InsertAssignment(VolunteerAssignment a)
        {
            string sql = @"
                INSERT INTO VOLUNTEER_ASSIGNMENTS (VOLUNTEER_ID, DISASTER_ID, TASK_NAME, DESCRIPTION, ASSIGNED_DATE, STATUS, HOURS_WORKED, SUPERVISOR_NAME, SUPERVISOR_CONTACT)
                VALUES (:volunteerId, :disasterId, :taskName, :desc, SYSTIMESTAMP, 'Active', 0, :supName, :supContact)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("volunteerId", a.VolunteerId),
                new OracleParameter("disasterId", a.DisasterId),
                new OracleParameter("taskName", a.TaskName),
                new OracleParameter("desc", a.Description ?? (object)DBNull.Value),
                new OracleParameter("supName", a.SupervisorName ?? (object)DBNull.Value),
                new OracleParameter("supContact", a.SupervisorContact ?? (object)DBNull.Value));

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
                CurrentMission = row["ORGANIZATION"] == DBNull.Value ? null : row["ORGANIZATION"].ToString(),
                TotalHoursServed = Convert.ToInt32(row["TOTAL_MISSIONS"]),
                BloodGroup = null,
                EmergencyContact = row["EMERGENCY_CONTACT"] == DBNull.Value ? null : row["EMERGENCY_CONTACT"].ToString(),
                CreatedAt = Convert.ToDateTime(row["CREATED_AT"]),
                Username = row.Table.Columns.Contains("USERNAME") && row["USERNAME"] != DBNull.Value ? row["USERNAME"].ToString() : null,
                Email = row["EMAIL"] == DBNull.Value ? null : row["EMAIL"].ToString()
            };
        }
    }
}
