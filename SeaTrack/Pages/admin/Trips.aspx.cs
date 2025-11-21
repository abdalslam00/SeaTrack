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
    public partial class Trips : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_ADMIN_NAME);
            if (!IsPostBack)
            {
                LoadDropDowns();
                LoadTrips();
            }
        }

        private void LoadDropDowns()
        {
            DataTable ships = ShipRepository.GetAllShips();
            ddlShip.DataSource = ships;
            ddlShip.DataTextField = "ship_name";
            ddlShip.DataValueField = "ship_id";
            ddlShip.DataBind();
            ddlShip.Items.Insert(0, new ListItem("-- اختر الباخرة --", "0"));

            DataTable ports = PortRepository.GetAllPorts();
            ddlDeparturePort.DataSource = ports;
            ddlDeparturePort.DataTextField = "port_name";
            ddlDeparturePort.DataValueField = "port_id";
            ddlDeparturePort.DataBind();
            ddlDeparturePort.Items.Insert(0, new ListItem("-- اختر الميناء --", "0"));

            ddlArrivalPort.DataSource = ports;
            ddlArrivalPort.DataTextField = "port_name";
            ddlArrivalPort.DataValueField = "port_id";
            ddlArrivalPort.DataBind();
            ddlArrivalPort.Items.Insert(0, new ListItem("-- اختر الميناء --", "0"));
        }

        private void LoadTrips()
        {
            DataTable trips = TripRepository.GetAllTrips();
            gvTrips.DataSource = trips;
            gvTrips.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string tripCode = txtTripCode.Text.Trim();
            int shipId = Convert.ToInt32(ddlShip.SelectedValue);
            int departurePortId = Convert.ToInt32(ddlDeparturePort.SelectedValue);
            int arrivalPortId = Convert.ToInt32(ddlArrivalPort.SelectedValue);
            DateTime departureDate = Convert.ToDateTime(txtDepartureDate.Text);
            DateTime expectedArrival = Convert.ToDateTime(txtExpectedArrival.Text);

            // نستخدم ID الخاص بالمستخدم الحالي
            int? createdBy = SessionManager.GetUserId();

            int tripId = TripRepository.CreateTrip(tripCode, shipId, departurePortId, arrivalPortId,
                                                   departureDate, expectedArrival, createdBy.Value);

            if (tripId > 0)
            {
                ShowMessage("تم إضافة الرحلة بنجاح", "success");
                ClearForm();
                LoadTrips();
            }
            else
            {
                ShowMessage("فشل إضافة الرحلة", "danger");
            }
        }

        protected void gvTrips_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int tripId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "DeleteTrip")
            {
                bool success = TripRepository.DeleteTrip(tripId);
                if (success)
                {
                    ShowMessage("تم حذف الرحلة بنجاح", "success");
                    LoadTrips();
                }
                else
                {
                    ShowMessage("فشل حذف الرحلة", "danger");
                }
            }
        }

        private void ClearForm()
        {
            txtTripCode.Text = "";
            ddlShip.SelectedIndex = 0;
            ddlDeparturePort.SelectedIndex = 0;
            ddlArrivalPort.SelectedIndex = 0;
            txtDepartureDate.Text = "";
            txtExpectedArrival.Text = "";
            ddlStatus.SelectedIndex = 0;
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