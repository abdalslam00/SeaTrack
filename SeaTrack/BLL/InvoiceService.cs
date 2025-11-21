using SeaTrack.DAL;
using SeaTrack.Utilities;
using System;
using System.Data;

namespace SeaTrack.BLL
{
    /// <summary>
    /// خدمة إدارة الفواتير ومنطق الأعمال المتعلق بها
    /// </summary>
    public class InvoiceService
    {
        /// <summary>
        /// إنشاء فاتورة جديدة للعميل
        /// </summary>
        public static int CreateInvoiceForCustomer(int customerId, decimal totalAmount, string notes = null)
        {
            try
            {
                // توليد رقم فاتورة فريد
                string invoiceNumber = InvoiceRepository.GenerateInvoiceNumber();
                
                // حالة الفاتورة: 1 = Pending (معلقة)
                int statusId = 1;
                
                // إنشاء الفاتورة
                int invoiceId = InvoiceRepository.CreateInvoice(customerId, invoiceNumber, totalAmount, statusId, notes);
                
                // إرسال إشعار للعميل
                if (invoiceId > 0)
                {
                    NotificationService.NotifyInvoiceIssued(customerId, invoiceId, totalAmount);
                }
                
                return invoiceId;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("InvoiceService.CreateInvoiceForCustomer", ex);
                throw;
            }
        }

        /// <summary>
        /// إنشاء فاتورة تلقائية بناءً على شحنة
        /// </summary>
        public static int CreateInvoiceForShipment(int shipmentId)
        {
            try
            {
                // الحصول على تفاصيل الشحنة
                DataRow shipment = GetShipmentDetails(shipmentId);
                
                if (shipment == null)
                    throw new Exception("الشحنة غير موجودة");
                
                int customerId = Convert.ToInt32(shipment["customer_id"]);
                decimal weight = Convert.ToDecimal(shipment["weight_kg"]);
                
                // حساب المبلغ بناءً على الوزن (سعر افتراضي: 50 ريال للكيلو)
                decimal pricePerKg = 50m;
                decimal totalAmount = weight * pricePerKg;
                
                string notes = $"فاتورة الشحنة رقم {shipmentId}";
                
                return CreateInvoiceForCustomer(customerId, totalAmount, notes);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("InvoiceService.CreateInvoiceForShipment", ex);
                throw;
            }
        }

        /// <summary>
        /// إنشاء فاتورة تلقائية بناءً على حجز
        /// </summary>
        public static int CreateInvoiceForBooking(int bookingId)
        {
            try
            {
                // الحصول على تفاصيل الحجز
                DataRow booking = GetBookingDetails(bookingId);
                
                if (booking == null)
                    throw new Exception("الحجز غير موجود");
                
                int customerId = Convert.ToInt32(booking["customer_id"]);
                int containerType = Convert.ToInt32(booking["container_type"]);
                int sizeId = Convert.ToInt32(booking["size_id"]);
                
                // حساب المبلغ بناءً على نوع وحجم الحاوية
                decimal totalAmount = CalculateBookingPrice(containerType, sizeId);
                
                string notes = $"فاتورة حجز الحاوية رقم {bookingId}";
                
                return CreateInvoiceForCustomer(customerId, totalAmount, notes);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("InvoiceService.CreateInvoiceForBooking", ex);
                throw;
            }
        }

        /// <summary>
        /// حساب سعر الحجز بناءً على نوع وحجم الحاوية
        /// </summary>
        private static decimal CalculateBookingPrice(int containerType, int sizeId)
        {
            // أسعار افتراضية
            decimal basePrice = 0;
            
            // حسب حجم الحاوية
            if (sizeId == 1) // 20 قدم
                basePrice = 5000m;
            else if (sizeId == 2) // 40 قدم
                basePrice = 8000m;
            
            // حسب نوع الحاوية
            if (containerType == 1) // حاوية كاملة (خاصة)
                return basePrice;
            else // نصف حاوية (عامة)
                return basePrice / 2;
        }

        /// <summary>
        /// تحديث حالة الفاتورة
        /// </summary>
        public static bool UpdateInvoiceStatus(int invoiceId, int newStatusId)
        {
            try
            {
                return InvoiceRepository.UpdateInvoiceStatus(invoiceId, newStatusId);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("InvoiceService.UpdateInvoiceStatus", ex);
                return false;
            }
        }

        /// <summary>
        /// تحديد الفاتورة كمدفوعة
        /// </summary>
        public static bool MarkInvoiceAsPaid(int invoiceId)
        {
            return UpdateInvoiceStatus(invoiceId, 2); // 2 = Paid
        }

        /// <summary>
        /// إلغاء الفاتورة
        /// </summary>
        public static bool CancelInvoice(int invoiceId)
        {
            return UpdateInvoiceStatus(invoiceId, 3); // 3 = Cancelled
        }

        /// <summary>
        /// الحصول على فواتير العميل
        /// </summary>
        public static DataTable GetCustomerInvoices(int customerId)
        {
            return InvoiceRepository.GetInvoicesByCustomer(customerId);
        }

        /// <summary>
        /// الحصول على الفواتير المعلقة للعميل
        /// </summary>
        public static DataTable GetCustomerPendingInvoices(int customerId)
        {
            DataTable allInvoices = InvoiceRepository.GetInvoicesByCustomer(customerId);
            DataView dv = allInvoices.DefaultView;
            dv.RowFilter = "status_id = 1";
            return dv.ToTable();
        }

        /// <summary>
        /// حساب إجمالي مديونية العميل
        /// </summary>
        public static decimal GetCustomerOutstandingBalance(int customerId)
        {
            return InvoiceRepository.GetCustomerPendingInvoices(customerId);
        }

        /// <summary>
        /// التحقق من وجود فواتير معلقة للعميل
        /// </summary>
        public static bool HasPendingInvoices(int customerId)
        {
            decimal pendingAmount = InvoiceRepository.GetCustomerPendingInvoices(customerId);
            return pendingAmount > 0;
        }

        /// <summary>
        /// الحصول على إحصائيات الفواتير
        /// </summary>
        public static DataTable GetInvoiceStatistics()
        {
            return InvoiceRepository.GetInvoiceStatistics();
        }

        // Helper Methods

        private static DataRow GetShipmentDetails(int shipmentId)
        {
            string query = "SELECT * FROM Shipments WHERE shipment_id = @shipment_id";
            System.Data.SqlClient.SqlParameter[] parameters = {
                new System.Data.SqlClient.SqlParameter("@shipment_id", shipmentId)
            };
            
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        private static DataRow GetBookingDetails(int bookingId)
        {
            string query = "SELECT * FROM BookingRequests WHERE booking_id = @booking_id";
            System.Data.SqlClient.SqlParameter[] parameters = {
                new System.Data.SqlClient.SqlParameter("@booking_id", bookingId)
            };
            
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
    }
}
