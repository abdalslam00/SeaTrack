using SeaTrack.BLL;
using SeaTrack.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;

namespace SeaTrack
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // إذا كان المستخدم مسجل دخول، إعادة توجيهه
                if (SessionManager.IsLoggedIn())
                {
                    RedirectToDashboard();
                }

                // عرض رسالة نجاح إذا كان قادماً من صفحة التسجيل
                if (Request.QueryString["registered"] == "1")
                {
                    ShowSuccess("تم التسجيل بنجاح! يمكنك الآن تسجيل الدخول.");
                }
            }
        }

        // في ملف SeaTrack.Login.aspx.cs

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            // قراءة قيمة الشيك بوكس
            bool rememberMe = chkRememberMe.Checked;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("يرجى إدخال اسم المستخدم وكلمة المرور");
                return;
            }

            string errorMessage;
            // تمرير rememberMe للدالة
            bool success = AuthenticationService.Login(username, password, rememberMe, out errorMessage);

            if (success)
            {
                RedirectToDashboard();
            }
            else
            {
                ShowError(errorMessage);
            }
        }
        private void RedirectToDashboard()
        {
            // الحصول على دور المستخدم من الجلسة التي تم إنشاؤها للتو
            string roleName = SessionManager.GetRoleName();
            string url; // لا داعي لإعطاء قيمة ابتدائية هنا

            // إلغاء التعليق عن هذا الجزء الهام
            if (roleName == Constants.ROLE_ADMIN_NAME)
            {
                url = "~/Pages/admin/Dashboard.aspx";
            }
            else if (roleName == Constants.ROLE_CUSTOMER_NAME)
            {
                url = "~/Pages/customer/Dashboard.aspx";
            }
            else if (roleName == Constants.ROLE_WAREHOUSE_NAME)
            {
                url = "~/Pages/warehouse/Dashboard.aspx";
            }
            else
            {
                // حالة احتياطية: إذا لم يكن للمستخدم دور معروف، وجهه للصفحة الرئيسية
                url = "~/Default.aspx";
            }

            // إعادة توجيه آمنة
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            ltError.Text = message;
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            ltSuccess.Text = message;
        }
    }

}