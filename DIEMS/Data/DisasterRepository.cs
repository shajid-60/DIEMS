using System;
using System.Collections.Generic;
using System.Data;
using DIEMS.Models;
using Oracle.ManagedDataAccess.Client;

namespace DIEMS.Data
{
    public class DisasterRepository
    {
        private readonly OracleDbHelper _db;

        public DisasterRepository(OracleDbHelper db)
        {
            _db = db;
        }

        public List<Disaster> GetAllDisasters()
        {
            var list = new List<Disaster>();
            string sql = @"
                SELECT d.*, dt.TYPE_NAME, dt.ICON, dt.COLOR_CODE AS TYPE_COLOR, 
                       s.LEVEL_NAME, s.COLOR_CODE AS SEVERITY_COLOR, s.LEVEL_CODE
                FROM DISASTERS d
                JOIN DISASTER_TYPES dt ON d.TYPE_ID = dt.TYPE_ID
                JOIN SEVERITY_LEVELS s ON d.SEVERITY_LEVEL_ID = s.LEVEL_ID
                ORDER BY d.START_DATE DESC";

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapDisaster(row));
            }
            return list;
        }

        public List<Disaster> GetFilteredDisasters(string status, string sort)
        {
            var list = new List<Disaster>();
            
            using (var conn = _db.GetConnection())
            using (var cmd = new OracleCommand("GET_FILTERED_DISASTERS", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var pCursor = new OracleParameter("v_cursor", OracleDbType.RefCursor);
                pCursor.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(pCursor);

                var pStatus = new OracleParameter("p_status", OracleDbType.Varchar2);
                pStatus.Value = string.IsNullOrEmpty(status) ? "ALL" : status;
                cmd.Parameters.Add(pStatus);

                var pSort = new OracleParameter("p_sort", OracleDbType.Varchar2);
                pSort.Value = string.IsNullOrEmpty(sort) ? "LATEST" : sort;
                cmd.Parameters.Add(pSort);
                
                using (var reader = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(reader);
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(MapDisaster(row));
                    }
                }
            }
            return list;
        }

        public List<Disaster> GetActiveDisasters()
        {
            var list = new List<Disaster>();
            // Using view ACTIVE_DISASTERS_VW
            string sql = "SELECT * FROM ACTIVE_DISASTERS_VW ORDER BY START_DATE DESC";
            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Disaster
                {
                    DisasterId = Convert.ToInt32(row["DISASTER_ID"]),
                    DisasterName = row["DISASTER_NAME"].ToString(),
                    District = row["DISTRICT"].ToString(),
                    Division = row["DIVISION"] == DBNull.Value ? null : row["DIVISION"].ToString(),
                    StartDate = Convert.ToDateTime(row["START_DATE"]),
                    Status = row["STATUS"].ToString(),
                    AffectedPopulation = Convert.ToInt32(row["AFFECTED_POPULATION"]),
                    Casualties = Convert.ToInt32(row["CASUALTIES"]),
                    Displaced = Convert.ToInt32(row["DISPLACED"]),
                    TypeName = row["TYPE_NAME"].ToString(),
                    TypeIcon = row["TYPE_ICON"] != DBNull.Value ? row["TYPE_ICON"].ToString() : "",
                    TypeColor = "#6c757d", // ACTIVE_DISASTERS_VW doesn't return TYPE_COLOR, providing default
                    SeverityName = row["SEVERITY"].ToString(),
                    SeverityColor = row["SEVERITY_COLOR"].ToString(),
                    SeverityCode = Convert.ToInt32(row["SEVERITY_CODE"])
                });
            }
            return list;
        }

        public Disaster GetDisasterById(int id)
        {
            string sql = @"
                SELECT d.*, dt.TYPE_NAME, dt.ICON, dt.COLOR_CODE AS TYPE_COLOR, 
                       s.LEVEL_NAME, s.COLOR_CODE AS SEVERITY_COLOR, s.LEVEL_CODE
                FROM DISASTERS d
                JOIN DISASTER_TYPES dt ON d.TYPE_ID = dt.TYPE_ID
                JOIN SEVERITY_LEVELS s ON d.SEVERITY_LEVEL_ID = s.LEVEL_ID
                WHERE d.DISASTER_ID = :id";

            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", id));
            if (dt.Rows.Count > 0)
            {
                return MapDisaster(dt.Rows[0]);
            }
            return null;
        }

        public bool InsertDisaster(Disaster d)
        {
            string sql = @"
                INSERT INTO DISASTERS (DISASTER_NAME, TYPE_ID, SEVERITY_LEVEL_ID, DISTRICT, DIVISION, 
                                       LATITUDE, LONGITUDE, START_DATE, END_DATE, STATUS, 
                                       AFFECTED_POPULATION, CASUALTIES, INJURED, DISPLACED, 
                                       ESTIMATED_DAMAGE, DESCRIPTION, WEATHER_CONDITIONS, RESPONSE_TEAMS, REPORTED_BY)
                VALUES (:name, :typeId, :severityId, :district, :division, 
                        :lat, :lng, :startDate, :endDate, :status, 
                        :population, :casualties, :injured, :displaced, 
                        :damage, :description, :weather, :teams, :reportedBy)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("name", d.DisasterName),
                new OracleParameter("typeId", d.TypeId),
                new OracleParameter("severityId", d.SeverityLevelId),
                new OracleParameter("district", d.District),
                new OracleParameter("division", d.Division ?? (object)DBNull.Value),
                new OracleParameter("lat", d.Latitude ?? (object)DBNull.Value),
                new OracleParameter("lng", d.Longitude ?? (object)DBNull.Value),
                new OracleParameter("startDate", d.StartDate),
                new OracleParameter("endDate", d.EndDate ?? (object)DBNull.Value),
                new OracleParameter("status", d.Status),
                new OracleParameter("population", d.AffectedPopulation),
                new OracleParameter("casualties", d.Casualties),
                new OracleParameter("injured", d.Injured),
                new OracleParameter("displaced", d.Displaced),
                new OracleParameter("damage", d.EstimatedDamage),
                new OracleParameter("description", d.Description ?? (object)DBNull.Value),
                new OracleParameter("weather", d.WeatherConditions ?? (object)DBNull.Value),
                new OracleParameter("teams", d.ResponseTeams),
                new OracleParameter("reportedBy", d.ReportedBy ?? (object)DBNull.Value));

            return rows > 0;
        }

        public bool UpdateDisaster(Disaster d)
        {
            string sql = @"
                UPDATE DISASTERS
                SET DISASTER_NAME = :name, TYPE_ID = :typeId, SEVERITY_LEVEL_ID = :severityId, 
                    DISTRICT = :district, DIVISION = :division, LATITUDE = :lat, LONGITUDE = :lng, 
                    START_DATE = :startDate, END_DATE = :endDate, STATUS = :status, 
                    AFFECTED_POPULATION = :population, CASUALTIES = :casualties, INJURED = :injured, 
                    DISPLACED = :displaced, ESTIMATED_DAMAGE = :damage, DESCRIPTION = :description, 
                    WEATHER_CONDITIONS = :weather, RESPONSE_TEAMS = :teams, UPDATED_AT = SYSTIMESTAMP
                WHERE DISASTER_ID = :id";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("name", d.DisasterName),
                new OracleParameter("typeId", d.TypeId),
                new OracleParameter("severityId", d.SeverityLevelId),
                new OracleParameter("district", d.District),
                new OracleParameter("division", d.Division ?? (object)DBNull.Value),
                new OracleParameter("lat", d.Latitude ?? (object)DBNull.Value),
                new OracleParameter("lng", d.Longitude ?? (object)DBNull.Value),
                new OracleParameter("startDate", d.StartDate),
                new OracleParameter("endDate", d.EndDate ?? (object)DBNull.Value),
                new OracleParameter("status", d.Status),
                new OracleParameter("population", d.AffectedPopulation),
                new OracleParameter("casualties", d.Casualties),
                new OracleParameter("injured", d.Injured),
                new OracleParameter("displaced", d.Displaced),
                new OracleParameter("damage", d.EstimatedDamage),
                new OracleParameter("description", d.Description ?? (object)DBNull.Value),
                new OracleParameter("weather", d.WeatherConditions ?? (object)DBNull.Value),
                new OracleParameter("teams", d.ResponseTeams),
                new OracleParameter("id", d.DisasterId));

            return rows > 0;
        }

        public List<DisasterType> GetDisasterTypes()
        {
            var list = new List<DisasterType>();
            var dt = _db.ExecuteQuery("SELECT * FROM DISASTER_TYPES WHERE IS_ACTIVE = 1");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DisasterType
                {
                    TypeId = Convert.ToInt32(row["TYPE_ID"]),
                    TypeName = row["TYPE_NAME"].ToString(),
                    Icon = row["ICON"].ToString(),
                    ColorCode = row["COLOR_CODE"].ToString(),
                    Description = row["DESCRIPTION"].ToString()
                });
            }
            return list;
        }

        public List<SeverityLevel> GetSeverityLevels()
        {
            var list = new List<SeverityLevel>();
            var dt = _db.ExecuteQuery("SELECT * FROM SEVERITY_LEVELS ORDER BY LEVEL_CODE DESC");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SeverityLevel
                {
                    LevelId = Convert.ToInt32(row["LEVEL_ID"]),
                    LevelName = row["LEVEL_NAME"].ToString(),
                    LevelCode = Convert.ToInt32(row["LEVEL_CODE"]),
                    ColorCode = row["COLOR_CODE"].ToString(),
                    Description = row["DESCRIPTION"].ToString()
                });
            }
            return list;
        }

        public decimal CalculateDamage(int disasterId)
        {
            // Call the Oracle function CALCULATE_DAMAGE(disasterId)
            string sql = "SELECT CALCULATE_DAMAGE(:id) FROM DUAL";
            var result = _db.ExecuteScalar(sql, new OracleParameter("id", disasterId));
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public List<AffectedArea> GetAffectedAreas(int disasterId)
        {
            var list = new List<AffectedArea>();
            string sql = "SELECT * FROM AFFECTED_AREAS WHERE DISASTER_ID = :id ORDER BY CREATED_AT DESC";
            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", disasterId));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AffectedArea
                {
                    AreaId = Convert.ToInt32(row["AREA_ID"]),
                    DisasterId = Convert.ToInt32(row["DISASTER_ID"]),
                    District = row["DISTRICT"].ToString(),
                    Upazila = row["UPAZILA"] == DBNull.Value ? null : row["UPAZILA"].ToString(),
                    UnionName = row["UNION_NAME"] == DBNull.Value ? null : row["UNION_NAME"].ToString(),
                    AreaKm2 = row["AREA_KM2"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["AREA_KM2"]),
                    PopulationAtRisk = Convert.ToInt32(row["POPULATION_AT_RISK"]),
                    IsEvacuated = Convert.ToInt32(row["IS_EVACUATED"]),
                    EvacuationDate = row["EVACUATION_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["EVACUATION_DATE"]),
                    Notes = row["NOTES"] == DBNull.Value ? null : row["NOTES"].ToString(),
                    CreatedAt = Convert.ToDateTime(row["CREATED_AT"])
                });
            }
            return list;
        }

        public bool InsertAffectedArea(AffectedArea area)
        {
            string sql = @"
                INSERT INTO AFFECTED_AREAS (DISASTER_ID, DISTRICT, UPAZILA, UNION_NAME, AREA_KM2, POPULATION_AT_RISK, IS_EVACUATED, EVACUATION_DATE, NOTES)
                VALUES (:disasterId, :district, :upazila, :unionName, :areaKm, :popRisk, :isEvacuated, :evacDate, :notes)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("disasterId", area.DisasterId),
                new OracleParameter("district", area.District),
                new OracleParameter("upazila", area.Upazila ?? (object)DBNull.Value),
                new OracleParameter("unionName", area.UnionName ?? (object)DBNull.Value),
                new OracleParameter("areaKm", area.AreaKm2 ?? (object)DBNull.Value),
                new OracleParameter("popRisk", area.PopulationAtRisk),
                new OracleParameter("isEvacuated", area.IsEvacuated),
                new OracleParameter("evacDate", area.EvacuationDate ?? (object)DBNull.Value),
                new OracleParameter("notes", area.Notes ?? (object)DBNull.Value));

            return rows > 0;
        }

        private Disaster MapDisaster(DataRow row)
        {
            return new Disaster
            {
                DisasterId = Convert.ToInt32(row["DISASTER_ID"]),
                DisasterName = row["DISASTER_NAME"].ToString(),
                TypeId = Convert.ToInt32(row["TYPE_ID"]),
                SeverityLevelId = Convert.ToInt32(row["SEVERITY_LEVEL_ID"]),
                District = row["DISTRICT"].ToString(),
                Division = row["DIVISION"] == DBNull.Value ? null : row["DIVISION"].ToString(),
                Latitude = row["LATITUDE"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["LATITUDE"]),
                Longitude = row["LONGITUDE"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["LONGITUDE"]),
                StartDate = Convert.ToDateTime(row["START_DATE"]),
                EndDate = row["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["END_DATE"]),
                Status = row["STATUS"].ToString(),
                AffectedPopulation = Convert.ToInt32(row["AFFECTED_POPULATION"]),
                Casualties = Convert.ToInt32(row["CASUALTIES"]),
                Injured = Convert.ToInt32(row["INJURED"]),
                Displaced = Convert.ToInt32(row["DISPLACED"]),
                EstimatedDamage = Convert.ToDecimal(row["ESTIMATED_DAMAGE"]),
                Description = row["DESCRIPTION"] == DBNull.Value ? null : row["DESCRIPTION"].ToString(),
                WeatherConditions = row["WEATHER_CONDITIONS"] == DBNull.Value ? null : row["WEATHER_CONDITIONS"].ToString(),
                ResponseTeams = Convert.ToInt32(row["RESPONSE_TEAMS"]),
                ReportedBy = row["REPORTED_BY"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["REPORTED_BY"]),
                CreatedAt = Convert.ToDateTime(row["CREATED_AT"]),
                TypeName = row["TYPE_NAME"].ToString(),
                TypeIcon = row["ICON"].ToString(),
                TypeColor = row["TYPE_COLOR"].ToString(),
                SeverityName = row["LEVEL_NAME"].ToString(),
                SeverityColor = row["SEVERITY_COLOR"].ToString(),
                SeverityCode = Convert.ToInt32(row["LEVEL_CODE"])
            };
        }
    }
}
