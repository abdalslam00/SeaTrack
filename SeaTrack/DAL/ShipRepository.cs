using System;
using System.Data;
using System.Data.SqlClient;

namespace SeaTrack.DAL
{
    public class ShipRepository
    {
        public static DataTable GetAllShips()
        {
            return DatabaseHelper.ExecuteQuery("SELECT * FROM Ships WHERE is_active = 1 ORDER BY ship_name");
        }

        public static DataTable GetShipById(int shipId)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@shipId", shipId) };
            return DatabaseHelper.ExecuteQuery("SELECT * FROM Ships WHERE ship_id = @shipId", parameters);
        }
        public static bool ChangeShipStatus(int shipId, bool isActive)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@shipId", shipId),
        new SqlParameter("@isActive", isActive)
            };

            int rows = DatabaseHelper.ExecuteNonQuery(@"
        UPDATE Ships 
        SET is_active = @isActive, updated_at = GETDATE()
        WHERE ship_id = @shipId
    ", parameters);

            return rows > 0;
        }


        public static bool DeleteShip(int shipId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@shipId", shipId)
            };

            int rows = DatabaseHelper.ExecuteNonQuery(@"
        UPDATE Ships 
        SET is_active = 0, updated_at = GETDATE()
        WHERE ship_id = @shipId
    ", parameters);

            return rows > 0;
        }


        public static bool UpdateShip(int shipId, string shipName, string shipCode, int capacityContainers,
                              decimal? maxWeightTons, string manufacturer, int? yearBuilt)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@shipId", shipId),
        new SqlParameter("@shipName", shipName),
        new SqlParameter("@shipCode", shipCode),
        new SqlParameter("@capacityContainers", capacityContainers),
        new SqlParameter("@maxWeightTons", (object)maxWeightTons ?? DBNull.Value),
        new SqlParameter("@manufacturer", (object)manufacturer ?? DBNull.Value),
        new SqlParameter("@yearBuilt", (object)yearBuilt ?? DBNull.Value)
            };

            int rows = DatabaseHelper.ExecuteNonQuery(@"
        UPDATE Ships SET 
            ship_name = @shipName,
            ship_code = @shipCode,
            capacity_containers = @capacityContainers,
            max_weight_tons = @maxWeightTons,
            manufacturer = @manufacturer,
            year_built = @yearBuilt,
            updated_at = GETDATE()
        WHERE ship_id = @shipId
    ", parameters);

            return rows > 0;
        }


        public static bool CreateShip(string shipName, string shipCode, int capacityContainers,
                              decimal? maxWeightTons, string manufacturer, int? yearBuilt)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@shipName", shipName),
        new SqlParameter("@shipCode", shipCode),
        new SqlParameter("@capacityContainers", capacityContainers),
        new SqlParameter("@maxWeightTons", (object)maxWeightTons ?? DBNull.Value),
        new SqlParameter("@manufacturer", (object)manufacturer ?? DBNull.Value),
        new SqlParameter("@yearBuilt", (object)yearBuilt ?? DBNull.Value)
            };

            int rows = DatabaseHelper.ExecuteNonQuery(@"
        INSERT INTO Ships 
        (ship_name, ship_code, capacity_containers, max_weight_tons, manufacturer, year_built, is_active, created_at)
        VALUES 
        (@shipName, @shipCode, @capacityContainers, @maxWeightTons, @manufacturer, @yearBuilt, 1, GETDATE())
    ", parameters);

            return rows > 0;
        }

    }
}
