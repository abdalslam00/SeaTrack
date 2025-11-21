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
        }
    }

}