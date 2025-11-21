using SeaTrack.DAL;
using SeaTrack.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SeaTrack
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                LoadUserProfile();
            }
        }

        private void LoadUserProfile()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DataRow user = GetUserById(userId);

                if (user != null)
                {
                    string fullName = user["full_name"].ToString();
                    lblInitials.Text = GetInitials(fullName);

                    string roleName = user["role_name"].ToString();
                    lblRoleBadge.Text = GetRoleDisplayName(roleName);
                    lblRoleBadge.CssClass = "role-badge " + GetRoleCssClass(roleName);

                    txtUsername.Text = user["username"].ToString();
                    txtEmail.Text = user["email"].ToString();
                    txtFullName.Text = fullName;
                    txtPhone.Text = user["phone"]?.ToString() ?? "";
                    txtAddress.Text = user["address"]?.ToString() ?? "";
                    txtCreatedAt.Text = Convert.ToDateTime(user["created_at"]).ToString("dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء تحميل البيانات: " + ex.Message, false);
            }
        }

        private DataRow GetUserById(int userId)
        {
            string query = @"SELECT u.*, r.role_name 
                        FROM Users u
                        INNER JOIN Roles r ON u.role_id = r.role_id
                        WHERE u.user_id = @user_id";

            SqlParameter[] parameters = { new SqlParameter("@user_id", userId) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "؟";

            string[] parts = fullName.Trim().Split(' ');
            if (parts.Length >= 2)
            {
                return parts[0].Substring(0, 1) + parts[1].Substring(0, 1);
            }
            return fullName.Substring(0, Math.Min(2, fullName.Length));
        }

        private string GetRoleDisplayName(string roleName)
        {
            switch (roleName.ToLower())
            {
                case "admin":
                    return "مسؤول";
                case "customer":
                    return "عميل";
                case "warehouse":
                    return "موظف مخزن";
                default:
                    return roleName;
            }
        }

        private string GetRoleCssClass(string roleName)
        {
            switch (roleName.ToLower())
            {
                case "admin":
                    return "role-admin";
                case "customer":
                    return "role-customer";
                case "warehouse":
                    return "role-warehouse";
                default:
                    return "";
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid) return;

                int userId = Convert.ToInt32(Session["UserId"]);
                string email = txtEmail.Text.Trim();
                string fullName = txtFullName.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string address = txtAddress.Text.Trim();

                string query = @"UPDATE Users 
                            SET email = @email, 
                                full_name = @full_name, 
                                phone = @phone, 
                                address = @address,
                                updated_at = GETDATE()
                            WHERE user_id = @user_id";

                SqlParameter[] parameters = {
                new SqlParameter("@email", email),
                new SqlParameter("@full_name", fullName),
                new SqlParameter("@phone", phone),
                new SqlParameter("@address", address ?? (object)DBNull.Value),
                new SqlParameter("@user_id", userId)
            };

                DatabaseHelper.ExecuteNonQuery(query, parameters);

                // تحديث Session
                Session["FullName"] = fullName;

                ShowMessage("تم تحديث المعلومات بنجاح!", true);
                LoadUserProfile();
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء التحديث: " + ex.Message, false);
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                // التحقق من أن جميع حقول كلمة المرور مملوءة
                if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtNewPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
                {
                    ShowMessage("يرجى ملء جميع حقول كلمة المرور", false);
                    return;
                }

                // التحقق من تطابق كلمة المرور الجديدة
                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    ShowMessage("كلمتا المرور الجديدتان غير متطابقتين", false);
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                string currentPassword = txtCurrentPassword.Text;
                string newPassword = txtNewPassword.Text;

                // التحقق من كلمة المرور الحالية
                if (!VerifyCurrentPassword(userId, currentPassword))
                {
                    ShowMessage("كلمة المرور الحالية غير صحيحة", false);
                    return;
                }

                // تحديث كلمة المرور
                string hashedPassword = EncryptionHelper.HashPassword(newPassword);
                string query = @"UPDATE Users 
                            SET password_hash = @password_hash,
                                updated_at = GETDATE()
                            WHERE user_id = @user_id";

                SqlParameter[] parameters = {
                new SqlParameter("@password_hash", hashedPassword),
                new SqlParameter("@user_id", userId)
            };

                DatabaseHelper.ExecuteNonQuery(query, parameters);

                // مسح حقول كلمة المرور
                txtCurrentPassword.Text = "";
                txtNewPassword.Text = "";
                txtConfirmPassword.Text = "";

                ShowMessage("تم تغيير كلمة المرور بنجاح!", true);
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء تغيير كلمة المرور: " + ex.Message, false);
            }
        }

        private bool VerifyCurrentPassword(int userId, string currentPassword)
        {
            string query = "SELECT password_hash FROM Users WHERE user_id = @user_id";
            SqlParameter[] parameters = { new SqlParameter("@user_id", userId) };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                string storedHash = dt.Rows[0]["password_hash"].ToString();
                return EncryptionHelper.VerifyPassword(currentPassword, storedHash);
            }
            return false;
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;
            pnlMessage.CssClass = isSuccess ? "message message-success" : "message message-error";
        }
    }

}