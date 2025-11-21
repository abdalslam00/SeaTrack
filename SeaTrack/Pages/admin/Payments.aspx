<%@ Page Title="إدارة المدفوعات" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Payments.aspx.cs" Inherits="Pages_Admin_Payments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .payments-container {
            padding: 20px;
        }
        .section-title {
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 20px;
            color: #2c3e50;
        }
        .tabs {
            display: flex;
            gap: 10px;
            margin-bottom: 20px;
            border-bottom: 2px solid #ddd;
        }
        .tab {
            padding: 10px 20px;
            cursor: pointer;
            border: none;
            background: none;
            font-size: 16px;
            color: #666;
        }
        .tab.active {
            color: #3498db;
            border-bottom: 3px solid #3498db;
            font-weight: bold;
        }
        .invoice-grid {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }
        .invoice-grid th {
            background-color: #34495e;
            color: white;
            padding: 12px;
            text-align: right;
        }
        .invoice-grid td {
            padding: 10px;
            border-bottom: 1px solid #ddd;
        }
        .btn-issue-receipt {
            background-color: #27ae60;
            color: white;
            padding: 8px 15px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }
        .btn-issue-receipt:hover {
            background-color: #229954;
        }
        .btn-view-receipt {
            background-color: #3498db;
            color: white;
            padding: 8px 15px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }
        .status-badge {
            padding: 5px 10px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: bold;
        }
        .status-pending {
            background-color: #f39c12;
            color: white;
        }
        .status-paid {
            background-color: #27ae60;
            color: white;
        }
        .modal {
            display: none;
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
        }
        .modal-content {
            background-color: white;
            margin: 5% auto;
            padding: 30px;
            border-radius: 8px;
            width: 80%;
            max-width: 600px;
        }
        .form-group {
            margin-bottom: 15px;
        }
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
        }
        .form-group input, .form-group select, .form-group textarea {
            width: 100%;
            padding: 8px;
            border: 1px solid #ddd;
            border-radius: 4px;
        }
        .btn-submit {
            background-color: #27ae60;
            color: white;
            padding: 10px 20px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 16px;
        }
        .btn-cancel {
            background-color: #95a5a6;
            color: white;
            padding: 10px 20px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 16px;
            margin-right: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="payments-container">
        <h1 class="section-title">إدارة المدفوعات وإيصالات الدفع</h1>

        <div class="tabs">
            <asp:Button ID="btnPendingTab" runat="server" Text="الفواتير المعلقة" CssClass="tab active" OnClick="btnPendingTab_Click" />
            <asp:Button ID="btnPaidTab" runat="server" Text="الفواتير المدفوعة" CssClass="tab" OnClick="btnPaidTab_Click" />
        </div>

        <asp:Panel ID="pnlPendingInvoices" runat="server" Visible="true">
            <h3>الفواتير المعلقة (في انتظار الدفع)</h3>
            <asp:GridView ID="gvPendingInvoices" runat="server" CssClass="invoice-grid" AutoGenerateColumns="false" 
                          OnRowCommand="gvPendingInvoices_RowCommand">
                <Columns>
                    <asp:BoundField DataField="invoice_id" HeaderText="رقم الفاتورة" />
                    <asp:BoundField DataField="invoice_number" HeaderText="رقم المرجع" />
                    <asp:BoundField DataField="customer_name" HeaderText="اسم العميل" />
                    <asp:BoundField DataField="total_amount" HeaderText="المبلغ (ريال)" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="created_at" HeaderText="تاريخ الإصدار" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:TemplateField HeaderText="الإجراءات">
                        <ItemTemplate>
                            <asp:Button ID="btnIssueReceipt" runat="server" Text="إصدار إيصال دفع" 
                                        CssClass="btn-issue-receipt" CommandName="IssueReceipt" 
                                        CommandArgument='<%# Eval("invoice_id") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>

        <asp:Panel ID="pnlPaidInvoices" runat="server" Visible="false">
            <h3>الفواتير المدفوعة</h3>
            <asp:GridView ID="gvPaidInvoices" runat="server" CssClass="invoice-grid" AutoGenerateColumns="false"
                          OnRowCommand="gvPaidInvoices_RowCommand">
                <Columns>
                    <asp:BoundField DataField="invoice_id" HeaderText="رقم الفاتورة" />
                    <asp:BoundField DataField="invoice_number" HeaderText="رقم المرجع" />
                    <asp:BoundField DataField="customer_name" HeaderText="اسم العميل" />
                    <asp:BoundField DataField="amount_paid" HeaderText="المبلغ المدفوع" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="payment_method" HeaderText="طريقة الدفع" />
                    <asp:BoundField DataField="payment_date" HeaderText="تاريخ الدفع" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="receipt_number" HeaderText="رقم الإيصال" />
                    <asp:TemplateField HeaderText="الإجراءات">
                        <ItemTemplate>
                            <asp:Button ID="btnViewReceipt" runat="server" Text="عرض الإيصال" 
                                        CssClass="btn-view-receipt" CommandName="ViewReceipt" 
                                        CommandArgument='<%# Eval("invoice_id") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>

        <!-- Modal لإصدار إيصال الدفع -->
        <asp:Panel ID="pnlPaymentModal" runat="server" CssClass="modal" Visible="false">
            <div class="modal-content">
                <h2>إصدار إيصال دفع رسمي</h2>
                
                <div class="form-group">
                    <label>رقم الفاتورة:</label>
                    <asp:Label ID="lblInvoiceNumber" runat="server" />
                </div>

                <div class="form-group">
                    <label>اسم العميل:</label>
                    <asp:Label ID="lblCustomerName" runat="server" />
                </div>

                <div class="form-group">
                    <label>المبلغ الإجمالي:</label>
                    <asp:Label ID="lblTotalAmount" runat="server" />
                </div>

                <div class="form-group">
                    <label>المبلغ المدفوع: <span style="color:red;">*</span></label>
                    <asp:TextBox ID="txtAmountPaid" runat="server" TextMode="Number" step="0.01" />
                    <asp:RequiredFieldValidator ID="rfvAmountPaid" runat="server" 
                                                ControlToValidate="txtAmountPaid" 
                                                ErrorMessage="المبلغ المدفوع مطلوب" 
                                                ForeColor="Red" Display="Dynamic" />
                </div>

                <div class="form-group">
                    <label>طريقة الدفع: <span style="color:red;">*</span></label>
                    <asp:DropDownList ID="ddlPaymentMethod" runat="server">
                        <asp:ListItem Value="">-- اختر طريقة الدفع --</asp:ListItem>
                        <asp:ListItem Value="نقداً">نقداً</asp:ListItem>
                        <asp:ListItem Value="تحويل بنكي">تحويل بنكي</asp:ListItem>
                        <asp:ListItem Value="شيك">شيك</asp:ListItem>
                        <asp:ListItem Value="بطاقة ائتمان">بطاقة ائتمان</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvPaymentMethod" runat="server" 
                                                ControlToValidate="ddlPaymentMethod" 
                                                ErrorMessage="طريقة الدفع مطلوبة" 
                                                ForeColor="Red" Display="Dynamic" />
                </div>

                <div class="form-group">
                    <label>ملاحظات:</label>
                    <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="3" />
                </div>

                <div style="text-align: center; margin-top: 20px;">
                    <asp:Button ID="btnCancelModal" runat="server" Text="إلغاء" CssClass="btn-cancel" OnClick="btnCancelModal_Click" CausesValidation="false" />
                    <asp:Button ID="btnSubmitPayment" runat="server" Text="إصدار الإيصال" CssClass="btn-submit" OnClick="btnSubmitPayment_Click" />
                </div>

                <asp:HiddenField ID="hfSelectedInvoiceId" runat="server" />
            </div>
        </asp:Panel>

        <asp:Label ID="lblMessage" runat="server" ForeColor="Green" Font-Bold="true" />
    </div>
</asp:Content>
