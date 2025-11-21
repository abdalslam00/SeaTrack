<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="SeaTrack.Pages.admin.Users" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><i class="fas fa-users"></i> إدارة المستخدمين</h2>
        <hr />
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert"></asp:Panel>
        <div class="card">
            <div class="card-header bg-info text-white">
                <h5>قائمة المستخدمين</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvUsers" runat="server" CssClass="table table-striped" AutoGenerateColumns="false" OnRowCommand="gvUsers_RowCommand" DataKeyNames="user_id">
                    <Columns>
                        <asp:BoundField DataField="username" HeaderText="اسم المستخدم" />
                        <asp:BoundField DataField="email" HeaderText="البريد الإلكتروني" />
                        <asp:BoundField DataField="full_name" HeaderText="الاسم الكامل" />
                        <asp:BoundField DataField="role_name" HeaderText="الدور" />
                        <asp:TemplateField HeaderText="الحالة">
                            <ItemTemplate>
                                <span class='badge <%# Convert.ToBoolean(Eval("is_active")) ? "bg-success" : "bg-danger" %>'>
                                    <%# Convert.ToBoolean(Eval("is_active")) ? "نشط" : "معطل" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="الإجراءات">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnToggle" runat="server" CssClass='<%# Convert.ToBoolean(Eval("is_active")) ? "btn btn-sm btn-warning" : "btn btn-sm btn-success" %>' 
                                    CommandName="ToggleStatus" CommandArgument='<%# Eval("user_id") %>'>
                                    <i class='<%# Convert.ToBoolean(Eval("is_active")) ? "fas fa-ban" : "fas fa-check" %>'></i> 
                                    <%# Convert.ToBoolean(Eval("is_active")) ? "تعطيل" : "تفعيل" %>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>

