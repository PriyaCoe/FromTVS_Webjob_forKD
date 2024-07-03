using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace webjob_AvailOEE_Live
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["con"].ToString();

            string CompanyCode = "TEAL_SVT";
            string PlantCode = "TEAL_SVT01";
            string LineCode = "Proj_Bat";

            Console.WriteLine(connectionstring);

            Console.WriteLine("Live Avail OEE Calculation web job for SP_Insert_Batchwise_data started");
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand com;

            try
            {
                conn.Open();
                Console.WriteLine("Connection established with DB");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to establish connection with DB");
                ExceptionSetting.SendErrorTomail(ex, connectionstring, CompanyCode, PlantCode, LineCode);
            }
           
            DataSet ds = GetDetails(connectionstring, CompanyCode, PlantCode, LineCode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Console.WriteLine(ds.Tables[0].Rows[i][0]);
                try
                {
                    com = new SqlCommand("SP_Insert_Batchwise_data", conn);
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.Add("@CompanyCode", SqlDbType.NVarChar, 150).Value = CompanyCode;
                    com.Parameters.Add("@PlantCode", SqlDbType.NVarChar, 150).Value = PlantCode;
                    com.Parameters.Add("@LineCode", SqlDbType.NVarChar, 150).Value = LineCode;
                    com.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;
                    com.Parameters.Add("@MachineCode", SqlDbType.NVarChar).Value = ds.Tables[0].Rows[i][0];
                    com.CommandTimeout = 0;
                    com.ExecuteNonQuery();
                    Console.WriteLine("SP_Insert_Batchwise_data ended");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    ExceptionSetting.SendErrorTomail(ex, connectionstring, CompanyCode, PlantCode, LineCode);
                   
                }
            }
            conn.Close();
        }

        public static DataSet GetDetails(string connStr, string CompanyCode, string PlantCode, string LineCode)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT AssetID FROM tbl_Assets WHERE CompanyCode = @company AND PlantCode = @plant  AND FunctionName = @line", con);
                    cmd.Parameters.AddWithValue("@company", CompanyCode);
                    cmd.Parameters.AddWithValue("@plant", PlantCode);
                    cmd.Parameters.AddWithValue("@line", LineCode);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    Console.WriteLine("No. of machine is collected");
                    con.Close();
                    return (ds);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to collect Company,Plant and Line data " + ex);
                    ExceptionSetting.SendErrorTomail(ex, connStr, CompanyCode, PlantCode, LineCode);
                    con.Close();
                    throw;
                }
                finally
                {
                    ds.Dispose();
                }
            }
        }
    }
}
