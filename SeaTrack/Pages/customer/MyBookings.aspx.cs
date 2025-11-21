using SeaTrack.DAL;
using SeaTrack.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SeaTrack.Pages.customer
{
    public partial class MyBookings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_CUSTOMER_NAME);
            if (!IsPostBack) LoadBookings();
        }

        private void LoadBookings()
        {
            int? userId = SessionManager.GetUserId();
            if (!userId.HasValue) return;

            DataTable bookings = BookingRepository.GetUserBookings(userId.Value);
            gvBookings.DataSource = bookings;
            gvBookings.DataBind();
        }

        protected string GetStatusClass(object statusId)
        {
            int status = Convert.ToInt32(statusId);
            return status == 1 ? "bg-warning" : status == 2 ? "bg-success" : "bg-danger";
        }
    }

}