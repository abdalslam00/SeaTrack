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
    public partial class Ships : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_ADMIN_NAME);
            if (!IsPostBack)
            {
                LoadShips();
            }
        }

        private void LoadShips()
        {
            DataTable ships = ShipRepository.GetAllShips();
            gvShips.DataSource = ships;
            gvShips.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string shipName = txtShipName.Text.Trim();
            string shipCode = txtShipCode.Text.Trim();
            int capacity = string.IsNullOrEmpty(txtCapacity.Text) ? 0 : Convert.ToInt32(txtCapacity.Text);

            decimal? maxWeight = string.IsNullOrEmpty(txtMaxWeight.Text)
                ? (decimal?)null
                : Convert.ToDecimal(txtMaxWeight.Text);

            string manufacturer = string.IsNullOrEmpty(txtManufacturer.Text)
                ? null
                : txtManufacturer.Text.Trim();

            int? yearBuilt = string.IsNullOrEmpty(txtYearBuilt.Text)
                ? (int?)null
                : Convert.ToInt32(txtYearBuilt.Text);

            bool success = ShipRepository.CreateShip(
                shipName,
                shipCode,
                capacity,
                maxWeight,
                manufacturer,
                yearBuilt
            );

            if (success)
            {
                ShowMessage("تم إضافة الباخرة بنجاح", "success");
                ClearForm();
                LoadShips();
            }
            else
            {
                ShowMessage("فشل إضافة الباخرة", "danger");
            }
        }

        protected void gvShips_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteShip")
            {
                int shipId = Convert.ToInt32(e.CommandArgument);
                bool success = ShipRepository.DeleteShip(shipId);

                if (success)
                {
                    ShowMessage("تم حذف الباخرة بنجاح", "success");
                    LoadShips();
                }
                else
                {
                    ShowMessage("فشل حذف الباخرة", "danger");
                }
            }
        }

        private void ClearForm()
        {
            txtShipName.Text = "";
            txtShipCode.Text = "";
            txtCapacity.Text = "";
            txtMaxWeight.Text = "";
            txtManufacturer.Text = "";
            txtYearBuilt.Text = "";
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