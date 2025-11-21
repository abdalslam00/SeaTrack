using System;
using System.Data;
using SeaTrack;
using SeaTrack.DAL;
using SeaTrack.Utilities;
namespace SeaTrack.Pages.Warehouse
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_WAREHOUSE_NAME);

            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            // الحجوزات المعلقة
            DataTable pendingBookings = BookingRepository.GetPendingBookings();
            ltPendingBookings.Text = pendingBookings.Rows.Count.ToString();
            ltPendingCount.Text = pendingBookings.Rows.Count.ToString();

            // الشحنات غير الممسوحة
            DataTable unscannedShipments = ShipmentRepository.GetUnscannedShipments();
            ltUnscannedShipments.Text = unscannedShipments.Rows.Count.ToString();
            ltUnscannedCount.Text = unscannedShipments.Rows.Count.ToString();

            // الرحلات النشطة
            DataTable activeTrips = TripRepository.GetActiveTrips();
            ltActiveTrips.Text = activeTrips.Rows.Count.ToString();
        }
    }
}