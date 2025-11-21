using System;
using System.Data;
using SeaTrack;
using SeaTrack.DAL;
using SeaTrack.Utilities;
namespace SeaTrack.Pages.Customer
{
    public partial class Invoices : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_CUSTOMER_NAME);
            if (!IsPostBack) LoadInvoices();
        }

        private void LoadInvoices()
        {
            int? userIdNullable = SessionManager.GetUserId();

            if (!userIdNullable.HasValue)
            {
                return;
            }

            int userId = userIdNullable.Value;

            DataTable invoices = BookingRepository.GetUserInvoices(userId);
            gvInvoices.DataSource = invoices;
            gvInvoices.DataBind();
        }
    }
}