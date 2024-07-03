using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;

namespace Hourly_webjob
{
    class Program
    {

        static void Main(string[] args)
        {

            DateTime today = DateTime.Today.AddDays(-1);
            var dat = today.ToString("yyyy-MM-dd");
            string stmnt = String.Concat("Web job started for the date : ", dat);

            Console.WriteLine(stmnt);



            SqlConnectionStringBuilder builder1 = new SqlConnectionStringBuilder();  //sql connection

            //var database = "TEAL_SVT";
            //builder1.DataSource = "HOSTEP-001";
            //builder1.UserID = "sa";
            //builder1.Password = "teal@123";
            //builder1.InitialCatalog = database;
            //string connStr = builder1.ConnectionString;
            //Console.WriteLine("Database :" + database);

            var database = "iotkpi";
            builder1.DataSource = "THSK06BTRRPTDB";
            builder1.UserID = "iotadmin";
            builder1.Password = "ciql#wOd9ufls";
            builder1.InitialCatalog = database;
            string connStr = builder1.ConnectionString;
            Console.WriteLine("Database :" + database);


            SqlConnection conn = new SqlConnection(connStr);
                SqlCommand cmdd = new SqlCommand("select AssetID as Machine_code, AssetName as MachineName,f.FunctionID as Line_code" +
                    " from tbl_Assets a inner join tbl_function f on a.FunctionName=f.FunctionID ", conn);
                SqlDataAdapter da1 = new SqlDataAdapter(cmdd);
                DataTable assetdt1 = new DataTable();
            da1.Fill(assetdt1);

            Console.WriteLine("Aset details"+ assetdt1);


            int[] array = {6,7,8,10,30,1,2,3,4,5,9,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,31};

            Stopwatch timer = new Stopwatch();

            // Using a for loop to iterate through the array elements
            for (int i = 0; i < array.Length; i++)
            {
                // get current timestamp
               DateTime stime = DateTime.Now;
                timer.Start();
                string machine = "M"+array[i];
                Console.WriteLine("SP Executing started for "+ machine);
                SqlCommand dps = new SqlCommand("[SP_MIS_Hourly_New]", conn);
                dps.CommandType = CommandType.StoredProcedure;
                dps.CommandTimeout = 0;
                dps.Parameters.Add("@Date", SqlDbType.Date).Value = dat; //"2023-08-16";
                dps.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = "TEAL_SVT";
                dps.Parameters.Add("@PlantCode", SqlDbType.VarChar).Value = "TEAL_SVT01";
                dps.Parameters.Add("@LineCode", SqlDbType.VarChar).Value = "Proj_Bat";
                dps.Parameters.Add("@MachineCode", SqlDbType.VarChar).Value = machine;

                SqlDataAdapter dpsda = new SqlDataAdapter(dps);
                DataTable dpst = new DataTable();
                dpsda.Fill(dpst);
                Console.WriteLine("SP Executed ended for "+ machine);
               
                timer.Stop();



                SqlConnection conn1 = new SqlConnection(connStr);
                SqlCommand rd = new SqlCommand("[sp_webjob_diag]", conn1);
                rd.CommandType = CommandType.StoredProcedure;
                rd.CommandTimeout = 0;
                rd.Parameters.Add("@webjob", SqlDbType.VarChar).Value = "Hourly_Job"; //"2023-08-16";
                rd.Parameters.Add("@timestamp", SqlDbType.DateTime).Value = stime;
                rd.Parameters.Add("@duration", SqlDbType.VarChar).Value = timer.ElapsedMilliseconds;
                // dps.Parameters.Add("@LineCode", SqlDbType.VarChar).Value = "mnal01";
                rd.Parameters.Add("@machine", SqlDbType.VarChar).Value = machine;


                SqlDataAdapter rda = new SqlDataAdapter(rd);
                DataTable rdat = new DataTable();
                rda.Fill(rdat);

                Console.WriteLine($"elpsed time:{timer.ElapsedMilliseconds} milliseconds");
                timer.Reset();


            }
            


        }

        }
    }

