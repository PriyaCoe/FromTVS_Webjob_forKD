using System;
using System.Configuration;
using System.Data.SqlClient;


namespace RejectionReason
{
    class Program
    {
        public static string CompanyCode = "TEAL_SVT";
        public static string PlantCode = "Teal_SVT01";
        public static string LineCode = "Proj_Bat";

       


        public static string[] exceptionMailID = new string[] { "tamilmozhimj@titan.co.in" };
        static void Main(string[] args)
        {

            string connectionstring = ConfigurationManager.ConnectionStrings["conn"].ToString();

            try
            {

                //Console.WriteLine(connectionstring);
                Console.WriteLine("Reject Reason web job started");
                SqlConnection conn = new SqlConnection(connectionstring);
                SqlCommand com;
                conn.Open();
                com = new SqlCommand("SP_Reject_Reasons", conn);
                com.CommandTimeout = 0;
                com.ExecuteNonQuery();
                Console.WriteLine("Reject Reason ended");

                com.CommandType = System.Data.CommandType.StoredProcedure;

                conn.Close();
            }
            catch (Exception ex)
            {
                ExceptionSetting.SendErrorTomail(ex, connectionstring, CompanyCode, PlantCode, LineCode, exceptionMailID);
                Console.WriteLine("error occured");
            }
        }
    }
}
