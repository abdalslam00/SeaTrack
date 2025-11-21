using System;
using System.Data;
using System.Data.SqlClient;

namespace SeaTrack.DAL
{
    /// <summary>
    /// مستودع المستخدمين - للتعامل مع جدول Users
    /// </summary>
    public class UserRepository
    {
        /// <summary>
        /// الحصول على مستخدم بواسطة اسم المستخدم
        /// </summary>
        public static DataTable GetUserByUsername(string username)
        {
            string query = @"SELECT u.*, r.role_name 
                           FROM Users u 
                           JOIN Roles r ON u.role_id = r.role_id 
                           WHERE u.username = @username AND u.is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@username", username)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// الحصول على مستخدم بواسطة البريد الإلكتروني
        /// </summary>
        public static DataTable GetUserByEmail(string email)
        {
            string query = @"SELECT u.*, r.role_name 
                           FROM Users u 
                           JOIN Roles r ON u.role_id = r.role_id 
                           WHERE u.email = @email AND u.is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@email", email)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// الحصول على مستخدم بواسطة المعرف
        /// </summary>
        public static DataTable GetUserById(int userId)
        {
            string query = @"SELECT u.*, r.role_name 
                           FROM Users u 
                           JOIN Roles r ON u.role_id = r.role_id 
                           WHERE u.user_id = @userId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userId", userId)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// إنشاء مستخدم جديد
        /// </summary>
        public static int CreateUser(string username, string email, string passwordHash,
                                     string fullName, string phone, string address,
                                     string country, int roleId)
        {
            string query = @"INSERT INTO Users (username, email, password_hash, full_name, phone, address, country, role_id, is_active, created_at)
                           VALUES (@username, @email, @passwordHash, @fullName, @phone, @address, @country, @roleId, 1, GETDATE());
                           SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@username", username),
                new SqlParameter("@email", email),
                new SqlParameter("@passwordHash", passwordHash),
                new SqlParameter("@fullName", fullName),
                new SqlParameter("@phone", phone ?? (object)DBNull.Value),
                new SqlParameter("@address", address ?? (object)DBNull.Value),
                new SqlParameter("@country", country ?? (object)DBNull.Value),
                new SqlParameter("@roleId", roleId)
            };

            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// تحديث آخر تسجيل دخول
        /// </summary>
        public static bool UpdateLastLogin(int userId)
        {
            string query = "UPDATE Users SET last_login_at = GETDATE() WHERE user_id = @userId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userId", userId)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// الحصول على جميع المستخدمين
        /// </summary>
        public static DataTable GetAllUsers()
        {
            string query = @"SELECT u.*, r.role_name 
                           FROM Users u 
                           JOIN Roles r ON u.role_id = r.role_id 
                           ORDER BY u.created_at DESC";

            return DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// الحصول على المستخدمين حسب الدور
        /// </summary>
        public static DataTable GetUsersByRole(int roleId)
        {
            string query = @"SELECT u.*, r.role_name 
                           FROM Users u 
                           JOIN Roles r ON u.role_id = r.role_id 
                           WHERE u.role_id = @roleId AND u.is_active = 1
                           ORDER BY u.created_at DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@roleId", roleId)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// تحديث معلومات المستخدم
        /// </summary>
        public static bool UpdateUser(int userId, string fullName, string phone,
                                      string address, string country)
        {
            string query = @"UPDATE Users 
                           SET full_name = @fullName, 
                               phone = @phone, 
                               address = @address, 
                               country = @country,
                               updated_at = GETDATE()
                           WHERE user_id = @userId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userId", userId),
                new SqlParameter("@fullName", fullName),
                new SqlParameter("@phone", phone ?? (object)DBNull.Value),
                new SqlParameter("@address", address ?? (object)DBNull.Value),
                new SqlParameter("@country", country ?? (object)DBNull.Value)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// تحديث كلمة المرور
        /// </summary>
        public static bool UpdatePassword(int userId, string newPasswordHash)
        {
            string query = @"UPDATE Users 
                           SET password_hash = @passwordHash,
                               updated_at = GETDATE()
                           WHERE user_id = @userId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userId", userId),
                new SqlParameter("@passwordHash", newPasswordHash)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// تعطيل/تفعيل مستخدم
        /// </summary>
        public static bool ToggleUserStatus(int userId, bool isActive)
        {
            string query = @"UPDATE Users 
                           SET is_active = @isActive,
                               updated_at = GETDATE()
                           WHERE user_id = @userId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userId", userId),
                new SqlParameter("@isActive", isActive)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// التحقق من وجود اسم المستخدم
        /// </summary>
        public static bool UsernameExists(string username)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE username = @username";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@username", username)
            };

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            return count > 0;
        }

        /// <summary>
        /// التحقق من وجود البريد الإلكتروني
        /// </summary>
        public static bool EmailExists(string email)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE email = @email";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@email", email)
            };

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            return count > 0;
        }


        // دوال إضافية
        public static bool ToggleUserStatus(int userId)
        {
            string query = "UPDATE Users SET is_active = CASE WHEN is_active = 1 THEN 0 ELSE 1 END WHERE user_id = @user_id";
            SqlParameter[] parameters = {
            new SqlParameter("@user_id", userId)
        };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
