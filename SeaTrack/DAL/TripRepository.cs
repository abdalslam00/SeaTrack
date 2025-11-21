using System;
using System.Data;
using System.Data.SqlClient;

namespace SeaTrack.DAL
{
    /// <summary>
    /// مستودع الرحلات - للتعامل مع جدول Trips
    /// </summary>
    public class TripRepository
    {
        /// <summary>
        /// إنشاء رحلة جديدة
        /// </summary>
        public static int CreateTrip(string tripCode, int shipId, int departurePortId,
                                     int arrivalPortId, DateTime departureDate,
                                     DateTime expectedArrivalDate, int createdBy)
        {
            string query = @"INSERT INTO Trips (trip_code, ship_id, departure_port_id, arrival_port_id, 
                                              departure_date, expected_arrival_date, status_id, created_by, created_at)
                           VALUES (@tripCode, @shipId, @departurePortId, @arrivalPortId, 
                                   @departureDate, @expectedArrivalDate, 1, @createdBy, GETDATE());
                           SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tripCode", tripCode),
                new SqlParameter("@shipId", shipId),
                new SqlParameter("@departurePortId", departurePortId),
                new SqlParameter("@arrivalPortId", arrivalPortId),
                new SqlParameter("@departureDate", departureDate),
                new SqlParameter("@expectedArrivalDate", expectedArrivalDate),
                new SqlParameter("@createdBy", createdBy)
            };

            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// الحصول على جميع الرحلات
        /// </summary>
        public static DataTable GetAllTrips()
        {
            string query = "SELECT * FROM vw_Trip_Details ORDER BY departure_date DESC";
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// الحصول على رحلة بواسطة المعرف
        /// </summary>
        public static DataTable GetTripById(int tripId)
        {
            string query = "SELECT * FROM vw_Trip_Details WHERE trip_id = @tripId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tripId", tripId)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// الحصول على الرحلات حسب الحالة
        /// </summary>
        public static DataTable GetTripsByStatus(int statusId)
        {
            string query = @"SELECT * FROM vw_Trip_Details 
                           WHERE trip_id IN (SELECT trip_id FROM Trips WHERE status_id = @statusId)
                           ORDER BY departure_date DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@statusId", statusId)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }



        /// <summary>
        /// تحديث حالة الرحلة
        /// </summary>
        public static bool UpdateTripStatus(int tripId, int newStatus)
        {
            string query = @"UPDATE Trips 
                           SET status_id = @newStatus,
                               updated_at = GETDATE()
                           WHERE trip_id = @tripId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tripId", tripId),
                new SqlParameter("@newStatus", newStatus)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// تحديث تاريخ الوصول الفعلي
        /// </summary>
        public static bool UpdateActualArrivalDate(int tripId, DateTime actualArrivalDate)
        {
            string query = @"UPDATE Trips 
                           SET actual_arrival_date = @actualArrivalDate,
                               status_id = 4,
                               updated_at = GETDATE()
                           WHERE trip_id = @tripId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tripId", tripId),
                new SqlParameter("@actualArrivalDate", actualArrivalDate)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// الحصول على إحصائيات الرحلة
        /// </summary>
        public static DataTable GetTripStatistics(int tripId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TripId", tripId)
            };

            return DatabaseHelper.ExecuteStoredProcedure("sp_GetTripStatistics", parameters);
        }

        /// <summary>
        /// الحصول على الحاويات المرتبطة بالرحلة
        /// </summary>
        public static DataTable GetTripContainers(int tripId)
        {
            string query = @"SELECT tc.trip_container_id, c.container_code, c.type, 
                                   cs.size_name, c.base_weight_kg, tc.cargo_weight_kg,
                                   (c.base_weight_kg + tc.cargo_weight_kg) AS gross_weight,
                                   c.max_weight_kg,
                                   (SELECT COUNT(*) FROM Shipments WHERE trip_container_id = tc.trip_container_id) AS shipment_count
                           FROM Trip_Containers tc
                           JOIN Containers c ON tc.container_id = c.container_id
                           JOIN ContainerSizes cs ON c.size_id = cs.size_id
                           WHERE tc.trip_id = @tripId
                           ORDER BY c.container_code";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tripId", tripId)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// إضافة حاوية إلى رحلة
        /// </summary>
        public static int AddContainerToTrip(int tripId, int containerId)
        {
            string query = @"INSERT INTO Trip_Containers (trip_id, container_id, cargo_weight_kg, assigned_at)
                           VALUES (@tripId, @containerId, 0, GETDATE());
                           SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tripId", tripId),
                new SqlParameter("@containerId", containerId)
            };

            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// الحصول على الحاويات المتاحة لرحلة معينة
        /// </summary>
        public static DataTable GetAvailableContainersForTrip(int tripId)
        {
            string query = @"SELECT c.container_id, c.container_code, c.type, cs.size_name, 
                                   c.base_weight_kg, c.max_weight_kg
                           FROM Containers c
                           JOIN ContainerSizes cs ON c.size_id = cs.size_id
                           WHERE c.is_available = 1
                           AND c.container_id NOT IN (
                               SELECT container_id FROM Trip_Containers WHERE trip_id = @tripId
                           )
                           ORDER BY c.container_code";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tripId", tripId)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// حذف رحلة
        /// </summary>
        public static bool DeleteTrip(int tripId)
        {
            string query = "DELETE FROM Trips WHERE trip_id = @tripId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tripId", tripId)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// التحقق من وجود رمز الرحلة
        /// </summary>
        public static bool TripCodeExists(string tripCode)
        {
            string query = "SELECT COUNT(*) FROM Trips WHERE trip_code = @tripCode";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tripCode", tripCode)
            };

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            return count > 0;
        }



        // دوال إضافية
        public static DataTable GetAvailableTrips()
        {
            string query = @"
            SELECT trip_id, 
                   CONCAT(trip_code, ' - ', dp.port_name, ' إلى ', ap.port_name, ' (', CONVERT(VARCHAR, departure_date, 23), ')') AS trip_info
            FROM trips t
            INNER JOIN ports dp ON t.departure_port_id = dp.port_id
            INNER JOIN ports ap ON t.arrival_port_id = ap.port_id
            WHERE t.status_id IN (1, 2)
            ORDER BY t.departure_date";
            return DatabaseHelper.ExecuteQuery(query);
        }

        public static DataTable GetActiveTrips()
        {
            string query = @"
            SELECT t.trip_id, t.trip_code, s.ship_name,
                   dp.port_name AS departure_port, ap.port_name AS arrival_port,
                   t.departure_date, t.expected_arrival_date,
                   t.status_id, ts.status_name
            FROM trips t
            INNER JOIN ships s ON t.ship_id = s.ship_id
            INNER JOIN ports dp ON t.departure_port_id = dp.port_id
            INNER JOIN ports ap ON t.arrival_port_id = ap.port_id
            INNER JOIN TripStatuses ts ON t.status_id = ts.status_id
            WHERE t.status_id IN (1, 2, 3)
            ORDER BY t.departure_date DESC";
            return DatabaseHelper.ExecuteQuery(query);
        }

        public static DataTable GetStatistics()
        {
            string query = @"
            SELECT 
                (SELECT COUNT(*) FROM trips) AS total_trips,
                (SELECT COUNT(*) FROM shipments) AS total_shipments,
                (SELECT COUNT(*) FROM users WHERE role_id = 2) AS total_customers,
                (SELECT COUNT(*) FROM bookings WHERE status_id = 1) AS pending_bookings";
            return DatabaseHelper.ExecuteQuery(query);
        }

        public static DataTable GetRecentTrips(int count)
        {
            // تصحيح: created_date -> created_at
            string query = $@"
    SELECT TOP {count} t.trip_code, s.ship_name,
            dp.port_name AS departure_port, ap.port_name AS arrival_port,
            t.departure_date
    FROM Trips t
    INNER JOIN Ships s ON t.ship_id = s.ship_id
    INNER JOIN Ports dp ON t.departure_port_id = dp.port_id
    INNER JOIN Ports ap ON t.arrival_port_id = ap.port_id
    ORDER BY t.created_at DESC"; // تم التصحيح هنا

            return DatabaseHelper.ExecuteQuery(query);
        }
    }
}
