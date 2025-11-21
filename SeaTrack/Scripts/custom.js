// SeaTrack Custom JavaScript

// تأكيد الحذف
function confirmDelete(message) {
    return confirm(message || 'هل أنت متأكد من الحذف؟');
}

// طباعة
function printPage() {
    window.print();
}

// تحديث التاريخ والوقت
function updateDateTime() {
    const now = new Date();
    const dateStr = now.toLocaleDateString('ar-SA');
    const timeStr = now.toLocaleTimeString('ar-SA');
    
    const dateTimeElements = document.querySelectorAll('.current-datetime');
    dateTimeElements.forEach(el => {
        el.textContent = `${dateStr} ${timeStr}`;
    });
}

// تشغيل عند تحميل الصفحة
document.addEventListener('DOMContentLoaded', function() {
    // تحديث التاريخ والوقت كل ثانية
    setInterval(updateDateTime, 1000);
    updateDateTime();
    
    // تفعيل tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // إخفاء الرسائل تلقائياً بعد 5 ثواني
    setTimeout(function() {
        const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
        alerts.forEach(alert => {
            alert.style.transition = 'opacity 0.5s';
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 500);
        });
    }, 5000);
});

// QR Code Scanner (يمكن استخدام مكتبة خارجية)
function initQRScanner(inputId) {
    const input = document.getElementById(inputId);
    if (input) {
        input.focus();
        input.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                // معالجة رمز QR
                const qrCode = input.value.trim();
                if (qrCode) {
                    // يمكن إضافة منطق إضافي هنا
                    console.log('QR Code scanned:', qrCode);
                }
            }
        });
    }
}

// تصدير إلى Excel (بسيط)
function exportTableToExcel(tableId, filename) {
    const table = document.getElementById(tableId);
    if (!table) return;
    
    let html = table.outerHTML;
    const url = 'data:application/vnd.ms-excel,' + encodeURIComponent(html);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename + '.xls';
    link.click();
}
