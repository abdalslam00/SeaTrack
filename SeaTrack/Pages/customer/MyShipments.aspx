<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyShipments.aspx.cs" Inherits="SeaTrack.Pages.customer.MyShipments" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><i class="fas fa-boxes"></i> شحناتي</h2>
        <hr />
        <div class="card">
            <div class="card-body">
                <asp:GridView ID="gvShipments" runat="server" CssClass="table table-striped" AutoGenerateColumns="false" OnRowCommand="gvShipments_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="tracking_number" HeaderText="رقم التتبع" />
                        <asp:BoundField DataField="trip_code" HeaderText="الرحلة" />
                        <asp:BoundField DataField="weight" HeaderText="الوزن (كجم)" />
                        <asp:BoundField DataField="created_date" HeaderText="تاريخ الإنشاء" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:TemplateField HeaderText="الحالة">
                            <ItemTemplate>
                                <span class='badge bg-info'>
                                    <%# Eval("status_name") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="QR Code">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnViewQR" runat="server" CssClass="btn btn-sm btn-primary" 
                                    CommandName="ViewQR" CommandArgument='<%# Eval("shipment_id") %>'>
                                    <i class="fas fa-qrcode"></i> عرض
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        
        <asp:Panel ID="pnlQRCode" runat="server" Visible="false" CssClass="card mt-4">
            <div class="card-header bg-primary text-white">
                <h5>QR Code للشحنة</h5>
            </div>
            <div class="card-body text-center">
                <asp:Image ID="imgQRCode" runat="server" CssClass="img-fluid" Style="max-width: 300px;" />
                <p class="mt-3"><asp:Literal ID="ltTrackingNumber" runat="server"></asp:Literal></p>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

