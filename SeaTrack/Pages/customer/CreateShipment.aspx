<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateShipment.aspx.cs" Inherits="SeaTrack.Pages.customer.CreateShipment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><i class="fas fa-box-open"></i> إنشاء شحنة جديدة</h2>
        <hr />
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert"></asp:Panel>
        <div class="card">
            <div class="card-header bg-primary text-white">
                <h5>تفاصيل الشحنة</h5>
            </div>
            <div class="card-body">
                <div class="row">

                    <div class="col-md-6 mb-3">
                        <label>نوع الشحن *</label>
                        <asp:DropDownList ID="ddlShippingType" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlShippingType_SelectedIndexChanged">
                            <asp:ListItem Value="1" Text="شحن خاص (حاوية خاصة)"></asp:ListItem>
                            <asp:ListItem Value="2" Text="شحن عام (بضائع عامة)"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label>الوزن الفعلي (كجم) *</label>
                        <asp:TextBox ID="txtWeight" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label>الحجم (متر مكعب)</label>
                        <asp:TextBox ID="txtVolume" runat="server" TextMode="Number" step="0.01" CssClass="form-control"></asp:TextBox>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="col-md-6 mb-3" id="divBooking" runat="server">
                                <label>الحجز *</label>
                                <asp:DropDownList ID="ddlBooking" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-6 mb-3" id="divDestinationCountry" runat="server" Visible="false">
                                <label>بلد الوجهة *</label>
                                <asp:TextBox ID="txtDestinationCountry" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-6 mb-3" id="divDestinationCity" runat="server" Visible="false">
                                <label>مدينة الوجهة *</label>
                                <asp:TextBox ID="txtDestinationCity" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlShippingType" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    </div>
                    <div class="col-12 mb-3">
                        <label>وصف البضاعة</label>
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-12">
                        <asp:Button ID="btnCreate" runat="server" Text="إنشاء شحنة" CssClass="btn btn-success btn-lg" OnClick="btnCreate_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
