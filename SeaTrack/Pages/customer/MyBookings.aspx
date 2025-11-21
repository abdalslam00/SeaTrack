<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyBookings.aspx.cs" Inherits="SeaTrack.Pages.customer.MyBookings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><i class="fas fa-list"></i> حجوزاتي</h2>
        <hr />
        <div class="card">
            <div class="card-body">
                <asp:GridView ID="gvBookings" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField DataField="booking_code" HeaderText="رمز الحجز" />
                        <asp:BoundField DataField="trip_code" HeaderText="الرحلة" />
                        <asp:BoundField DataField="container_type_name" HeaderText="نوع الحاوية" />
                        <asp:BoundField DataField="booking_date" HeaderText="تاريخ الحجز" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:TemplateField HeaderText="الحالة">
                            <ItemTemplate>
                                <span class='badge <%# GetStatusClass(Eval("status_id")) %>'>
                                    <%# Eval("status_name") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
