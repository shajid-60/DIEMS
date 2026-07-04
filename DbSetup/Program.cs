using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

class Program
{
    static void Main()
    {
        string connStr = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XE)));User Id=system;Password=DIEMS2026;";
        
        var commands = new List<string>();

        // 1. Victims
        commands.Add(@"
CREATE OR REPLACE FUNCTION GET_FILTERED_VICTIMS(p_status IN VARCHAR2, p_sort IN VARCHAR2) RETURN SYS_REFCURSOR IS
    v_cursor SYS_REFCURSOR;
    v_sql VARCHAR2(4000);
BEGIN
    v_sql := 'SELECT v.*, d.DISASTER_NAME, s.SHELTER_NAME, u.FULL_NAME AS REGISTERED_BY_NAME ' ||
             'FROM VICTIMS v ' ||
             'LEFT JOIN DISASTERS d ON v.DISASTER_ID = d.DISASTER_ID ' ||
             'LEFT JOIN SHELTERS s ON v.SHELTER_ID = s.SHELTER_ID ' ||
             'LEFT JOIN USERS u ON v.REGISTERED_BY = u.USER_ID WHERE 1=1';
    
    IF p_status IS NOT NULL AND p_status != 'ALL' THEN
        v_sql := v_sql || ' AND v.STATUS = ''' || p_status || '''';
    END IF;

    IF p_sort = 'LATEST' THEN
        v_sql := v_sql || ' ORDER BY v.REGISTERED_AT DESC';
    ELSIF p_sort = 'OLDEST' THEN
        v_sql := v_sql || ' ORDER BY v.REGISTERED_AT ASC';
    ELSIF p_sort = 'AGE' THEN
        v_sql := v_sql || ' ORDER BY v.AGE ASC';
    ELSE
        v_sql := v_sql || ' ORDER BY v.REGISTERED_AT DESC';
    END IF;

    OPEN v_cursor FOR v_sql;
    RETURN v_cursor;
END GET_FILTERED_VICTIMS;");

        // 2. Volunteers
        commands.Add(@"
CREATE OR REPLACE FUNCTION GET_FILTERED_VOLUNTEERS(p_status IN VARCHAR2, p_sort IN VARCHAR2) RETURN SYS_REFCURSOR IS
    v_cursor SYS_REFCURSOR;
    v_sql VARCHAR2(4000);
BEGIN
    v_sql := 'SELECT v.*, u.USERNAME, u.EMAIL ' ||
             'FROM VOLUNTEERS v ' ||
             'JOIN USERS u ON v.USER_ID = u.USER_ID WHERE 1=1';
    
    IF p_status IS NOT NULL AND p_status != 'ALL' THEN
        v_sql := v_sql || ' AND v.AVAILABILITY_STATUS = ''' || p_status || '''';
    END IF;

    IF p_sort = 'LATEST' THEN
        v_sql := v_sql || ' ORDER BY v.CREATED_AT DESC';
    ELSIF p_sort = 'HOURS' THEN
        v_sql := v_sql || ' ORDER BY v.TOTAL_HOURS_SERVED DESC';
    ELSE
        v_sql := v_sql || ' ORDER BY v.CREATED_AT DESC';
    END IF;

    OPEN v_cursor FOR v_sql;
    RETURN v_cursor;
END GET_FILTERED_VOLUNTEERS;");

        // 3. Shelters
        commands.Add(@"
CREATE OR REPLACE FUNCTION GET_FILTERED_SHELTERS(p_status IN VARCHAR2, p_sort IN VARCHAR2) RETURN SYS_REFCURSOR IS
    v_cursor SYS_REFCURSOR;
    v_sql VARCHAR2(4000);
BEGIN
    v_sql := 'SELECT s.*, c.MAX_CAPACITY, c.CURRENT_OCCUPIED, c.AVAILABLE_BEDS, c.RESERVED_SPOTS, c.HAS_OVERFLOW, c.OVERFLOW_LOCATION ' ||
             'FROM SHELTERS s ' ||
             'LEFT JOIN SHELTER_CAPACITY c ON s.SHELTER_ID = c.SHELTER_ID WHERE 1=1';
    
    IF p_status = 'OPEN' THEN
        v_sql := v_sql || ' AND s.IS_ACTIVE = 1';
    ELSIF p_status = 'CLOSED' THEN
        v_sql := v_sql || ' AND s.IS_ACTIVE = 0';
    ELSIF p_status = 'FULL' THEN
        v_sql := v_sql || ' AND c.AVAILABLE_BEDS <= 0';
    END IF;

    IF p_sort = 'CAPACITY' THEN
        v_sql := v_sql || ' ORDER BY c.MAX_CAPACITY DESC';
    ELSIF p_sort = 'AVAILABLE' THEN
        v_sql := v_sql || ' ORDER BY c.AVAILABLE_BEDS DESC';
    ELSE
        v_sql := v_sql || ' ORDER BY s.OPENED_DATE DESC';
    END IF;

    OPEN v_cursor FOR v_sql;
    RETURN v_cursor;
END GET_FILTERED_SHELTERS;");

        // 4. Resources
        commands.Add(@"
CREATE OR REPLACE FUNCTION GET_FILTERED_RESOURCES(p_category IN VARCHAR2, p_sort IN VARCHAR2) RETURN SYS_REFCURSOR IS
    v_cursor SYS_REFCURSOR;
    v_sql VARCHAR2(4000);
BEGIN
    v_sql := 'SELECT r.*, c.CATEGORY_NAME, c.UNIT, c.ICON, c.CRITICAL_THRESHOLD ' ||
             'FROM RESOURCES r ' ||
             'JOIN RESOURCE_CATEGORIES c ON r.CATEGORY_ID = c.CATEGORY_ID WHERE 1=1';
    
    IF p_category IS NOT NULL AND p_category != 'ALL' THEN
        v_sql := v_sql || ' AND c.CATEGORY_NAME = ''' || p_category || '''';
    END IF;

    IF p_sort = 'EXPIRY' THEN
        v_sql := v_sql || ' ORDER BY r.EXPIRY_DATE ASC';
    ELSIF p_sort = 'STOCK' THEN
        v_sql := v_sql || ' ORDER BY r.TOTAL_QUANTITY DESC';
    ELSE
        v_sql := v_sql || ' ORDER BY r.LAST_UPDATED DESC';
    END IF;

    OPEN v_cursor FOR v_sql;
    RETURN v_cursor;
END GET_FILTERED_RESOURCES;");

        // 5. Reports
        commands.Add(@"
CREATE OR REPLACE FUNCTION GET_FILTERED_REPORTS(p_status IN VARCHAR2, p_sort IN VARCHAR2) RETURN SYS_REFCURSOR IS
    v_cursor SYS_REFCURSOR;
    v_sql VARCHAR2(4000);
BEGIN
    v_sql := 'SELECT r.*, d.DISASTER_NAME, u.FULL_NAME AS ASSIGNED_TO_NAME ' ||
             'FROM INCIDENT_REPORTS r ' ||
             'LEFT JOIN DISASTERS d ON r.DISASTER_ID = d.DISASTER_ID ' ||
             'LEFT JOIN USERS u ON r.ASSIGNED_TO = u.USER_ID WHERE 1=1';
    
    IF p_status IS NOT NULL AND p_status != 'ALL' THEN
        v_sql := v_sql || ' AND r.STATUS = ''' || p_status || '''';
    END IF;

    IF p_sort = 'LATEST' THEN
        v_sql := v_sql || ' ORDER BY r.REPORTED_AT DESC';
    ELSIF p_sort = 'OLDEST' THEN
        v_sql := v_sql || ' ORDER BY r.REPORTED_AT ASC';
    ELSE
        v_sql := v_sql || ' ORDER BY r.REPORTED_AT DESC';
    END IF;

    OPEN v_cursor FOR v_sql;
    RETURN v_cursor;
END GET_FILTERED_REPORTS;");

        using (var conn = new OracleConnection(connStr))
        {
            conn.Open();
            foreach (var ddl in commands)
            {
                using (var cmd = new OracleCommand(ddl, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("All 5 Functions created successfully.");
        }
    }
}
