<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SeaTrack.Login" %>

<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>تسجيل الدخول - SeaTrack</title>
    
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.rtl.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    
    <style>
        body {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .login-container {
            max-width: 450px;
            width: 100%;
        }
        .login-card {
            background: white;
            border-radius: 15px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.2);
            padding: 40px;
        }
        .logo-section {
            text-align: center;
            margin-bottom: 30px;
        }
        .logo-section i {
            font-size: 60px;
            color: #667eea;
        }
        .logo-section h2 {
            margin-top: 15px;
            color: #333;
            font-weight: bold;
        }
        .form-control:focus {
            border-color: #667eea;
            box-shadow: 0 0 0 0.2rem rgba(102, 126, 234, 0.25);
        }
        .btn-login {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border: none;
            padding: 12px;
            font-size: 16px;
            font-weight: bold;
        }
        .btn-login:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="login-card">
                <div class="logo-section">
                    <i class="fas fa-ship"></i>
                    <h2>SeaTrack</h2>
                    <p class="text-muted">نظام إدارة الشحن البحري</p>
                </div>

                <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
                    <i class="fas fa-exclamation-circle"></i>
                    <asp:Literal ID="ltError" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
                    <i class="fas fa-check-circle"></i>
                    <asp:Literal ID="ltSuccess" runat="server"></asp:Literal>
                </asp:Panel>

                <div class="mb-3">
                    <label for="txtUsername" class="form-label">
                        <i class="fas fa-user"></i> اسم المستخدم
                    </label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" 
                                 placeholder="أدخل اسم المستخدم" required></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label for="txtPassword" class="form-label">
                        <i class="fas fa-lock"></i> كلمة المرور
                    </label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" 
                                 CssClass="form-control" placeholder="أدخل كلمة المرور" required></asp:TextBox>
                </div>

                <div class="mb-3 form-check">
                    <asp:CheckBox ID="chkRememberMe" runat="server" CssClass="form-check-input" />
                    <label class="form-check-label" for="chkRememberMe">
                        تذكرني
                    </label>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="تسجيل الدخول" 
                           CssClass="btn btn-primary btn-login w-100 mb-3" 
                           OnClick="btnLogin_Click" />

                <div class="text-center">
                    <p class="mb-0">
                        ليس لديك حساب؟ 
                        <a href="Register.aspx" class="text-primary">سجل الآن</a>
                    </p>
                </div>

                <hr class="my-4" />

                <div class="text-center text-muted small">
                    <p class="mb-1"><strong>حسابات تجريبية:</strong></p>
                    <p class="mb-0">مسؤول: admin / Admin@123</p>
                    <p class="mb-0">مستودع: warehouse1 / Staff@123</p>
                    <p class="mb-0">عميل: customer1 / Customer@123</p>
                </div>
            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>

