using System;
using System.Data;
using System.Data.SqlClient;

namespace SeaTrack.DAL
{
    /// <summary>
    /// مستودع الشحنات - للتعامل مع جدول Shipments
    /// </summary>
    public class ShipmentRepository
    {
        /// <summary>
        /// إنشاء شحنة جديدة
        /// </summary>
        public static int CreateShipment(string shipmentCode, string qrCode, string description,
                                        decimal weightKg, decimal? lengthCm, decimal? widthCm,
                                        decimal? heightCm, int shippingTypeId, int customerId,
                                        string departureCountry, string arrivalCountry,
                                        int? bookingRequestId)
        {
            string query = @"INSERT INTO Shipments (shipment_code, qr_code, description, weight_kg, 
                                                   length_cm, width_cm, height_cm, shipping_type_id, 
                                                   customer_id, departure_country, arrival_country, 
                                                   booking_request_id, status_id, created_at)
                           VALUES (@shipmentCode, @qrCode, @description, @weightKg, @lengthCm, 
                                   @widthCm, @heightCm, @shippingTypeId, @customerId, 
                                   @departureCountry, @arrivalCountry, @bookingRequestId, 1, GETDATE());
                           SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@shipmentCode", shipmentCode),
                new SqlParameter("@qrCode", qrCode),
                new SqlParameter("@description", description),
                new SqlParameter("@weightKg", weightKg),
                new SqlParameter("@lengthCm", lengthCm ?? (object)DBNull.Value),
                new SqlParameter("@widthCm", widthCm ?? (object)DBNull.Value),
                new SqlParameter("@heightCm", heightCm ?? (object)DBNull.Value),
                new SqlParameter("@shippingTypeId", shippingTypeId),
                new SqlParameter("@customerId", customerId),
                new SqlParameter("@departureCountry", departureCountry ?? (object)DBNull.Value),
                new SqlParameter("@arrivalCountry", arrivalCountry ?? (object)DBNull.Value),
                new SqlParameter("@bookingRequestId", bookingRequestId ?? (object)DBNull.Value)
            };

            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// الحصول على شحنة بواسطة QR Code
        /// </summary>
        public static DataTable GetShipmentByQRCode(string qrCode)
        {
            string query = "SELECT * FROM vw_Shipment_Details_Enhanced WHERE qr_code = @qrCode";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@qrCode", qrCode)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// الحصول على شحنات العميل
        /// </summary>
        public static DataTable GetCustomerShipments(int customerId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CustomerId", customerId)
            };

            return DatabaseHelper.ExecuteStoredProcedure("sp_GetCustomerShipments", parameters);
        }

        /// <summary>
        /// الحصول على شحنة بواسطة المعرف
        /// </summary>

