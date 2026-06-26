using System;
using System.Collections.Generic;
using System.Data;
using DIEMS.Models;
using Oracle.ManagedDataAccess.Client;

namespace DIEMS.Data
{
    public class ReportRepository
    {
        private readonly OracleDbHelper _db;

        public ReportRepository(OracleDbHelper db)
        {
            _db = db;
        }

        public List<IncidentReport> GetAllReports()
        {
            var list = new List<IncidentReport>();
            string sql = @"
                SELECT r.*, d.DISASTER_NAME, u.FULL_NAME AS ASSIGNED_TO_NAME
                FROM INCIDENT_REPORTS r
                LEFT JOIN DISASTERS d ON r.DISASTER_ID = d.DISASTER_ID
                LEFT JOIN USERS u ON r.ASSIGNED_TO = u.USER_ID
                ORDER BY r.CREATED_AT DESC";

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapIncidentReport(row));
            }
            return list;
        }

        public IncidentReport GetReportById(int id)
        {
            string sql = @"
                SELECT r.*, d.DISASTER_NAME, u.FULL_NAME AS ASSIGNED_TO_NAME
                FROM INCIDENT_REPORTS r
                LEFT JOIN DISASTERS d ON r.DISASTER_ID = d.DISASTER_ID
                LEFT JOIN USERS u ON r.ASSIGNED_TO = u.USER_ID
                WHERE r.REPORT_ID = :id";

            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", id));
            if (dt.Rows.Count > 0)
            {
                return MapIncidentReport(dt.Rows[0]);
            }
            return null;
        }

        public bool InsertReport(IncidentReport r)
        {
            string sql = @"
                INSERT INTO INCIDENT_REPORTS (DISASTER_ID, REPORTER_NAME, REPORTER_PHONE, INCIDENT_TYPE, 
                                              DESCRIPTION, LOCATION, DISTRICT, LATITUDE, LONGITUDE, 
                                              SEVERITY, STATUS, CREATED_AT)
                VALUES (:disasterId, :reporter, :phone, :type, 
                        :desc, :location, :district, :lat, :lng, 
                        :severity, 'Pending', SYSTIMESTAMP)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("disasterId", r.DisasterId ?? (object)DBNull.Value),
                new OracleParameter("reporter", r.ReporterName),
                new OracleParameter("phone", r.ReporterPhone),
                new OracleParameter("type", r.IncidentType),
                new OracleParameter("desc", r.Description),
                new OracleParameter("location", r.Location ?? (object)DBNull.Value),
                new OracleParameter("district", r.District),
                new OracleParameter("lat", r.Latitude ?? (object)DBNull.Value),
                new OracleParameter("lng", r.Longitude ?? (object)DBNull.Value),
                new OracleParameter("severity", r.SeverityLevel));

            return rows > 0;
        }

        public bool UpdateReport(IncidentReport r)
        {
            string sql = @"
                UPDATE INCIDENT_REPORTS
                SET STATUS = :status, ASSIGNED_TO = :assignedTo, RESOLUTION_NOTES = :notes
                WHERE REPORT_ID = :id";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("status", r.Status),
                new OracleParameter("assignedTo", r.AssignedTo ?? (object)DBNull.Value),
                new OracleParameter("notes", r.ResolutionNotes ?? (object)DBNull.Value),
                new OracleParameter("id", r.ReportId));

            return rows > 0;
        }

        public List<AuditLog> GetAuditLogs()
        {
            var list = new List<AuditLog>();
            string sql = "SELECT * FROM AUDIT_LOG ORDER BY CHANGED_AT DESC";
            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AuditLog
                {
                    LogId = Convert.ToInt32(row["LOG_ID"]),
                    TableName = row["TABLE_NAME"].ToString(),
                    RecordId = row["RECORD_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["RECORD_ID"]),
                    Operation = row["OPERATION"].ToString(),
                    ColumnName = row["COLUMN_NAME"] == DBNull.Value ? null : row["COLUMN_NAME"].ToString(),
                    OldValue = row["OLD_VALUE"] == DBNull.Value ? null : row["OLD_VALUE"].ToString(),
                    NewValue = row["NEW_VALUE"] == DBNull.Value ? null : row["NEW_VALUE"].ToString(),
                    ChangedBy = row["CHANGED_BY"].ToString(),
                    ChangedAt = Convert.ToDateTime(row["CHANGED_AT"]),
                    IpAddress = row["IP_ADDRESS"] == DBNull.Value ? null : row["IP_ADDRESS"].ToString(),
                    SessionId = row["SESSION_ID"] == DBNull.Value ? null : row["SESSION_ID"].ToString(),
                    Module = row["MODULE"] == DBNull.Value ? null : row["MODULE"].ToString(),
                    Notes = row["NOTES"] == DBNull.Value ? null : row["NOTES"].ToString()
                });
            }
            return list;
        }

        private IncidentReport MapIncidentReport(DataRow row)
        {
            return new IncidentReport
            {
                ReportId = Convert.ToInt32(row["REPORT_ID"]),
                DisasterId = row["DISASTER_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["DISASTER_ID"]),
                ReporterName = row["REPORTER_NAME"].ToString(),
                ReporterPhone = row["REPORTER_PHONE"].ToString(),
                IncidentType = row["INCIDENT_TYPE"].ToString(),
                Description = row["DESCRIPTION"].ToString(),
                Location = row["LOCATION"] == DBNull.Value ? null : row["LOCATION"].ToString(),
                District = row["DISTRICT"].ToString(),
                Upazila = null, // UPAZILA column does not exist in INCIDENT_REPORTS table
                Latitude = row["LATITUDE"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["LATITUDE"]),
                Longitude = row["LONGITUDE"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["LONGITUDE"]),
                SeverityLevel = row["SEVERITY"].ToString(),
                Status = row["STATUS"].ToString(),
                ReportedAt = Convert.ToDateTime(row["CREATED_AT"]),
                AssignedTo = row["ASSIGNED_TO"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ASSIGNED_TO"]),
                ResolutionNotes = row["RESOLUTION_NOTES"] == DBNull.Value ? null : row["RESOLUTION_NOTES"].ToString(),
                DisasterName = row["DISASTER_NAME"] == DBNull.Value ? "Unlinked Incident" : row["DISASTER_NAME"].ToString(),
                AssignedToName = row["ASSIGNED_TO_NAME"] == DBNull.Value ? "Unassigned" : row["ASSIGNED_TO_NAME"].ToString()
            };
        }
    }
}
