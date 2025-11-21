using System;
using System.Data;
using System.Data.SqlClient;
using SeaTrack.DAL;
using SeaTrack.Utilities;

namespace SeaTrack.BLL
{
    /// <summary>
    /// خدمة حساب الوزن الديناميكي للحاويات
    /// تقوم بحساب الوزن الإجمالي (Gross Weight) = الوزن الأساسي (Base Weight) + وزن البضائع (Cargo Weight)
    /// </summary>
    public class WeightCalculationService
    {
        /// <summary>
        /// حساب الوزن الإجمالي للحاوية في رحلة معينة
        /// </summary>
        /// <param name="tripId">معرف الرحلة</param>
        /// <param name="containerId">معرف الحاوية</param>
        /// <returns>الوزن الإجمالي بالكيلوجرام</returns>
        public static decimal CalculateGrossWeight(int tripId, int containerId)
        {
            try
            {
                // الحصول على الوزن الأساسي للحاوية
                decimal baseWeight = GetContainerBaseWeight(containerId);
                
                // الحصول على وزن البضائع الحالي
                decimal cargoWeight = GetCurrentCargoWeight(tripId, containerId);
                
                // حساب الوزن الإجمالي
                decimal grossWeight = baseWeight + cargoWeight;
                
                return grossWeight;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("WeightCalculationService.CalculateGrossWeight", ex);
                throw;
            }
        }

        /// <summary>
        /// الحصول على الوزن الأساسي للحاوية (وهي فارغة)
        /// </summary>
        private static decimal GetContainerBaseWeight(int containerId)
        {
            string query = "SELECT base_weight_kg FROM Containers WHERE container_id = @container_id";
            SqlParameter[] parameters = { new SqlParameter("@container_id", containerId) };
            
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToDecimal(dt.Rows[0]["base_weight_kg"]);
            }
            return 0;
        }

        /// <summary>
        /// الحصول على وزن البضائع الحالي في الحاوية لرحلة معينة
        /// </summary>
        private static decimal GetCurrentCargoWeight(int tripId, int containerId)
        {
            string query = @"SELECT ISNULL(SUM(s.weight_kg), 0) as total_cargo_weight
                            FROM Shipments s
                            WHERE s.trip_id = @trip_id 
                            AND s.container_id = @container_id
                            AND s.status_id NOT IN (5, 6)"; // استثناء الشحنات الملغاة أو المرفوضة
            
            SqlParameter[] parameters = {
                new SqlParameter("@trip_id", tripId),
                new SqlParameter("@container_id", containerId)
            };
            
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToDecimal(dt.Rows[0]["total_cargo_weight"]);
            }
            return 0;
        }

        /// <summary>
        /// تحديث الوزن الإجمالي للحاوية في جدول Trip_Containers
        /// </summary>
        public static bool UpdateTripContainerWeight(int tripId, int containerId)
        {
            try
            {
                // حساب الوزن الإجمالي الجديد
                decimal grossWeight = CalculateGrossWeight(tripId, containerId);
                
                // حساب وزن البضائع
                decimal cargoWeight = GetCurrentCargoWeight(tripId, containerId);
                
                // تحديث الجدول
                string query = @"UPDATE Trip_Containers 
                                SET cargo_weight_kg = @cargo_weight,
                                    gross_weight = @gross_weight,
                                    updated_at = GETDATE()
                                WHERE trip_id = @trip_id AND container_id = @container_id";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@cargo_weight", cargoWeight),
                    new SqlParameter("@gross_weight", grossWeight),
                    new SqlParameter("@trip_id", tripId),
                    new SqlParameter("@container_id", containerId)
                };
                
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("WeightCalculationService.UpdateTripContainerWeight", ex);
                return false;
            }
        }

        /// <summary>
        /// التحقق من أن إضافة شحنة جديدة لن تتجاوز الحد الأقصى للوزن
        /// </summary>
        public static bool CanAddShipment(int tripId, int containerId, decimal shipmentWeight)
        {
            try
            {
                // الحصول على الوزن الأقصى للحاوية
                decimal maxWeight = GetContainerMaxWeight(containerId);
                
                // حساب الوزن الإجمالي الحالي
                decimal currentGrossWeight = CalculateGrossWeight(tripId, containerId);
                
                // حساب الوزن المتوقع بعد إضافة الشحنة
                decimal expectedWeight = currentGrossWeight + shipmentWeight;
                
                // التحقق من عدم تجاوز الحد الأقصى
                return expectedWeight <= maxWeight;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("WeightCalculationService.CanAddShipment", ex);
                return false;
            }
        }

        /// <summary>
        /// الحصول على الوزن الأقصى المسموح للحاوية
        /// </summary>
        private static decimal GetContainerMaxWeight(int containerId)
        {
            string query = "SELECT max_weight_kg FROM Containers WHERE container_id = @container_id";
            SqlParameter[] parameters = { new SqlParameter("@container_id", containerId) };
            
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToDecimal(dt.Rows[0]["max_weight_kg"]);
            }
            return 0;
        }

        /// <summary>
        /// الحصول على السعة المتاحة في الحاوية (بالكيلوجرام)
        /// </summary>
        public static decimal GetAvailableCapacity(int tripId, int containerId)
        {
            try
            {
                decimal maxWeight = GetContainerMaxWeight(containerId);
                decimal currentGrossWeight = CalculateGrossWeight(tripId, containerId);
                
                return maxWeight - currentGrossWeight;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("WeightCalculationService.GetAvailableCapacity", ex);
                return 0;
            }
        }

        /// <summary>
        /// الحصول على نسبة الامتلاء للحاوية (%)
        /// </summary>
        public static decimal GetCapacityPercentage(int tripId, int containerId)
        {
            try
            {
                decimal maxWeight = GetContainerMaxWeight(containerId);
                if (maxWeight == 0) return 0;
                
                decimal currentGrossWeight = CalculateGrossWeight(tripId, containerId);
                
                return Math.Round((currentGrossWeight / maxWeight) * 100, 2);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("WeightCalculationService.GetCapacityPercentage", ex);
                return 0;
            }
        }

        /// <summary>
        /// تحديث أوزان جميع الحاويات في رحلة معينة
        /// </summary>
        public static bool UpdateAllContainersInTrip(int tripId)
        {
            try
            {
                // الحصول على جميع الحاويات في الرحلة
                DataTable containers = ContainerRepository.GetContainersByTrip(tripId);
                
                foreach (DataRow row in containers.Rows)
                {
                    int containerId = Convert.ToInt32(row["container_id"]);
                    UpdateTripContainerWeight(tripId, containerId);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("WeightCalculationService.UpdateAllContainersInTrip", ex);
                return false;
            }
        }
    }
}
