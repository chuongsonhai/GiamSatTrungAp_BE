using DevExpress.Xpo.Logger.Transport;
using EVN.Core.IServices;
using FX.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Models
{
    public class DataProvide_Oracle
    {
        private OracleConnection connection;
        private ISystemConfigService cfgservice;
        private Dictionary<string, string> conStrConfigs;
        private static string conStr_CMIS3_UNGDUNG;
        private static string conStr;
        private static string conStr_CMIS3;
        private static string conStr_CMIS3_TUPT;
        private static string conStr_CSKH;
        public string tracuu;

        public DataProvide_Oracle()
        {
            cfgservice = IoC.Resolve<ISystemConfigService>();
            conStrConfigs = new Dictionary<string, string>(cfgservice.GetDictionary("conStr"));
            conStr_CMIS3_UNGDUNG = conStrConfigs["conStr_CMIS3_UNGDUNG"];
            conStr = conStrConfigs["conStr"];
            conStr_CMIS3 = conStrConfigs["conStr_CMIS3"];
            conStr_CMIS3_TUPT = conStrConfigs["conStr_CMIS3_TUPT"];
            conStr_CSKH = conStrConfigs["conStr_CSKH"];
        }

        //Check connection to database
        public static string checkSqlConnection()
        {
            // bool blReValue = false;
            string a;
            using (OracleConnection cnnSql = new OracleConnection())
            {
                try
                {
                    cnnSql.ConnectionString = conStr;
                    cnnSql.Open();
                    cnnSql.Close();
                    // blReValue = true;
                    a = "1";
                }
                catch (Exception ex)
                {
                    //blReValue = false;
                    a = ex.ToString();
                }
            }
            //return blReValue;
            return a;
        }

       

        public string change(String query)
        {
            using (OracleConnection conn = new OracleConnection(conStr))
            {
                conn.Open();
                string a = null;
                try
                {
                    OracleCommand cm = new OracleCommand(query, conn);
                    a = cm.ExecuteNonQuery().ToString();
                    conn.Close();
                    conn.Dispose();
                    cm.Dispose();
                }
                catch
                {
                    conn.Close();
                    conn.Dispose();
                }
                return a;
            }
        }

        public string change_ungdung(String query)
        {
            using (OracleConnection conn = new OracleConnection(conStr_CMIS3_UNGDUNG))
            {
                conn.Open();
                string a = null;
                try
                {
                    OracleCommand cm = new OracleCommand(query, conn);
                    a = cm.ExecuteNonQuery().ToString();
                    conn.Close();
                    conn.Dispose();
                    cm.Dispose();
                }
                catch
                {
                    conn.Close();
                    conn.Dispose();
                }
                return a;
            }
        }

        public Int64 getIntegerNumber(String query)
        {
            Int64 result = 0;
            using (OracleConnection conn = new OracleConnection(conStr))
            {
                conn.Open();

                OracleCommand cm = new OracleCommand(query, conn);
                OracleDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult);
                while (dr.Read())
                {
                    try
                    {
                        result = Int64.Parse(dr[0].ToString());
                    }
                    catch (System.Exception ex)
                    {

                    }
                }
                conn.Close();
                conn.Dispose();
                cm.Dispose();
                return result;
            }
        }

        public string getString(String query)
        {
            string result = "";
            using (OracleConnection conn = new OracleConnection(conStr))
            {
                conn.Open();
                OracleCommand cm = new OracleCommand(query, conn);
                OracleDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult);
                try
                {
                    while (dr.Read())
                    {
                        result = dr[0].ToString();
                    }
                    conn.Close();
                    conn.Dispose();
                    cm.Dispose();
                    return result;
                }
                catch
                {
                    conn.Close();
                    conn.Dispose();
                    cm.Dispose();
                    return result;
                }
            }
        }

        public DataTable ExecuteQuery(string query, object[] parameter = null)
        {
            DataTable data = new DataTable();

            using (OracleConnection connection = new OracleConnection(conStr))
            {
                connection.Open();

                OracleCommand command = new OracleCommand(query, connection);

                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');
                    int i = 0;
                    foreach (string item in listPara)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.Add(item, parameter[i]);

                            i++;
                        }
                    }
                }

                OracleDataAdapter adapter = new OracleDataAdapter(command);

                adapter.Fill(data);

                connection.Close();
            }

            return data;
        }

        public static DataSet ExecuteQuery_Dataset(string query, object[] parameter = null)
        {
            DataSet data = new DataSet();

            using (OracleConnection connection = new OracleConnection(conStr))
            {
                connection.Open();

                OracleCommand command = new OracleCommand(query, connection);

                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');
                    int i = 0;
                    foreach (string item in listPara)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.Add(item, parameter[i]);
                            i++;
                        }
                    }
                }

                OracleDataAdapter adapter = new OracleDataAdapter(command);

                adapter.Fill(data);

                connection.Close();
            }

            return data;
        }

        public static DataSet ExecuteQuery_Dataset_UngDung(string query, object[] parameter = null)
        {
            DataSet data = new DataSet();

            using (OracleConnection connection = new OracleConnection(conStr_CMIS3_UNGDUNG))
            {
                connection.Open();

                OracleCommand command = new OracleCommand(query, connection);

                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');
                    int i = 0;
                    foreach (string item in listPara)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.Add(item, parameter[i]);
                            i++;
                        }
                    }
                }

                OracleDataAdapter adapter = new OracleDataAdapter(command);

                adapter.Fill(data);

                connection.Close();
            }

            return data;
        }

        public static int ExecuteNonQuery(string query, object[] parameter = null)
        {
            int data = 0;

            using (OracleConnection connection = new OracleConnection(conStr))
            {
                connection.Open();

                OracleCommand command = new OracleCommand(query, connection);

                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');
                    int i = 0;
                    foreach (string item in listPara)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.Add(item, parameter[i]);
                            i++;
                        }
                    }
                }

                data = command.ExecuteNonQuery();

                connection.Close();
            }

            return data;
        }

        public static DataSet getDataSet(String query)
        {
            DataSet ds = new DataSet();
            try
            {
                using (OracleConnection conn = new OracleConnection(conStr))
                {
                    conn.Open();
                    OracleDataAdapter da = new OracleDataAdapter(query, conn);

                    da.Fill(ds/*, query.Split(' ')[3]*/);
                    conn.Close();
                    conn.Dispose();
                }
                return ds;
            }
            catch (Exception ex)
            {
                return ds;
            }
        }

        public DataSet getDataSet_NEW(String query)
        {
            DataSet ds = new DataSet();
            try
            {
                using (OracleConnection conn = new OracleConnection(conStr))
                {
                    conn.Open();
                    OracleDataAdapter da = new OracleDataAdapter(query, conn);

                    da.Fill(ds/*, query.Split(' ')[3]*/);
                    conn.Close();
                    conn.Dispose();
                }
                return ds;
            }
            catch (Exception ex)
            {
                return ds;
            }
        }

        public DataSet getDataSet_NEW_CMIS3_TUPT(String query)
        {
            DataSet ds = new DataSet();
            try
            {
                using (OracleConnection conn = new OracleConnection(conStr_CMIS3_TUPT))
                {
                    conn.Open();
                    OracleDataAdapter da = new OracleDataAdapter(query, conn);

                    da.Fill(ds/*, query.Split(' ')[3]*/);
                    conn.Close();
                    conn.Dispose();
                }
                return ds;
            }
            catch (Exception ex)
            {
                return ds;
            }
        }

        public DataSet getDataSet_UngDung(String query)
        {
            DataSet ds = new DataSet();
            try
            {
                using (OracleConnection conn = new OracleConnection(conStr_CMIS3_UNGDUNG))
                {
                    conn.Open();
                    OracleDataAdapter da = new OracleDataAdapter(query, conn);

                    da.Fill(ds/*, query.Split(' ')[3]*/);
                    conn.Close();
                    conn.Dispose();
                }
                return ds;
            }
            catch (Exception ex)
            {
                return ds;
            }
        }

        public OracleConnection OraConn
        {
            get
            {
                if (connection == null)
                {
                    //string strConn = "server = CMIS2HM" + ";user=CMIS01" + ";password =CmIs01";

                    connection = new OracleConnection();
                    connection.ConnectionString = conStr;
                }
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();
                return connection;
            }
        }

        public OracleConnection OraConn_CMIS3
        {
            get
            {
                if (connection == null)
                {
                    //string strConn = "server = CMIS2HM" + ";user=CMIS01" + ";password =CmIs01";

                    connection = new OracleConnection();
                    connection.ConnectionString = conStr_CMIS3;
                }
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();
                return connection;
            }
        }

        public OracleConnection OraConn_CMIS3_TUPT
        {
            get
            {
                if (connection == null)
                {
                    //string strConn = "server = CMIS2HM" + ";user=CMIS01" + ";password =CmIs01";

                    connection = new OracleConnection();
                    connection.ConnectionString = conStr_CMIS3_TUPT;
                }
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();
                return connection;
            }
        }

        public OracleConnection OraConn_CMIS3_UNGDUNG
        {
            get
            {
                if (connection == null)
                {
                    //string strConn = "server = CMIS2HM" + ";user=CMIS01" + ";password =CmIs01";

                    connection = new OracleConnection();
                    connection.ConnectionString = conStr_CMIS3_UNGDUNG;
                }
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();
                return connection;
            }
        }

        public OracleConnection OraConn_CMIS3_CSKH
        {
            get
            {
                if (connection == null)
                {
                    //string strConn = "server = CMIS2HM" + ";user=CMIS01" + ";password =CmIs01";

                    connection = new OracleConnection();
                    connection.ConnectionString = conStr_CSKH;
                }
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();
                return connection;
            }
        }

        public DataSet get_DonVi_CuocGoi()
        {
            DataSet ds;
            string dvi = "select * From TBL_DONVI_CUOCGOI order by ma_dviqly ";
            ds = getDataSet_UngDung(dvi);
            return ds;
        }

        //Get Token
        public DataTable Get_token_zalo()
        {
            using (OracleConnection conn = new OracleConnection(conStr))
            {
                DataSet ds = new DataSet();
                OracleCommand cmd2 = new OracleCommand();
                cmd2.Parameters.Clear();
                cmd2.Connection = conn;
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.CommandText = "CONGCSKH.GET_DATA.GET_TOKEN";
                cmd2.Parameters.Add(new OracleParameter("p_ttin_khang", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                OracleDataAdapter daTemp1 = new OracleDataAdapter(cmd2);
                daTemp1.Fill(ds, "SMS05");
                conn.Close();
                conn.Dispose();
                return ds.Tables[0];
            }
        }

        public DataTable Get_sdt_cmis()
        {
            using (OracleConnection conn = new OracleConnection(conStr_CMIS3_UNGDUNG))
            {
                DataSet ds = new DataSet();
                OracleCommand cmd2 = new OracleCommand();
                cmd2.Parameters.Clear();
                cmd2.Connection = conn;
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.CommandText = "UNGDUNG_TRUNGAP.YEUCAU_TRUNGAP.DS_SDT";

                cmd2.Parameters.Add(new OracleParameter("pcusor", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                OracleDataAdapter daTemp1 = new OracleDataAdapter(cmd2);
                daTemp1.Fill(ds);
                conn.Close();
                conn.Dispose();
                return ds.Tables[0];
            }
        }

        public DataTable Get_mayc_cmis_HT( string madvi, DateTime tungay, DateTime denngay )
        {
            madvi = (madvi == "") ? "-1" : madvi;
            using (OracleConnection conn = new OracleConnection(conStr_CMIS3_UNGDUNG))
            {
                DataSet ds = new DataSet();
                OracleCommand cmd2 = new OracleCommand();
                cmd2.Parameters.Clear();
                cmd2.Connection = conn;
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.CommandText = "UNGDUNG_TRUNGAP.YEUCAU_TRUNGAP.DS_YCAU_CMIS_HT";
                cmd2.Parameters.Add("PMADVI", madvi).Direction = ParameterDirection.Input;
                cmd2.Parameters.Add("PTUNGAY", tungay).Direction = ParameterDirection.Input;
                cmd2.Parameters.Add("PDENNGAY", denngay).Direction = ParameterDirection.Input;
                cmd2.Parameters.Add(new OracleParameter("pcusor", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                OracleDataAdapter daTemp1 = new OracleDataAdapter(cmd2);
                daTemp1.Fill(ds);
                conn.Close();
                conn.Dispose();

                return ds.Tables[0];
            }
        }

        public DataTable Get_mayc_cmis_HUY(string madvi, DateTime tungay, DateTime denngay)
        {
            madvi = (madvi == "") ? "-1" : madvi;
            using (OracleConnection conn = new OracleConnection(conStr_CMIS3_UNGDUNG))
            {
                DataSet ds = new DataSet();
                OracleCommand cmd2 = new OracleCommand();
                cmd2.Parameters.Clear();
                cmd2.Connection = conn;
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.CommandText = "UNGDUNG_TRUNGAP.YEUCAU_TRUNGAP.DS_YCAU_CMIS_HU";
                cmd2.Parameters.Add("PMADVI", madvi).Direction = ParameterDirection.Input;
                cmd2.Parameters.Add("PTUNGAY", tungay).Direction = ParameterDirection.Input;
                cmd2.Parameters.Add("PDENNGAY", denngay).Direction = ParameterDirection.Input;
                cmd2.Parameters.Add(new OracleParameter("pcusor", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                OracleDataAdapter daTemp1 = new OracleDataAdapter(cmd2);
                daTemp1.Fill(ds);
                conn.Close();
                conn.Dispose();
                return ds.Tables[0];
            }
        }

        public DataTable Get_mayc_cmis(string maycau)
        {

            using (OracleConnection conn = new OracleConnection(conStr_CMIS3_UNGDUNG))
            {
                DataSet ds = new DataSet();
                OracleCommand cmd2 = new OracleCommand();
                cmd2.Parameters.Clear();
                cmd2.Connection = conn;
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.CommandText = "UNGDUNG_TRUNGAP.YEUCAU_TRUNGAP.MYCAU_CMIS";
                cmd2.Parameters.Add("PYCAU", maycau).Direction = ParameterDirection.Input;
                cmd2.Parameters.Add(new OracleParameter("pcusor", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                OracleDataAdapter daTemp1 = new OracleDataAdapter(cmd2);
                daTemp1.Fill(ds);
                conn.Close();
                conn.Dispose();
                return ds.Tables[0];
            }
        }

    }
}
