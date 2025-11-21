<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SeaTrack.Pages.Customer.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-12">
            <h2><i class="fas fa-home"></i> مرحباً، <asp:Literal ID="ltCustomerName" runat="server"></asp:Literal></h2>
            <hr />
        </div>
    </div>

    <div class="row">
        <div class="col-md-4">
            <div class="card text-white bg-primary mb-3">
                <div class="card-body">
                    <h5 class="card-title"><i class="fas fa-calendar-check"></i> حجوزاتي</h5>
                    <h2><asp:Literal ID="ltMyBookings" runat="server">0</asp:Literal></h2>
                    <a href="MyBookings.aspx" class="btn btn-light btn-sm">عرض الكل</a>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card text-white bg-success mb-3">
                <div class="card-body">
                    <h5 class="card-title"><i class="fas fa-box"></i> شحناتي</h5>
                    <h2><asp:Literal ID="ltMyShipments" runat="server">0</asp:Literal></h2>
                    <a href="MyShipments.aspx" class="btn btn-light btn-sm">عرض الكل</a>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card text-white bg-warning mb-3">
                <div class="card-body">
                    <h5 class="card-title"><i class="fas fa-file-invoice"></i> الفواتير</h5>
                    <h2><asp:Literal ID="ltMyInvoices" runat="server">0</asp:Literal></h2>
                    <a href="Invoices.aspx" class="btn btn-light btn-sm">عرض الكل</a>
                </div>
            </div>
        </div>
    </div>

    <div class="row mt-4">
        <div class="col-md-6">
            <div class="card">
                <div class="card-header bg-primary text-white">
                    <h5><i class="fas fa-shipping-fast"></i> آخر الشحنات</h5>
                </div>
                <div class="card-body">
                    <asp:GridView ID="gvRecentShipments" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="shipment_code" HeaderText="رمز الشحنة" />
                            <asp:BoundField DataField="description" HeaderText="الوصف" />
                            <asp:BoundField DataField="status" HeaderText="الحالة" />
                            <asp:BoundField DataField="created_at" HeaderText="التاريخ" DataFormatString="{0:yyyy-MM-dd}" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <div class="col-md-6">
            <div class="card">
                <div class="card-header bg-info text-white">
                    <h5><i class="fas fa-info-circle"></i> إجراءات سريعة</h5>
                </div>
                <div class="card-body">
                    <div class="d-grid gap-2">
                        <a href="Booking.aspx" class="btn btn-primary">
                            <i class="fas fa-calendar-plus"></i> حجز حاوية جديدة
                        </a>
                        <a href="CreateShipment.aspx" class="btn btn-success">
                            <i class="fas fa-box"></i> إنشاء شحنة جديدة
                        </a>
                        <a href="TrackShipment.aspx" class="btn btn-info">
                            <i class="fas fa-search"></i> تتبع شحنة
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
