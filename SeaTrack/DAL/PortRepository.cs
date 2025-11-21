using System;
using System.Data;
using System.Data.SqlClient;

namespace SeaTrack.DAL
{
    public class PortRepository
    {
        public static DataTable GetAllPorts()
        {
            return DatabaseHelper.ExecuteQuery("SELECT * FROM Ports WHERE is_active = 1 ORDER BY port_name");
        }

        public static DataTable GetPortById(int portId)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@portId", portId) };
            return DatabaseHelper.ExecuteQuery("SELECT * FROM Ports WHERE port_id = @portId", parameters);
        }
        public static bool DeletePort(int portId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@portId", portId)
            };

            int rows = DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM Ports WHERE port_id = @portId",
                parameters
            );

            return rows > 0;
        }
        public static bool CreatePort(string portName, string country, string portCode,  string city)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@portName", portName),
        new SqlParameter("@portCode", portCode),
        new SqlParameter("@country", country),
        new SqlParameter("@city", (object)city ?? DBNull.Value)
            };

            int rows = DatabaseHelper.ExecuteNonQuery(@"
        INSERT INTO Ports (port_name, port_code, country, city, is_active, created_at)
        VALUES (@portName, @portCode, @country, @city, 1, GETDATE())
    ", parameters);

            return rows > 0;
        }

        public static bool ChangePortStatus(int portId, bool isActive)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@portId", portId),
        new SqlParameter("@isActive", isActive)
            };

            int rows = DatabaseHelper.ExecuteNonQuery(@"
        UPDATE Ports 
        SET is_active = @isActive, updated_at = GETDATE()
        WHERE port_id = @portId
    ", parameters);

            return rows > 0;
        }

    }
}
