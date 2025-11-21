using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;
using SeaTrack.Utilities;

namespace SeaTrack.BLL
{
    /// <summary>
    /// خدمة رموز QR - لتوليد وحفظ رموز QR
    /// </summary>
    public class QRCodeService
    {
        private static string _qrCodesPath = System.Web.HttpContext.Current.Server.MapPath("~/QRCodes/");

        /// <summary>
        /// توليد رمز QR للشحنة
        /// </summary>
        public static string GenerateShipmentQRCode(string shipmentCode, int shipmentId)
        {
            try
            {
                // التأكد من وجود المجلد
                if (!Directory.Exists(_qrCodesPath))
                {
                    Directory.CreateDirectory(_qrCodesPath);
                }

                // محتوى رمز QR
                string qrContent = $"SEATRACK-SHIPMENT-{shipmentCode}-{shipmentId}";

                // توليد رمز QR
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                // تحويل إلى صورة
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                // حفظ الصورة
                string fileName = $"QR_{shipmentCode}_{DateTime.Now:yyyyMMddHHmmss}.png";
                string filePath = Path.Combine(_qrCodesPath, fileName);

                qrCodeImage.Save(filePath, ImageFormat.Png);

                return qrContent;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("GenerateShipmentQRCode", ex);
                throw;
            }
        }

        /// <summary>
        /// توليد رمز QR كصورة Base64
        /// </summary>
        public static string GenerateQRCodeBase64(string content)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                using (MemoryStream ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    byte[] imageBytes = ms.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    return $"data:image/png;base64,{base64String}";
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("GenerateQRCodeBase64", ex);
                throw;
            }
        }

        /// <summary>
        /// الحصول على مسار ملف QR
        /// </summary>
        public static string GetQRCodePath(string shipmentCode)
        {
            string[] files = Directory.GetFiles(_qrCodesPath, $"QR_{shipmentCode}_*.png");
            if (files.Length > 0)
            {
                return files[0];
            }
            return null;
        }

        /// <summary>
        /// حذف ملف QR
        /// </summary>
        public static bool DeleteQRCode(string shipmentCode)
        {
            try
            {
                string[] files = Directory.GetFiles(_qrCodesPath, $"QR_{shipmentCode}_*.png");
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("DeleteQRCode", ex);
                return false;
            }
        }
    }
}
