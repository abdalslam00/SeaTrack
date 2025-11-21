using SeaTrack.DAL;
using SeaTrack.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;

namespace SeaTrack.Pages.admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_ADMIN_NAME);

            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            // الرحلات النشطة
            DataTable activeTrips = TripRepository.GetActiveTrips();
            ltActiveTrips.Text = activeTrips.Rows.Count.ToString();

            // الشحنات قيد المعالجة
            DataTable pendingShipments = ShipmentRepository.GetUnscannedShipments();
            ltPendingShipments.Text = pendingShipments.Rows.Count.ToString();

            // الحجوزات المعلقة
            DataTable pendingBookings = BookingRepository.GetPendingBookings();
            ltPendingBookings.Text = pendingBookings.Rows.Count.ToString();

            // إجمالي المستخدمين
            DataTable allUsers = UserRepository.GetAllUsers();
            ltTotalUsers.Text = allUsers.Rows.Count.ToString();

            // الرحلات القادمة (أول 5)
            DataTable upcomingTrips = TripRepository.GetTripsByStatus(Constants.TRIP_STATUS_PLANNED);
            gvUpcomingTrips.DataSource = upcomingTrips;
            gvUpcomingTrips.DataBind();

            // الحجوزات المعلقة (أول 5)
            gvPendingBookings.DataSource = pendingBookings;
            gvPendingBookings.DataBind();
        }
    }

}