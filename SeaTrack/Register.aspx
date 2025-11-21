<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="SeaTrack.Register" %>

<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head runat="server">
    <meta charset="utf-8" />
    <title>التسجيل - SeaTrack</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.rtl.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <style>
        body { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh; padding: 50px 0; }
        .register-card { background: white; border-radius: 15px; box-shadow: 0 10px 40px rgba(0,0,0,0.2); padding: 40px; max-width: 600px; margin: 0 auto; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="register-card">
            <div class="text-center mb-4">
                <i class="fas fa-ship" style="font-size: 50px; color: #667eea;"></i>
                <h2>إنشاء حساب جديد</h2>
            </div>
            <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
                <asp:Literal ID="ltError" runat="server"></asp:Literal>
            </asp:Panel>
            <div class="row">
                <div class="col-md-6 mb-3">
                    <label>اسم المستخدم *</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" required></asp:TextBox>
                </div>
                <div class="col-md-6 mb-3">
                    <label>البريد الإلكتروني *</label>
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" required></asp:TextBox>
                </div>
                <div class="col-md-6 mb-3">
                    <label>كلمة المرور *</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" required></asp:TextBox>
                </div>
                <div class="col-md-6 mb-3">
                    <label>تأكيد كلمة المرور *</label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" required></asp:TextBox>
                </div>
                <div class="col-12 mb-3">
                    <label>الاسم الكامل *</label>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" required></asp:TextBox>
                </div>
                <div class="col-md-6 mb-3">
                    <label>رقم الهاتف</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-6 mb-3">
                    <label>الدولة</label>
                    <asp:TextBox ID="txtCountry" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-12 mb-3">
                    <label>العنوان</label>
                    <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <asp:Button ID="btnRegister" runat="server" Text="تسجيل" CssClass="btn btn-primary w-100 mb-3" OnClick="btnRegister_Click" />
            <div class="text-center">
                <p>لديك حساب؟ <a href="Login.aspx">سجل دخول</a></p>
            </div>
        </div>
    </form>
</body>
</html>
