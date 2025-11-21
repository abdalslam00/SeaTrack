<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Ships.aspx.cs" Inherits="SeaTrack.Pages.admin.Ships" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><i class="fas fa-anchor"></i> إدارة البواخر</h2>
        <hr />

        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert"></asp:Panel>

        
        <div class="card mb-4">
    <div class="card-header bg-primary text-white">
        <h5>إضافة باخرة جديدة</h5>
    </div>
    <div class="card-body">
        <div class="row">
            
            <div class="col-md-4 mb-3">
                <label>اسم الباخرة *</label>
                <asp:TextBox ID="txtShipName" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <div class="col-md-4 mb-3">
                <label>رمز الباخرة / رقم التسجيل *</label>
                <asp:TextBox ID="txtShipCode" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <div class="col-md-4 mb-3">
                <label>السعة (عدد الحاويات)</label>
                <asp:TextBox ID="txtCapacity" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
            </div>

            <div class="col-md-4 mb-3">
                <label>الوزن الأقصى (طن)</label>
                <asp:TextBox ID="txtMaxWeight" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
            </div>

            <div class="col-md-4 mb-3">
                <label>الشركة المصنعة</label>
                <asp:TextBox ID="txtManufacturer" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <div class="col-md-4 mb-3">
                <label>سنة الصنع</label>
                <asp:TextBox ID="txtYearBuilt" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
            </div>

            <div class="col-12">
                <asp:Button ID="btnAdd" runat="server" Text="إضافة باخرة" CssClass="btn btn-success" OnClick="btnAdd_Click" />
            </div>

        </div>
    </div>
</div>

        <div class="card">
            <div class="card-header bg-info text-white">
                <h5>قائمة البواخر</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvShips" runat="server" CssClass="table table-striped"
    AutoGenerateColumns="false" OnRowCommand="gvShips_RowCommand" DataKeyNames="ship_id">

    <Columns>
        <asp:BoundField DataField="ship_name" HeaderText="اسم الباخرة" />
        <asp:BoundField DataField="ship_code" HeaderText="رمز الباخرة" />
        <asp:BoundField DataField="capacity_containers" HeaderText="السعة" />
        <asp:BoundField DataField="max_weight_tons" HeaderText="الوزن الأقصى" />
        <asp:BoundField DataField="manufacturer" HeaderText="الشركة المصنعة" />
        <asp:BoundField DataField="year_built" HeaderText="سنة الصنع" />

        <asp:TemplateField HeaderText="الإجراءات">
            <ItemTemplate>
                <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-sm btn-danger"
                    CommandName="DeleteShip" CommandArgument='<%# Eval("ship_id") %>'
                    OnClientClick="return confirm('هل أنت متأكد من حذف هذه الباخرة؟');">
                    <i class="fas fa-trash"></i> حذف
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>

</asp:GridView>

            </div>
        </div>
    </div>
</asp:Content>