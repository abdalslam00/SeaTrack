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
    public partial class ScanQR : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_WAREHOUSE_NAME);

            if (!IsPostBack)
            {
                LoadTrips();
                LoadScannedToday();
            }
        }

        private void LoadTrips()
        {
            DataTable trips = TripRepository.GetTripsByStatus(Constants.TRIP_STATUS_LOADING);
            ddlTrips.DataSource = trips;
            ddlTrips.DataTextField = "trip_code";
            ddlTrips.DataValueField = "trip_id";
            ddlTrips.DataBind();
            ddlTrips.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- اختر الرحلة --", "0"));
        }

        protected void ddlTrips_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTrips.SelectedValue != "0")
            {
                int tripId = Convert.ToInt32(ddlTrips.SelectedValue);
                DataTable containers = TripRepository.GetTripContainers(tripId);
                ddlContainers.DataSource = containers;
                ddlContainers.DataTextField = "container_code";
                ddlContainers.DataValueField = "trip_container_id";
                ddlContainers.DataBind();
                ddlContainers.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- اختر الحاوية --", "0"));
            }
        }

        protected void btnScan_Click(object sender, EventArgs e)
        {
            string qrCode = txtQRCode.Text.Trim();
            if (string.IsNullOrEmpty(qrCode) || ddlContainers.SelectedValue == "0")
            {
                ShowMessage("يرجى إدخال رمز QR واختيار الحاوية", "danger");
                return;
            }

            DataTable shipment = ShipmentRepository.GetShipmentByQRCode(qrCode);
            if (shipment.Rows.Count == 0)
            {
                ShowMessage("رمز QR غير صحيح", "danger");
                return;
            }

            int shipmentId = Convert.ToInt32(shipment.Rows[0]["shipment_id"]);
            int tripContainerId = Convert.ToInt32(ddlContainers.SelectedValue);
            int userId = SessionManager.GetUserId().Value;

            bool success = ShipmentRepository.AssignShipmentToContainer(shipmentId, tripContainerId, userId);
            if (success)
            {
                ShowMessage("تم مسح الشحنة بنجاح", "success");
                txtQRCode.Text = "";
                LoadScannedToday();
            }
            else
            {
                ShowMessage("فشل مسح الشحنة", "danger");
            }
        }

        private void LoadScannedToday()
        {
            DataTable scanned = ShipmentRepository.GetShipmentsByStatus(Constants.STATUS_SCANNED);
            gvScannedToday.DataSource = scanned;
            gvScannedToday.DataBind();
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