using SeaTrack.DAL;
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
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            string currentUsername = HttpContext.Current.User.Identity.Name;

            MessageBox.Show("currentUsername " + currentUsername + "isAuthenticated " + isAuthenticated);
            if (!IsPostBack)
            {
                // التحقق من حالة تسجيل الدخول باستخدام SessionManager
                if (SessionManager.IsLoggedIn())
                {
                    // المستخدم مسجل دخول
                    pnlGuestButtons.Visible = false;
                    pnlUserButtons.Visible = true;

                    // تحديد رابط لوحة التحكم حسب الدور باستخدام SessionManager
                    string userRole = SessionManager.GetRoleName(); // <-- التصحيح
                    switch ((userRole ?? "").ToLower())
                    {
                        case "admin":
                            hlDashboard.NavigateUrl = "~/Pages/admin/Dashboard.aspx";
                            break;

                        case "customer":
                            hlDashboard.NavigateUrl = "~/Pages/customer/Dashboard.aspx";
                            break;

                        case "warehouse":
                            hlDashboard.NavigateUrl = "~/Pages/warehouse/Dashboard.aspx";
                            break;

                        default:
                            // إذا كان المستخدم مسجلاً ولكن ليس له دور، يمكن توجيهه لصفحة خطأ أو تسجيل الخروج
                            hlDashboard.NavigateUrl = "~/Login.aspx";
                            break;
                    }
                }
                else
                {
                    // المستخدم غير مسجل دخول
                    pnlGuestButtons.Visible = true;
                    pnlUserButtons.Visible = false;
                }

                // تحميل الإحصائيات
                LoadStatistics();
            }
        }
        private void LoadStatistics()
        {
            try
            {
                // إجمالي الرحلات
                string tripsQuery = "SELECT COUNT(*) FROM Trips";
                object tripsResult = DatabaseHelper.ExecuteScalar(tripsQuery);
                lblTotalTrips.Text = tripsResult?.ToString() ?? "0";

                // إجمالي الشحنات
                string shipmentsQuery = "SELECT COUNT(*) FROM Shipments";
                object shipmentsResult = DatabaseHelper.ExecuteScalar(shipmentsQuery);
                lblTotalShipments.Text = shipmentsResult?.ToString() ?? "0";

                // إجمالي الحاويات
                string containersQuery = "SELECT COUNT(*) FROM Containers";
                object containersResult = DatabaseHelper.ExecuteScalar(containersQuery);
                lblTotalContainers.Text = containersResult?.ToString() ?? "0";

                // إجمالي العملاء
                string customersQuery = "SELECT COUNT(*) FROM Users WHERE role_id = (SELECT role_id FROM Roles WHERE role_name = 'Customer')";
                object customersResult = DatabaseHelper.ExecuteScalar(customersQuery);
                lblTotalCustomers.Text = customersResult?.ToString() ?? "0";
            }
            catch (Exception ex)
            {
                // في حالة حدوث خطأ، نعرض أصفار
                lblTotalTrips.Text = "0";
                lblTotalShipments.Text = "0";
                lblTotalContainers.Text = "0";
                lblTotalCustomers.Text = "0";

                LogHelper.LogError("Default.LoadStatistics", ex);
            }
        }
    }

}