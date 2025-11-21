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
    public partial class Ports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_ADMIN_NAME);
            if (!IsPostBack)
            {
                LoadPorts();
            }
        }

        private void LoadPorts()
        {
            DataTable ports = PortRepository.GetAllPorts();
            gvPorts.DataSource = ports;
            gvPorts.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string portName = txtPortName.Text.Trim();
            string country = txtCountry.Text.Trim();
            string portCode = txtPortCode.Text.Trim();

            bool success = PortRepository.CreatePort(portName, country, portCode,"city");

            if (success)
            {
                ShowMessage("تم إضافة الميناء بنجاح", "success");
                ClearForm();
                LoadPorts();
            }
            else
            {
                ShowMessage("فشل إضافة الميناء", "danger");
            }
        }

        protected void gvPorts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeletePort")
            {
                int portId = Convert.ToInt32(e.CommandArgument);
                bool success = PortRepository.DeletePort(portId);

                if (success)
                {
                    ShowMessage("تم حذف الميناء بنجاح", "success");
                    LoadPorts();
                }
                else
                {
                    ShowMessage("فشل حذف الميناء", "danger");
                }
            }
        }

        private void ClearForm()
        {
            txtPortName.Text = "";
            txtCountry.Text = "";
            txtPortCode.Text = "";
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