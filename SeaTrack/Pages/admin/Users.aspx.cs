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
    public partial class Users : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_ADMIN_NAME);
            if (!IsPostBack) LoadUsers();
        }

        private void LoadUsers()
        {
            DataTable users = UserRepository.GetAllUsers();
            gvUsers.DataSource = users;
            gvUsers.DataBind();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int userId = Convert.ToInt32(e.CommandArgument);
                bool success = UserRepository.ToggleUserStatus(userId);
                if (success)
                {
                    ShowMessage("تم تحديث حالة المستخدم بنجاح", "success");
                    LoadUsers();
                }
                else ShowMessage("فشل تحديث حالة المستخدم", "danger");
            }
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