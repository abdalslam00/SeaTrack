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