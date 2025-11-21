using SeaTrack.BLL;
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
    public partial class MyShipments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_CUSTOMER_NAME);
            if (!IsPostBack) LoadShipments();
        }

        private void LoadShipments()
        {
            int? userId = SessionManager.GetUserId();
            DataTable shipments = ShipmentRepository.GetUserShipments(userId.Value);
            if (!userId.HasValue) return;

            gvShipments.DataSource = shipments;
            gvShipments.DataBind();
        }

        protected void gvShipments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewQR")
            {
                int shipmentId = Convert.ToInt32(e.CommandArgument);
                DataTable shipment = ShipmentRepository.GetShipmentById(shipmentId);

                if (shipment.Rows.Count > 0)
                {
                    string trackingNumber = shipment.Rows[0]["tracking_number"].ToString();
                    string qrPath = QRCodeService.GenerateShipmentQRCode(trackingNumber, shipmentId);

                    imgQRCode.ImageUrl = qrPath;
                    ltTrackingNumber.Text = "رقم التتبع: " + trackingNumber;
                    pnlQRCode.Visible = true;
                }
            }
        }
    }

}