<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="SeaTrack.Profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .profile-container {
            max-width: 800px;
            margin: 40px auto;
            padding: 30px;
            background-color: white;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        .profile-header {
            text-align: center;
            margin-bottom: 30px;
            padding-bottom: 20px;
            border-bottom: 2px solid #3498db;
        }
        .profile-header h1 {
            color: #2c3e50;
            margin-bottom: 10px;
        }
        .profile-avatar {
            width: 120px;
            height: 120px;
            border-radius: 50%;
            background-color: #3498db;
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 48px;
            font-weight: bold;
            margin: 0 auto 20px;
        }
        .form-section {
            margin-bottom: 30px;
        }
        .form-section h3 {
            color: #34495e;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 1px solid #ddd;
        }
        .form-group {
            margin-bottom: 20px;
        }
        .form-group label {
            display: block;
            margin-bottom: 8px;
            font-weight: bold;
            color: #555;
        }
        .form-group input, .form-group select {
            width: 100%;
            padding: 12px;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 14px;
        }
        .form-group input:focus, .form-group select:focus {
            outline: none;
            border-color: #3498db;
            box-shadow: 0 0 5px rgba(52, 152, 219, 0.3);
        }
        .form-group .readonly {
            background-color: #f5f5f5;
            cursor: not-allowed;
        }
        .btn-update {
            background-color: #27ae60;
            color: white;
            padding: 12px 30px;
            border: none;
            border-radius: 4px;
            font-size: 16px;
            cursor: pointer;
            width: 100%;
            margin-top: 20px;
        }
        .btn-update:hover {
            background-color: #229954;
        }
        .btn-change-password {
            background-color: #e67e22;
            color: white;
            padding: 12px 30px;
            border: none;
            border-radius: 4px;
            font-size: 16px;
            cursor: pointer;
            width: 100%;
            margin-top: 10px;
        }
        .btn-change-password:hover {
            background-color: #d35400;
        }
        .role-badge {
            display: inline-block;
            padding: 5px 15px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: bold;
            margin-top: 10px;
        }
        .role-admin {
            background-color: #e74c3c;
            color: white;
        }
        .role-customer {
            background-color: #3498db;
            color: white;
        }
        .role-warehouse {
            background-color: #f39c12;
            color: white;
        }
        .message {
            padding: 15px;
            border-radius: 4px;
            margin-bottom: 20px;
            text-align: center;
        }
        .message-success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        .message-error {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="profile-container">
        <div class="profile-header">
            <div class="profile-avatar">
                <asp:Label ID="lblInitials" runat="server" />
            </div>
            <h1>الملف الشخصي</h1>
            <asp:Label ID="lblRoleBadge" runat="server" CssClass="role-badge" />
        </div>

        <asp:Panel ID="pnlMessage" runat="server" Visible="false">
            <asp:Label ID="lblMessage" runat="server" />
        </asp:Panel>

        <div class="form-section">
            <h3>المعلومات الأساسية</h3>
            
            <div class="form-group">
                <label>اسم المستخدم:</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="readonly" ReadOnly="true" />
            </div>

            <div class="form-group">
                <label>البريد الإلكتروني:</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" />
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                                            ControlToValidate="txtEmail" 
                                            ErrorMessage="البريد الإلكتروني مطلوب" 
                                            ForeColor="Red" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                                                ControlToValidate="txtEmail" 
                                                ErrorMessage="صيغة البريد الإلكتروني غير صحيحة" 
                                                ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$" 
                                                ForeColor="Red" Display="Dynamic" />
            </div>

            <div class="form-group">
                <label>الاسم الكامل:</label>
                <asp:TextBox ID="txtFullName" runat="server" />
                <asp:RequiredFieldValidator ID="rfvFullName" runat="server" 
                                            ControlToValidate="txtFullName" 
                                            ErrorMessage="الاسم الكامل مطلوب" 
                                            ForeColor="Red" Display="Dynamic" />
            </div>

            <div class="form-group">
                <label>رقم الهاتف:</label>
                <asp:TextBox ID="txtPhone" runat="server" />
                <asp:RequiredFieldValidator ID="rfvPhone" runat="server" 
                                            ControlToValidate="txtPhone" 
                                            ErrorMessage="رقم الهاتف مطلوب" 
                                            ForeColor="Red" Display="Dynamic" />
            </div>

            <div class="form-group">
                <label>العنوان:</label>
                <asp:TextBox ID="txtAddress" runat="server" />
            </div>

            <asp:Button ID="btnUpdate" runat="server" Text="تحديث المعلومات" CssClass="btn-update" OnClick="btnUpdate_Click" />
        </div>

        <div class="form-section">
            <h3>تغيير كلمة المرور</h3>
            
            <div class="form-group">
                <label>كلمة المرور الحالية:</label>
                <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" />
            </div>

            <div class="form-group">
                <label>كلمة المرور الجديدة:</label>
                <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" />
            </div>

            <div class="form-group">
                <label>تأكيد كلمة المرور الجديدة:</label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" />
                <asp:CompareValidator ID="cvPassword" runat="server" 
                                      ControlToValidate="txtConfirmPassword" 
                                      ControlToCompare="txtNewPassword" 
                                      ErrorMessage="كلمتا المرور غير متطابقتين" 
                                      ForeColor="Red" Display="Dynamic" />
            </div>

            <asp:Button ID="btnChangePassword" runat="server" Text="تغيير كلمة المرور" 
                        CssClass="btn-change-password" OnClick="btnChangePassword_Click" CausesValidation="false" />
        </div>

        <div class="form-section">
            <h3>معلومات إضافية</h3>
            <div class="form-group">
                <label>تاريخ التسجيل:</label>
                <asp:TextBox ID="txtCreatedAt" runat="server" CssClass="readonly" ReadOnly="true" />
            </div>
        </div>
    </div>
</asp:Content>