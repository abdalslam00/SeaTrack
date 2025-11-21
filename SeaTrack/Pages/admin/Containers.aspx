<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/admin/Admin.Master" AutoEventWireup="true" CodeBehind="Containers.aspx.cs" Inherits="SeaTrack.Pages.admin.Containers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid mt-4">
        <div class="row">
            <div class="col-12">
                <div class="d-flex justify-content-between align-items-center mb-4">
                    <h2><i class="fas fa-box"></i> إدارة الحاويات</h2>
                    <asp:Button ID="btnAddContainer" runat="server" Text="إضافة حاوية جديدة" CssClass="btn btn-primary" OnClick="btnAddContainer_Click" />
                </div>

                <asp:Panel ID="pnlAddContainer" runat="server" Visible="false" CssClass="card mb-4">
                    <div class="card-header bg-primary text-white">
                        <h5 class="mb-0">إضافة/تعديل حاوية</h5>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="mb-3">
                                    <label>رمز الحاوية:</label>
                                    <asp:TextBox ID="txtContainerCode" runat="server" CssClass="form-control" placeholder="مثال: CONT-40-001" />
                                    <asp:RequiredFieldValidator ID="rfvContainerCode" runat="server" ControlToValidate="txtContainerCode" 
                                        ErrorMessage="رمز الحاوية مطلوب" CssClass="text-danger" Display="Dynamic" ValidationGroup="SaveContainer" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="mb-3">
                                    <label>نوع الحاوية:</label>
                                    <asp:DropDownList ID="ddlContainerType" runat="server" CssClass="form-select">
                                        <asp:ListItem Value="1">خاصة</asp:ListItem>
                                        <asp:ListItem Value="2">عامة</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="mb-3">
                                    <label>الحجم:</label>
                                    <asp:DropDownList ID="ddlSize" runat="server" CssClass="form-select" DataTextField="size_name" DataValueField="size_id" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="mb-3">
                                    <label>الوزن الأساسي (كجم):</label>
                                    <asp:TextBox ID="txtBaseWeight" runat="server" CssClass="form-control" TextMode="Number" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="mb-3">
                                    <label>الوزن الأقصى (كجم):</label>
                                    <asp:TextBox ID="txtMaxWeight" runat="server" CssClass="form-control" TextMode="Number" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="mb-3">
                                    <label>متاحة:</label>
                                    <asp:CheckBox ID="chkIsAvailable" runat="server" Checked="true" Text="نعم" CssClass="form-check" />
                                </div>
                            </div>
                        </div>
                        <div class="text-end">
                            <asp:Button ID="btnSaveContainer" runat="server" Text="حفظ" CssClass="btn btn-success" OnClick="btnSaveContainer_Click" ValidationGroup="SaveContainer" />
                            <asp:Button ID="btnCancelContainer" runat="server" Text="إلغاء" CssClass="btn btn-secondary" OnClick="btnCancelContainer_Click" CausesValidation="false" />
                        </div>
                    </div>
                </asp:Panel>

                <div class="card">
                    <div class="card-header">
                        <h5 class="mb-0">قائمة الحاويات</h5>
                    </div>
                    <div class="card-body">
                        <asp:GridView ID="gvContainers" runat="server" CssClass="table table-striped table-hover" AutoGenerateColumns="False" 
                            OnRowCommand="gvContainers_RowCommand" DataKeyNames="container_id">
                            <Columns>
                                <asp:BoundField DataField="container_code" HeaderText="رمز الحاوية" />
                                <asp:BoundField DataField="type_name" HeaderText="النوع" />
                                <asp:BoundField DataField="size_name" HeaderText="الحجم" />
                                <asp:BoundField DataField="base_weight_kg" HeaderText="الوزن الأساسي (كجم)" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="max_weight_kg" HeaderText="الوزن الأقصى (كجم)" DataFormatString="{0:N2}" />
                                <asp:TemplateField HeaderText="الحالة">
                                    <ItemTemplate>
                                        <span class='<%# Convert.ToBoolean(Eval("is_available")) ? "badge bg-success" : "badge bg-danger" %>'>
                                            <%# Convert.ToBoolean(Eval("is_available")) ? "متاحة" : "غير متاحة" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="الإجراءات">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditContainer" CommandArgument='<%# Eval("container_id") %>' 
                                            CssClass="btn btn-sm btn-warning" ToolTip="تعديل">
                                            <i class="fas fa-edit"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteContainer" CommandArgument='<%# Eval("container_id") %>' 
                                            CssClass="btn btn-sm btn-danger" ToolTip="حذف" OnClientClick="return confirm('هل أنت متأكد من حذف هذه الحاوية؟');">
                                            <i class="fas fa-trash"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="alert alert-info">لا توجد حاويات مسجلة</div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

