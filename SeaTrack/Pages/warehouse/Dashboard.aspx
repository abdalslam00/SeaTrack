<%@ Page Title="لوحة التحكم" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="Pages_Warehouse_Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-12">
            <h2><i class="fas fa-warehouse"></i> لوحة تحكم المستودع</h2>
            <hr />
        </div>
    </div>

    <div class="row">
        <div class="col-md-4">
            <div class="card text-white bg-warning mb-3">
                <div class="card-body">
                    <h5 class="card-title"><i class="fas fa-tasks"></i> حجوزات معلقة</h5>
                    <h2><asp:Literal ID="ltPendingBookings" runat="server">0</asp:Literal></h2>
                    <a href="ProcessBooking.aspx" class="btn btn-light btn-sm">معالجة</a>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card text-white bg-info mb-3">
                <div class="card-body">
                    <h5 class="card-title"><i class="fas fa-box"></i> شحنات غير ممسوحة</h5>
                    <h2><asp:Literal ID="ltUnscannedShipments" runat="server">0</asp:Literal></h2>
                    <a href="ScanQR.aspx" class="btn btn-light btn-sm">مسح QR</a>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card text-white bg-primary mb-3">
                <div class="card-body">
                    <h5 class="card-title"><i class="fas fa-route"></i> رحلات نشطة</h5>
                    <h2><asp:Literal ID="ltActiveTrips" runat="server">0</asp:Literal></h2>
                    <a href="Trips.aspx" class="btn btn-light btn-sm">عرض</a>
                </div>
            </div>
        </div>
    </div>

    <div class="row mt-4">
        <div class="col-12">
            <div class="card">
                <div class="card-header bg-primary text-white">
                    <h5><i class="fas fa-list"></i> المهام اليومية</h5>
                </div>
                <div class="card-body">
                    <div class="list-group">
                        <a href="ProcessBooking.aspx" class="list-group-item list-group-item-action">
                            <i class="fas fa-check-circle text-warning"></i> معالجة الحجوزات المعلقة
                            <span class="badge bg-warning float-end"><asp:Literal ID="ltPendingCount" runat="server">0</asp:Literal></span>
                        </a>
                        <a href="ScanQR.aspx" class="list-group-item list-group-item-action">
                            <i class="fas fa-qrcode text-info"></i> مسح رموز QR للشحنات
                            <span class="badge bg-info float-end"><asp:Literal ID="ltUnscannedCount" runat="server">0</asp:Literal></span>
                        </a>
                        <a href="Trips.aspx" class="list-group-item list-group-item-action">
                            <i class="fas fa-ship text-primary"></i> تحديث حالة الرحلات
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
