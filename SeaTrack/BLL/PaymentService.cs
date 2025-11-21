using System;
using System.Data;
using System.Data.SqlClient;
using SeaTrack.DAL;
using SeaTrack.Utilities;

namespace SeaTrack.BLL
{
    /// <summary>
    /// خدمة إدارة المدفوعات وإيصالات الدفع
    /// </summary>
    public class PaymentService
    {
        /// <summary>
        /// إنشاء إيصال دفع رسمي (صلاحية حصرية للمسؤول)
        /// </summary>
        public static int CreatePaymentReceipt(int invoiceId, decimal amountPaid, string paymentMethod, string notes, int createdBy)
        {
            try
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
                int paymentId = Convert.ToInt32(result);
                
                // تحديث حالة الفاتورة إلى "مدفوعة"
                UpdateInvoiceStatus(invoiceId, 2); // 2 = Paid
                
                return paymentId;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("PaymentService.CreatePaymentReceipt", ex);
                throw;
            }
        }

        /// <summary>
        /// تحديث حالة الفاتورة
        /// </summary>
        private static void UpdateInvoiceStatus(int invoiceId, int statusId)
        {
            string query = @"UPDATE Invoices 
                            SET status_id = @status_id, 
                                updated_at = GETDATE() 
                            WHERE invoice_id = @invoice_id";
            
            SqlParameter[] parameters = {
                new SqlParameter("@status_id", statusId),
                new SqlParameter("@invoice_id", invoiceId)
            };
            
            DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// الحصول على جميع الفواتير المدفوعة
        /// </summary>
        public static DataTable GetPaidInvoices()
        {
            string query = @"SELECT i.*, u.full_name as customer_name, p.amount_paid, 
                                   p.payment_method, p.payment_date, p.receipt_number
                            FROM Invoices i
                            INNER JOIN Users u ON i.customer_id = u.user_id
                            LEFT JOIN Payments p ON i.invoice_id = p.invoice_id
                            WHERE i.status_id = 2
                            ORDER BY p.payment_date DESC";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// الحصول على جميع الفواتير المعلقة
        /// </summary>
        public static DataTable GetPendingInvoices()
        {
            string query = @"SELECT i.*, u.full_name as customer_name
                            FROM Invoices i
                            INNER JOIN Users u ON i.customer_id = u.user_id
                            WHERE i.status_id = 1
                            ORDER BY i.created_at DESC";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// الحصول على تفاصيل إيصال دفع معين
        /// </summary>
        public static DataRow GetPaymentReceiptById(int paymentId)
        {
            string query = @"SELECT p.*, i.invoice_number, i.total_amount, i.customer_id,
                                   u.full_name as customer_name, u.email, u.phone,
                                   admin.full_name as issued_by_name
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
        /// الحصول على إيصال الدفع الخاص بفاتورة معينة
        /// </summary>
        public static DataRow GetPaymentByInvoiceId(int invoiceId)
        {
            string query = @"SELECT p.*, admin.full_name as issued_by_name
                            FROM Payments p
                            INNER JOIN Users admin ON p.created_by = admin.user_id
                            WHERE p.invoice_id = @invoice_id";
            
            SqlParameter[] parameters = { new SqlParameter("@invoice_id", invoiceId) };
            
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        /// <summary>
        /// حساب إجمالي الإيرادات في فترة زمنية معينة
        /// </summary>
        public static decimal GetTotalRevenue(DateTime? startDate = null, DateTime? endDate = null)
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
        /// الحصول على مدفوعات عميل معين
        /// </summary>
        public static DataTable GetCustomerPayments(int customerId)
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
        /// التحقق من وجود إيصال دفع لفاتورة معينة
        /// </summary>
        public static bool HasPaymentReceipt(int invoiceId)
        {
            string query = "SELECT COUNT(*) FROM Payments WHERE invoice_id = @invoice_id";
            SqlParameter[] parameters = { new SqlParameter("@invoice_id", invoiceId) };
            
            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// إلغاء إيصال دفع (في حالات استثنائية)
        /// </summary>
        public static bool CancelPaymentReceipt(int paymentId, string cancellationReason)
        {
            try
            {
                // الحصول على معرف الفاتورة
                string getInvoiceQuery = "SELECT invoice_id FROM Payments WHERE payment_id = @payment_id";
                SqlParameter[] getParams = { new SqlParameter("@payment_id", paymentId) };
                object invoiceIdObj = DatabaseHelper.ExecuteScalar(getInvoiceQuery, getParams);
                
                if (invoiceIdObj == null) return false;
                
                int invoiceId = Convert.ToInt32(invoiceIdObj);
                
                // حذف إيصال الدفع
                string deleteQuery = "DELETE FROM Payments WHERE payment_id = @payment_id";
                SqlParameter[] deleteParams = { new SqlParameter("@payment_id", paymentId) };
                DatabaseHelper.ExecuteNonQuery(deleteQuery, deleteParams);
                
                // إعادة حالة الفاتورة إلى "معلقة"
                UpdateInvoiceStatus(invoiceId, 1); // 1 = Pending
                
                // تسجيل السبب في اللوج
                LogHelper.LogInfo($"Payment receipt {paymentId} cancelled. Reason: {cancellationReason}");
                
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("PaymentService.CancelPaymentReceipt", ex);
                return false;
            }
        }
    }
}
