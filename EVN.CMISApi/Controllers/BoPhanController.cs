using EVN.CMISApi.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EVN.CMISApi.Controllers
{
    [RoutePrefix("api/bophan")]
    public class BoPhanController : ApiController
    {
        [HttpGet]
        [Route("sync")]
        public IHttpActionResult Sync()
        {
            var config = ConnectConfig.Instance;
            OracleConnection con = new OracleConnection(config.connectstring);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select * from DMUC.D_BO_PHAN";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    IList<BoPhanData> result = new List<BoPhanData>();
                    while (dr.Read())
                    {
                        BoPhanData data = new BoPhanData();
                        data.MA_DVIQLY = dr["MA_DVIQLY"].ToString();
                        data.MA_BPHAN = dr["MA_BPHAN"].ToString();

                        data.TEN_BPHAN = dr["TEN_BPHAN"].ToString();
                        
                        data.MO_TA = dr["MO_TA"].ToString();
                        data.GHI_CHU = dr["GHI_CHU"].ToString();
                        result.Add(data);
                    }                    
                    return Ok(result);
                }
                return NotFound();
            }
            finally
            {
                con.Close();
            }
        }
    }
}
