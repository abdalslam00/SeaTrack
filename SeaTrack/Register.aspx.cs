using SeaTrack.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SeaTrack
{
    public partial class Register : System.Web.UI.Page
    {

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            string fullName = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string country = txtCountry.Text.Trim();
            string address = txtAddress.Text.Trim();

            if (password != confirmPassword)
            {
                ShowError("كلمة المرور وتأكيد كلمة المرور غير متطابقتين");
                return;
            }

            string errorMessage;
            bool success = AuthenticationService.Register(username, email, password, fullName, phone, address, country, out errorMessage);

            if (success)
            {
                Response.Redirect("Login.aspx?registered=1");
            }
            else
            {
                ShowError(errorMessage);
            }
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            ltError.Text = message;
        }
    }

}