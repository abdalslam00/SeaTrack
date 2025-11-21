using SeaTrack.Utilities;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SeaTrack.DAL
{
    /// <summary>
    /// مساعد قاعدة البيانات - يوفر طرق للتعامل مع SQL Server
    /// </summary>
    public class DatabaseHelper
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["SeaTrackDB"].ConnectionString;

        /// <summary>
        /// الحصول على اتصال جديد بقاعدة البيانات
        /// </summary>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// تنفيذ استعلام وإرجاع DataTable
        /// </summary>
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("ExecuteQuery", ex);
                throw;
            }
        }

        /// <summary>
        /// تنفيذ استعلام بدون إرجاع بيانات (INSERT, UPDATE, DELETE)
        /// </summary>
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("ExecuteNonQuery", ex);
                throw;
            }
        }

        /// <summary>
        /// تنفيذ استعلام وإرجاع قيمة واحدة
        /// </summary>
        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("ExecuteScalar", ex);
                throw;
            }
        }

        /// <summary>
        /// تنفيذ إجراء مخزن وإرجاع DataTable
        /// </summary>
        public static DataTable ExecuteStoredProcedure(string procedureName, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(procedureName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("ExecuteStoredProcedure", ex);
                throw;
            }
        }

        /// <summary>
        /// تنفيذ إجراء مخزن بدون إرجاع بيانات
        /// </summary>
        public static int ExecuteStoredProcedureNonQuery(string procedureName, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(procedureName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery();
                }
            }
            
            catch (Exception ex)
            {
                LogHelper.LogError("ExecuteStoredProcedureNonQuery", ex);
                throw;
            }
        }

        /// <summary>
        /// اختبار الاتصال بقاعدة البيانات
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
