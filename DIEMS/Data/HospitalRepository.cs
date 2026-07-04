using System;
using System.Collections.Generic;
using System.Data;
using DIEMS.Models;
using Oracle.ManagedDataAccess.Client;

namespace DIEMS.Data
{
    public class HospitalRepository
    {
        private readonly OracleDbHelper _db;

        public HospitalRepository(OracleDbHelper db)
        {
            _db = db;
        }

        public List<Hospital> GetAllHospitals(string sort = "NAME")
        {
            var list = new List<Hospital>();
            string sql = "SELECT * FROM HOSPITALS WHERE IS_ACTIVE = 1 ";
            
            if (sort == "BEDS") {
                sql += "ORDER BY AVAILABLE_BEDS DESC";
            } else if (sort == "ICU") {
                sql += "ORDER BY ICU_AVAILABLE DESC";
            } else {
                sql += "ORDER BY HOSPITAL_NAME";
            }

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapHospital(row));
            }
            return list;
        }

        public Hospital GetHospitalById(int id)
        {
            string sql = "SELECT * FROM HOSPITALS WHERE HOSPITAL_ID = :id";
            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", id));
            if (dt.Rows.Count > 0)
            {
                return MapHospital(dt.Rows[0]);
            }
            return null;
        }

        public bool InsertHospital(Hospital h)
        {
            string sql = @"
                INSERT INTO HOSPITALS (HOSPITAL_NAME, TOTAL_BEDS, AVAILABLE_BEDS, ICU_BEDS, ICU_AVAILABLE, 
                                       HAS_EMERGENCY, CONTACT_NUMBER, EMAIL, ADDRESS, DISTRICT, 
                                       LATITUDE, LONGITUDE, BLOOD_O_POS, BLOOD_O_NEG, BLOOD_A_POS, IS_ACTIVE)
                VALUES (:name, :beds, :availBeds, :icu, :availIcu, 
                        :surgery, :phone, :email, :address, :district, 
                        :lat, :lng, :oPos, :oNeg, :aPos, 1)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("name", h.HospitalName),
                new OracleParameter("beds", h.CapacityBeds),
                new OracleParameter("availBeds", h.AvailableBeds),
                new OracleParameter("icu", h.CapacityIcu),
                new OracleParameter("availIcu", h.AvailableIcu),
                new OracleParameter("surgery", h.HasSurgery),
                new OracleParameter("phone", h.ContactPhone ?? (object)DBNull.Value),
                new OracleParameter("email", h.Email ?? (object)DBNull.Value),
                new OracleParameter("address", h.Address ?? (object)DBNull.Value),
                new OracleParameter("district", h.District),
                new OracleParameter("lat", h.Latitude ?? (object)DBNull.Value),
                new OracleParameter("lng", h.Longitude ?? (object)DBNull.Value),
                new OracleParameter("oPos", h.BloodStockOPos),
                new OracleParameter("oNeg", h.BloodStockONeg),
                new OracleParameter("aPos", h.BloodStockAPos));

            return rows > 0;
        }

        public bool UpdateHospital(Hospital h)
        {
            string sql = @"
                UPDATE HOSPITALS
                SET HOSPITAL_NAME = :name, TOTAL_BEDS = :beds, AVAILABLE_BEDS = :availBeds, 
                    ICU_BEDS = :icu, ICU_AVAILABLE = :availIcu, HAS_EMERGENCY = :surgery, 
                    CONTACT_NUMBER = :phone, EMAIL = :email, ADDRESS = :address, DISTRICT = :district, 
                    LATITUDE = :lat, LONGITUDE = :lng, BLOOD_O_POS = :oPos, 
                    BLOOD_O_NEG = :oNeg, BLOOD_A_POS = :aPos, IS_ACTIVE = :isActive
                WHERE HOSPITAL_ID = :id";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("name", h.HospitalName),
                new OracleParameter("beds", h.CapacityBeds),
                new OracleParameter("availBeds", h.AvailableBeds),
                new OracleParameter("icu", h.CapacityIcu),
                new OracleParameter("availIcu", h.AvailableIcu),
                new OracleParameter("surgery", h.HasSurgery),
                new OracleParameter("phone", h.ContactPhone ?? (object)DBNull.Value),
                new OracleParameter("email", h.Email ?? (object)DBNull.Value),
                new OracleParameter("address", h.Address ?? (object)DBNull.Value),
                new OracleParameter("district", h.District),
                new OracleParameter("lat", h.Latitude ?? (object)DBNull.Value),
                new OracleParameter("lng", h.Longitude ?? (object)DBNull.Value),
                new OracleParameter("oPos", h.BloodStockOPos),
                new OracleParameter("oNeg", h.BloodStockONeg),
                new OracleParameter("aPos", h.BloodStockAPos),
                new OracleParameter("isActive", h.IsActive),
                new OracleParameter("id", h.HospitalId));

            return rows > 0;
        }

        public List<Doctor> GetDoctors(int hospitalId)
        {
            var list = new List<Doctor>();
            string sql = @"
                SELECT d.*, h.HOSPITAL_NAME, dis.DISASTER_NAME
                FROM DOCTORS d
                JOIN HOSPITALS h ON d.HOSPITAL_ID = h.HOSPITAL_ID
                LEFT JOIN DISASTERS dis ON d.ASSIGNED_DISASTER_ID = dis.DISASTER_ID
                WHERE d.HOSPITAL_ID = :id
                ORDER BY d.FULL_NAME";

            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", hospitalId));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Doctor
                {
                    DoctorId = Convert.ToInt32(row["DOCTOR_ID"]),
                    FullName = row["FULL_NAME"].ToString(),
                    Specialization = row["SPECIALIZATION"].ToString(),
                    Phone = row["PHONE"] == DBNull.Value ? null : row["PHONE"].ToString(),
                    Email = row["EMAIL"] == DBNull.Value ? null : row["EMAIL"].ToString(),
                    HospitalId = Convert.ToInt32(row["HOSPITAL_ID"]),
                    AvailabilityStatus = row["AVAILABILITY_STATUS"].ToString(),
                    IsAvailable = Convert.ToInt32(row["IS_AVAILABLE"]),
                    DeploymentLocation = row["DEPLOYMENT_LOCATION"] == DBNull.Value ? null : row["DEPLOYMENT_LOCATION"].ToString(),
                    AssignedDisasterId = row["ASSIGNED_DISASTER_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ASSIGNED_DISASTER_ID"]),
                    HospitalName = row["HOSPITAL_NAME"].ToString(),
                    DisasterName = row["DISASTER_NAME"] == DBNull.Value ? "None" : row["DISASTER_NAME"].ToString()
                });
            }
            return list;
        }

        public List<Ambulance> GetAmbulances(int hospitalId)
        {
            var list = new List<Ambulance>();
            string sql = @"
                SELECT a.*, h.HOSPITAL_NAME
                FROM AMBULANCES a
                JOIN HOSPITALS h ON a.HOSPITAL_ID = h.HOSPITAL_ID
                WHERE a.HOSPITAL_ID = :id
                ORDER BY a.VEHICLE_NO";

            var dt = _db.ExecuteQuery(sql, new OracleParameter("id", hospitalId));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Ambulance
                {
                    AmbulanceId = Convert.ToInt32(row["AMBULANCE_ID"]),
                    VehicleNo = row["VEHICLE_NO"].ToString(),
                    DriverName = row["DRIVER_NAME"].ToString(),
                    DriverPhone = row["DRIVER_PHONE"] == DBNull.Value ? null : row["DRIVER_PHONE"].ToString(),
                    HospitalId = Convert.ToInt32(row["HOSPITAL_ID"]),
                    Latitude = row["LATITUDE"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["LATITUDE"]),
                    Longitude = row["LONGITUDE"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["LONGITUDE"]),
                    Status = row["STATUS"].ToString(),
                    AmbulanceType = row["AMBULANCE_TYPE"].ToString(),
                    CurrentLocation = row["CURRENT_LOCATION"] == DBNull.Value ? null : row["CURRENT_LOCATION"].ToString(),
                    LastUpdated = Convert.ToDateTime(row["LAST_UPDATED"]),
                    HospitalName = row["HOSPITAL_NAME"].ToString()
                });
            }
            return list;
        }

        public List<MedicalRequest> GetMedicalRequests()
        {
            var list = new List<MedicalRequest>();
            string sql = @"
                SELECT mr.*, h.HOSPITAL_NAME, d.DISASTER_NAME, u.FULL_NAME AS REQ_BY_NAME
                FROM MEDICAL_REQUESTS mr
                JOIN HOSPITALS h ON mr.HOSPITAL_ID = h.HOSPITAL_ID
                LEFT JOIN DISASTERS d ON mr.DISASTER_ID = d.DISASTER_ID
                JOIN USERS u ON mr.REQUESTED_BY = u.USER_ID
                ORDER BY mr.REQUESTED_AT DESC";

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new MedicalRequest
                {
                    RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                    HospitalId = Convert.ToInt32(row["HOSPITAL_ID"]),
                    RequestType = row["REQUEST_TYPE"].ToString(),
                    RequestDetails = row["REQUEST_DETAILS"].ToString(),
                    Priority = row["PRIORITY"].ToString(),
                    BloodGroup = row["BLOOD_GROUP"] == DBNull.Value ? null : row["BLOOD_GROUP"].ToString(),
                    DisasterId = row["DISASTER_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["DISASTER_ID"]),
                    Status = row["STATUS"].ToString(),
                    RequestedBy = Convert.ToInt32(row["REQUESTED_BY"]),
                    RequestedAt = Convert.ToDateTime(row["REQUESTED_AT"]),
                    ResponseNotes = row["RESPONSE_NOTES"] == DBNull.Value ? null : row["RESPONSE_NOTES"].ToString(),
                    HospitalName = row["HOSPITAL_NAME"].ToString(),
                    DisasterName = row["DISASTER_NAME"] == DBNull.Value ? "General Operations" : row["DISASTER_NAME"].ToString(),
                    RequestedByName = row["REQ_BY_NAME"].ToString()
                });
            }
            return list;
        }

        public bool InsertMedicalRequest(MedicalRequest req)
        {
            string sql = @"
                INSERT INTO MEDICAL_REQUESTS (HOSPITAL_ID, REQUEST_TYPE, REQUEST_DETAILS, PRIORITY, 
                                              BLOOD_GROUP, DISASTER_ID, STATUS, REQUESTED_BY)
                VALUES (:hospitalId, :reqType, :details, :priority, :blood, :disasterId, 'Pending', :requestedBy)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("hospitalId", req.HospitalId),
                new OracleParameter("reqType", req.RequestType),
                new OracleParameter("details", req.RequestDetails),
                new OracleParameter("priority", req.Priority),
                new OracleParameter("blood", req.BloodGroup ?? (object)DBNull.Value),
                new OracleParameter("disasterId", req.DisasterId ?? (object)DBNull.Value),
                new OracleParameter("requestedBy", req.RequestedBy));

            return rows > 0;
        }

        private Hospital MapHospital(DataRow row)
        {
            return new Hospital
            {
                HospitalId = Convert.ToInt32(row["HOSPITAL_ID"]),
                HospitalName = row["HOSPITAL_NAME"].ToString(),
                CapacityBeds = Convert.ToInt32(row["TOTAL_BEDS"]),
                AvailableBeds = Convert.ToInt32(row["AVAILABLE_BEDS"]),
                CapacityIcu = Convert.ToInt32(row["ICU_BEDS"]),
                AvailableIcu = Convert.ToInt32(row["ICU_AVAILABLE"]),
                HasSurgery = Convert.ToInt32(row["HAS_EMERGENCY"]),
                ContactPhone = row["CONTACT_NUMBER"] == DBNull.Value ? null : row["CONTACT_NUMBER"].ToString(),
                Email = row["EMAIL"] == DBNull.Value ? null : row["EMAIL"].ToString(),
                Address = row["ADDRESS"] == DBNull.Value ? null : row["ADDRESS"].ToString(),
                District = row["DISTRICT"].ToString(),
                Latitude = row["LATITUDE"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["LATITUDE"]),
                Longitude = row["LONGITUDE"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["LONGITUDE"]),
                BloodStockOPos = Convert.ToInt32(row["BLOOD_O_POS"]),
                BloodStockONeg = Convert.ToInt32(row["BLOOD_O_NEG"]),
                BloodStockAPos = Convert.ToInt32(row["BLOOD_A_POS"]),
                IsActive = Convert.ToInt32(row["IS_ACTIVE"]),
                CreatedAt = Convert.ToDateTime(row["CREATED_AT"]),
                UpdatedAt = Convert.ToDateTime(row["CREATED_AT"])
            };
        }
    }
}
