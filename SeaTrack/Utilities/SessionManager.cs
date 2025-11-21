using System;
using System.Web;
using System.Windows;

namespace SeaTrack.Utilities
{
    /// <summary>
    /// مدير الجلسات - للتعامل مع بيانات الجلسة
    /// </summary>
    public class SessionManager
    {
        // مفاتيح الجلسة
        private const string USER_ID_KEY = "UserId";
        private const string USERNAME_KEY = "Username";
        private const string FULL_NAME_KEY = "FullName";
        private const string ROLE_ID_KEY = "RoleId";
        private const string ROLE_NAME_KEY = "RoleName";
        private const string EMAIL_KEY = "Email";

        /// <summary>
        /// حفظ بيانات المستخدم في الجلسة
        /// </summary>
        public static void SetUserSession(int userId, string username, string fullName, 
                                         int roleId, string roleName, string email)
        {
            HttpContext.Current.Session[USER_ID_KEY] = userId;
            HttpContext.Current.Session[USERNAME_KEY] = username;
            HttpContext.Current.Session[FULL_NAME_KEY] = fullName;
            HttpContext.Current.Session[ROLE_ID_KEY] = roleId;
            HttpContext.Current.Session[ROLE_NAME_KEY] = roleName;
            HttpContext.Current.Session[EMAIL_KEY] = email;
        }

        /// <summary>
        /// الحصول على معرف المستخدم
        /// </summary>
        public static int? GetUserId()
        {
            return HttpContext.Current.Session[USER_ID_KEY] as int?;
        }

        /// <summary>
        /// الحصول على اسم المستخدم
        /// </summary>
        public static string GetUsername()
        {
            return HttpContext.Current.Session[USERNAME_KEY] as string;
        }

        /// <summary>
        /// الحصول على الاسم الكامل
        /// </summary>
        public static string GetFullName()
        {
            return HttpContext.Current.Session[FULL_NAME_KEY] as string;
        }

        /// <summary>
        /// الحصول على معرف الدور
        /// </summary>
        public static int? GetRoleId()
        {
            return HttpContext.Current.Session[ROLE_ID_KEY] as int?;
        }

        /// <summary>
        /// الحصول على اسم الدور
        /// </summary>
        public static string GetRoleName()
        {
            return HttpContext.Current.Session[ROLE_NAME_KEY] as string;
        }

        /// <summary>
        /// الحصول على البريد الإلكتروني
        /// </summary>
        public static string GetEmail()
        {
            return HttpContext.Current.Session[EMAIL_KEY] as string;
        }

        /// <summary>
        /// التحقق من تسجيل الدخول
        /// </summary>
        public static bool IsLoggedIn()
        {
            return GetUserId().HasValue;
        }

        /// <summary>
        /// التحقق من دور المستخدم
        /// </summary>
        public static bool IsInRole(string roleName)
        {
            return GetRoleName()?.Equals(roleName, StringComparison.OrdinalIgnoreCase) == true;
        }
        /// <summary>
        /// التحقق مما إذا كان المستخدم مسؤولاً
        /// </summary>
        public static bool IsAdmin()
        {
            return IsInRole("Admin");
        }
        /// <summary>
        /// مسح الجلسة (تسجيل الخروج)
        /// </summary>
        public static void ClearSession()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }

        /// <summary>
        /// إعادة توجيه إلى صفحة تسجيل الدخول
        /// </summary>
        public static void RedirectToLogin()
        {
            //HttpContext.Current.Response.Redirect("~/Login.aspx");
        }

        /// <summary>
        /// التحقق من الصلاحية وإعادة التوجيه إذا لزم الأمر
        /// </summary>
        public static void RequireLogin()
        {
            if (!IsLoggedIn())
            {
                RedirectToLogin();
            }
        }

        /// <summary>
        /// التحقق من دور معين وإعادة التوجيه إذا لم يكن مصرحاً
        /// </summary>
        public static void RequireRole(string roleName)
        {
            RequireLogin();
            if (!IsInRole(roleName))
            {
                HttpContext.Current.Response.Redirect("~/Unauthorized.aspx");
            }
        }
    }
}
