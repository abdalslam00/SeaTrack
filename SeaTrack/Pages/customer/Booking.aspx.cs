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
    public partial class Booking : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_CUSTOMER_NAME);
            if (!IsPostBack) LoadTrips();
        }

        private void LoadTrips()
        {
            DataTable trips = TripRepository.GetAvailableTrips();
            ddlTrip.DataSource = trips;
            ddlTrip.DataTextField = "trip_info";
            ddlTrip.DataValueField = "trip_id";
            ddlTrip.DataBind();
            ddlTrip.Items.Insert(0, new ListItem("-- اختر الرحلة --", "0"));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // ------------------- الحل الأول: التعامل مع معرّف المستخدم -------------------
            int? userIdNullable = SessionManager.GetUserId();
            if (!userIdNullable.HasValue)
            {
                ShowMessage("لا يمكن إتمام الحجز. الرجاء تسجيل الدخول أولاً", "danger");
                return;
            }
            int userId = userIdNullable.Value;

            // ------------------- استخلاص القيم من النموذج -------------------
            int tripId = Convert.ToInt32(ddlTrip.SelectedValue);
            int containerTypeId = Convert.ToInt32(ddlContainerType.SelectedValue);

            int containerSize = 0; 
            if (containerTypeId == 1) 
            {
                containerSize = 40;
            }
            else if (containerTypeId == 2) 
            {
                containerSize = 20;
            }

            decimal weight = string.IsNullOrEmpty(txtWeight.Text) ? 0 : Convert.ToDecimal(txtWeight.Text);
            string cargoType = txtCargoType.Text.Trim();
            string notes = txtNotes.Text.Trim();


            int newBookingId = BookingRepository.CreateBookingRequest(userId, tripId, containerTypeId, containerSize, weight, cargoType, notes);

            // التحقق من نجاح العملية
            if (newBookingId > 0)
            {
                ShowMessage("تم إرسال الحجز بنجاح! سيتم مراجعته قريباً", "success");
                ClearForm();
            }
            else
            {
                ShowMessage("فشل إرسال الحجز", "danger");
            }
        }
        private void ClearForm()
        {
            ddlTrip.SelectedIndex = 0;
            ddlContainerType.SelectedIndex = 0;
            txtWeight.Text = "";
            txtCargoType.Text = "";
            txtNotes.Text = "";
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