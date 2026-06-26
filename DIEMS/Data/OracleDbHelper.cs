using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace DIEMS.Data
{
    public class OracleDbHelper
    {
        private readonly string _connectionString;

        public OracleDbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDB") 
                ?? throw new InvalidOperationException("Connection string 'OracleDB' is missing in configuration.");
        }

        public OracleConnection GetConnection()
        {
            var conn = new OracleConnection(_connectionString);
            conn.Open();
            return conn;
        }

        // Execute queries and return a DataTable
        public DataTable ExecuteQuery(string sql, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (var adapter = new OracleDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // Execute INSERT, UPDATE, DELETE queries
        public int ExecuteNonQuery(string sql, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        // Execute a query that returns a single scalar value
        public object ExecuteScalar(string sql, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                var val = cmd.ExecuteScalar();
                return val == DBNull.Value ? null : val;
            }
        }
    }
}
