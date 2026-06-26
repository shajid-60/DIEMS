using System;
using System.Collections.Generic;
using System.Data;

namespace DIEMS.Data
{
    public class AnalyticsRepository
    {
        private readonly OracleDbHelper _db;

        public AnalyticsRepository(OracleDbHelper db)
        {
            _db = db;
        }

        public Dictionary<string, object> GetDashboardSummary()
        {
            var summary = new Dictionary<string, object>();

            // Get core counts
            summary["TotalDisasters"] = _db.ExecuteScalar("SELECT COUNT(*) FROM DISASTERS") ?? 0;
            summary["ActiveDisasters"] = _db.ExecuteScalar("SELECT COUNT(*) FROM DISASTERS WHERE STATUS = 'ACTIVE'") ?? 0;
            summary["TotalVictims"] = _db.ExecuteScalar("SELECT COUNT(*) FROM VICTIMS") ?? 0;
            summary["ShelteredVictims"] = _db.ExecuteScalar("SELECT COUNT(*) FROM VICTIMS WHERE STATUS = 'Sheltered'") ?? 0;
            summary["TotalVolunteers"] = _db.ExecuteScalar("SELECT COUNT(*) FROM VOLUNTEERS") ?? 0;
            summary["ActiveVolunteers"] = _db.ExecuteScalar("SELECT COUNT(*) FROM VOLUNTEERS WHERE AVAILABILITY = 'Available'") ?? 0;
            summary["TotalHospitals"] = _db.ExecuteScalar("SELECT COUNT(*) FROM HOSPITALS WHERE IS_ACTIVE = 1") ?? 0;
            
            // Total financial damage sum calling Oracle CALCULATE_DAMAGE function per active/all disasters
            summary["TotalDamageBDT"] = _db.ExecuteScalar(@"
                SELECT NVL(SUM(CALCULATE_DAMAGE(DISASTER_ID)), 0) 
                FROM DISASTERS") ?? 0;

            // Total available beds across all active shelters
            summary["TotalAvailableBeds"] = _db.ExecuteScalar(@"
                SELECT NVL(SUM(AVAILABLE_BEDS), 0) 
                FROM SHELTER_CAPACITY sc
                JOIN SHELTERS s ON sc.SHELTER_ID = s.SHELTER_ID
                WHERE s.IS_ACTIVE = 1") ?? 0;

            // Total active incidents pending
            summary["PendingIncidents"] = _db.ExecuteScalar("SELECT COUNT(*) FROM INCIDENT_REPORTS WHERE STATUS = 'Pending'") ?? 0;

            return summary;
        }

        public List<Dictionary<string, object>> GetResourceCategoryLevels()
        {
            var list = new List<Dictionary<string, object>>();
            // Select category stock level percentages
            string sql = @"
                SELECT rc.CATEGORY_NAME, rc.UNIT, rc.ICON, rc.CRITICAL_THRESHOLD,
                       NVL(SUM(r.TOTAL_QUANTITY), 0) as TOTAL_STOCK,
                       NVL(SUM(r.AVAILABLE_QUANTITY), 0) as AVAIL_STOCK
                FROM RESOURCE_CATEGORIES rc
                LEFT JOIN RESOURCES r ON rc.CATEGORY_ID = r.CATEGORY_ID
                GROUP BY rc.CATEGORY_NAME, rc.UNIT, rc.ICON, rc.CRITICAL_THRESHOLD
                ORDER BY rc.CATEGORY_NAME";

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                dict["CategoryName"] = row["CATEGORY_NAME"].ToString();
                dict["Unit"] = row["UNIT"].ToString();
                dict["Icon"] = row["ICON"].ToString();
                dict["Threshold"] = Convert.ToInt32(row["CRITICAL_THRESHOLD"]);
                
                int total = Convert.ToInt32(row["TOTAL_STOCK"]);
                int avail = Convert.ToInt32(row["AVAIL_STOCK"]);
                dict["TotalStock"] = total;
                dict["AvailableStock"] = avail;
                
                double pct = total > 0 ? ((double)avail / total) * 100 : 100;
                dict["StockPercentage"] = Math.Round(pct, 1);
                list.Add(dict);
            }
            return list;
        }

        public List<Dictionary<string, object>> GetDisasterTrendData()
        {
            var list = new List<Dictionary<string, object>>();
            string sql = @"
                SELECT TO_CHAR(START_DATE, 'YYYY-MM') AS MONTH_YEAR, COUNT(*) as DISASTER_COUNT,
                       SUM(AFFECTED_POPULATION) as TOTAL_AFFECTED
                FROM DISASTERS
                GROUP BY TO_CHAR(START_DATE, 'YYYY-MM')
                ORDER BY MONTH_YEAR ASC";

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                dict["MonthYear"] = row["MONTH_YEAR"].ToString();
                dict["Count"] = Convert.ToInt32(row["DISASTER_COUNT"]);
                dict["Affected"] = Convert.ToInt32(row["TOTAL_AFFECTED"]);
                list.Add(dict);
            }
            return list;
        }

        public List<Dictionary<string, object>> GetHospitalStatusData()
        {
            var list = new List<Dictionary<string, object>>();
            string sql = @"
                SELECT HOSPITAL_NAME, TOTAL_BEDS AS CAPACITY_BEDS, AVAILABLE_BEDS, ICU_BEDS AS CAPACITY_ICU, ICU_AVAILABLE AS AVAILABLE_ICU
                FROM HOSPITALS
                WHERE IS_ACTIVE = 1
                ORDER BY CAPACITY_BEDS DESC";

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                dict["HospitalName"] = row["HOSPITAL_NAME"].ToString();
                dict["BedsCapacity"] = Convert.ToInt32(row["CAPACITY_BEDS"]);
                dict["BedsAvailable"] = Convert.ToInt32(row["AVAILABLE_BEDS"]);
                dict["IcuCapacity"] = Convert.ToInt32(row["CAPACITY_ICU"]);
                dict["IcuAvailable"] = Convert.ToInt32(row["AVAILABLE_ICU"]);
                list.Add(dict);
            }
            return list;
        }
    }
}
