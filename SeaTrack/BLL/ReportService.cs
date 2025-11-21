using System;
using System.Data;
using System.Data.SqlClient;
using SeaTrack.DAL;

namespace SeaTrack.BLL
{
    /// <summary>
    /// خدمة إنشاء التقارير والإحصائيات
    /// </summary>
    public class ReportService
    {
        /// <summary>
        /// تقرير الإيرادات الشامل
        /// </summary>
        public static DataTable GetRevenueReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            string query = @"SELECT 
                                CONVERT(DATE, p.payment_date) as payment_date,
                                COUNT(p.payment_id) as payment_count,
                                SUM(p.amount_paid) as daily_revenue,
                                AVG(p.amount_paid) as average_payment
                            FROM Payments p
                            WHERE 1=1";
            
            if (startDate.HasValue)
                query += " AND p.payment_date >= @start_date";
            
            if (endDate.HasValue)
                query += " AND p.payment_date <= @end_date";
            
            query += @" GROUP BY CONVERT(DATE, p.payment_date)
                       ORDER BY payment_date DESC";
            
            SqlParameter[] parameters = {
                new SqlParameter("@start_date", startDate ?? (object)DBNull.Value),
                new SqlParameter("@end_date", endDate ?? (object)DBNull.Value)
            };
            
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// تقرير الإيرادات الشهرية
        /// </summary>
        public static DataTable GetMonthlyRevenueReport(int year)
        {
            string query = @"SELECT 
                                MONTH(p.payment_date) as month_number,
                                DATENAME(MONTH, p.payment_date) as month_name,
                                COUNT(p.payment_id) as payment_count,
                                SUM(p.amount_paid) as monthly_revenue
                            FROM Payments p
                            WHERE YEAR(p.payment_date) = @year
                            GROUP BY MONTH(p.payment_date), DATENAME(MONTH, p.payment_date)
                            ORDER BY month_number";
            
            SqlParameter[] parameters = { new SqlParameter("@year", year) };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// تقرير أداء الرحلات
        /// </summary>
        public static DataTable GetTripsPerformanceReport()
        {
            string query = @"SELECT 
                                t.trip_id,
                                t.trip_code,
                                s.ship_name,
                                dp.port_name as departure_port,
                                ap.port_name as arrival_port,
                                t.departure_date,
                                t.expected_arrival_date,
                                t.actual_arrival_date,
                                ts.status_name_ar as trip_status,
                                COUNT(DISTINCT tc.container_id) as container_count,
                                COUNT(DISTINCT sh.shipment_id) as shipment_count,
                                SUM(tc.cargo_weight_kg) as total_cargo_weight,
                                SUM(tc.gross_weight) as total_gross_weight
                            FROM Trips t
                            INNER JOIN Ships s ON t.ship_id = s.ship_id
                            INNER JOIN Ports dp ON t.departure_port_id = dp.port_id
                            INNER JOIN Ports ap ON t.arrival_port_id = ap.port_id
                            INNER JOIN TripStatuses ts ON t.status = ts.status_id
                            LEFT JOIN Trip_Containers tc ON t.trip_id = tc.trip_id
                            LEFT JOIN Shipments sh ON t.trip_id = sh.trip_id
                            GROUP BY t.trip_id, t.trip_code, s.ship_name, dp.port_name, ap.port_name,
                                     t.departure_date, t.expected_arrival_date, t.actual_arrival_date, ts.status_name_ar
                            ORDER BY t.departure_date DESC";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// تقرير حجم الشحنات
        /// </summary>
        public static DataTable GetShipmentsVolumeReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            string query = @"SELECT 
                                CONVERT(DATE, s.created_at) as shipment_date,
                                COUNT(s.shipment_id) as shipment_count,
                                SUM(s.weight_kg) as total_weight,
                                AVG(s.weight_kg) as average_weight,
                                SUM(CASE WHEN s.shipping_type_id = 1 THEN 1 ELSE 0 END) as private_shipments,
                                SUM(CASE WHEN s.shipping_type_id = 2 THEN 1 ELSE 0 END) as shared_shipments
                            FROM Shipments s
                            WHERE 1=1";
            
            if (startDate.HasValue)
                query += " AND s.created_at >= @start_date";
            
            if (endDate.HasValue)
                query += " AND s.created_at <= @end_date";
            
            query += @" GROUP BY CONVERT(DATE, s.created_at)
                       ORDER BY shipment_date DESC";
            
            SqlParameter[] parameters = {
                new SqlParameter("@start_date", startDate ?? (object)DBNull.Value),
                new SqlParameter("@end_date", endDate ?? (object)DBNull.Value)
            };
            
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// تقرير العملاء الأكثر نشاطاً
        /// </summary>
        public static DataTable GetTopCustomersReport(int topCount = 10)
        {
            string query = @"SELECT TOP (@top_count)
                                u.user_id,
                                u.full_name,
                                u.email,
                                u.phone,
                                COUNT(DISTINCT s.shipment_id) as shipment_count,
                                COUNT(DISTINCT br.booking_id) as booking_count,
                                SUM(i.total_amount) as total_invoiced,
                                SUM(p.amount_paid) as total_paid
                            FROM Users u
                            LEFT JOIN Shipments s ON u.user_id = s.customer_id
                            LEFT JOIN BookingRequests br ON u.user_id = br.customer_id
                            LEFT JOIN Invoices i ON u.user_id = i.customer_id
                            LEFT JOIN Payments p ON i.invoice_id = p.invoice_id
                            WHERE u.role_id = (SELECT role_id FROM Roles WHERE role_name = 'Customer')
                            GROUP BY u.user_id, u.full_name, u.email, u.phone
                            ORDER BY shipment_count DESC, total_paid DESC";
            
            SqlParameter[] parameters = { new SqlParameter("@top_count", topCount) };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// تقرير استخدام الحاويات
        /// </summary>
        public static DataTable GetContainerUtilizationReport()
        {
            string query = @"SELECT 
                                c.container_id,
                                c.container_code,
                                cs.size_name,
                                c.base_weight_kg,
                                c.max_weight_kg,
                                COUNT(DISTINCT tc.trip_id) as trip_count,
                                COUNT(DISTINCT s.shipment_id) as shipment_count,
                                AVG(tc.cargo_weight_kg) as avg_cargo_weight,
                                AVG(tc.gross_weight) as avg_gross_weight,
                                AVG((tc.gross_weight * 100.0) / c.max_weight_kg) as avg_capacity_percentage
                            FROM Containers c
                            INNER JOIN ContainerSizes cs ON c.size_id = cs.size_id
                            LEFT JOIN Trip_Containers tc ON c.container_id = tc.container_id
                            LEFT JOIN Shipments s ON tc.trip_id = s.trip_id AND tc.container_id = s.container_id
                            GROUP BY c.container_id, c.container_code, cs.size_name, c.base_weight_kg, c.max_weight_kg
                            ORDER BY trip_count DESC";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// تقرير حالات الشحنات
        /// </summary>
        public static DataTable GetShipmentStatusReport()
        {
            string query = @"SELECT 
                                ss.status_name_ar as status_name,
                                COUNT(s.shipment_id) as shipment_count,
                                SUM(s.weight_kg) as total_weight,
                                AVG(s.weight_kg) as average_weight
                            FROM ShipmentStatuses ss
                            LEFT JOIN Shipments s ON ss.status_id = s.status_id
                            GROUP BY ss.status_id, ss.status_name_ar
                            ORDER BY shipment_count DESC";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// تقرير الفواتير والمدفوعات
        /// </summary>
        public static DataTable GetInvoicePaymentReport()
        {
            string query = @"SELECT 
                                u.full_name as customer_name,
                                COUNT(i.invoice_id) as invoice_count,
                                SUM(i.total_amount) as total_invoiced,
                                SUM(CASE WHEN i.status_id = 2 THEN i.total_amount ELSE 0 END) as total_paid,
                                SUM(CASE WHEN i.status_id = 1 THEN i.total_amount ELSE 0 END) as total_pending,
                                COUNT(p.payment_id) as payment_count
                            FROM Users u
                            LEFT JOIN Invoices i ON u.user_id = i.customer_id
                            LEFT JOIN Payments p ON i.invoice_id = p.invoice_id
                            WHERE u.role_id = (SELECT role_id FROM Roles WHERE role_name = 'Customer')
                            GROUP BY u.user_id, u.full_name
                            HAVING COUNT(i.invoice_id) > 0
                            ORDER BY total_invoiced DESC";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// لوحة معلومات شاملة (Dashboard Summary)
        /// </summary>
        public static DataTable GetDashboardSummary()
        {
            string query = @"SELECT 
                                (SELECT COUNT(*) FROM Trips) as total_trips,
                                (SELECT COUNT(*) FROM Trips WHERE status = 1) as active_trips,
                                (SELECT COUNT(*) FROM Shipments) as total_shipments,
                                (SELECT COUNT(*) FROM Shipments WHERE status_id = 2) as in_transit_shipments,
                                (SELECT COUNT(*) FROM Containers) as total_containers,
                                (SELECT COUNT(*) FROM Containers WHERE is_available = 1) as available_containers,
                                (SELECT COUNT(*) FROM Users WHERE role_id = (SELECT role_id FROM Roles WHERE role_name = 'Customer')) as total_customers,
                                (SELECT COUNT(*) FROM BookingRequests WHERE status_id = 1) as pending_bookings,
                                (SELECT ISNULL(SUM(amount_paid), 0) FROM Payments WHERE MONTH(payment_date) = MONTH(GETDATE()) AND YEAR(payment_date) = YEAR(GETDATE())) as monthly_revenue,
                                (SELECT COUNT(*) FROM Invoices WHERE status_id = 1) as pending_invoices";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// تقرير الموانئ الأكثر استخداماً
        /// </summary>
        public static DataTable GetPortActivityReport()
        {
            string query = @"SELECT 
                                p.port_name,
                                p.country,
                                COUNT(DISTINCT t1.trip_id) as departure_count,
                                COUNT(DISTINCT t2.trip_id) as arrival_count,
                                COUNT(DISTINCT t1.trip_id) + COUNT(DISTINCT t2.trip_id) as total_activity
                            FROM Ports p
                            LEFT JOIN Trips t1 ON p.port_id = t1.departure_port_id
                            LEFT JOIN Trips t2 ON p.port_id = t2.arrival_port_id
                            GROUP BY p.port_id, p.port_name, p.country
                            ORDER BY total_activity DESC";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// تقرير البواخر وأدائها
        /// </summary>
        public static DataTable GetShipPerformanceReport()
        {
            string query = @"SELECT 
                                s.ship_name,
                                s.capacity,
                                COUNT(t.trip_id) as trip_count,
                                COUNT(DISTINCT tc.container_id) as container_count,
                                SUM(tc.cargo_weight_kg) as total_cargo_transported,
                                AVG(tc.gross_weight) as avg_weight_per_trip
                            FROM Ships s
                            LEFT JOIN Trips t ON s.ship_id = t.ship_id
                            LEFT JOIN Trip_Containers tc ON t.trip_id = tc.trip_id
                            GROUP BY s.ship_id, s.ship_name, s.capacity
                            ORDER BY trip_count DESC";
            
            return DatabaseHelper.ExecuteQuery(query);
        }
    }
}
