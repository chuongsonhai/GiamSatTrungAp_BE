using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using log4net;
using Oracle.ManagedDataAccess.Client;

namespace EVNService
{
    public static class Utils
    {
        private static ILog log = LogManager.GetLogger(typeof(Utils));

        private static string connectionString = ConfigurationManager.ConnectionStrings["SendMailDB"].ConnectionString;

        public static void SendMail()
        {
            try
            {
                IDeliverService deliver = new DeliverService();
                IList<SendMail> list = GetList();
                deliver.SendMail(list);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private static IList<SendMail> GetList()
        {
            try
            {
                IList<SendMail> list = new List<SendMail>();
                string cmdText = "select * from SENDMAIL where TRANG_THAI = 0 OR TRANG_THAI = 3 FETCH FIRST 20 ROWS ONLY";
                OracleConnection con = new OracleConnection(connectionString);
                try
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.CommandText = cmdText;
                    cmd.Connection = con;
                    con.Open();
                    OracleDataReader row = cmd.ExecuteReader();
                    if (row.HasRows)
                    {
                        while (row.Read())
                        {
                            list.Add(new SendMail
                            {
                                ID = row["ID"].ToString(),
                                EMAIL = row["EMAIL"].ToString(),
                                TIEUDE = row["TIEUDE"].ToString(),
                                NOIDUNG = row["NOIDUNG"].ToString(),
                                TRANG_THAI = row["TRANG_THAI"].ToString()
                            });
                        }
                    }
                    return list;
                }

                finally
                {
                    con.Close();
                }
            }
            catch (Exception message)
            {
                log.Error(message);
                return new List<SendMail>();
            }
        }

        public static void UpdateMailStatus(string id, int status)
        {
            try
            {
                OracleConnection con = new OracleConnection(connectionString);
                try
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.CommandText = string.Format("UPDATE SENDMAIL SET TRANG_THAI = {0} WHERE ID = '{1}';", status, id);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception message2)
            {
                log.Error(message2);
            }
        }
    }
}
