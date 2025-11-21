<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Booking.aspx.cs" Inherits="SeaTrack.Pages.customer.Booking" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><i class="fas fa-calendar-plus"></i> حجز حاوية جديدة</h2>
        <hr />
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert"></asp:Panel>
        <div class="card">
            <div class="card-header bg-primary text-white">
                <h5>تفاصيل الحجز</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label>الرحلة *</label>
                        <asp:DropDownList ID="ddlTrip" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label>نوع الحاوية *</label>
                        <asp:DropDownList ID="ddlContainerType" runat="server" CssClass="form-select">
                            <asp:ListItem Value="1" Text="حاوية كاملة 40 قدم"></asp:ListItem>
                            <asp:ListItem Value="2" Text="نصف حاوية 20 قدم"></asp:ListItem>
                            <asp:ListItem Value="3" Text="شحن عام"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label>الوزن المتوقع (كجم)</label>
                        <asp:TextBox ID="txtWeight" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label>نوع البضاعة</label>
                        <asp:TextBox ID="txtCargoType" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-12 mb-3">
                        <label>ملاحظات</label>
                        <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-12">
                        <asp:Button ID="btnSubmit" runat="server" Text="إرسال الحجز" CssClass="btn btn-success btn-lg" OnClick="btnSubmit_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
