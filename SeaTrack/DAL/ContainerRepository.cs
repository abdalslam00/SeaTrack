using System;
using System.Data;
using System.Data.SqlClient;

namespace SeaTrack.DAL
{
    public class ContainerRepository
    {
        public static DataTable GetAllContainers()
        {
            string query = @"SELECT c.*, cs.size_name, cs.size_feet 
                           FROM Containers c 
                           JOIN ContainerSizes cs ON c.size_id = cs.size_id 
                           ORDER BY c.container_code";
            return DatabaseHelper.ExecuteQuery(query);
        }

        public static DataTable GetAvailableContainers()
        {
            string query = @"SELECT c.*, cs.size_name, cs.size_feet 
                           FROM Containers c 
                           JOIN ContainerSizes cs ON c.size_id = cs.size_id 
                           WHERE c.is_available = 1 
                           ORDER BY c.container_code";
            return DatabaseHelper.ExecuteQuery(query);
        }

        public static DataTable GetContainersByType(int type)
        {
            string query = @"SELECT c.*, cs.size_name 
                           FROM Containers c 
                           JOIN ContainerSizes cs ON c.size_id = cs.size_id 
                           WHERE c.type = @type AND c.is_available = 1";
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@type", type) };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }
   

    // دوال إضافية للحاويات
    public static DataTable GetAllContainersWithDetails()
    {
        string query = @"
            SELECT c.*, cs.size_name, 
                   CASE WHEN c.type = 1 THEN 'خاصة' ELSE 'عامة' END as type_name
            FROM Containers c
            INNER JOIN ContainerSizes cs ON c.size_id = cs.size_id
            ORDER BY c.created_at DESC";
        
        return DatabaseHelper.ExecuteQuery(query);
    }

    public static DataTable GetAllContainerSizes()
    {
        string query = "SELECT * FROM ContainerSizes ORDER BY size_feet DESC";
        return DatabaseHelper.ExecuteQuery(query);
    }

    public static DataRow GetContainerById(int containerId)
    {
        string query = "SELECT * FROM Containers WHERE container_id = @container_id";
        SqlParameter[] parameters = {
            new SqlParameter("@container_id", containerId)
        };
        
        DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public static void AddContainer(string containerCode, int type, int sizeId, decimal baseWeight, decimal maxWeight, bool isAvailable)
    {
        string query = @"
            INSERT INTO Containers (container_code, type, size_id, base_weight_kg, max_weight_kg, is_available, created_at)
            VALUES (@container_code, @type, @size_id, @base_weight, @max_weight, @is_available, GETDATE())";
        
        SqlParameter[] parameters = {
            new SqlParameter("@container_code", containerCode),
            new SqlParameter("@type", type),
            new SqlParameter("@size_id", sizeId),
            new SqlParameter("@base_weight", baseWeight),
            new SqlParameter("@max_weight", maxWeight),
            new SqlParameter("@is_available", isAvailable)
        };
        
        DatabaseHelper.ExecuteNonQuery(query, parameters);
    }

    public static void UpdateContainer(int containerId, string containerCode, int type, int sizeId, decimal baseWeight, decimal maxWeight, bool isAvailable)
    {
        string query = @"
            UPDATE Containers 
            SET container_code = @container_code, 
                type = @type, 
                size_id = @size_id, 
                base_weight_kg = @base_weight, 
                max_weight_kg = @max_weight, 
                is_available = @is_available,
                updated_at = GETDATE()
            WHERE container_id = @container_id";
        
        SqlParameter[] parameters = {
            new SqlParameter("@container_id", containerId),
            new SqlParameter("@container_code", containerCode),
            new SqlParameter("@type", type),
            new SqlParameter("@size_id", sizeId),
            new SqlParameter("@base_weight", baseWeight),
            new SqlParameter("@max_weight", maxWeight),
            new SqlParameter("@is_available", isAvailable)
        };
        
        DatabaseHelper.ExecuteNonQuery(query, parameters);
    }

    public static void DeleteContainer(int containerId)
    {
        string query = "DELETE FROM Containers WHERE container_id = @container_id";
        SqlParameter[] parameters = {
            new SqlParameter("@container_id", containerId)
        };
        
        DatabaseHelper.ExecuteNonQuery(query, parameters);
    }

    // دوال خاصة بنظام الوزن الديناميكي والرحلات
    public static DataTable GetContainersByTrip(int tripId)
    {
        string query = @"SELECT tc.*, c.container_code, c.base_weight_kg, c.max_weight_kg,
                               cs.size_name, cs.size_feet
                        FROM Trip_Containers tc
                        INNER JOIN Containers c ON tc.container_id = c.container_id
                        INNER JOIN ContainerSizes cs ON c.size_id = cs.size_id
                        WHERE tc.trip_id = @trip_id
                        ORDER BY c.container_code";
        SqlParameter[] parameters = { new SqlParameter("@trip_id", tripId) };
        return DatabaseHelper.ExecuteQuery(query, parameters);
    }

        public static bool AssignContainerToTrip(int tripId, int containerId)
        {
            // تم حذف gross_weight من جملة الإدخال
            string query = @"INSERT INTO Trip_Containers (trip_id, container_id, cargo_weight_kg, assigned_at)
                    VALUES (@trip_id, @container_id, 0, GETDATE())";

            SqlParameter[] parameters = {
        new SqlParameter("@trip_id", tripId),
        new SqlParameter("@container_id", containerId)
    };

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool UpdateCargoWeight(int tripId, int containerId, decimal cargoWeight)
    {
            string query = @"UPDATE Trip_Containers 
                    SET cargo_weight_kg = @cargo_weight,
                        assigned_at = GETDATE() -- أو updated_at لو موجود
                    WHERE trip_id = @trip_id AND container_id = @container_id";

            SqlParameter[] parameters = {
        new SqlParameter("@trip_id", tripId),
        new SqlParameter("@container_id", containerId),
        new SqlParameter("@cargo_weight", cargoWeight)
    };

            try
        {
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static decimal GetContainerGrossWeight(int tripId, int containerId)
    {
        string query = @"SELECT gross_weight FROM Trip_Containers 
                        WHERE trip_id = @trip_id AND container_id = @container_id";
        SqlParameter[] parameters = {
            new SqlParameter("@trip_id", tripId),
            new SqlParameter("@container_id", containerId)
        };
        
        DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
        if (dt.Rows.Count > 0)
        {
            return Convert.ToDecimal(dt.Rows[0]["gross_weight"]);
        }
        return 0;
    }

    public static DataTable GetAvailableContainersForTrip(int tripId)
    {
        string query = @"SELECT tc.*, c.container_code, c.base_weight_kg, c.max_weight_kg,
                               cs.size_name, cs.size_feet,
                               (c.max_weight_kg - tc.gross_weight) as available_capacity
                        FROM Trip_Containers tc
                        INNER JOIN Containers c ON tc.container_id = c.container_id
                        INNER JOIN ContainerSizes cs ON c.size_id = cs.size_id
                        WHERE tc.trip_id = @trip_id 
                        AND tc.gross_weight < c.max_weight_kg
                        ORDER BY available_capacity DESC";
        SqlParameter[] parameters = { new SqlParameter("@trip_id", tripId) };
        return DatabaseHelper.ExecuteQuery(query, parameters);
    }
  }
}
