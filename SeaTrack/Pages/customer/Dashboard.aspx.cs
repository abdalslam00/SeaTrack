using System;
using System.Data;
using SeaTrack;
using SeaTrack.DAL;
using SeaTrack.Utilities;

namespace SeaTrack.Pages.Customer
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_CUSTOMER_NAME);

            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            int customerId = SessionManager.GetUserId().Value;
            ltCustomerName.Text = SessionManager.GetFullName();

            // حجوزاتي
            DataTable myBookings = BookingRepository.GetCustomerBookings(customerId);
            ltMyBookings.Text = myBookings.Rows.Count.ToString();

            // شحناتي
            DataTable myShipments = ShipmentRepository.GetCustomerShipments(customerId);
            ltMyShipments.Text = myShipments.Rows.Count.ToString();

            // الفواتير (افتراضي 0 - يمكن إضافة repository للفواتير)
            ltMyInvoices.Text = "0";

            // آخر الشحنات
            gvRecentShipments.DataSource = myShipments;
            gvRecentShipments.DataBind();
        }
    }
}