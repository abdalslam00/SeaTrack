using System;
using System.Web;
using System.Web.Security;
using System.Data;
using SeaTrack.DAL;
using SeaTrack.Utilities;

namespace SeaTrack
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // كود بدء التشغيل
        }

        // يتم تنفيذ هذا الكود عند بدء جلسة جديدة
        void Session_Start(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                if (SessionManager.GetUserId() == null)
                {
                    string username = HttpContext.Current.User.Identity.Name;

                    try
                    {
                        DataTable dt = UserRepository.GetUserByUsername(username);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow user = dt.Rows[0];

                            int userId = Convert.ToInt32(user["user_id"]);
                            string fullName = user["full_name"].ToString();
                            int roleId = Convert.ToInt32(user["role_id"]);
                            string roleName = user["role_name"].ToString();
                            string email = user["email"].ToString();

                            SessionManager.SetUserSession(userId, username, fullName, roleId, roleName, email);
                        }
                        else
                        {
                            // إذا لم يتم العثور على المستخدم (ربما حُذف)، نسجل الخروج
                            FormsAuthentication.SignOut();
                        }
                    }
                    catch
                    {
                        // في حال حدوث خطأ، نسجل الخروج للأمان
                        FormsAuthentication.SignOut();
                    }
                }
            }
        }
    }
}