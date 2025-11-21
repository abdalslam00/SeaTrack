using System;
using System.Data;
using System.Data.SqlClient;
using SeaTrack.DAL;
using SeaTrack.Utilities;

namespace SeaTrack.BLL
{
    /// <summary>
    /// خدمة إدارة الإشعارات للمستخدمين
    /// </summary>
    public class NotificationService
    {
        /// <summary>
        /// إنشاء إشعار جديد
        /// </summary>
        public static int CreateNotification(int userId, string title, string message, string notificationType = "Info")
        {
            try
            {
                string query = @"INSERT INTO Notifications 
                                (user_id, title, message, notification_type, is_read, created_at)
                                VALUES 
                                (@user_id, @title, @message, @notification_type, 0, GETDATE());
                                SELECT SCOPE_IDENTITY();";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@title", title),
                    new SqlParameter("@message", message),
                    new SqlParameter("@notification_type", notificationType)
                };
                
                object result = DatabaseHelper.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("NotificationService.CreateNotification", ex);
                return 0;
            }
        }

        /// <summary>
        /// إرسال إشعار بتأكيد الحجز للعميل
        /// </summary>
        public static void NotifyBookingConfirmed(int customerId, int bookingId, string containerCode, string tripDetails)
        {
            string title = "تم تأكيد حجزك";
            string message = $"تم تأكيد حجزك رقم {bookingId}. تم تخصيص الحاوية {containerCode} لك في الرحلة: {tripDetails}";
            CreateNotification(customerId, title, message, "Success");
        }

        /// <summary>
        /// إرسال إشعار برفض الحجز
        /// </summary>
        public static void NotifyBookingRejected(int customerId, int bookingId, string reason)
        {
            string title = "تم رفض حجزك";
            string message = $"عذراً، تم رفض حجزك رقم {bookingId}. السبب: {reason}";
            CreateNotification(customerId, title, message, "Warning");
        }

        /// <summary>
        /// إرسال إشعار بتغيير حالة الشحنة
        /// </summary>
        public static void NotifyShipmentStatusChanged(int customerId, int shipmentId, string newStatus)
        {
            string title = "تحديث حالة الشحنة";
            string message = $"تم تحديث حالة شحنتك رقم {shipmentId} إلى: {newStatus}";
            CreateNotification(customerId, title, message, "Info");
        }

        /// <summary>
        /// إرسال إشعار بتسليم الشحنة
        /// </summary>
        public static void NotifyShipmentDelivered(int customerId, int shipmentId, string trackingNumber)
        {
            string title = "تم تسليم شحنتك";
            string message = $"تم تسليم شحنتك رقم {shipmentId} (رقم التتبع: {trackingNumber}) بنجاح. شكراً لاستخدامك خدماتنا!";
            CreateNotification(customerId, title, message, "Success");
        }

        /// <summary>
        /// إرسال إشعار بتغيير حالة الرحلة
        /// </summary>
        public static void NotifyTripStatusChanged(int tripId, string newStatus)
        {
            try
            {
                // الحصول على جميع العملاء الذين لديهم شحنات في هذه الرحلة
                string query = @"SELECT DISTINCT s.customer_id 
                                FROM Shipments s 
                                WHERE s.trip_id = @trip_id";
                
                SqlParameter[] parameters = { new SqlParameter("@trip_id", tripId) };
                DataTable customers = DatabaseHelper.ExecuteQuery(query, parameters);
                
                string title = "تحديث حالة الرحلة";
                string message = $"تم تحديث حالة الرحلة رقم {tripId} إلى: {newStatus}";
                
                foreach (DataRow row in customers.Rows)
                {
                    int customerId = Convert.ToInt32(row["customer_id"]);
                    CreateNotification(customerId, title, message, "Info");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("NotificationService.NotifyTripStatusChanged", ex);
            }
        }

        /// <summary>
        /// إرسال إشعار بإصدار فاتورة جديدة
        /// </summary>
        public static void NotifyInvoiceIssued(int customerId, int invoiceId, decimal amount)
        {
            string title = "فاتورة جديدة";
            string message = $"تم إصدار فاتورة جديدة رقم {invoiceId} بمبلغ {amount:N2} ريال. يرجى المراجعة والدفع.";
            CreateNotification(customerId, title, message, "Info");
        }

        /// <summary>
        /// إرسال إشعار بتأكيد الدفع
        /// </summary>
        public static void NotifyPaymentReceived(int customerId, int invoiceId, decimal amount, string receiptNumber)
        {
            string title = "تم استلام الدفع";
            string message = $"تم استلام دفعتك بمبلغ {amount:N2} ريال للفاتورة رقم {invoiceId}. رقم الإيصال: {receiptNumber}";
            CreateNotification(customerId, title, message, "Success");
        }

        /// <summary>
        /// الحصول على جميع إشعارات مستخدم معين
        /// </summary>
        public static DataTable GetUserNotifications(int userId, bool unreadOnly = false)
        {
            string query = @"SELECT * FROM Notifications 
                            WHERE user_id = @user_id";
            
            if (unreadOnly)
                query += " AND is_read = 0";
            
            query += " ORDER BY created_at DESC";
            
            SqlParameter[] parameters = { new SqlParameter("@user_id", userId) };
            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// الحصول على عدد الإشعارات غير المقروءة
        /// </summary>
        public static int GetUnreadCount(int userId)
        {
            string query = "SELECT COUNT(*) FROM Notifications WHERE user_id = @user_id AND is_read = 0";
            SqlParameter[] parameters = { new SqlParameter("@user_id", userId) };
            
            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// تحديد إشعار كمقروء
        /// </summary>
        public static bool MarkAsRead(int notificationId)
        {
            try
            {
                string query = @"UPDATE Notifications 
                                SET is_read = 1, read_at = GETDATE() 
                                WHERE notification_id = @notification_id";
                
                SqlParameter[] parameters = { new SqlParameter("@notification_id", notificationId) };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("NotificationService.MarkAsRead", ex);
                return false;
            }
        }

        /// <summary>
        /// تحديد جميع إشعارات المستخدم كمقروءة
        /// </summary>
        public static bool MarkAllAsRead(int userId)
        {
            try
            {
                string query = @"UPDATE Notifications 
                                SET is_read = 1, read_at = GETDATE() 
                                WHERE user_id = @user_id AND is_read = 0";
                
                SqlParameter[] parameters = { new SqlParameter("@user_id", userId) };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("NotificationService.MarkAllAsRead", ex);
                return false;
            }
        }

        /// <summary>
        /// حذف إشعار
        /// </summary>
        public static bool DeleteNotification(int notificationId)
        {
            try
            {
                string query = "DELETE FROM Notifications WHERE notification_id = @notification_id";
                SqlParameter[] parameters = { new SqlParameter("@notification_id", notificationId) };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("NotificationService.DeleteNotification", ex);
                return false;
            }
        }

        /// <summary>
        /// حذف جميع الإشعارات القديمة (أكثر من 30 يوم)
        /// </summary>
        public static int DeleteOldNotifications(int daysOld = 30)
        {
            try
            {
                string query = @"DELETE FROM Notifications 
                                WHERE created_at < DATEADD(day, -@days_old, GETDATE())";
                
                SqlParameter[] parameters = { new SqlParameter("@days_old", daysOld) };
                return DatabaseHelper.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("NotificationService.DeleteOldNotifications", ex);
                return 0;
            }
        }

        /// <summary>
        /// إرسال إشعار جماعي لجميع المستخدمين بدور معين
        /// </summary>
        public static void BroadcastToRole(int roleId, string title, string message, string notificationType = "Info")
        {
            try
            {
                string query = "SELECT user_id FROM Users WHERE role_id = @role_id";
                SqlParameter[] parameters = { new SqlParameter("@role_id", roleId) };
                DataTable users = DatabaseHelper.ExecuteQuery(query, parameters);
                
                foreach (DataRow row in users.Rows)
                {
                    int userId = Convert.ToInt32(row["user_id"]);
                    CreateNotification(userId, title, message, notificationType);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("NotificationService.BroadcastToRole", ex);
            }
        }
    }
}
