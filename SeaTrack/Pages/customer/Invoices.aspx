<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Invoices.aspx.cs" Inherits="SeaTrack.Pages.Customer.Invoices" %><asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><i class="fas fa-file-invoice-dollar"></i> فواتيري</h2>
        <hr />
        <div class="card">
            <div class="card-body">
                <asp:GridView ID="gvInvoices" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField DataField="invoice_number" HeaderText="رقم الفاتورة" />
                        <asp:BoundField DataField="booking_code" HeaderText="رمز الحجز" />
                        <asp:BoundField DataField="total_amount" HeaderText="المبلغ الإجمالي" DataFormatString="{0:N2} ريال" />
                        <asp:BoundField DataField="issue_date" HeaderText="تاريخ الإصدار" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:TemplateField HeaderText="الحالة">
                            <ItemTemplate>
                                <span class='badge <%# Convert.ToBoolean(Eval("is_paid")) ? "bg-success" : "bg-warning" %>'>
                                    <%# Convert.ToBoolean(Eval("is_paid")) ? "مدفوعة" : "معلقة" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="الإجراءات">
                            <ItemTemplate>
                                <a href='<%# "~/InvoiceDetails.aspx?id=" + Eval("invoice_id") %>' class="btn btn-sm btn-info" target="_blank">
                                    <i class="fas fa-eye"></i> عرض
                                </a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
