<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SeaTrack.Pages.admin.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-12">
            <h2><i class="fas fa-tachometer-alt"></i> لوحة تحكم المسؤول</h2>
            <hr />
        </div>
    </div>

    <div class="row">
        <div class="col-md-3">
            <div class="card text-white bg-primary mb-3">
                <div class="card-body">
                    <h5 class="card-title"><i class="fas fa-route"></i> الرحلات النشطة</h5>
                    <h2><asp:Literal ID="ltActiveTrips" runat="server">0</asp:Literal></h2>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card text-white bg-success mb-3">
                <div class="card-body">
                    <h5 class="card-title"><i class="fas fa-box"></i> الشحنات قيد المعالجة</h5>
                    <h2><asp:Literal ID="ltPendingShipments" runat="server">0</asp:Literal></h2>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card text-white bg-warning mb-3">
                <div class="card-body">
                    <h5 class="card-title"><i class="fas fa-calendar-check"></i> الحجوزات المعلقة</h5>
                    <h2><asp:Literal ID="ltPendingBookings" runat="server">0</asp:Literal></h2>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card text-white bg-info mb-3">
                <div class="card-body">
                    <h5 class="card-title"><i class="fas fa-users"></i> إجمالي المستخدمين</h5>
                    <h2><asp:Literal ID="ltTotalUsers" runat="server">0</asp:Literal></h2>
                </div>
            </div>
        </div>
    </div>

    <div class="row mt-4">
        <div class="col-md-6">
            <div class="card">
                <div class="card-header bg-primary text-white">
                    <h5><i class="fas fa-route"></i> الرحلات القادمة</h5>
                </div>
                <div class="card-body">
                    <asp:GridView ID="gvUpcomingTrips" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="trip_code" HeaderText="رمز الرحلة" />
                            <asp:BoundField DataField="departure_port" HeaderText="من" />
                            <asp:BoundField DataField="arrival_port" HeaderText="إلى" />
                            <asp:BoundField DataField="departure_date" HeaderText="تاريخ الانطلاق" DataFormatString="{0:yyyy-MM-dd}" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <div class="col-md-6">
            <div class="card">
                <div class="card-header bg-warning text-dark">
                    <h5><i class="fas fa-tasks"></i> الحجوزات المعلقة</h5>
                </div>
                <div class="card-body">
                    <asp:GridView ID="gvPendingBookings" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="customer_name" HeaderText="العميل" />
                            <asp:BoundField DataField="trip_code" HeaderText="الرحلة" />
                            <asp:BoundField DataField="container_size" HeaderText="الحجم" />
                            <asp:BoundField DataField="created_at" HeaderText="التاريخ" DataFormatString="{0:yyyy-MM-dd}" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
