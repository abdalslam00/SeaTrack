using SeaTrack.DAL;
using SeaTrack.Utilities;
using System;
using System.Data;
using System.Web.Security;

namespace SeaTrack.BLL
{
    /// <summary>
    /// خدمة المصادقة - للتعامل مع تسجيل الدخول والخروج
    /// </summary>
    public class AuthenticationService
    {
        // في ملف SeaTrack.BLL.AuthenticationService.cs

        // 1. عدل تعريف الدالة لتستقبل parameter جديد (bool rememberMe)
        public static bool Login(string username, string password, bool rememberMe, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // ... (نفس كود التحقق من المستخدم وكلمة المرور كما هو) ...
                DataTable dt = UserRepository.GetUserByUsername(username);
                if (dt.Rows.Count == 0) { /* ... */ return false; }
                DataRow user = dt.Rows[0];
                // ... (التحقق من الباسورد والتفعيل) ...

                // --- إصدار الرخص ---
                int userId = Convert.ToInt32(user["user_id"]);
                string fullName = user["full_name"].ToString();
                int roleId = Convert.ToInt32(user["role_id"]);
                string roleName = user["role_name"].ToString();
                string email = user["email"].ToString();

                SessionManager.SetUserSession(userId, username, fullName, roleId, roleName, email);


                FormsAuthentication.SetAuthCookie(username, rememberMe);

                UserRepository.UpdateLastLogin(userId);
                LogHelper.LogActivity(username, "تسجيل دخول ناجح");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static void Logout()
        {
            string username = SessionManager.GetUsername();
            if (!string.IsNullOrEmpty(username))
            {
                LogHelper.LogActivity(username, "تسجيل خروج");
            }

            // يجب إلغاء كلتا الرخصتين عند تسجيل الخروج

            // 1. إلغاء الرخصة الداخلية (الجلسة)
            SessionManager.ClearSession();

            // 2. إلغاء الرخصة الرسمية (كوكي المصادقة)
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// تسجيل مستخدم جديد (للعملاء فقط)
        /// </summary>
        public static bool Register(string username, string email, string password, 
                                   string fullName, string phone, string address, 
                                   string country, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // التحقق من وجود اسم المستخدم
                if (UserRepository.UsernameExists(username))
                {
                    errorMessage = "اسم المستخدم موجود مسبقاً";
                    return false;
                }

                // التحقق من وجود البريد الإلكتروني
                if (UserRepository.EmailExists(email))
                {
                    errorMessage = "البريد الإلكتروني مسجل مسبقاً";
                    return false;
                }

                // تشفير كلمة المرور
                string passwordHash = EncryptionHelper.HashPassword(password);

                // إنشاء المستخدم (دور العميل = 2)
                int userId = UserRepository.CreateUser(username, email, passwordHash, fullName, 
                                                      phone, address, country, Constants.ROLE_CUSTOMER);

                if (userId > 0)
                {
                    LogHelper.LogInfo($"تسجيل مستخدم جديد: {username}");
                    return true;
                }
                else
                {
                    errorMessage = "فشل إنشاء الحساب";
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Register", ex);
                errorMessage = "حدث خطأ أثناء التسجيل";
                return false;
            }
        }

        /// <summary>
        /// تغيير كلمة المرور
        /// </summary>
        public static bool ChangePassword(int userId, string oldPassword, string newPassword, 
                                         out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // الحصول على المستخدم
                DataTable dt = UserRepository.GetUserById(userId);

                if (dt.Rows.Count == 0)
                {
                    errorMessage = "المستخدم غير موجود";
                    return false;
                }

                DataRow user = dt.Rows[0];

                // التحقق من كلمة المرور القديمة
                string storedHash = user["password_hash"].ToString();
                if (!EncryptionHelper.VerifyPassword(oldPassword, storedHash))
                {
                    errorMessage = "كلمة المرور القديمة غير صحيحة";
                    return false;
                }

                // تشفير كلمة المرور الجديدة
                string newPasswordHash = EncryptionHelper.HashPassword(newPassword);

                // تحديث كلمة المرور
                bool success = UserRepository.UpdatePassword(userId, newPasswordHash);

                if (success)
                {
                    string username = user["username"].ToString();
                    LogHelper.LogActivity(username, "تغيير كلمة المرور");
                    return true;
                }
                else
                {
                    errorMessage = "فشل تحديث كلمة المرور";
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("ChangePassword", ex);
                errorMessage = "حدث خطأ أثناء تغيير كلمة المرور";
                return false;
            }
        }

        /// <summary>
        /// التحقق من الصلاحيات
        /// </summary>
        public static bool HasPermission(string requiredRole)
        {
            if (!SessionManager.IsLoggedIn())
                return false;

            string userRole = SessionManager.GetRoleName();
            return userRole?.Equals(requiredRole, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}
