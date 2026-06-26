using System;
using System.Collections.Generic;
using System.Data;
using DIEMS.Models;
using Oracle.ManagedDataAccess.Client;

namespace DIEMS.Data
{
    public class ResourceRepository
    {
        private readonly OracleDbHelper _db;

        public ResourceRepository(OracleDbHelper db)
        {
            _db = db;
        }

        public List<Resource> GetAllResources()
        {
            var list = new List<Resource>();
            string sql = @"
                SELECT r.*, c.CATEGORY_NAME, c.UNIT, c.ICON, c.CRITICAL_THRESHOLD
                FROM RESOURCES r
                JOIN RESOURCE_CATEGORIES c ON r.CATEGORY_ID = c.CATEGORY_ID
                ORDER BY r.RESOURCE_NAME";

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapResource(row));
            }
            return list;
        }

        public List<Resource> GetCriticalResources()
        {
            var list = new List<Resource>();
            // Using CRITICAL_RESOURCES_VW view
            string sql = "SELECT * FROM CRITICAL_RESOURCES_VW ORDER BY STOCK_PCT ASC";
            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Resource
                {
                    ResourceId = Convert.ToInt32(row["RESOURCE_ID"]),
                    ResourceName = row["RESOURCE_NAME"].ToString(),
                    TotalQuantity = Convert.ToInt32(row["TOTAL_QUANTITY"]),
                    AvailableQuantity = Convert.ToInt32(row["AVAILABLE_QUANTITY"]),
                    CategoryName = row["CATEGORY_NAME"].ToString(),
                    Unit = row["UNIT"].ToString(),
                    Icon = row["ICON"].ToString(),
                    CriticalThreshold = Convert.ToInt32(row["CRITICAL_THRESHOLD"]),
                    Notes = $"Stock level: {row["STOCK_PCT"]}% ({row["ALERT_LEVEL"]})"
                });
            }
            return list;
        }

        public List<ResourceCategory> GetResourceCategories()
        {
            var list = new List<ResourceCategory>();
            var dt = _db.ExecuteQuery("SELECT * FROM RESOURCE_CATEGORIES WHERE IS_ACTIVE = 1");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ResourceCategory
                {
                    CategoryId = Convert.ToInt32(row["CATEGORY_ID"]),
                    CategoryName = row["CATEGORY_NAME"].ToString(),
                    Unit = row["UNIT"].ToString(),
                    Icon = row["ICON"].ToString(),
                    CriticalThreshold = Convert.ToInt32(row["CRITICAL_THRESHOLD"]),
                    Description = row["DESCRIPTION"].ToString()
                });
            }
            return list;
        }

        public bool InsertResource(Resource r)
        {
            string sql = @"
                INSERT INTO RESOURCES (CATEGORY_ID, RESOURCE_NAME, TOTAL_QUANTITY, AVAILABLE_QUANTITY, 
                                       RESERVED_QUANTITY, UNIT_COST, STORAGE_LOCATION, SUPPLIER_NAME, 
                                       SUPPLIER_CONTACT, EXPIRY_DATE, LAST_RESTOCKED, UPDATED_BY, NOTES)
                VALUES (:catId, :name, :totalQty, :availQty, 
                        :resQty, :cost, :location, :supplier, 
                        :contact, :expiry, SYSTIMESTAMP, :updatedBy, :notes)";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("catId", r.CategoryId),
                new OracleParameter("name", r.ResourceName),
                new OracleParameter("totalQty", r.TotalQuantity),
                new OracleParameter("availQty", r.AvailableQuantity),
                new OracleParameter("resQty", r.ReservedQuantity ?? 0),
                new OracleParameter("cost", r.UnitCost),
                new OracleParameter("location", r.StorageLocation ?? (object)DBNull.Value),
                new OracleParameter("supplier", r.SupplierName ?? (object)DBNull.Value),
                new OracleParameter("contact", r.SupplierContact ?? (object)DBNull.Value),
                new OracleParameter("expiry", r.ExpiryDate ?? (object)DBNull.Value),
                new OracleParameter("updatedBy", r.UpdatedBy ?? (object)DBNull.Value),
                new OracleParameter("notes", r.Notes ?? (object)DBNull.Value));

            return rows > 0;
        }

        public bool UpdateResource(Resource r)
        {
            string sql = @"
                UPDATE RESOURCES
                SET CATEGORY_ID = :catId, RESOURCE_NAME = :name, TOTAL_QUANTITY = :totalQty, 
                    AVAILABLE_QUANTITY = :availQty, RESERVED_QUANTITY = :resQty, UNIT_COST = :cost, 
                    STORAGE_LOCATION = :location, SUPPLIER_NAME = :supplier, SUPPLIER_CONTACT = :contact, 
                    EXPIRY_DATE = :expiry, LAST_UPDATED = SYSTIMESTAMP, UPDATED_BY = :updatedBy, NOTES = :notes
                WHERE RESOURCE_ID = :id";

            int rows = _db.ExecuteNonQuery(sql,
                new OracleParameter("catId", r.CategoryId),
                new OracleParameter("name", r.ResourceName),
                new OracleParameter("totalQty", r.TotalQuantity),
                new OracleParameter("availQty", r.AvailableQuantity),
                new OracleParameter("resQty", r.ReservedQuantity ?? 0),
                new OracleParameter("cost", r.UnitCost),
                new OracleParameter("location", r.StorageLocation ?? (object)DBNull.Value),
                new OracleParameter("supplier", r.SupplierName ?? (object)DBNull.Value),
                new OracleParameter("contact", r.SupplierContact ?? (object)DBNull.Value),
                new OracleParameter("expiry", r.ExpiryDate ?? (object)DBNull.Value),
                new OracleParameter("updatedBy", r.UpdatedBy ?? (object)DBNull.Value),
                new OracleParameter("notes", r.Notes ?? (object)DBNull.Value),
                new OracleParameter("id", r.ResourceId));

            return rows > 0;
        }

        public (int distId, string message) DistributeResources(ResourceDistribution d)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new OracleCommand("DISTRIBUTE_RESOURCES", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_resource_id", OracleDbType.Int32).Value = d.ResourceId;
                cmd.Parameters.Add("p_quantity", OracleDbType.Int32).Value = d.Quantity;
                cmd.Parameters.Add("p_shelter_id", OracleDbType.Int32).Value = (object)d.ShelterId ?? DBNull.Value;
                cmd.Parameters.Add("p_disaster_id", OracleDbType.Int32).Value = (object)d.DisasterId ?? DBNull.Value;
                cmd.Parameters.Add("p_distributed_by", OracleDbType.Int32).Value = d.DistributedBy;
                cmd.Parameters.Add("p_priority", OracleDbType.Varchar2).Value = d.Priority ?? "Normal";
                cmd.Parameters.Add("p_notes", OracleDbType.Varchar2).Value = (object)d.Notes ?? DBNull.Value;

                var paramDistId = new OracleParameter("p_dist_id", OracleDbType.Int32, ParameterDirection.Output);
                var paramMsg = new OracleParameter("p_message", OracleDbType.Varchar2, 2000, null, ParameterDirection.Output);
                
                cmd.Parameters.Add(paramDistId);
                cmd.Parameters.Add(paramMsg);

                cmd.ExecuteNonQuery();

                int distId = paramDistId.Value != DBNull.Value ? Convert.ToInt32(paramDistId.Value) : -1;
                string msg = paramMsg.Value != DBNull.Value ? paramMsg.Value.ToString() : "Completed";

                return (distId, msg);
            }
        }

        public List<ResourceDistribution> GetDistributionLog()
        {
            var list = new List<ResourceDistribution>();
            string sql = @"
                SELECT rd.*, r.RESOURCE_NAME, rc.CATEGORY_NAME, s.SHELTER_NAME, d.DISASTER_NAME, u.FULL_NAME AS DIST_BY_NAME
                FROM RESOURCE_DISTRIBUTION rd
                JOIN RESOURCES r ON rd.RESOURCE_ID = r.RESOURCE_ID
                JOIN RESOURCE_CATEGORIES rc ON r.CATEGORY_ID = rc.CATEGORY_ID
                LEFT JOIN SHELTERS s ON rd.SHELTER_ID = s.SHELTER_ID
                LEFT JOIN DISASTERS d ON rd.DISASTER_ID = d.DISASTER_ID
                JOIN USERS u ON rd.DISTRIBUTED_BY = u.USER_ID
                ORDER BY rd.DISTRIBUTED_AT DESC";

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ResourceDistribution
                {
                    DistId = Convert.ToInt32(row["DIST_ID"]),
                    ResourceId = Convert.ToInt32(row["RESOURCE_ID"]),
                    Quantity = Convert.ToInt32(row["QUANTITY"]),
                    ShelterId = row["SHELTER_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["SHELTER_ID"]),
                    DisasterId = row["DISASTER_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["DISASTER_ID"]),
                    Priority = row["PRIORITY"].ToString(),
                    DistributedBy = Convert.ToInt32(row["DISTRIBUTED_BY"]),
                    DistributedAt = Convert.ToDateTime(row["DISTRIBUTED_AT"]),
                    Status = row["STATUS"].ToString(),
                    DeliveryDate = row["DELIVERY_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["DELIVERY_DATE"]),
                    ReceivedBy = row["RECEIVED_BY"] == DBNull.Value ? null : row["RECEIVED_BY"].ToString(),
                    Notes = row["NOTES"] == DBNull.Value ? null : row["NOTES"].ToString(),
                    ResourceName = row["RESOURCE_NAME"].ToString(),
                    CategoryName = row["CATEGORY_NAME"].ToString(),
                    ShelterName = row["SHELTER_NAME"] == DBNull.Value ? "Field Operation" : row["SHELTER_NAME"].ToString(),
                    DisasterName = row["DISASTER_NAME"] == DBNull.Value ? "General Relief" : row["DISASTER_NAME"].ToString(),
                    DistributedByName = row["DIST_BY_NAME"].ToString()
                });
            }
            return list;
        }

        private Resource MapResource(DataRow row)
        {
            return new Resource
            {
                ResourceId = Convert.ToInt32(row["RESOURCE_ID"]),
                CategoryId = Convert.ToInt32(row["CATEGORY_ID"]),
                ResourceName = row["RESOURCE_NAME"].ToString(),
                TotalQuantity = Convert.ToInt32(row["TOTAL_QUANTITY"]),
                AvailableQuantity = Convert.ToInt32(row["AVAILABLE_QUANTITY"]),
                ReservedQuantity = row["RESERVED_QUANTITY"] == DBNull.Value ? 0 : Convert.ToInt32(row["RESERVED_QUANTITY"]),
                UnitCost = Convert.ToDecimal(row["UNIT_COST"]),
                StorageLocation = row["STORAGE_LOCATION"] == DBNull.Value ? null : row["STORAGE_LOCATION"].ToString(),
                SupplierName = row["SUPPLIER_NAME"] == DBNull.Value ? null : row["SUPPLIER_NAME"].ToString(),
                SupplierContact = row["SUPPLIER_CONTACT"] == DBNull.Value ? null : row["SUPPLIER_CONTACT"].ToString(),
                ExpiryDate = row["EXPIRY_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["EXPIRY_DATE"]),
                LastRestocked = row["LAST_RESTOCKED"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["LAST_RESTOCKED"]),
                LastUpdated = Convert.ToDateTime(row["LAST_UPDATED"]),
                UpdatedBy = row["UPDATED_BY"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["UPDATED_BY"]),
                Notes = row["NOTES"] == DBNull.Value ? null : row["NOTES"].ToString(),
                CategoryName = row["CATEGORY_NAME"].ToString(),
                Unit = row["UNIT"].ToString(),
                Icon = row["ICON"].ToString(),
                CriticalThreshold = Convert.ToInt32(row["CRITICAL_THRESHOLD"])
            };
        }
    }
}
