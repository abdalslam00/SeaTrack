<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SeaTrack.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .hero-section {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 80px 0;
            text-align: center;
            border-radius: 10px;
            margin-bottom: 40px;
        }
        .hero-section h1 {
            font-size: 48px;
            font-weight: bold;
            margin-bottom: 20px;
        }
        .hero-section p {
            font-size: 20px;
            margin-bottom: 30px;
        }
        .btn-hero {
            background-color: white;
            color: #667eea;
            padding: 15px 40px;
            font-size: 18px;
            font-weight: bold;
            border: none;
            border-radius: 50px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            transition: all 0.3s;
        }
        .btn-hero:hover {
            transform: translateY(-3px);
            box-shadow: 0 10px 20px rgba(0,0,0,0.2);
        }
        .features-section {
            margin: 60px 0;
        }
        .feature-card {
            text-align: center;
            padding: 30px;
            border-radius: 10px;
            background: white;
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            margin-bottom: 30px;
            transition: all 0.3s;
        }
        .feature-card:hover {
            transform: translateY(-10px);
            box-shadow: 0 15px 30px rgba(0,0,0,0.2);
        }
        .feature-icon {
            font-size: 60px;
            color: #667eea;
            margin-bottom: 20px;
        }
        .feature-title {
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 15px;
            color: #2c3e50;
        }
        .feature-description {
            font-size: 16px;
            color: #7f8c8d;
            line-height: 1.6;
        }
        .stats-section {
            background-color: #f8f9fa;
            padding: 60px 0;
            text-align: center;
            border-radius: 10px;
            margin: 40px 0;
        }
        .stat-item {
            padding: 20px;
        }
        .stat-number {
            font-size: 48px;
            font-weight: bold;
            color: #667eea;
        }
        .stat-label {
            font-size: 18px;
            color: #7f8c8d;
            margin-top: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <!-- Hero Section -->
        <div class="hero-section">
            <h1>مرحباً بك في SeaTrack</h1>
            <p>نظام متكامل لإدارة الشحن البحري بكفاءة واحترافية</p>
            <asp:Panel ID="pnlGuestButtons" runat="server" Visible="true">
                <a href="/Login.aspx" class="btn-hero" style="margin-left: 15px;">تسجيل الدخول</a>
                <a href="/Register.aspx" class="btn-hero">إنشاء حساب جديد</a>
            </asp:Panel>
            <asp:Panel ID="pnlUserButtons" runat="server" Visible="false">
                <asp:HyperLink ID="hlDashboard" runat="server" CssClass="btn-hero">الانتقال إلى لوحة التحكم</asp:HyperLink>
            </asp:Panel>
        </div>

        <!-- Features Section -->
        <div class="features-section">
            <h2 class="text-center mb-5" style="font-size: 36px; font-weight: bold; color: #2c3e50;">مميزات النظام</h2>
            <div class="row">
                <div class="col-md-4">
                    <div class="feature-card">
                        <div class="feature-icon">
                            <i class="fas fa-ship"></i>
                        </div>
                        <div class="feature-title">إدارة الرحلات</div>
                        <div class="feature-description">
                            تتبع شامل لجميع الرحلات البحرية من الانطلاق حتى الوصول مع تحديثات فورية للحالة
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="feature-card">
                        <div class="feature-icon">
                            <i class="fas fa-box-open"></i>
                        </div>
                        <div class="feature-title">إدارة الحاويات</div>
                        <div class="feature-description">
                            نظام ذكي لإدارة الحاويات مع حساب ديناميكي للأوزان والسعات المتاحة
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="feature-card">
                        <div class="feature-icon">
                            <i class="fas fa-qrcode"></i>
                        </div>
                        <div class="feature-title">نظام QR Code</div>
                        <div class="feature-description">
                            توليد ومسح رموز QR للشحنات لتسهيل عمليات التتبع والتحقق
                        </div>
                    </div>
                </div>
            </div>
            <div class="row mt-4">
                <div class="col-md-4">
                    <div class="feature-card">
                        <div class="feature-icon">
                            <i class="fas fa-file-invoice-dollar"></i>
                        </div>
                        <div class="feature-title">إدارة الفواتير</div>
                        <div class="feature-description">
                            نظام مالي متكامل لإصدار الفواتير وإيصالات الدفع الرسمية
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="feature-card">
                        <div class="feature-icon">
                            <i class="fas fa-chart-line"></i>
                        </div>
                        <div class="feature-title">التقارير والإحصائيات</div>
                        <div class="feature-description">
                            تقارير شاملة عن الإيرادات وأداء الرحلات لدعم اتخاذ القرارات
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="feature-card">
                        <div class="feature-icon">
                            <i class="fas fa-bell"></i>
                        </div>
                        <div class="feature-title">نظام الإشعارات</div>
                        <div class="feature-description">
                            إشعارات فورية للعملاء بتحديثات الحجوزات والشحنات والمدفوعات
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Stats Section -->
        <div class="stats-section">
            <h2 class="mb-5" style="font-size: 36px; font-weight: bold; color: #2c3e50;">إحصائيات النظام</h2>
            <div class="row">
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">
                            <asp:Label ID="lblTotalTrips" runat="server" Text="0" />
                        </div>
                        <div class="stat-label">إجمالي الرحلات</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">
                            <asp:Label ID="lblTotalShipments" runat="server" Text="0" />
                        </div>
                        <div class="stat-label">إجمالي الشحنات</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">
                            <asp:Label ID="lblTotalContainers" runat="server" Text="0" />
                        </div>
                        <div class="stat-label">إجمالي الحاويات</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">
                            <asp:Label ID="lblTotalCustomers" runat="server" Text="0" />
                        </div>
                        <div class="stat-label">إجمالي العملاء</div>
                    </div>
                </div>
            </div>
        </div>

        <!-- About Section -->
        <div class="text-center mb-5">
            <h2 style="font-size: 36px; font-weight: bold; color: #2c3e50; margin-bottom: 20px;">عن SeaTrack</h2>
            <p style="font-size: 18px; color: #7f8c8d; max-width: 800px; margin: 0 auto; line-height: 1.8;">
                نظام SeaTrack هو حل رقمي متكامل لإدارة عمليات الشحن البحري. يوفر النظام أدوات متقدمة لإدارة الرحلات، 
                الحاويات، الشحنات، والفواتير بكفاءة عالية. مع واجهة سهلة الاستخدام ونظام صلاحيات محكم، يضمن SeaTrack 
                تجربة سلسة لجميع الأطراف من المسؤولين والعملاء وموظفي المخازن.
            </p>
        </div>
    </div>
</asp:Content>