        /// <summary>
        /// تحديث حالة الشحنة
        /// </summary>
        public static bool UpdateShipmentStatus(int shipmentId, int newStatusId, int? userId = null)
        {
            string query = @"UPDATE Shipments 
                           SET status_id = @newStatusId,
                               updated_at = GETDATE()";

            if (userId.HasValue && newStatusId == 2) // Scanned
            {
                query += ", scanned_by = @userId, scanned_at = GETDATE()";
            }
            else if (userId.HasValue && newStatusId == 4) // Delivered
            {
                query += ", delivered_by = @userId, delivered_at = GETDATE()";
            }

            query += " WHERE shipment_id = @shipmentId";

            SqlParameter[] parameters = userId.HasValue
                ? new SqlParameter[]
                {
                    new SqlParameter("@shipmentId", shipmentId),
                    new SqlParameter("@newStatusId", newStatusId),
                    new SqlParameter("@userId", userId.Value)
                }
                : new SqlParameter[]
                {
                    new SqlParameter("@shipmentId", shipmentId),
                    new SqlParameter("@newStatusId", newStatusId)
                };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// تخصيص شحنة لحاوية
        /// </summary>
        public static bool AssignShipmentToContainer(int shipmentId, int tripContainerId, int scannedBy)
        {
            string query = @"UPDATE Shipments 
                           SET trip_container_id = @tripContainerId,
                               status_id = 2,
                               scanned_by = @scannedBy,
                               scanned_at = GETDATE(),
                               updated_at = GETDATE()
                           WHERE shipment_id = @shipmentId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@shipmentId", shipmentId),
                new SqlParameter("@tripContainerId", tripContainerId),
                new SqlParameter("@scannedBy", scannedBy)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// الحصول على الشحنات حسب الحالة
        /// </summary>
        public static DataTable GetShipmentsByStatus(int statusId)
        {
            string query = @"SELECT * FROM vw_Shipment_Details_Enhanced 
                           WHERE status_id = @statusId 
                           ORDER BY created_at DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@statusId", statusId)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// الحصول على الشحنات الغير ممسوحة
        /// </summary>
        public static DataTable GetUnscannedShipments()
        {
            string query = @"SELECT * FROM vw_Shipment_Details_Enhanced 
                           WHERE status_id = 1 
                           ORDER BY created_at ASC";

            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// الحصول على شحنات حاوية معينة
        /// </summary>
        public static DataTable GetContainerShipments(int tripContainerId)
        {
            string query = @"SELECT * FROM vw_Shipment_Details_Enhanced 
                           WHERE trip_container_id = @tripContainerId 
                           ORDER BY created_at DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tripContainerId", tripContainerId)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// تحديث بيانات الشحنة
        /// </summary>
        public static bool UpdateShipment(int shipmentId, string description, decimal weightKg,
                                         decimal? lengthCm, decimal? widthCm, decimal? heightCm)
        {
            string query = @"UPDATE Shipments 
                           SET description = @description,
                               weight_kg = @weightKg,
                               length_cm = @lengthCm,
                               width_cm = @widthCm,
                               height_cm = @heightCm,
                               updated_at = GETDATE()
                           WHERE shipment_id = @shipmentId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@shipmentId", shipmentId),
                new SqlParameter("@description", description),
                new SqlParameter("@weightKg", weightKg),
                new SqlParameter("@lengthCm", lengthCm ?? (object)DBNull.Value),
                new SqlParameter("@widthCm", widthCm ?? (object)DBNull.Value),
                new SqlParameter("@heightCm", heightCm ?? (object)DBNull.Value)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// حذف شحنة
        /// </summary>
        public static bool DeleteShipment(int shipmentId)
        {
            string query = "DELETE FROM Shipments WHERE shipment_id = @shipmentId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@shipmentId", shipmentId)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// التحقق من وجود رمز الشحنة
        /// </summary>
        public static bool ShipmentCodeExists(string shipmentCode)
        {
            string query = "SELECT COUNT(*) FROM Shipments WHERE shipment_code = @shipmentCode";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@shipmentCode", shipmentCode)
            };

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            return count > 0;
        }

        /// <summary>
        /// الحصول على الشحنات الجاهزة للتسليم
        /// </summary>
        public static DataTable GetShipmentsReadyForDelivery()
        {
            string query = @"SELECT * FROM vw_Shipment_Details_Enhanced 
                           WHERE status_id = 3 
                           AND trip_status = 4
                           ORDER BY created_at ASC";

            return DatabaseHelper.ExecuteQuery(query);
        }



        public static DataTable GetUserShipments(int userId)
        {
            // تصحيح: tracking_number -> shipment_code
            // تصحيح: weight -> weight_kg
            // تصحيح: created_date -> created_at
            // تصحيح: bookings -> BookingRequests
            string query = @"
    SELECT s.shipment_id, 
           s.shipment_code as tracking_number, 
           t.trip_code,
           s.weight_kg as weight, 
           s.created_at as created_date, 
           s.status_id, 
           ss.status_name
    FROM Shipments s
    INNER JOIN BookingRequests b ON s.booking_request_id = b.booking_id
    INNER JOIN Trips t ON b.trip_id = t.trip_id
    INNER JOIN ShipmentStatuses ss ON s.status_id = ss.status_id
    WHERE b.customer_id = @user_id
    ORDER BY s.created_at DESC";

            SqlParameter[] parameters = {
        new SqlParameter("@user_id", userId)
    };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        public static DataTable GetShipmentById(int shipmentId)
        {
            string query = "SELECT * FROM Shipments WHERE shipment_id = @shipment_id";
            SqlParameter[] parameters = {
            new SqlParameter("@shipment_id", shipmentId)
        };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        public static DataTable GetShipmentByTracking(string trackingNumber)
        {
            string query = @"
            SELECT s.*, b.booking_code, t.trip_code, u.full_name AS customer_name
            FROM Shipments s
            INNER JOIN bookings b ON s.booking_id = b.booking_id
            INNER JOIN trips t ON b.trip_id = t.trip_id
            INNER JOIN users u ON b.user_id = u.user_id
            WHERE s.tracking_number = @tracking_number";

            SqlParameter[] parameters = {
            new SqlParameter("@tracking_number", trackingNumber)
        };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        public static bool UpdateShipmentStatus(int shipmentId, int statusId)
        {
            string query = "UPDATE Shipments SET status_id = @status_id WHERE shipment_id = @shipment_id";
            SqlParameter[] parameters = {
            new SqlParameter("@shipment_id", shipmentId),
            new SqlParameter("@status_id", statusId)
        };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
