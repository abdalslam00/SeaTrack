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
            if (!IsPostBack)
            {
                LoadBookings();
                // تعيين الحالة الأولية للواجهة
                UpdateDynamicUI(ddlShippingType.SelectedValue);
            }
        }

        protected void ddlShippingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDynamicUI(ddlShippingType.SelectedValue);
        }

        private void UpdateDynamicUI(string selectedValue)
        {
            if (selectedValue == "1") // شحن خاص (Private Container)
            {
                divBooking.Visible = true;
                divDestinationCountry.Visible = false;
                divDestinationCity.Visible = false;
            }
            else if (selectedValue == "2") // شحن عام (General Shipping)
            {
                divBooking.Visible = false;
                divDestinationCountry.Visible = true;
                divDestinationCity.Visible = true;
            }
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
            int shippingTypeId = Convert.ToInt32(ddlShippingType.SelectedValue);
            decimal weight = Convert.ToDecimal(txtWeight.Text);
            string description = txtDescription.Text.Trim();

            // تهيئة المتغيرات بناءً على نوع الشحن
            int? bookingId = null;
            string arrivalCountry = null;
            string arrivalCity = null;

            if (shippingTypeId == 1) // شحن خاص (Private Container)
            {
                // التحقق من اختيار الحجز
                if (ddlBooking.SelectedValue == "0")
                {
                    ShowMessage("الرجاء اختيار حجز للحاوية الخاصة.", "danger");
                    return;
                }
                bookingId = Convert.ToInt32(ddlBooking.SelectedValue);
                // الوجهة ستحدد لاحقاً من تفاصيل الرحلة المرتبطة بالحجز
            }
            else if (shippingTypeId == 2) // شحن عام (General Shipping)
            {
                // التحقق من إدخال الوجهة
                if (string.IsNullOrWhiteSpace(txtDestinationCountry.Text) || string.IsNullOrWhiteSpace(txtDestinationCity.Text))
                {
                    ShowMessage("الرجاء إدخال بلد ومدينة الوجهة للشحن العام.", "danger");
                    return;
                }
                arrivalCountry = txtDestinationCountry.Text.Trim();
                arrivalCity = txtDestinationCity.Text.Trim();
                // لا يوجد حجز مرتبط
            }

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
                arrivalCountry: arrivalCountry,   // الوجهة للشحن العام
                bookingRequestId: bookingId
            );

            if (newShipmentId > 0)
            {
                // --- الخطوة 5: توليد الفاتورة تلقائياً (متطلب: Financial Automation) ---
                // منطق حساب السعر: الوزن * 10 USD
                decimal pricePerKg = 10.00M;
                decimal totalAmount = weight * pricePerKg;
                string invoiceCode = $"INV-{shipmentCode}";

                // استدعاء دالة إنشاء الفاتورة
                // استدعاء دالة إنشاء الفاتورة
                int newInvoiceId = InvoiceRepository.CreateInvoice(
                    invoiceCode: invoiceCode,
                    customerId: customerId,
                    amount: totalAmount,
                    statusId: 1, // 1 = Unpaid
                    shipmentId: newShipmentId, // تأكد أنك طبقت تعديل shipmentId السابق
                                               // دمجنا العملة وتاريخ الاستحقاق في الملاحظات لأن الأعمدة غير موجودة في الجدول
                    notes: $"Currency: USD, Due Date: {DateTime.Now.AddDays(30):yyyy-MM-dd}"
                );

                if (newInvoiceId > 0)
                {
                    ShowMessage($"تم إنشاء الشحنة بنجاح! وتم توليد الفاتورة رقم {invoiceCode}.", "success");
                }
                else
                {
                    ShowMessage("تم إنشاء الشحنة بنجاح، ولكن فشل توليد الفاتورة تلقائياً.", "warning");
                }
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