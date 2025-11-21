using SeaTrack.DAL;
using SeaTrack.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SeaTrack.Pages.warehouse
{
    public partial class ProcessBooking : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_WAREHOUSE_NAME);
            if (!IsPostBack) LoadPendingBookings();
        }

        private void LoadPendingBookings()
        {
            DataTable bookings = BookingRepository.GetPendingBookings();
            gvPendingBookings.DataSource = bookings;
            gvPendingBookings.DataBind();
        }

        protected void gvPendingBookings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int bookingId = Convert.ToInt32(e.CommandArgument);
            bool success = false;
            string message = "";

            if (e.CommandName == "ApproveBooking")
            {
                success = BookingRepository.ApproveBooking(bookingId);
                message = success ? "تم قبول الحجز بنجاح" : "فشل قبول الحجز";
            }
            else if (e.CommandName == "RejectBooking")
            {
                success = BookingRepository.RejectBooking(bookingId);
                message = success ? "تم رفض الحجز" : "فشل رفض الحجز";
            }

            ShowMessage(message, success ? "success" : "danger");
            LoadPendingBookings();
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = $"alert alert-{type}";
            pnlMessage.Controls.Clear();
            pnlMessage.Controls.Add(new System.Web.UI.LiteralControl(message));
        }
    }

}