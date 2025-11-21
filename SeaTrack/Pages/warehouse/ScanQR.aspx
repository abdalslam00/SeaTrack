<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ScanQR.aspx.cs" Inherits="SeaTrack.Pages.warehouse.ScanQR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-12">
            <h2><i class="fas fa-qrcode"></i> مسح رمز QR للشحنة</h2>
            <hr />
        </div>
    </div>

    <div class="row">
        <div class="col-md-6">
            <div class="card">
                <div class="card-header bg-primary text-white">
                    <h5>إدخال رمز QR</h5>
                </div>
                <div class="card-body">
                    <asp:Panel ID="pnlMessage" runat="server" Visible="false"></asp:Panel>
                    
                    <div class="mb-3">
                        <label>رمز QR</label>
                        <asp:TextBox ID="txtQRCode" runat="server" CssClass="form-control" placeholder="امسح أو أدخل رمز QR"></asp:TextBox>
                    </div>

                    <div class="mb-3">
                        <label>اختر الرحلة</label>
                        <asp:DropDownList ID="ddlTrips" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTrips_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <div class="mb-3">
                        <label>اختر الحاوية</label>
                        <asp:DropDownList ID="ddlContainers" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>

                    <asp:Button ID="btnScan" runat="server" Text="تأكيد المسح" CssClass="btn btn-success w-100" OnClick="btnScan_Click" />
                </div>
            </div>
        </div>

        <div class="col-md-6">
            <div class="card">
                <div class="card-header bg-info text-white">
                    <h5>معلومات الشحنة</h5>
                </div>
                <div class="card-body">
                    <asp:Panel ID="pnlShipmentInfo" runat="server" Visible="false">
                        <table class="table">
                            <tr><th>رمز الشحنة:</th><td><asp:Literal ID="ltShipmentCode" runat="server"></asp:Literal></td></tr>
                            <tr><th>الوصف:</th><td><asp:Literal ID="ltDescription" runat="server"></asp:Literal></td></tr>
                            <tr><th>الوزن:</th><td><asp:Literal ID="ltWeight" runat="server"></asp:Literal> كجم</td></tr>
                            <tr><th>العميل:</th><td><asp:Literal ID="ltCustomer" runat="server"></asp:Literal></td></tr>
                            <tr><th>الحالة:</th><td><asp:Literal ID="ltStatus" runat="server"></asp:Literal></td></tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlNoInfo" runat="server" Visible="true">
                        <p class="text-muted text-center">أدخل رمز QR لعرض المعلومات</p>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>

    <div class="row mt-4">
        <div class="col-12">
            <div class="card">
                <div class="card-header bg-success text-white">
                    <h5>الشحنات الممسوحة اليوم</h5>
                </div>
                <div class="card-body">
                    <asp:GridView ID="gvScannedToday" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="shipment_code" HeaderText="رمز الشحنة" />
                            <asp:BoundField DataField="description" HeaderText="الوصف" />
                            <asp:BoundField DataField="weight_kg" HeaderText="الوزن (كجم)" />
                            <asp:BoundField DataField="scanned_at" HeaderText="وقت المسح" DataFormatString="{0:HH:mm}" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>