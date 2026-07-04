using System;
using System.Collections.Generic;
using System.Data;
using DIEMS.Models;
using Oracle.ManagedDataAccess.Client;

namespace DIEMS.Data
{
    public class VictimRepository
    {
        private readonly OracleDbHelper _db;

        public VictimRepository(OracleDbHelper db)
        {
            _db = db;
        }

        public List<Victim> GetAllVictims()
        {
            return GetFilteredVictims("ALL", "LATEST");
        }

        public List<Victim> GetFilteredVictims(string status, string sort)
        {
            var list = new List<Victim>();
            
            using (var conn = _db.GetConnection())
            using (var cmd = new OracleCommand("GET_FILTERED_VICTIMS", conn))
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
                        list.Add(MapVictim(row));
                    }
                }
            }
            return list;
        }

        public Victim GetVictimById(int id)
        {
            string sql = @"
                SELECT v.*, d.DISASTER_NAME, s.SHELTER_NAME, u.FULL_NAME AS REGISTERED_BY_NAME
                FROM VICTIMS v
                JOIN DISASTERS d ON v.DISASTER_ID = d.DISASTER_ID
                LEFT JOIN SHELTERS s ON v.SHELTER_ID = s.SHELTER_ID
                LEFT JOIN USERS u ON v.REGISTERED_BY = u.USER_ID
                WHERE v.VICTIM_ID = :id";

            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", id));
            if (dt.Rows.Count > 0)
            {
                return MapVictim(dt.Rows[0]);
            }
            return null;
        }

        public bool InsertVictim(Victim v, bool autoAllocateShelter = true)
        {
            using (var conn = _db.GetConnection())
            {
                // Create a transaction to guarantee atomic execution of insert + procedure call
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = @"
                            INSERT INTO VICTIMS (NID, FULL_NAME, AGE, DATE_OF_BIRTH, GENDER, PHONE, 
                                                 EMERGENCY_CONTACT, ADDRESS, DISTRICT, DISASTER_ID, 
                                                 SHELTER_ID, MEDICAL_CONDITION, BLOOD_GROUP, STATUS, REGISTERED_BY)
                            VALUES (:nid, :name, :age, :dob, :gender, :phone, 
                                    :emergency, :address, :district, :disasterId, 
                                    :shelterId, :medCond, :bloodGroup, :status, :registeredBy)
                            RETURNING VICTIM_ID INTO :newId";

                        var paramNewId = new OracleParameter("newId", OracleDbType.Int32, ParameterDirection.ReturnValue);

                        using (var cmd = new OracleCommand(sql, conn))
                        {
                            cmd.Transaction = trans;
                            cmd.Parameters.Add("nid", OracleDbType.Varchar2).Value = (object)v.Nid ?? DBNull.Value;
                            cmd.Parameters.Add("name", OracleDbType.Varchar2).Value = v.FullName;
                            cmd.Parameters.Add("age", OracleDbType.Int32).Value = (object)v.Age ?? DBNull.Value;
                            cmd.Parameters.Add("dob", OracleDbType.Date).Value = (object)v.DateOfBirth ?? DBNull.Value;
                            cmd.Parameters.Add("gender", OracleDbType.Char).Value = v.Gender;
                            cmd.Parameters.Add("phone", OracleDbType.Varchar2).Value = (object)v.Phone ?? DBNull.Value;
                            cmd.Parameters.Add("emergency", OracleDbType.Varchar2).Value = (object)v.EmergencyContact ?? DBNull.Value;
                            cmd.Parameters.Add("address", OracleDbType.Varchar2).Value = (object)v.Address ?? DBNull.Value;
                            cmd.Parameters.Add("district", OracleDbType.Varchar2).Value = (object)v.District ?? DBNull.Value;
                            cmd.Parameters.Add("disasterId", OracleDbType.Int32).Value = v.DisasterId;
                            cmd.Parameters.Add("shelterId", OracleDbType.Int32).Value = (object)v.ShelterId ?? DBNull.Value;
                            cmd.Parameters.Add("medCond", OracleDbType.Varchar2).Value = v.MedicalCondition;
                            cmd.Parameters.Add("bloodGroup", OracleDbType.Varchar2).Value = (object)v.BloodGroup ?? DBNull.Value;
                            cmd.Parameters.Add("status", OracleDbType.Varchar2).Value = v.Status;
                            cmd.Parameters.Add("registeredBy", OracleDbType.Int32).Value = (object)v.RegisteredBy ?? DBNull.Value;
                            cmd.Parameters.Add(paramNewId);

                            cmd.ExecuteNonQuery();
                        }

                        int newVictimId = Convert.ToInt32(paramNewId.Value);

                        // If auto-allocating shelter and shelter was not set manually, run procedure
                        if (autoAllocateShelter && v.ShelterId == null)
                        {
                            using (var cmdProc = new OracleCommand("ALLOCATE_SHELTER", conn))
                            {
                                cmdProc.Transaction = trans;
                                cmdProc.CommandType = CommandType.StoredProcedure;
                                cmdProc.Parameters.Add("p_victim_id", OracleDbType.Int32).Value = newVictimId;
                                cmdProc.Parameters.Add("p_disaster_id", OracleDbType.Int32).Value = v.DisasterId;
                                cmdProc.ExecuteNonQuery();
                            }
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
        }

        public bool UpdateVictim(Victim v)
        {
            string sql = @"
                UPDATE VICTIMS
                SET NID = :nid, FULL_NAME = :name, AGE = :age, DATE_OF_BIRTH = :dob, GENDER = :gender, 
                    PHONE = :phone, EMERGENCY_CONTACT = :emergency, ADDRESS = :address, DISTRICT = :district, 
                    DISASTER_ID = :disasterId, SHELTER_ID = :shelterId, MEDICAL_CONDITION = :medCond, 
                    BLOOD_GROUP = :bloodGroup, STATUS = :status, UPDATED_AT = SYSTIMESTAMP
                WHERE VICTIM_ID = :id";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("nid", v.Nid ?? (object)DBNull.Value),
                new OracleParameter("name", v.FullName),
                new OracleParameter("age", v.Age ?? (object)DBNull.Value),
                new OracleParameter("dob", v.DateOfBirth ?? (object)DBNull.Value),
                new OracleParameter("gender", v.Gender),
                new OracleParameter("phone", v.Phone ?? (object)DBNull.Value),
                new OracleParameter("emergency", v.EmergencyContact ?? (object)DBNull.Value),
                new OracleParameter("address", v.Address ?? (object)DBNull.Value),
                new OracleParameter("district", v.District ?? (object)DBNull.Value),
                new OracleParameter("disasterId", v.DisasterId),
                new OracleParameter("shelterId", v.ShelterId ?? (object)DBNull.Value),
                new OracleParameter("medCond", v.MedicalCondition),
                new OracleParameter("bloodGroup", v.BloodGroup ?? (object)DBNull.Value),
                new OracleParameter("status", v.Status),
                new OracleParameter("id", v.VictimId));

            return rows > 0;
        }

        public List<FamilyMember> GetFamilyMembers(int victimId)
        {
            var list = new List<FamilyMember>();
            string sql = "SELECT * FROM FAMILY_MEMBERS WHERE VICTIM_ID = :id";
            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", victimId));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new FamilyMember
                {
                    FmId = Convert.ToInt32(row["FM_ID"]),
                    VictimId = Convert.ToInt32(row["VICTIM_ID"]),
                    FullName = row["FULL_NAME"].ToString(),
                    Relation = row["RELATION"].ToString(),
                    Age = row["AGE"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["AGE"]),
                    Gender = row["GENDER"].ToString(),
                    Phone = row["PHONE"] == DBNull.Value ? null : row["PHONE"].ToString(),
                    IsSeparated = Convert.ToInt32(row["IS_SEPARATED"]),
                    LastKnownLoc = row["LAST_KNOWN_LOC"] == DBNull.Value ? null : row["LAST_KNOWN_LOC"].ToString(),
                    Status = row["STATUS"].ToString(),
                    Notes = row["NOTES"] == DBNull.Value ? null : row["NOTES"].ToString()
                });
            }
            return list;
        }

        public bool InsertFamilyMember(FamilyMember m)
        {
            string sql = @"
                INSERT INTO FAMILY_MEMBERS (VICTIM_ID, FULL_NAME, RELATION, AGE, GENDER, PHONE, IS_SEPARATED, LAST_KNOWN_LOC, STATUS, NOTES)
                VALUES (:victimId, :name, :rel, :age, :gender, :phone, :sep, :loc, :status, :notes)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("victimId", m.VictimId),
                new OracleParameter("name", m.FullName),
                new OracleParameter("rel", m.Relation),
                new OracleParameter("age", m.Age ?? (object)DBNull.Value),
                new OracleParameter("gender", m.Gender ?? (object)DBNull.Value),
                new OracleParameter("phone", m.Phone ?? (object)DBNull.Value),
                new OracleParameter("sep", m.IsSeparated),
                new OracleParameter("loc", m.LastKnownLoc ?? (object)DBNull.Value),
                new OracleParameter("status", m.Status ?? "Unknown"),
                new OracleParameter("notes", m.Notes ?? (object)DBNull.Value));

            return rows > 0;
        }

        private Victim MapVictim(DataRow row)
        {
            return new Victim
            {
                VictimId = Convert.ToInt32(row["VICTIM_ID"]),
                Nid = row["NID"] == DBNull.Value ? null : row["NID"].ToString(),
                FullName = row["FULL_NAME"].ToString(),
                Age = row["AGE"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["AGE"]),
                DateOfBirth = row["DATE_OF_BIRTH"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["DATE_OF_BIRTH"]),
                Gender = row["GENDER"].ToString(),
                Phone = row["PHONE"] == DBNull.Value ? null : row["PHONE"].ToString(),
                EmergencyContact = row["EMERGENCY_CONTACT"] == DBNull.Value ? null : row["EMERGENCY_CONTACT"].ToString(),
                Address = row["ADDRESS"] == DBNull.Value ? null : row["ADDRESS"].ToString(),
                District = row["DISTRICT"] == DBNull.Value ? null : row["DISTRICT"].ToString(),
                DisasterId = Convert.ToInt32(row["DISASTER_ID"]),
                ShelterId = row["SHELTER_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["SHELTER_ID"]),
                MedicalCondition = row["MEDICAL_CONDITION"].ToString(),
                BloodGroup = row["BLOOD_GROUP"] == DBNull.Value ? null : row["BLOOD_GROUP"].ToString(),
                Status = row["STATUS"].ToString(),
                RegisteredBy = row["REGISTERED_BY"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["REGISTERED_BY"]),
                RegisteredAt = Convert.ToDateTime(row["REGISTERED_AT"]),
                UpdatedAt = Convert.ToDateTime(row["UPDATED_AT"]),
                Notes = row["NOTES"] == DBNull.Value ? null : row["NOTES"].ToString(),
                DisasterName = row["DISASTER_NAME"].ToString(),
                ShelterName = row["SHELTER_NAME"] == DBNull.Value ? "None Assigned" : row["SHELTER_NAME"].ToString(),
                RegisteredByName = row["REGISTERED_BY_NAME"] == DBNull.Value ? "System" : row["REGISTERED_BY_NAME"].ToString()
            };
        }
    }
}
