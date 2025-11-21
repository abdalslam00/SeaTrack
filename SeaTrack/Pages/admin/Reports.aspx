<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/admin/Admin.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="SeaTrack.Pages.admin.Reports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><i class="fas fa-chart-bar"></i> التقارير والإحصائيات</h2>
        <hr />
        <div class="row">
            <div class="col-md-3 mb-4">
                <div class="card dashboard-card text-center">
                    <div class="card-body">
                        <i class="fas fa-ship fa-3x text-primary mb-3"></i>
                        <h3><asp:Literal ID="ltTotalTrips" runat="server" Text="0"></asp:Literal></h3>
                        <p>إجمالي الرحلات</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3 mb-4">
                <div class="card dashboard-card text-center">
                    <div class="card-body">
                        <i class="fas fa-box fa-3x text-success mb-3"></i>
                        <h3><asp:Literal ID="ltTotalShipments" runat="server" Text="0"></asp:Literal></h3>
                        <p>إجمالي الشحنات</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3 mb-4">
                <div class="card dashboard-card text-center">
                    <div class="card-body">
                        <i class="fas fa-users fa-3x text-info mb-3"></i>
                        <h3><asp:Literal ID="ltTotalCustomers" runat="server" Text="0"></asp:Literal></h3>
                        <p>إجمالي العملاء</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3 mb-4">
                <div class="card dashboard-card text-center">
                    <div class="card-body">
                        <i class="fas fa-calendar-check fa-3x text-warning mb-3"></i>
                        <h3><asp:Literal ID="ltPendingBookings" runat="server" Text="0"></asp:Literal></h3>
                        <p>حجوزات معلقة</p>
                    </div>
                </div>
            </div>
        </div>
        <div class="card">
            <div class="card-header bg-primary text-white">
                <h5>الرحلات الأخيرة</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvRecentTrips" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField DataField="trip_code" HeaderText="رمز الرحلة" />
                        <asp:BoundField DataField="ship_name" HeaderText="الباخرة" />
                        <asp:BoundField DataField="departure_port" HeaderText="من" />
                        <asp:BoundField DataField="arrival_port" HeaderText="إلى" />
                        <asp:BoundField DataField="departure_date" HeaderText="تاريخ المغادرة" DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>

