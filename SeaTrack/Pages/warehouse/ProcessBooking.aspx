<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProcessBooking.aspx.cs" Inherits="SeaTrack.Pages.warehouse.ProcessBooking" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><i class="fas fa-tasks"></i> معالجة الحجوزات</h2>
        <hr />
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert"></asp:Panel>
        <div class="card">
            <div class="card-header bg-warning text-dark">
                <h5>الحجوزات المعلقة</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvPendingBookings" runat="server" CssClass="table table-striped" 
                    AutoGenerateColumns="false" OnRowCommand="gvPendingBookings_RowCommand" DataKeyNames="booking_id">
                    <Columns>
                        <asp:BoundField DataField="booking_code" HeaderText="رمز الحجز" />
                        <asp:BoundField DataField="customer_name" HeaderText="العميل" />
                        <asp:BoundField DataField="trip_code" HeaderText="الرحلة" />
                        <asp:BoundField DataField="container_type_name" HeaderText="نوع الحاوية" />
                        <asp:BoundField DataField="expected_weight" HeaderText="الوزن المتوقع (كجم)" />
                        <asp:BoundField DataField="booking_date" HeaderText="تاريخ الحجز" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:TemplateField HeaderText="الإجراءات">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnApprove" runat="server" CssClass="btn btn-sm btn-success" 
                                    CommandName="ApproveBooking" CommandArgument='<%# Eval("booking_id") %>'>
                                    <i class="fas fa-check"></i> قبول
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnReject" runat="server" CssClass="btn btn-sm btn-danger" 
                                    CommandName="RejectBooking" CommandArgument='<%# Eval("booking_id") %>'
                                    OnClientClick="return confirm('هل أنت متأكد من رفض هذا الحجز؟');">
                                    <i class="fas fa-times"></i> رفض
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center text-muted p-4">
                            <i class="fas fa-inbox fa-3x mb-3"></i>
                            <p>لا توجد حجوزات معلقة</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
