using System;
using System.Data;
using System.Data.SqlClient;

namespace SeaTrack.DAL
{
    /// <summary>
    /// Repository لإدارة عمليات المدفوعات في قاعدة البيانات
    /// </summary>
    public class PaymentRepository
    {
        /// <summary>
        /// الحصول على جميع المدفوعات
        /// </summary>
        public static DataTable GetAllPayments()
        {
            string query = @"SELECT p.*, i.invoice_number, i.customer_id,
                                   u.full_name as customer_name,
                                   admin.full_name as created_by_name
                            FROM Payments p
                            INNER JOIN Invoices i ON p.invoice_id = i.invoice_id
                            INNER JOIN Users u ON i.customer_id = u.user_id
                            INNER JOIN Users admin ON p.created_by = admin.user_id
                            ORDER BY p.payment_date DESC";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// الحصول على تفاصيل دفعة معينة
        /// </summary>
        public static DataRow GetPaymentById(int paymentId)
        {
            string query = @"SELECT p.*, i.invoice_number, i.total_amount, i.customer_id,
                                   u.full_name as customer_name, u.email, u.phone,
                                   admin.full_name as created_by_name
                            FROM Payments p
                            INNER JOIN Invoices i ON p.invoice_id = i.invoice_id
                            INNER JOIN Users u ON i.customer_id = u.user_id
                            INNER JOIN Users admin ON p.created_by = admin.user_id
                            WHERE p.payment_id = @payment_id";
            
            SqlParameter[] parameters = { new SqlParameter("@payment_id", paymentId) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        /// <summary>
        /// الحصول على دفعة بواسطة معرف الفاتورة
        /// </summary>
        public static DataRow GetPaymentByInvoiceId(int invoiceId)
        {
            string query = @"SELECT p.*, admin.full_name as created_by_name
                            FROM Payments p
                            INNER JOIN Users admin ON p.created_by = admin.user_id
                            WHERE p.invoice_id = @invoice_id";
            
            SqlParameter[] parameters = { new SqlParameter("@invoice_id", invoiceId) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        /// <summary>
        /// إنشاء دفعة جديدة
        /// </summary>
        public static int CreatePayment(int invoiceId, decimal amountPaid, string paymentMethod, 
                                       string notes, int createdBy)
        {
            string query = @"INSERT INTO Payments 
                            (invoice_id, amount_paid, payment_method, payment_date, notes, created_by, created_at)
                            VALUES 
                            (@invoice_id, @amount_paid, @payment_method, GETDATE(), @notes, @created_by, GETDATE());
                            SELECT SCOPE_IDENTITY();";
            
            SqlParameter[] parameters = {
                new SqlParameter("@invoice_id", invoiceId),
                new SqlParameter("@amount_paid", amountPaid),
                new SqlParameter("@payment_method", paymentMethod),
                new SqlParameter("@notes", notes ?? (object)DBNull.Value),
                new SqlParameter("@created_by", createdBy)
            };
            
            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// حذف دفعة
        /// </summary>
        public static bool DeletePayment(int paymentId)
        {
            string query = "DELETE FROM Payments WHERE payment_id = @payment_id";
            SqlParameter[] parameters = { new SqlParameter("@payment_id", paymentId) };
            
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

        /// <summary>
        /// الحصول على مدفوعات عميل معين
        /// </summary>
        public static DataTable GetPaymentsByCustomer(int customerId)
        {
            string query = @"SELECT p.*, i.invoice_number, i.total_amount
                            FROM Payments p
                            INNER JOIN Invoices i ON p.invoice_id = i.invoice_id
                            WHERE i.customer_id = @customer_id
                            ORDER BY p.payment_date DESC";
            
            SqlParameter[] parameters = { new SqlParameter("@customer_id", customerId) };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// حساب إجمالي المدفوعات في فترة زمنية
        /// </summary>
        public static decimal GetTotalPayments(DateTime? startDate = null, DateTime? endDate = null)
        {
            string query = "SELECT ISNULL(SUM(amount_paid), 0) FROM Payments WHERE 1=1";
            
            if (startDate.HasValue)
                query += " AND payment_date >= @start_date";
            
            if (endDate.HasValue)
                query += " AND payment_date <= @end_date";
            
            SqlParameter[] parameters = {
                new SqlParameter("@start_date", startDate ?? (object)DBNull.Value),
                new SqlParameter("@end_date", endDate ?? (object)DBNull.Value)
            };
            
            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToDecimal(result);
        }

        /// <summary>
        /// الحصول على المدفوعات حسب طريقة الدفع
        /// </summary>
        public static DataTable GetPaymentsByMethod()
        {
            string query = @"SELECT payment_method, 
                                   COUNT(*) as payment_count,
                                   SUM(amount_paid) as total_amount
                            FROM Payments
                            GROUP BY payment_method
                            ORDER BY total_amount DESC";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// التحقق من وجود دفعة لفاتورة معينة
        /// </summary>
        public static bool HasPayment(int invoiceId)
        {
            string query = "SELECT COUNT(*) FROM Payments WHERE invoice_id = @invoice_id";
            SqlParameter[] parameters = { new SqlParameter("@invoice_id", invoiceId) };
            
            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// توليد رقم إيصال فريد
        /// </summary>
        public static string GenerateReceiptNumber(int paymentId)
        {
            return $"REC-{DateTime.Now.Year}-{paymentId:D6}";
        }

        /// <summary>
        /// الحصول على إحصائيات المدفوعات
        /// </summary>
        public static DataTable GetPaymentStatistics()
        {
            string query = @"SELECT 
                                COUNT(*) as total_payments,
                                SUM(amount_paid) as total_revenue,
                                AVG(amount_paid) as average_payment,
                                MIN(payment_date) as first_payment_date,
                                MAX(payment_date) as last_payment_date
                            FROM Payments";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// الحصول على المدفوعات الأخيرة
        /// </summary>
        public static DataTable GetRecentPayments(int count = 10)
        {
            string query = @"SELECT TOP (@count) p.*, i.invoice_number, u.full_name as customer_name
                            FROM Payments p
                            INNER JOIN Invoices i ON p.invoice_id = i.invoice_id
                            INNER JOIN Users u ON i.customer_id = u.user_id
                            ORDER BY p.payment_date DESC";
            
            SqlParameter[] parameters = { new SqlParameter("@count", count) };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }
    }
}
