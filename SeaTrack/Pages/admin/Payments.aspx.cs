using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SeaTrack.BLL;

public partial class Pages_Admin_Payments : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["role_name"] == null || Session["role_name"].ToString() != "Admin")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            LoadPendingInvoices();
        }
    }

    protected void btnPendingTab_Click(object sender, EventArgs e)
    {
        // تفعيل تبويب الفواتير المعلقة
        btnPendingTab.CssClass = "tab active";
        btnPaidTab.CssClass = "tab";
        pnlPendingInvoices.Visible = true;
        pnlPaidInvoices.Visible = false;
        
        LoadPendingInvoices();
    }

    protected void btnPaidTab_Click(object sender, EventArgs e)
    {
        // تفعيل تبويب الفواتير المدفوعة
        btnPendingTab.CssClass = "tab";
        btnPaidTab.CssClass = "tab active";
        pnlPendingInvoices.Visible = false;
        pnlPaidInvoices.Visible = true;
        
        LoadPaidInvoices();
    }

    private void LoadPendingInvoices()
    {
        try
        {
            DataTable dt = PaymentService.GetPendingInvoices();
            gvPendingInvoices.DataSource = dt;
            gvPendingInvoices.DataBind();
        }
        catch (Exception ex)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "حدث خطأ أثناء تحميل الفواتير المعلقة: " + ex.Message;
        }
    }

    private void LoadPaidInvoices()
    {
        try
        {
            DataTable dt = PaymentService.GetPaidInvoices();
            gvPaidInvoices.DataSource = dt;
            gvPaidInvoices.DataBind();
        }
        catch (Exception ex)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "حدث خطأ أثناء تحميل الفواتير المدفوعة: " + ex.Message;
        }
    }

    protected void gvPendingInvoices_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "IssueReceipt")
        {
            int invoiceId = Convert.ToInt32(e.CommandArgument);
            ShowPaymentModal(invoiceId);
        }
    }

    protected void gvPaidInvoices_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ViewReceipt")
        {
            int invoiceId = Convert.ToInt32(e.CommandArgument);
            // يمكن إضافة صفحة عرض الإيصال أو طباعته
            Response.Redirect($"PrintReceipt.aspx?invoiceId={invoiceId}");
        }
    }

    private void ShowPaymentModal(int invoiceId)
    {
        try
        {
            // الحصول على تفاصيل الفاتورة
            DataRow invoice = GetInvoiceDetails(invoiceId);
            
            if (invoice != null)
            {
                hfSelectedInvoiceId.Value = invoiceId.ToString();
                lblInvoiceNumber.Text = invoice["invoice_number"].ToString();
                lblCustomerName.Text = invoice["customer_name"].ToString();
                lblTotalAmount.Text = string.Format("{0:N2} ريال", invoice["total_amount"]);
                txtAmountPaid.Text = invoice["total_amount"].ToString();
                
                pnlPaymentModal.Visible = true;
                pnlPaymentModal.Style["display"] = "block";
            }
        }
        catch (Exception ex)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "حدث خطأ: " + ex.Message;
        }
    }

    private DataRow GetInvoiceDetails(int invoiceId)
    {
        string query = @"SELECT i.*, u.full_name as customer_name 
                        FROM Invoices i
                        INNER JOIN Users u ON i.customer_id = u.user_id
                        WHERE i.invoice_id = @invoice_id";
        
        System.Data.SqlClient.SqlParameter[] parameters = {
            new System.Data.SqlClient.SqlParameter("@invoice_id", invoiceId)
        };
        
        DataTable dt = SeaTrack.DAL.DatabaseHelper.ExecuteQuery(query, parameters);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    protected void btnCancelModal_Click(object sender, EventArgs e)
    {
        pnlPaymentModal.Visible = false;
        pnlPaymentModal.Style["display"] = "none";
        ClearModalFields();
    }

    protected void btnSubmitPayment_Click(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsValid) return;

            int invoiceId = Convert.ToInt32(hfSelectedInvoiceId.Value);
            decimal amountPaid = Convert.ToDecimal(txtAmountPaid.Text);
            string paymentMethod = ddlPaymentMethod.SelectedValue;
            string notes = txtNotes.Text.Trim();
            int createdBy = Convert.ToInt32(Session["UserId"]);

            // إنشاء إيصال الدفع
            int paymentId = PaymentService.CreatePaymentReceipt(invoiceId, amountPaid, paymentMethod, notes, createdBy);

            if (paymentId > 0)
            {
                // إرسال إشعار للعميل
                DataRow invoice = GetInvoiceDetails(invoiceId);
                if (invoice != null)
                {
                    int customerId = Convert.ToInt32(invoice["customer_id"]);
                    string receiptNumber = $"REC-{paymentId:D6}";
                    NotificationService.NotifyPaymentReceived(customerId, invoiceId, amountPaid, receiptNumber);
                }

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "تم إصدار إيصال الدفع بنجاح!";
                
                // إخفاء النافذة المنبثقة
                pnlPaymentModal.Visible = false;
                pnlPaymentModal.Style["display"] = "none";
                ClearModalFields();
                
                // إعادة تحميل البيانات
                LoadPendingInvoices();
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "فشل إصدار إيصال الدفع. يرجى المحاولة مرة أخرى.";
            }
        }
        catch (Exception ex)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "حدث خطأ: " + ex.Message;
        }
    }

    private void ClearModalFields()
    {
        hfSelectedInvoiceId.Value = "";
        lblInvoiceNumber.Text = "";
        lblCustomerName.Text = "";
        lblTotalAmount.Text = "";
        txtAmountPaid.Text = "";
        ddlPaymentMethod.SelectedIndex = 0;
        txtNotes.Text = "";
    }
}
