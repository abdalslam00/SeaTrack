<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/admin/Admin.Master" AutoEventWireup="true" CodeBehind="Ports.aspx.cs" Inherits="SeaTrack.Pages.admin.Ports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><i class="fas fa-map-marker-alt"></i> إدارة الموانئ</h2>
        <hr />

        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert"></asp:Panel>

        <div class="card mb-4">
            <div class="card-header bg-primary text-white">
                <h5>إضافة ميناء جديد</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-4 mb-3">
                        <label>اسم الميناء *</label>
                        <asp:TextBox ID="txtPortName" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label>الدولة *</label>
                        <asp:TextBox ID="txtCountry" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label>رمز الميناء</label>
                        <asp:TextBox ID="txtPortCode" runat="server" CssClass="form-control" placeholder="مثال: JEDDAH"></asp:TextBox>
                    </div>
                    <div class="col-12">
                        <asp:Button ID="btnAdd" runat="server" Text="إضافة ميناء" CssClass="btn btn-success" OnClick="btnAdd_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-header bg-info text-white">
                <h5>قائمة الموانئ</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvPorts" runat="server" CssClass="table table-striped" AutoGenerateColumns="false" OnRowCommand="gvPorts_RowCommand" DataKeyNames="port_id">
                    <Columns>
                        <asp:BoundField DataField="port_name" HeaderText="اسم الميناء" />
                        <asp:BoundField DataField="country" HeaderText="الدولة" />
                        <asp:BoundField DataField="port_code" HeaderText="الرمز" />
                        <asp:TemplateField HeaderText="الإجراءات">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-sm btn-danger" 
                                    CommandName="DeletePort" CommandArgument='<%# Eval("port_id") %>'
                                    OnClientClick="return confirm('هل أنت متأكد من حذف هذا الميناء؟');">
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
