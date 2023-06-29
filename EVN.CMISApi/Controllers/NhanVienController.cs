using EVN.CMISApi.Models;
using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EVN.CMISApi.Controllers
{
    [RoutePrefix("api/nhanvien")]
    public class NhanVienController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(NhanVienController));

        [HttpGet]
        [Route("sync")]
        public IHttpActionResult Sync()
        {
            var config = ConnectConfig.Instance;
            OracleConnection con = new OracleConnection(config.connectstring);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select n.*,b.MA_BPHAN from DMUC.D_NHAN_VIEN n inner join DMUC.D_NVIEN_BPHAN b on n.ma_nvien = b.MA_NVIEN";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    log.Error("HasRows");
                    IList<NhanVienData> result = new List<NhanVienData>();
                    while (dr.Read())
                    {
                        NhanVienData data = new NhanVienData();
                        data.MA_DVIQLY = dr["MA_DVIQLY"].ToString();
                        data.MA_BPHAN = dr["MA_BPHAN"].ToString();

                        data.MA_NVIEN = dr["MA_NVIEN"].ToString();
                        
                        data.TEN_NVIEN = dr["TEN_NVIEN"].ToString();
                        result.Add(data);
                    }
                    log.Error("Total: " + result.Count());
                    return Ok(result);
                }
                return NotFound();
            }
            catch(Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
            finally
            {
                con.Close();
            }
        }
    }
}
