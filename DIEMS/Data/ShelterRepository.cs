using System;
using System.Collections.Generic;
using System.Data;
using DIEMS.Models;
using Oracle.ManagedDataAccess.Client;

namespace DIEMS.Data
{
    public class ShelterRepository
    {
        private readonly OracleDbHelper _db;

        public ShelterRepository(OracleDbHelper db)
        {
            _db = db;
        }

        public List<Shelter> GetAllShelters()
        {
            var list = new List<Shelter>();
            string sql = @"
                SELECT s.*, c.MAX_CAPACITY, c.CURRENT_OCCUPIED, c.AVAILABLE_BEDS, c.RESERVED_SPOTS, c.HAS_OVERFLOW, c.OVERFLOW_LOCATION
                FROM SHELTERS s
                LEFT JOIN SHELTER_CAPACITY c ON s.SHELTER_ID = c.SHELTER_ID
                ORDER BY s.SHELTER_NAME";

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapShelter(row));
            }
            return list;
        }

        public List<Shelter> GetAvailableShelters()
        {
            var list = new List<Shelter>();
            // Using AVAILABLE_SHELTERS_VW view
            string sql = "SELECT * FROM AVAILABLE_SHELTERS_VW ORDER BY AVAILABLE_BEDS DESC";
            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Shelter
                {
                    ShelterId = Convert.ToInt32(row["SHELTER_ID"]),
                    ShelterName = row["SHELTER_NAME"].ToString(),
                    Location = row["LOCATION"].ToString(),
                    District = row["DISTRICT"].ToString(),
                    MaxCapacity = Convert.ToInt32(row["MAX_CAPACITY"]),
                    CurrentOccupied = Convert.ToInt32(row["CURRENT_OCCUPIED"]),
                    AvailableBeds = Convert.ToInt32(row["AVAILABLE_BEDS"])
                });
            }
            return list;
        }

        public Shelter GetShelterById(int id)
        {
            string sql = @"
                SELECT s.*, c.MAX_CAPACITY, c.CURRENT_OCCUPIED, c.AVAILABLE_BEDS, c.RESERVED_SPOTS, c.HAS_OVERFLOW, c.OVERFLOW_LOCATION
                FROM SHELTERS s
                LEFT JOIN SHELTER_CAPACITY c ON s.SHELTER_ID = c.SHELTER_ID
                WHERE s.SHELTER_ID = :id";

            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", id));
            if (dt.Rows.Count > 0)
            {
                return MapShelter(dt.Rows[0]);
            }
            return null;
        }

        public bool InsertShelter(Shelter s)
        {
            using (var conn = _db.GetConnection())
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    string sqlShelter = @"
                        INSERT INTO SHELTERS (SHELTER_NAME, SHELTER_TYPE, LOCATION, DISTRICT, UPAZILA, 
                                              LATITUDE, LONGITUDE, CONTACT_PERSON, CONTACT_PHONE, 
                                              FACILITIES, HAS_MEDICAL, HAS_GENERATOR, HAS_WIFI, IS_ACTIVE, CREATED_BY)
                        VALUES (:name, :type, :location, :district, :upazila, 
                                :lat, :lng, :contact, :phone, 
                                :facilities, :medical, :generator, :wifi, 1, :createdBy)
                        RETURNING SHELTER_ID INTO :newId";

                    var paramNewId = new OracleParameter("newId", OracleDbType.Int32, ParameterDirection.ReturnValue);

                    using (var cmd = new OracleCommand(sqlShelter, conn))
                    {
                        cmd.Transaction = trans;
                        cmd.Parameters.Add("name", OracleDbType.Varchar2).Value = s.ShelterName;
                        cmd.Parameters.Add("type", OracleDbType.Varchar2).Value = s.ShelterType ?? "General";
                        cmd.Parameters.Add("location", OracleDbType.Varchar2).Value = s.Location;
                        cmd.Parameters.Add("district", OracleDbType.Varchar2).Value = s.District;
                        cmd.Parameters.Add("upazila", OracleDbType.Varchar2).Value = (object)s.Upazila ?? DBNull.Value;
                        cmd.Parameters.Add("lat", OracleDbType.Double).Value = (object)s.Latitude ?? DBNull.Value;
                        cmd.Parameters.Add("lng", OracleDbType.Double).Value = (object)s.Longitude ?? DBNull.Value;
                        cmd.Parameters.Add("contact", OracleDbType.Varchar2).Value = (object)s.ContactPerson ?? DBNull.Value;
                        cmd.Parameters.Add("phone", OracleDbType.Varchar2).Value = (object)s.ContactPhone ?? DBNull.Value;
                        cmd.Parameters.Add("facilities", OracleDbType.Varchar2).Value = (object)s.Facilities ?? DBNull.Value;
                        cmd.Parameters.Add("medical", OracleDbType.Int16).Value = s.HasMedical;
                        cmd.Parameters.Add("generator", OracleDbType.Int16).Value = s.HasGenerator;
                        cmd.Parameters.Add("wifi", OracleDbType.Int16).Value = s.HasWifi;
                        cmd.Parameters.Add("createdBy", OracleDbType.Int32).Value = (object)s.CreatedBy ?? DBNull.Value;
                        cmd.Parameters.Add(paramNewId);

                        cmd.ExecuteNonQuery();
                    }

                    int newShelterId = Convert.ToInt32(paramNewId.Value);

                    string sqlCapacity = @"
                        INSERT INTO SHELTER_CAPACITY (SHELTER_ID, MAX_CAPACITY, CURRENT_OCCUPIED, AVAILABLE_BEDS, RESERVED_SPOTS, HAS_OVERFLOW, OVERFLOW_LOCATION)
                        VALUES (:shelterId, :max, 0, :max, :reserved, :overflow, :overflowLoc)";

                    using (var cmdCap = new OracleCommand(sqlCapacity, conn))
                    {
                        cmdCap.Transaction = trans;
                        cmdCap.Parameters.Add("shelterId", OracleDbType.Int32).Value = newShelterId;
                        cmdCap.Parameters.Add("max", OracleDbType.Int32).Value = s.MaxCapacity;
                        cmdCap.Parameters.Add("reserved", OracleDbType.Int32).Value = s.ReservedSpots;
                        cmdCap.Parameters.Add("overflow", OracleDbType.Int16).Value = s.HasOverflow;
                        cmdCap.Parameters.Add("overflowLoc", OracleDbType.Varchar2).Value = (object)s.OverflowLocation ?? DBNull.Value;

                        cmdCap.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return true;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public bool UpdateShelter(Shelter s)
        {
            using (var conn = _db.GetConnection())
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    string sqlShelter = @"
                        UPDATE SHELTERS
                        SET SHELTER_NAME = :name, SHELTER_TYPE = :type, LOCATION = :location, DISTRICT = :district, 
                            UPAZILA = :upazila, LATITUDE = :lat, LONGITUDE = :lng, CONTACT_PERSON = :contact, 
                            CONTACT_PHONE = :phone, FACILITIES = :facilities, HAS_MEDICAL = :medical, 
                            HAS_GENERATOR = :generator, HAS_WIFI = :wifi, IS_ACTIVE = :isActive, CLOSED_DATE = :closedDate
                        WHERE SHELTER_ID = :id";

                    using (var cmd = new OracleCommand(sqlShelter, conn))
                    {
                        cmd.Transaction = trans;
                        cmd.Parameters.Add("name", OracleDbType.Varchar2).Value = s.ShelterName;
                        cmd.Parameters.Add("type", OracleDbType.Varchar2).Value = s.ShelterType ?? "General";
                        cmd.Parameters.Add("location", OracleDbType.Varchar2).Value = s.Location;
                        cmd.Parameters.Add("district", OracleDbType.Varchar2).Value = s.District;
                        cmd.Parameters.Add("upazila", OracleDbType.Varchar2).Value = (object)s.Upazila ?? DBNull.Value;
                        cmd.Parameters.Add("lat", OracleDbType.Double).Value = (object)s.Latitude ?? DBNull.Value;
                        cmd.Parameters.Add("lng", OracleDbType.Double).Value = (object)s.Longitude ?? DBNull.Value;
                        cmd.Parameters.Add("contact", OracleDbType.Varchar2).Value = (object)s.ContactPerson ?? DBNull.Value;
                        cmd.Parameters.Add("phone", OracleDbType.Varchar2).Value = (object)s.ContactPhone ?? DBNull.Value;
                        cmd.Parameters.Add("facilities", OracleDbType.Varchar2).Value = (object)s.Facilities ?? DBNull.Value;
                        cmd.Parameters.Add("medical", OracleDbType.Int16).Value = s.HasMedical;
                        cmd.Parameters.Add("generator", OracleDbType.Int16).Value = s.HasGenerator;
                        cmd.Parameters.Add("wifi", OracleDbType.Int16).Value = s.HasWifi;
                        cmd.Parameters.Add("isActive", OracleDbType.Int16).Value = s.IsActive;
                        cmd.Parameters.Add("closedDate", OracleDbType.Date).Value = (object)s.ClosedDate ?? DBNull.Value;
                        cmd.Parameters.Add("id", OracleDbType.Int32).Value = s.ShelterId;

                        cmd.ExecuteNonQuery();
                    }

                    string sqlCapacity = @"
                        UPDATE SHELTER_CAPACITY
                        SET MAX_CAPACITY = :max, RESERVED_SPOTS = :reserved, HAS_OVERFLOW = :overflow, 
                            OVERFLOW_LOCATION = :overflowLoc, 
                            AVAILABLE_BEDS = :max - CURRENT_OCCUPIED - :reserved, LAST_UPDATED = SYSTIMESTAMP
                        WHERE SHELTER_ID = :shelterId";

                    using (var cmdCap = new OracleCommand(sqlCapacity, conn))
                    {
                        cmdCap.Transaction = trans;
                        cmdCap.Parameters.Add("max", OracleDbType.Int32).Value = s.MaxCapacity;
                        cmdCap.Parameters.Add("reserved", OracleDbType.Int32).Value = s.ReservedSpots;
                        cmdCap.Parameters.Add("overflow", OracleDbType.Int16).Value = s.HasOverflow;
                        cmdCap.Parameters.Add("overflowLoc", OracleDbType.Varchar2).Value = (object)s.OverflowLocation ?? DBNull.Value;
                        cmdCap.Parameters.Add("shelterId", OracleDbType.Int32).Value = s.ShelterId;

                        cmdCap.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return true;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public List<ShelterResident> GetResidents(int shelterId)
        {
            var list = new List<ShelterResident>();
            string sql = @"
                SELECT sr.*, v.FULL_NAME AS VICTIM_NAME, s.SHELTER_NAME
                FROM SHELTER_RESIDENTS sr
                JOIN VICTIMS v ON sr.VICTIM_ID = v.VICTIM_ID
                JOIN SHELTERS s ON sr.SHELTER_ID = s.SHELTER_ID
                WHERE sr.SHELTER_ID = :id
                ORDER BY sr.CHECK_IN_DATE DESC";

            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", shelterId));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ShelterResident
                {
                    SrId = Convert.ToInt32(row["SR_ID"]),
                    ShelterId = Convert.ToInt32(row["SHELTER_ID"]),
                    VictimId = Convert.ToInt32(row["VICTIM_ID"]),
                    BedNumber = row["BED_NUMBER"] == DBNull.Value ? null : row["BED_NUMBER"].ToString(),
                    CheckInDate = Convert.ToDateTime(row["CHECK_IN_DATE"]),
                    CheckOutDate = row["CHECK_OUT_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["CHECK_OUT_DATE"]),
                    Status = row["STATUS"].ToString(),
                    CheckedInBy = row["CHECKED_IN_BY"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["CHECKED_IN_BY"]),
                    CheckedOutBy = row["CHECKED_OUT_BY"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["CHECKED_OUT_BY"]),
                    Notes = row["NOTES"] == DBNull.Value ? null : row["NOTES"].ToString(),
                    VictimName = row["VICTIM_NAME"].ToString(),
                    ShelterName = row["SHELTER_NAME"].ToString()
                });
            }
            return list;
        }

        public bool CheckInResident(ShelterResident r)
        {
            // Note: This insert will trigger SHELTER_CAP_VAL_TRG and SHELTER_CAP_UPD_TRG in Oracle
            string sql = @"
                INSERT INTO SHELTER_RESIDENTS (SHELTER_ID, VICTIM_ID, BED_NUMBER, CHECK_IN_DATE, STATUS, CHECKED_IN_BY, NOTES)
                VALUES (:shelterId, :victimId, :bed, SYSTIMESTAMP, 'Active', :checkedInBy, :notes)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("shelterId", r.ShelterId),
                new OracleParameter("victimId", r.VictimId),
                new OracleParameter("bed", r.BedNumber ?? (object)DBNull.Value),
                new OracleParameter("checkedInBy", r.CheckedInBy ?? (object)DBNull.Value),
                new OracleParameter("notes", r.Notes ?? (object)DBNull.Value));

            if (rows > 0)
            {
                // Update victim status to 'Sheltered'
                _db.ExecuteNonQuery("UPDATE VICTIMS SET STATUS = 'Sheltered', SHELTER_ID = :shelterId WHERE VICTIM_ID = :victimId",
                    new OracleParameter("shelterId", r.ShelterId),
                    new OracleParameter("victimId", r.VictimId));
                return true;
            }
            return false;
        }

        public bool CheckOutResident(int srId, int userId)
        {
            // Get resident info
            string sqlGet = "SELECT SHELTER_ID, VICTIM_ID FROM SHELTER_RESIDENTS WHERE SR_ID = :id";
            var dt = _db.ExecuteQuery(sqlGet, new OracleParameter("id", srId));
            if (dt.Rows.Count == 0) return false;

            int shelterId = Convert.ToInt32(dt.Rows[0]["SHELTER_ID"]);
            int victimId = Convert.ToInt32(dt.Rows[0]["VICTIM_ID"]);

            // Update status in SHELTER_RESIDENTS (triggers occupancy update)
            string sqlUpdate = @"
                UPDATE SHELTER_RESIDENTS
                SET STATUS = 'Discharged', CHECK_OUT_DATE = SYSTIMESTAMP, CHECKED_OUT_BY = :userId
                WHERE SR_ID = :id";

            int rows = _db.ExecuteNonQuery(sqlUpdate, 
                new OracleParameter("userId", userId),
                new OracleParameter("id", srId));

            if (rows > 0)
            {
                // Update victim status back to 'Displaced' or clear shelter assignment
                _db.ExecuteNonQuery("UPDATE VICTIMS SET STATUS = 'Evacuated', SHELTER_ID = NULL WHERE VICTIM_ID = :victimId",
                    new OracleParameter("victimId", victimId));
                return true;
            }
            return false;
        }

        private Shelter MapShelter(DataRow row)
        {
            return new Shelter
            {
                ShelterId = Convert.ToInt32(row["SHELTER_ID"]),
                ShelterName = row["SHELTER_NAME"].ToString(),
                ShelterType = row["SHELTER_TYPE"].ToString(),
                Location = row["LOCATION"].ToString(),
                District = row["DISTRICT"].ToString(),
                Upazila = row["UPAZILA"] == DBNull.Value ? null : row["UPAZILA"].ToString(),
                Latitude = row["LATITUDE"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["LATITUDE"]),
                Longitude = row["LONGITUDE"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["LONGITUDE"]),
                ContactPerson = row["CONTACT_PERSON"] == DBNull.Value ? null : row["CONTACT_PERSON"].ToString(),
                ContactPhone = row["CONTACT_PHONE"] == DBNull.Value ? null : row["CONTACT_PHONE"].ToString(),
                Facilities = row["FACILITIES"] == DBNull.Value ? null : row["FACILITIES"].ToString(),
                HasMedical = Convert.ToInt32(row["HAS_MEDICAL"]),
                HasGenerator = Convert.ToInt32(row["HAS_GENERATOR"]),
                HasWifi = Convert.ToInt32(row["HAS_WIFI"]),
                IsActive = Convert.ToInt32(row["IS_ACTIVE"]),
                OpenedDate = Convert.ToDateTime(row["OPENED_DATE"]),
                ClosedDate = row["CLOSED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["CLOSED_DATE"]),
                CreatedBy = row["CREATED_BY"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["CREATED_BY"]),
                CreatedAt = Convert.ToDateTime(row["CREATED_AT"]),
                MaxCapacity = row["MAX_CAPACITY"] == DBNull.Value ? 0 : Convert.ToInt32(row["MAX_CAPACITY"]),
                CurrentOccupied = row["CURRENT_OCCUPIED"] == DBNull.Value ? 0 : Convert.ToInt32(row["CURRENT_OCCUPIED"]),
                AvailableBeds = row["AVAILABLE_BEDS"] == DBNull.Value ? 0 : Convert.ToInt32(row["AVAILABLE_BEDS"]),
                ReservedSpots = row["RESERVED_SPOTS"] == DBNull.Value ? 0 : Convert.ToInt32(row["RESERVED_SPOTS"]),
                HasOverflow = row["HAS_OVERFLOW"] == DBNull.Value ? 0 : Convert.ToInt32(row["HAS_OVERFLOW"]),
                OverflowLocation = row["OVERFLOW_LOCATION"] == DBNull.Value ? null : row["OVERFLOW_LOCATION"].ToString()
            };
        }
    }
}
