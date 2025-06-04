
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace EVN.Core
{
    public class PMISService : IPMISService
    {
        ILog log = LogManager.GetLogger(typeof(PMISService));
        public bool GhiDuLieuPMIS(string fileName, decimal filesize, string fileType,byte[] fileData)
        {
            try
            {
                INoTemplateService notempsrv = IoC.Resolve<INoTemplateService>();

                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
                var conPmisConfigs = cfgservice.GetDictionary("conPmis");
                string conPmis = conPmisConfigs["conPmis"];

                SqlConnection con = new SqlConnection(conPmis);
                try
                {
                    string AFFILEID = "";
                    NoTransaction notran = new NoTransaction(notempsrv, NoType.PMIS);
                        lock (notran)
                        {
                            decimal no = notran.GetNextNo();
                            AFFILEID = notran.GetCodePMIS("TA", no);
                        }
                    SqlCommand cmd1 = new SqlCommand();
                    StringBuilder sql1 = new StringBuilder();
                    SqlCommand cmd2 = new SqlCommand();
                    StringBuilder sql2 = new StringBuilder();
                    sql1.AppendLine("insert into AF_A_ASSET_ATT_ITEM (AFFILEID,FILENAME,FILESIZEB,UPDDTIME,FILETYPEID) values " +
                        "(@AFFILEID,@FILENAME,@FILESIZEB,@UPDDTIME,@FILETYPEID)");
                    string checkImg = ".bmp,.gif,.jpg,.jpeg,.png,.tif";
                    string fileTypeID = "DOC";
                    if (checkImg.Contains(fileType.ToLower()))
                    {
                        fileTypeID = "IMG";
                    }

                    cmd1.CommandText = sql1.ToString();
                    cmd1.Parameters.Add(new SqlParameter("@AFFILEID", AFFILEID));
                    cmd1.Parameters.Add(new SqlParameter("@FILENAME", fileName));
                    cmd1.Parameters.Add(new SqlParameter("@FILESIZEB", filesize));
                    cmd1.Parameters.Add(new SqlParameter("@UPDDTIME", DateTime.Now));
                    cmd1.Parameters.Add(new SqlParameter("@FILETYPEID", fileTypeID));

                    sql2.AppendLine("insert into AF_A_ASSET_ATT_ITEM_FILE (AFFILEID,FILEDATA) values " +
                        "(@AFFILEID,@FILEDATA)");
                    cmd2.Parameters.Add(new SqlParameter("@AFFILEID", AFFILEID));
                    cmd2.Parameters.Add(new SqlParameter("@FILEDATA", fileData));
                    cmd2.CommandText = sql2.ToString();

                    cmd1.Connection = con;
                    cmd2.Connection = con;
                    con.Open();
                    cmd1.ExecuteNonQuery();
                    cmd2.ExecuteNonQuery();
                    notran.CommitTran();
                    return true;
                }
                finally
                {
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }

        }
    }
}
