using System;
using System.Collections.Generic;
using System.Data;
using DIEMS.Models;
using Oracle.ManagedDataAccess.Client;

namespace DIEMS.Data
{
    public class UserRepository
    {
        private readonly OracleDbHelper _db;

        public UserRepository(OracleDbHelper db)
        {
            _db = db;
        }

        public User ValidateLogin(string username, string passwordHash)
        {
            string sql = @"
                SELECT u.USER_ID, u.USERNAME, u.EMAIL, u.FULL_NAME, u.PHONE, u.NID, u.ROLE_ID, u.DISTRICT, u.ADDRESS, u.PROFILE_PIC, u.IS_ACTIVE, r.ROLE_NAME 
                FROM USERS u
                JOIN ROLES r ON u.ROLE_ID = r.ROLE_ID
                WHERE u.USERNAME = :username AND u.PASSWORD_HASH = :passwordHash AND u.IS_ACTIVE = 1";

            var dt = _db.ExecuteQuery(sql, 
                new OracleParameter("username", username),
                new OracleParameter("passwordHash", passwordHash));

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                var user = new User
                {
                    UserId = Convert.ToInt32(row["USER_ID"]),
                    Username = row["USERNAME"].ToString(),
                    Email = row["EMAIL"].ToString(),
                    FullName = row["FULL_NAME"].ToString(),
                    Phone = row["PHONE"] == DBNull.Value ? null : row["PHONE"].ToString(),
                    Nid = row["NID"] == DBNull.Value ? null : row["NID"].ToString(),
                    RoleId = Convert.ToInt32(row["ROLE_ID"]),
                    District = row["DISTRICT"] == DBNull.Value ? null : row["DISTRICT"].ToString(),
                    Address = row["ADDRESS"] == DBNull.Value ? null : row["ADDRESS"].ToString(),
                    ProfilePic = row["PROFILE_PIC"] == DBNull.Value ? null : row["PROFILE_PIC"].ToString(),
                    IsActive = Convert.ToInt32(row["IS_ACTIVE"]),
                    RoleName = row["ROLE_NAME"].ToString()
                };

                // Update last login
                _db.ExecuteNonQuery("UPDATE USERS SET LAST_LOGIN = SYSTIMESTAMP WHERE USER_ID = :id", new OracleParameter("id", user.UserId));

                return user;
            }

            return null;
        }

        public bool RegisterUser(User user)
        {
            string sql = @"
                INSERT INTO USERS (USERNAME, PASSWORD_HASH, EMAIL, FULL_NAME, PHONE, NID, ROLE_ID, DISTRICT, ADDRESS, IS_ACTIVE)
                VALUES (:username, :passwordHash, :email, :fullName, :phone, :nid, :roleId, :district, :address, 1)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("username", user.Username),
                new OracleParameter("passwordHash", user.PasswordHash),
                new OracleParameter("email", user.Email),
                new OracleParameter("fullName", user.FullName),
                new OracleParameter("phone", user.Phone ?? (object)DBNull.Value),
                new OracleParameter("nid", user.Nid ?? (object)DBNull.Value),
                new OracleParameter("roleId", user.RoleId),
                new OracleParameter("district", user.District ?? (object)DBNull.Value),
                new OracleParameter("address", user.Address ?? (object)DBNull.Value));

            return rows > 0;
        }

        public List<Role> GetRoles()
        {
            var list = new List<Role>();
            var dt = _db.ExecuteQuery("SELECT ROLE_ID, ROLE_NAME, DESCRIPTION, IS_ACTIVE FROM ROLES WHERE IS_ACTIVE = 1");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Role
                {
                    RoleId = Convert.ToInt32(row["ROLE_ID"]),
                    RoleName = row["ROLE_NAME"].ToString(),
                    Description = row["DESCRIPTION"].ToString(),
                    IsActive = Convert.ToInt32(row["IS_ACTIVE"])
                });
            }
            return list;
        }
    }
}
