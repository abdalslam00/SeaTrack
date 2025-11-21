using SeaTrack.BLL;
using SeaTrack.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SeaTrack
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    //// التحقق من تسجيل الدخول
            //    //if (!SessionManager.IsLoggedIn())
            //    //{
            //    //    Response.Redirect("~/Login.aspx");
            //    //    return;
            //    //}

            //    // عرض اسم المستخدم
            //    ltUserName.Text = SessionManager.GetFullName();

            //    // عرض القائمة حسب الدور
            //    string roleName = SessionManager.GetRoleName();

            //    if (roleName == Constants.ROLE_ADMIN_NAME)
            //    {
            //        phAdminMenu.Visible = true;
            //    }
            //    else if (roleName == Constants.ROLE_CUSTOMER_NAME)
            //    {
            //        phCustomerMenu.Visible = true;
            //    }
            //    else if (roleName == Constants.ROLE_WAREHOUSE_NAME)
            //    {
            //        phWarehouseMenu.Visible = true;
            //    }
            //}
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            AuthenticationService.Logout();
            Response.Redirect("~/Login.aspx");
        }
    }

}