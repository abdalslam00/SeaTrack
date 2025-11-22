using System;
using System.Data;
using System.Data.SqlClient;

namespace SeaTrack.DAL
{
    /// <summary>
    /// Repository لإدارة عمليات الفواتير في قاعدة البيانات
    /// </summary>
    public class InvoiceRepository
    {
        /// <summary>
        /// الحصول على جميع الفواتير
        /// </summary>
        public static DataTable GetAllInvoices()
        {
            string query = @"SELECT i.*, u.full_name as customer_name, u.email
                            FROM Invoices i
                            INNER JOIN Users u ON i.customer_id = u.user_id
                            ORDER BY i.created_at DESC";
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// الحصول على فواتير عميل معين
        /// </summary>
        public static DataTable GetInvoicesByCustomer(int customerId)
        {
            string query = @"SELECT i.*, p.amount_paid, p.payment_date, p.receipt_number
                            FROM Invoices i
                            LEFT JOIN Payments p ON i.invoice_id = p.invoice_id
                            WHERE i.customer_id = @customer_id
                            ORDER BY i.created_at DESC";
            
            SqlParameter[] parameters = { new SqlParameter("@customer_id", customerId) };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// الحصول على تفاصيل فاتورة معينة
        /// </summary>
        public static DataRow GetInvoiceById(int invoiceId)
        {
            string query = @"SELECT i.*, u.full_name as customer_name, u.email, u.phone, u.address
                            FROM Invoices i
                            INNER JOIN Users u ON i.customer_id = u.user_id
                            WHERE i.invoice_id = @invoice_id";
            
            SqlParameter[] parameters = { new SqlParameter("@invoice_id", invoiceId) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        /// <summary>
        /// إنشاء فاتورة جديدة
        /// </summary>
        public static int CreateInvoice(string invoicCode, int customerId, decimal amount, int statusId, int? shipmentId = null, string notes = null)
        {
            string query = @"INSERT INTO Invoices 
                     (customer_id, invoice_code, total_amount, status_id, shipment_id, notes, created_at)
                     VALUES 
                     (@customer_id, @invoice_code, @total_amount, @status_id, @shipment_id, @notes, GETDATE());
                     SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
        new SqlParameter("@customer_id", customerId),
        new SqlParameter("@invoice_code", invoiceCode),
        new SqlParameter("@total_amount", amount),
        new SqlParameter("@status_id", statusId),
        new SqlParameter("@shipment_id", shipmentId ?? (object)DBNull.Value),
        new SqlParameter("@notes", notes ?? (object)DBNull.Value)
    };

            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }
        /// <summary>
        /// تحديث حالة الفاتورة
        /// </summary>
        public static bool UpdateInvoiceStatus(int invoiceId, int statusId)
        {
            string query = @"UPDATE Invoices 
                            SET status_id = @status_id, 
                                updated_at = GETDATE()
                            WHERE invoice_id = @invoice_id";
            
            SqlParameter[] parameters = {
                new SqlParameter("@status_id", statusId),
                new SqlParameter("@invoice_id", invoiceId)
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

        /// <summary>
        /// تحديث بيانات الفاتورة
        /// </summary>
        public static bool UpdateInvoice(int invoiceId, decimal totalAmount, string notes)
        {
            string query = @"UPDATE Invoices 
                            SET total_amount = @total_amount,
                                notes = @notes,
                                updated_at = GETDATE()
                            WHERE invoice_id = @invoice_id";
            
            SqlParameter[] parameters = {
                new SqlParameter("@total_amount", totalAmount),
                new SqlParameter("@notes", notes ?? (object)DBNull.Value),
                new SqlParameter("@invoice_id", invoiceId)
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

        /// <summary>
        /// حذف فاتورة
        /// </summary>
        public static bool DeleteInvoice(int invoiceId)
        {
            string query = "DELETE FROM Invoices WHERE invoice_id = @invoice_id";
            SqlParameter[] parameters = { new SqlParameter("@invoice_id", invoiceId) };
            
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
        /// الحصول على الفواتير المعلقة
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
        /// الحصول على الفواتير المدفوعة
        /// </summary>
        public static DataTable GetPaidInvoices()
        {
            string query = @"SELECT i.*, u.full_name as customer_name, 
                                   p.amount_paid, p.payment_method, p.payment_date
                            FROM Invoices i
                            INNER JOIN Users u ON i.customer_id = u.user_id
                            LEFT JOIN Payments p ON i.invoice_id = p.invoice_id
                            WHERE i.status_id = 2
                            ORDER BY p.payment_date DESC";
            
            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// حساب إجمالي الفواتير لعميل معين
        /// </summary>
        public static decimal GetCustomerTotalInvoices(int customerId)
        {
            string query = "SELECT ISNULL(SUM(total_amount), 0) FROM Invoices WHERE customer_id = @customer_id";
            SqlParameter[] parameters = { new SqlParameter("@customer_id", customerId) };
            
            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToDecimal(result);
        }

        /// <summary>
        /// حساب إجمالي الفواتير المعلقة لعميل معين
        /// </summary>
        public static decimal GetCustomerPendingInvoices(int customerId)
        {
            string query = @"SELECT ISNULL(SUM(total_amount), 0) 
                            FROM Invoices 
                            WHERE customer_id = @customer_id AND status_id = 1";
            
            SqlParameter[] parameters = { new SqlParameter("@customer_id", customerId) };
            
            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToDecimal(result);
        }

        /// <summary>
        /// توليد رقم فاتورة فريد
        /// </summary>
        public static string GenerateInvoiceNumber()
        {
            string query = "SELECT COUNT(*) FROM Invoices";
            object result = DatabaseHelper.ExecuteScalar(query);
            int count = Convert.ToInt32(result) + 1;
            
            return $"INV-{DateTime.Now.Year}-{count:D6}";
        }

        /// <summary>
        /// الحصول على إحصائيات الفواتير
        /// </summary>
        public static DataTable GetInvoiceStatistics()
        {
            string query = @"SELECT 
                                COUNT(*) as total_invoices,
                                SUM(CASE WHEN status_id = 1 THEN 1 ELSE 0 END) as pending_count,
                                SUM(CASE WHEN status_id = 2 THEN 1 ELSE 0 END) as paid_count,
                                SUM(CASE WHEN status_id = 3 THEN 1 ELSE 0 END) as cancelled_count,
                                SUM(total_amount) as total_amount,
                                SUM(CASE WHEN status_id = 1 THEN total_amount ELSE 0 END) as pending_amount,
                                SUM(CASE WHEN status_id = 2 THEN total_amount ELSE 0 END) as paid_amount
                            FROM Invoices";
            
            return DatabaseHelper.ExecuteQuery(query);
        }
    }
}
