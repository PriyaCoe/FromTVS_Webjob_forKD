using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text;

namespace webjob_AvailOEE_Live
{
    class ExceptionSetting
    {

        private static String ErrorlineNo, Errormsg, ErrorLocation, extype, Sub, EmailHead, EmailSing;


        public static void SendErrorTomail(Exception exmail, String connStr, String company, String plant, String line)
        {

            try
            {

                var newline = "<br/>";
                ErrorlineNo = exmail.StackTrace.Substring(exmail.StackTrace.Length - 7, 7);
                Errormsg = exmail.GetType().Name.ToString();
                extype = exmail.GetType().ToString();
                //exurl = context.Current.Request.Url.ToString();
                ErrorLocation = exmail.Message.ToString();
                EmailHead = "<b>Dear Team,</b>" + "<br/>" + "An exception occurred while running Avail_OEE_Live webjob for SP_Insert_Batchwise_data in " + company + "-" + plant + "-" + line + " With following Details" + "<br/>" + "<br/>";
                EmailSing = newline + "Thanks and Regards" + newline + "    " + "     " + "<b>IIOT Team.TEAL </b>" + "</br>";
                Sub = "Exception occurred" + " " + "in Application";
                //HostAdd = ConfigurationManager.AppSettings["Host"].ToString();
                string errortomail = EmailHead + "<b>Log Written Date: </b>" + " " + DateTime.Now.ToString() + newline + "<b>Web Job: </b>" + " " + "Avail_OEE_Live" + newline + "<b>Error Line No :</b>" + " " + ErrorlineNo + newline + "<b>Error Message:</b>" + " " + Errormsg + newline + "<b>Exception Type:</b>" + " " + extype + newline + "<b> Error Details :</b>" + " " + ErrorLocation + newline + " " + newline + newline + newline + newline + EmailSing;


                using (SqlConnection con = new SqlConnection(connStr))
                {


                    MailMessage mail = new MailMessage();
                    DataTable dt = new DataTable();
                    SqlCommand cmd_mail = new SqlCommand("SELECT * FROM tbl_gmail_settings", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd_mail);
                    da.Fill(dt);
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = dt.Rows[0]["Smtp_host"].ToString();
                    smtp.Port = Convert.ToInt32(dt.Rows[0]["Smtp_port"].ToString());
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(dt.Rows[0]["Smtp_user"].ToString(), dt.Rows[0]["Smtp_pass"].ToString());
                    smtp.EnableSsl = true;

                    mail.To.Add("tamilmozhimj@titan.co.in");
                    mail.To.Add("sahanac@titan.co.in");


                    mail.From = new MailAddress(dt.Rows[0]["Smtp_user"].ToString());
                    string dts = DateTime.Today.AddDays(-1).ToString("dd-MM-yyyy");
                    mail.Subject = "Exception mail while running Avail_OEE_Live webjob for SP_Insert_Batchwise_data in " + company + "-" + plant + "-" + line;
                    mail.Body = errortomail;
                    mail.IsBodyHtml = true;


                    smtp.Send(mail);
                    Console.WriteLine("Exception Email sent successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send Exception email" + ex);
            }
        }

    }
}
