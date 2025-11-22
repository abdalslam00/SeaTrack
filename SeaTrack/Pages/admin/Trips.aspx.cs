using SeaTrack.DAL;
using SeaTrack.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SeaTrack.Pages.admin
{
    public partial class Trips : System.Web.UI.Page
    {
        // خاصية لتخزين ID الرحلة التي يتم تعديلها حالياً
        private int? EditTripId
        {
            get { return ViewState["EditTripId"] as int?; }
            set { ViewState["EditTripId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_ADMIN_NAME);
            if (!IsPostBack)
            {
                InitializeCulture();
                LoadDropDowns();
                LoadTrips();
            }
        }
        protected override void InitializeCulture()
        {
            CultureInfo ci = new CultureInfo("ar-EG");

            ci.DateTimeFormat.Calendar = new GregorianCalendar();

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            base.InitializeCulture();
        }
        private void LoadDropDowns()
        {
            DataTable ships = ShipRepository.GetAllShips();
            ddlShip.DataSource = ships;
            ddlShip.DataTextField = "ship_name";
            ddlShip.DataValueField = "ship_id";
            ddlShip.DataBind();
            ddlShip.Items.Insert(0, new ListItem("-- اختر الباخرة --", "0"));

            DataTable ports = PortRepository.GetAllPorts();
            ddlDeparturePort.DataSource = ports;
            ddlDeparturePort.DataTextField = "port_name";
            ddlDeparturePort.DataValueField = "port_id";
            ddlDeparturePort.DataBind();
            ddlDeparturePort.Items.Insert(0, new ListItem("-- اختر الميناء --", "0"));

            ddlArrivalPort.DataSource = ports;
            ddlArrivalPort.DataTextField = "port_name";
            ddlArrivalPort.DataValueField = "port_id";
            ddlArrivalPort.DataBind();
            ddlArrivalPort.Items.Insert(0, new ListItem("-- اختر الميناء --", "0"));
        }

        private void LoadTrips()
        {
            DataTable trips = TripRepository.GetAllTrips();
            gvTrips.DataSource = trips;
            gvTrips.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTripCode.Text) ||
                    ddlShip.SelectedValue == "0" ||
                    ddlDeparturePort.SelectedValue == "0" ||
                    ddlArrivalPort.SelectedValue == "0" ||
                    string.IsNullOrWhiteSpace(txtDepartureDate.Text) ||
                    string.IsNullOrWhiteSpace(txtExpectedArrival.Text))
                {
                    ShowMessage("يرجى تعبئة جميع الحقول المطلوبة", "danger");
                    return;
                }

                string tripCode = txtTripCode.Text.Trim();
                int shipId = Convert.ToInt32(ddlShip.SelectedValue);
                int departurePortId = Convert.ToInt32(ddlDeparturePort.SelectedValue);
                int arrivalPortId = Convert.ToInt32(ddlArrivalPort.SelectedValue);

                DateTime departureDate;
                DateTime expectedArrival;

                bool isDepDateValid = DateTime.TryParseExact(txtDepartureDate.Text, "yyyy-MM-dd",
                                        CultureInfo.InvariantCulture, DateTimeStyles.None, out departureDate);

                bool isArrDateValid = DateTime.TryParseExact(txtExpectedArrival.Text, "yyyy-MM-dd",
                                        CultureInfo.InvariantCulture, DateTimeStyles.None, out expectedArrival);

                if (!isDepDateValid || !isArrDateValid)
                {
                    ShowMessage("صيغة التاريخ غير صحيحة", "danger");
                    return;
                }

                if (expectedArrival <= departureDate)
                {
                    ShowMessage("تاريخ الوصول يجب أن يكون بعد تاريخ المغادرة", "danger");
                    return;
                }

                int? createdBy = SessionManager.GetUserId();
                if (createdBy == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                if (EditTripId.HasValue)
                {
                    int statusId = Convert.ToInt32(ddlStatus.SelectedValue);

                    bool success = TripRepository.UpdateTripFull(EditTripId.Value, tripCode, shipId, departurePortId, arrivalPortId, departureDate, expectedArrival, statusId);

                    if (success)
                    {
                        ShowMessage("تم تعديل الرحلة بنجاح", "success");
                        EditTripId = null;
                        btnAdd.Text = "إضافة رحلة";
                        btnAdd.CssClass = "btn btn-success w-100";
                    }
                    else
                    {
                        ShowMessage("فشل تعديل الرحلة", "danger");
                        return;
                    }
                }
                else
                {
                    int tripId = TripRepository.CreateTrip(tripCode, shipId, departurePortId, arrivalPortId,
                                                           departureDate, expectedArrival, createdBy.Value);
                    if (tripId > 0)
                    {
                        ShowMessage("تم إضافة الرحلة بنجاح", "success");
                    }
                    else
                    {
                        ShowMessage("فشل إضافة الرحلة", "danger");
                        return;
                    }
                }

                ClearForm();
                LoadTrips();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Trips.btnAdd_Click", ex);
                ShowMessage("حدث خطأ غير متوقع: " + ex.Message, "danger");
            }
        }

        protected void gvTrips_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int tripId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "DeleteTrip")
            {
                bool success = TripRepository.DeleteTrip(tripId);
                if (success)
                {
                    ShowMessage("تم حذف الرحلة بنجاح", "success");
                    LoadTrips();
                }
                else
                {
                    ShowMessage("فشل حذف الرحلة", "danger");
                }
            }
            else if (e.CommandName == "EditTrip") // <--- هذا هو الجزء الناقص الذي تم إصلاحه
            {
                LoadTripForEdit(tripId);
            }
        }

        private void LoadTripForEdit(int tripId)
        {
            DataTable dt = TripRepository.GetTripById(tripId);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                EditTripId = tripId;

                txtTripCode.Text = row["trip_code"].ToString();

                if (ddlShip.Items.FindByValue(row["ship_id"].ToString()) != null)
                    ddlShip.SelectedValue = row["ship_id"].ToString();

                if (ddlDeparturePort.Items.FindByValue(row["departure_port_id"].ToString()) != null)
                    ddlDeparturePort.SelectedValue = row["departure_port_id"].ToString();

                if (ddlArrivalPort.Items.FindByValue(row["arrival_port_id"].ToString()) != null)
                {
                    ddlArrivalPort.SelectedValue = row["arrival_port_id"].ToString();
                }

                // --- التعديل هنا لفرض التاريخ الميلادي ---
                if (row["departure_date"] != DBNull.Value)
                {
                    // استخدام CultureInfo.InvariantCulture يضمن أن التاريخ 2025 وليس 1446
                    DateTime depDate = Convert.ToDateTime(row["departure_date"]);
                    txtDepartureDate.Text = depDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                }

                if (row["expected_arrival_date"] != DBNull.Value)
                {
                    DateTime arrDate = Convert.ToDateTime(row["expected_arrival_date"]);
                    txtExpectedArrival.Text = arrDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                // ------------------------------------------

                if (ddlStatus.Items.FindByValue(row["status_id"].ToString()) != null)
                {
                    ddlStatus.SelectedValue = row["status_id"].ToString();
                }

                btnAdd.Text = "حفظ التعديلات";
                btnAdd.CssClass = "btn btn-warning w-100";
            }
        }

        private void ClearForm()
        {
            txtTripCode.Text = "";
            ddlShip.SelectedIndex = 0;
            ddlDeparturePort.SelectedIndex = 0;
            ddlArrivalPort.SelectedIndex = 0;
            txtDepartureDate.Text = "";
            txtExpectedArrival.Text = "";
            ddlStatus.SelectedIndex = 0;

            // إعادة تعيين زر الإضافة
            EditTripId = null;
            btnAdd.Text = "إضافة رحلة";
            btnAdd.CssClass = "btn btn-success w-100";
        }

        // ... (باقي الدوال المساعدة GetStatusClass و GetStatusText كما هي) ...
        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = $"alert alert-{type}";
            pnlMessage.Controls.Clear();
            pnlMessage.Controls.Add(new System.Web.UI.LiteralControl(message));
        }

        protected string GetStatusClass(object statusIdObj)
        {
            if (statusIdObj == null || statusIdObj == DBNull.Value) return "bg-secondary";
            int statusId = Convert.ToInt32(statusIdObj);
            switch (statusId)
            {
                case 1: return "bg-secondary";
                case 2: return "bg-warning text-dark";
                case 3: return "bg-primary";
                case 4: return "bg-success";
                default: return "bg-light text-dark";
            }
        }

        protected string GetStatusText(object statusIdObj)
        {
            if (statusIdObj == null || statusIdObj == DBNull.Value) return "غير معروف";
            int statusId = Convert.ToInt32(statusIdObj);
            switch (statusId)
            {
                case 1: return "مخططة";
                case 2: return "قيد التحميل";
                case 3: return "غادرت";
                case 4: return "وصلت";
                default: return "غير معروف";
            }
        }
    }
}