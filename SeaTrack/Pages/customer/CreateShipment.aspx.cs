using SeaTrack.DAL;
using SeaTrack.Utilities;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace SeaTrack.Pages.customer
{
    public partial class CreateShipment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.RequireRole(Constants.ROLE_CUSTOMER_NAME);
            if (!IsPostBack) LoadBookings();
        }

        private void LoadBookings()
        {
            // --- حل خطأ int? ---
            int? userIdNullable = SessionManager.GetUserId();
            if (!userIdNullable.HasValue)
            {
                // لا تقم بتحميل أي شيء إذا لم يكن المستخدم مسجلاً
                return;
            }
            int userId = userIdNullable.Value;
            // ---------------------

            DataTable bookings = BookingRepository.GetApprovedBookings(userId);
            ddlBooking.DataSource = bookings;
            ddlBooking.DataTextField = "booking_info";
            ddlBooking.DataValueField = "booking_id";
            ddlBooking.DataBind();
            ddlBooking.Items.Insert(0, new ListItem("-- اختر الحجز --", "0"));
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            // --- الخطوة 1: الحصول على معرّف العميل (customerId) من الـ Session ---
            int? userIdNullable = SessionManager.GetUserId();
            if (!userIdNullable.HasValue)
            {
                ShowMessage("انتهت صلاحية الجلسة، يرجى تسجيل الدخول مرة أخرى.", "danger");
                return;
            }
            int customerId = userIdNullable.Value;

            // --- الخطوة 2: جمع البيانات المتوفرة من النموذج ---
            int bookingId = Convert.ToInt32(ddlBooking.SelectedValue);
            int shippingTypeId = Convert.ToInt32(ddlShippingType.SelectedValue);
            decimal weight = Convert.ToDecimal(txtWeight.Text);
            string description = txtDescription.Text.Trim();

            // ملاحظة: النموذج لا يحتوي على الطول والعرض والارتفاع، لذا سنرسلها كـ null

            // --- الخطوة 3: توليد البيانات المطلوبة التي ليست في النموذج ---
            string uniqueCode = Guid.NewGuid().ToString("N"); // إنشاء كود فريد للشحنة
            string shipmentCode = $"SH-{uniqueCode.Substring(0, 10).ToUpper()}";
            string qrCode = uniqueCode; // يمكن استخدام نفس الكود الفريد كبيانات للـ QR Code

            // --- الخطوة 4: استدعاء الدالة الصحيحة بالترتيب الصحيح لجميع المتغيرات ---
            int newShipmentId = ShipmentRepository.CreateShipment(
                shipmentCode: shipmentCode,
                qrCode: qrCode,
                description: description,
                weightKg: weight,
                lengthCm: null, // لا يوجد في النموذج
                widthCm: null,  // لا يوجد في النموذج
                heightCm: null, // لا يوجد في النموذج
                shippingTypeId: shippingTypeId,
                customerId: customerId, // حصلنا عليه من الـ Session
                departureCountry: null, // يمكن تطويره لاحقاً
                arrivalCountry: null,   // يمكن تطويره لاحقاً
                bookingRequestId: bookingId
            );

            if (newShipmentId > 0)
            {
                ShowMessage("تم إنشاء الشحنة بنجاح!", "success");
                ClearForm();
            }
            else
            {
                ShowMessage("فشل إنشاء الشحنة", "danger");
            }
        }
        private void ClearForm()
        {
            ddlBooking.SelectedIndex = 0;
            ddlShippingType.SelectedIndex = 0;
            txtWeight.Text = "";
            txtVolume.Text = "";
            txtDescription.Text = "";
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = $"alert alert-{type}";
            pnlMessage.Controls.Clear();
            pnlMessage.Controls.Add(new System.Web.UI.LiteralControl(message));
        }
    }
}