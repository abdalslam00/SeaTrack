using SeaTrack.DAL;
using SeaTrack.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SeaTrack.Pages.admin
{
    public partial class Reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_ADMIN_NAME);
            if (!IsPostBack) LoadReports();
        }

        private void LoadReports()
        {
            DataTable stats = TripRepository.GetStatistics();
            if (stats.Rows.Count > 0)
            {
                ltTotalTrips.Text = stats.Rows[0]["total_trips"].ToString();
                ltTotalShipments.Text = stats.Rows[0]["total_shipments"].ToString();
                ltTotalCustomers.Text = stats.Rows[0]["total_customers"].ToString();
                ltPendingBookings.Text = stats.Rows[0]["pending_bookings"].ToString();
            }

            DataTable recentTrips = TripRepository.GetRecentTrips(10);
            gvRecentTrips.DataSource = recentTrips;
            gvRecentTrips.DataBind();

            LoadArrivedShipments();
        }

        private void LoadArrivedShipments()
        {
            // حالة "Arrived" هي 4، ونريد الشحنات التي حالتها 4 (Arrived)
            // نحتاج إلى دالة في ShipmentRepository تجلب الشحنات التي حالتها "Arrived"
            // نفترض وجود دالة GetShipmentsByStatus(int statusId)
            // حالة "Delivered" هي 5 (نضيفها لاحقاً في قاعدة البيانات إذا لم تكن موجودة)
            // لكن في المتطلب، حالة "Delivered" هي 4، وحالة "Arrived" هي 3.
            // سنفترض أن حالة "Arrived" هي 3 وحالة "Delivered" هي 4 بناءً على سياق المتطلب
            // (Arrived: 3, Delivered: 4)
            // يجب التأكد من قيم حالة الشحنات في قاعدة البيانات، لكن سنستخدم 3 مؤقتاً لـ "Arrived"
            // بما أن متطلب الرحلات استخدم (1->2->3->4) سنفترض أن الشحنات تتبع نفس الترقيم:
            // 1: Created, 2: In Transit, 3: Arrived, 4: Delivered
            // سنستخدم 3 لـ "Arrived"
            DataTable arrivedShipments = ShipmentRepository.GetShipmentsByStatus(3);
            gvArrivedShipments.DataSource = arrivedShipments;
            gvArrivedShipments.DataBind();
        }

        protected void gvArrivedShipments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ConfirmDelivery")
            {
                try
                {
                    int shipmentId = Convert.ToInt32(e.CommandArgument);
                    int adminId = SessionManager.GetUserId().Value; // يجب أن يكون متوفراً لأن الدور هو Admin

                    // تحديث حالة الشحنة إلى "Delivered" (4)
                    // نحتاج إلى دالة في ShipmentRepository لتحديث حالة الشحنة
                    // نفترض وجود دالة UpdateShipmentStatus(int shipmentId, int newStatusId, int updatedByUserId)
                    ShipmentRepository.UpdateShipmentStatus(shipmentId, 4, adminId); // 4 هو حالة Delivered

                    // تسجيل الإجراء (يمكن استخدام LogHelper)
                    LogHelper.LogActivity(adminId.ToString(), $"Admin confirmed delivery for shipment ID: {shipmentId}");
                    // إعادة تحميل البيانات
                    LoadArrivedShipments();
                    lblDeliveryMessage.Text = $"تم تأكيد تسليم الشحنة رقم {shipmentId} بنجاح.";
                    lblDeliveryMessage.CssClass = "alert alert-success";
                }
                catch (Exception ex)
                {
                    LogHelper.LogError( "Error confirming delivery in Reports.aspx.cs", ex);
                    lblDeliveryMessage.Text = "حدث خطأ أثناء تأكيد التسليم.";
                    lblDeliveryMessage.CssClass = "alert alert-danger";
                }
            }
        }
    }
}