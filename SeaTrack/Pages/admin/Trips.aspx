<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/admin/Admin.Master" AutoEventWireup="true" CodeBehind="Trips.aspx.cs" Inherits="SeaTrack.Pages.admin.Trips" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <div class="row">
            <div class="col-12">
                <h2><i class="fas fa-ship"></i> إدارة الرحلات البحرية</h2>
                <hr />
            </div>
        </div>

        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert"></asp:Panel>

        <!-- إضافة رحلة جديدة -->
        <div class="card mb-4">
            <div class="card-header bg-primary text-white">
                <h5><i class="fas fa-plus-circle"></i> إضافة رحلة جديدة</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-3 mb-3">
                        <label>رمز الرحلة *</label>
                        <asp:TextBox ID="txtTripCode" runat="server" CssClass="form-control" placeholder="مثال: TR-2025-001"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label>الباخرة *</label>
                        <asp:DropDownList ID="ddlShip" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label>ميناء المغادرة *</label>
                        <asp:DropDownList ID="ddlDeparturePort" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label>ميناء الوصول *</label>
                        <asp:DropDownList ID="ddlArrivalPort" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label>تاريخ المغادرة *</label>
                        <asp:TextBox ID="txtDepartureDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label>تاريخ الوصول المتوقع *</label>
                        <asp:TextBox ID="txtExpectedArrival" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label>الحالة</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                            <asp:ListItem Value="1" Text="مخطط"></asp:ListItem>
                            <asp:ListItem Value="2" Text="قيد التحميل"></asp:ListItem>
                            <asp:ListItem Value="3" Text="منطلق"></asp:ListItem>
                            <asp:ListItem Value="4" Text="وصل"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label>&nbsp;</label>
                        <asp:Button ID="btnAdd" runat="server" Text="إضافة رحلة" CssClass="btn btn-success w-100" OnClick="btnAdd_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- قائمة الرحلات -->
        <div class="card">
            <div class="card-header bg-info text-white">
                <h5><i class="fas fa-list"></i> قائمة الرحلات</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvTrips" runat="server" CssClass="table table-striped table-hover" 
                    AutoGenerateColumns="false" OnRowCommand="gvTrips_RowCommand" DataKeyNames="trip_id">
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
                                    <%# GetStatusText(Eval("status_id")) %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="الإجراءات">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn btn-sm btn-warning" 
                                    CommandName="EditTrip" CommandArgument='<%# Eval("trip_id") %>'>
                                    <i class="fas fa-edit"></i> تعديل
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-sm btn-danger" 
                                    CommandName="DeleteTrip" CommandArgument='<%# Eval("trip_id") %>'
                                    OnClientClick="return confirmDelete('هل أنت متأكد من حذف هذه الرحلة؟');">
                                    <i class="fas fa-trash"></i> حذف
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center text-muted p-4">
                            <i class="fas fa-inbox fa-3x mb-3"></i>
                            <p>لا توجد رحلات مسجلة</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
