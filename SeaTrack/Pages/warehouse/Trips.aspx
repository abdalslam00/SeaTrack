<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Trips.aspx.cs" Inherits="SeaTrack.Pages.warehouse.Trips" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><i class="fas fa-ship"></i> الرحلات الحالية</h2>
        <hr />
        <div class="card">
            <div class="card-header bg-info text-white">
                <h5>قائمة الرحلات</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvTrips" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField DataField="trip_code" HeaderText="رمز الرحلة" />
                        <asp:BoundField DataField="ship_name" HeaderText="الباخرة" />
                        <asp:BoundField DataField="departure_port" HeaderText="من" />
                        <asp:BoundField DataField="arrival_port" HeaderText="إلى" />
                        <asp:BoundField DataField="departure_date" HeaderText="تاريخ المغادرة" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="expected_arrival_date" HeaderText="الوصول المتوقع" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:TemplateField HeaderText="الحالة">
                            <ItemTemplate>
                                <span class='badge <%# GetStatusClass(Eval("status_id")) %>'>
                                    <%# Eval("status_name") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="الشحنات">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnViewShipments" runat="server" CssClass="btn btn-sm btn-primary" 
                                    PostBackUrl='<%# "~/Pages/warehouse/ViewShipments.aspx?trip_id=" + Eval("trip_id") %>'>
                                    <i class="fas fa-boxes"></i> عرض الشحنات
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center text-muted p-4">
                            <i class="fas fa-inbox fa-3x mb-3"></i>
                            <p>لا توجد رحلات</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
