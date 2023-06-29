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
    [RoutePrefix("api/donviqly")]
    public class DonViQLyController : ApiController
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
                cmd.CommandText = "select * from DMUC.D_DVI_QLY";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    IList<DonViQLyData> result = new List<DonViQLyData>();
                    while (dr.Read())
                    {
                        DonViQLyData data = new DonViQLyData();
                        data.orgCode = dr["MA_DVIQLY"].ToString();
                        data.orgName = dr["TEN_DVIQLY"].ToString();

                        data.parentCode = dr["MA_DVICTREN"].ToString();
                        data.capDvi = int.Parse(dr["CAP_DVI"].ToString());
                        data.address = dr["DIA_CHI"].ToString();
                        data.idDiaChinh = int.Parse(dr["ID_DIA_CHINH"].ToString());
                        data.phone = dr["DIEN_THOAI"].ToString();
                        data.fax = dr["FAX"].ToString();
                        data.email = dr["EMAIL"].ToString();
                        data.taxCode = dr["MA_STHUE"].ToString();
                        data.daiDien = dr["DAI_DIEN"].ToString();
                        data.chucVu = dr["CHUC_VU"].ToString();
                        
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
