using System;
using System.Data;
using System.Data.SqlClient;

namespace SeaTrack.DAL
{
    /// <summary>
    /// مستودع الحجوزات - للتعامل مع جدول BookingRequests
    /// </summary>
    public class BookingRepository
    {

        public static int CreateBookingRequest(int customerId, int tripId, int containerTypeId,
                                              int containerSizeId, decimal expectedWeightKg,
                                              string cargoType, string notes)
        {

            string query = @"INSERT INTO BookingRequests 
                    (customer_id, trip_id, container_type, container_size, 
                     expected_weight_kg, cargo_type, notes, status, created_at)
                   VALUES 
                    (@customer_id, @trip_id, @container_type, @container_size, 
                     @expected_weight_kg, @cargo_type, @notes, 1, GETDATE());
                   SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@customer_id", customerId),
        new SqlParameter("@trip_id", tripId),
        new SqlParameter("@container_type", containerTypeId),
        new SqlParameter("@container_size", containerSizeId), // يفترض أن هذا هو size_id من جدول ContainerSizes
        new SqlParameter("@expected_weight_kg", expectedWeightKg),
        // التعامل مع القيم النصية التي قد تكون فارغة (Null Handling)
        new SqlParameter("@cargo_type", (object)cargoType ?? DBNull.Value),
        new SqlParameter("@notes", (object)notes ?? DBNull.Value)
            };

            try
            {
                object result = DatabaseHelper.ExecuteScalar(query, parameters);

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                return 0;
            }
            catch (Exception ex)
            {
                // يفضل تسجيل الخطأ هنا إذا كان لديك LogHelper
                // LogHelper.LogError("CreateBookingRequest", ex);
                throw ex; // أو أعد رمي الخطأ ليظهر في الواجهة
            }
        }
        /// <summary>
        /// الحصول على حجوزات العميل
        /// </summary>
        public static DataTable GetCustomerBookings(int customerId)
        {
            string query = @"SELECT br.booking_id, br.customer_id, br.trip_id, br.container_type, 
                                   br.container_size, br.expected_weight_kg, br.status,
                                   br.created_at, br.updated_at,
                                   t.trip_code, dp.port_name AS departure_port, 
                                   ap.port_name AS arrival_port, t.departure_date,
                                   c.container_code, tc.trip_container_id
                           FROM BookingRequests br
                           JOIN Trips t ON br.trip_id = t.trip_id
                           JOIN Ports dp ON t.departure_port_id = dp.port_id
                           JOIN Ports ap ON t.arrival_port_id = ap.port_id
                           LEFT JOIN Trip_Containers tc ON br.trip_container_id = tc.trip_container_id
                           LEFT JOIN Containers c ON tc.container_id = c.container_id
                           WHERE br.customer_id = @customerId
                           ORDER BY br.created_at DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@customerId", customerId)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }




        /// <summary>
        /// الموافقة على طلب حجز
        /// </summary>
        public static bool ApproveBooking(int bookingId, int tripContainerId)
        {
            string query = @"UPDATE BookingRequests 
                           SET status_id= 2,
                               trip_container_id = @tripContainerId,
                               updated_at = GETDATE()
                           WHERE booking_id = @bookingId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@bookingId", bookingId),
                new SqlParameter("@tripContainerId", tripContainerId)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// رفض طلب حجز
        /// </summary>

        /// <summary>
        /// الحصول على حجز بواسطة المعرف
        /// </summary>
        public static DataTable GetBookingById(int bookingId)
        {
            string query = @"SELECT br.*, u.full_name AS customer_name, t.trip_code,
                                   dp.port_name AS departure_port, ap.port_name AS arrival_port,
                                   c.container_code
                           FROM BookingRequests br
                           JOIN Users u ON br.customer_id = u.user_id
                           JOIN Trips t ON br.trip_id = t.trip_id
                           JOIN Ports dp ON t.departure_port_id = dp.port_id
                           JOIN Ports ap ON t.arrival_port_id = ap.port_id
                           LEFT JOIN Trip_Containers tc ON br.trip_container_id = tc.trip_container_id
                           LEFT JOIN Containers c ON tc.container_id = c.container_id
                           WHERE br.booking_id = @bookingId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@bookingId", bookingId)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }



        public static DataTable GetUserBookings(int userId)
        {
            string query = @"
    SELECT b.booking_id, 
           t.trip_code,
           ct.type_name AS container_type_name, 
           b.created_at as booking_date,
           b.status_id as status_id, 
           bs.status_name
    FROM BookingRequests b
    INNER JOIN Trips t ON b.trip_id = t.trip_id
    -- تأكد من وجود جدول لربط نوع الحاوية أو استخدم ShippingTypes
    LEFT JOIN ShippingTypes ct ON b.container_type = ct.type_id 
    LEFT JOIN BookingStatuses bs ON b.status_id= bs.status_id
    WHERE b.customer_id = @user_id
    ORDER BY b.created_at DESC";

            SqlParameter[] parameters = {
        new SqlParameter("@user_id", userId)
    };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        public static DataTable GetApprovedBookings(int userId)
        {
            string query = @"
            SELECT booking_id, 
                   CONCAT(booking_code, ' - ', (SELECT trip_code FROM trips WHERE trip_id = b.trip_id)) AS booking_info
            FROM BookingRequests b
            WHERE user_id = @user_id AND status_id = 2
            ORDER BY booking_date DESC";

            SqlParameter[] parameters = {
            new SqlParameter("@user_id", userId)
        };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        public static DataTable GetPendingBookings()
        {
            // تم التعديل: إضافة الربط مع جدول ContainerSizes وجلب اسم الحجم
            string query = @"
    SELECT b.booking_id, 
           b.created_at, -- تأكدنا من عدم تغيير الاسم ليتطابق مع الـ GridView
           u.full_name AS customer_name,
           t.trip_code, 
           st.type_name AS container_type_name,
           cs.size_name AS container_size, -- هذا هو العمود الذي كان ناقصاً ويسبب الخطأ
           b.expected_weight_kg AS expected_weight
    FROM BookingRequests b
    INNER JOIN Users u ON b.customer_id = u.user_id
    INNER JOIN Trips t ON b.trip_id = t.trip_id
    INNER JOIN ContainerTypes st ON b.container_type = st.container_type_id -- أو ShippingTypes حسب تسميتك
    INNER JOIN ContainerSizes cs ON b.container_size = cs.size_id
    WHERE b.status_id= 1 -- 1 يمثل حالة 'معلق'
    ORDER BY b.created_at DESC";

            return DatabaseHelper.ExecuteQuery(query);
        }

        public static bool ApproveBooking(int bookingId)
        {
            string query = "UPDATE BookingRequests SET status_id = 2 WHERE booking_id = @booking_id";
            SqlParameter[] parameters = {
            new SqlParameter("@booking_id", bookingId)
        };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public static bool RejectBooking(int bookingId)
        {
            string query = "UPDATE BookingRequests SET status_id = 3 WHERE booking_id = @booking_id";
            SqlParameter[] parameters = {
            new SqlParameter("@booking_id", bookingId)
        };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public static DataTable GetUserInvoices(int userId)
        {
            string query = @"
            SELECT i.invoice_id, i.invoice_number, b.booking_code,
                   i.total_amount, i.issue_date, i.is_paid
            FROM invoices i
            INNER JOIN BookingRequests b ON i.booking_id = b.booking_id
            WHERE b.user_id = @user_id
            ORDER BY i.issue_date DESC";

            SqlParameter[] parameters = {
            new SqlParameter("@user_id", userId)
        };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }
    }
}
