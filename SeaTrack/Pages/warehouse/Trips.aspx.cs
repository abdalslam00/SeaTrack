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
    public partial class Trips : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_WAREHOUSE_NAME);
            if (!IsPostBack) LoadTrips();
        }

        private void LoadTrips()
        {
            DataTable trips = TripRepository.GetActiveTrips();
            gvTrips.DataSource = trips;
            gvTrips.DataBind();
            gvTrips.RowCommand += gvTrips_RowCommand;
        }

        protected void gvTrips_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ChangeStatus")
            {
                try
                {
                    string[] args = e.CommandArgument.ToString().Split(',');
                    int tripId = Convert.ToInt32(args[0]);
                    int newStatusId = Convert.ToInt32(args[1]);

                    // تطبيق منطق تغيير الحالة
                    TripRepository.UpdateTripStatus(tripId, newStatusId);

                    // إعادة تحميل البيانات بعد التحديث
                    LoadTrips();

                    // إظهار رسالة نجاح (يمكن استخدام Label أو ScriptManager)
                    // حالياً، سنعتمد على إعادة تحميل الصفحة كإشارة للنجاح
                }
                catch (Exception ex)
                {
                    // تسجيل الخطأ وإظهار رسالة للمستخدم
                    LogHelper.LogError( "Error changing trip status in Trips.aspx.cs", ex);
                    // يمكنك إضافة رسالة خطأ للمستخدم هنا
                }
            }
        }

        protected string GetStatusClass(object statusId)
        {
            int status = Convert.ToInt32(statusId);
            switch (status)
            {
                case 1: return "bg-secondary";
                case 2: return "bg-warning";
                case 3: return "bg-primary";
                case 4: return "bg-success";
                default: return "bg-secondary";
            }
        }
    }

}