using EVN.CMISApi.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;

namespace EVN.CMISApi.Controllers
{
    [RoutePrefix("api/hosoti")]
    public class HoSoTiController : ApiController
    {
        [HttpGet]
        [Route("filter/{maDVi}/{keyword}")]
        public IHttpActionResult Filter(string maDVi, string keyword)
        {
            var config = ConnectConfig.Instance;
            OracleConnection con = new OracleConnection(config.connectstring);
            try
            {
                OracleCommand cmd = new OracleCommand();
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select * from DIEMDO.DD_HSO_TI WHERE MA_DVI_SD=:MA_DVI_SD AND SO_TI=:SO_TI");
                cmd.CommandText = sql.ToString();
                cmd.Parameters.Add(new OracleParameter("MA_DVI_SD", maDVi));
                cmd.Parameters.Add(new OracleParameter("SO_TI", keyword));

                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    IList<HoSoTI> result = new List<HoSoTI>();
                    while (dr.Read())
                    {
                        HoSoTI data = new HoSoTI();
                        data.MA_TI = dr["MA_TI"].ToString();
                        data.SO_TI = dr["SO_TI"].ToString();
                        data.NAM_SX = dr["NAM_SX"].ToString();
                        data.MA_CLOAI = dr["MA_CLOAI"].ToString();
                        data.SO_HUU = int.Parse(dr["SO_HUU"].ToString());
                        data.MA_DVI_SD = dr["MA_DVI_SD"].ToString();
                        
                        data.SLAN_SDUNG = int.Parse(dr["SLAN_SDUNG"].ToString());
                        data.TTRANG_KD = int.Parse(dr["TTRANG_KD"].ToString());
                        data.MA_BDONG = dr["MA_BDONG"].ToString();
                        data.NGAY_BDONG = dr["NGAY_BDONG"].ToString();
                        data.SO_BBAN = dr["SO_BBAN"].ToString();

                        data.MA_KHO = dr["MA_KHO"].ToString();
                        data.MA_NVIEN = dr["MA_NVIEN"].ToString();
                        data.NGAY_KDINH = dr["NGAY_KDINH"].ToString();
                        data.NGAY_TAO = dr["NGAY_TAO"].ToString();
                        data.NGUOI_TAO = dr["NGUOI_TAO"].ToString();
                        data.NGAY_SUA = dr["NGAY_SUA"].ToString();
                        data.NGUOI_SUA = dr["NGUOI_SUA"].ToString();
                        data.MA_CNANG = dr["MA_CNANG"].ToString();
                        data.NGAY_NHAP = dr["NGAY_NHAP"].ToString();

                        data.SO_HDONG = dr["SO_HDONG"].ToString();
                        data.SO_BBAN_KD = dr["SO_BBAN_KD"].ToString();
                        data.MTEM_KD = dr["MTEM_KD"].ToString();
                        data.SERY_TEMKD = dr["SERY_TEMKD"].ToString();
                        data.MA_DVIKD = dr["MA_DVIKD"].ToString();
                        data.MA_NVIENKD = dr["MA_NVIENKD"].ToString();

                        data.MCHI_KDINH = dr["MCHI_KDINH"].ToString();
                        data.SO_CHIKD = dr["SO_CHIKD"].ToString();
                        data.TTHAI_INTTHAO = int.Parse(dr["TTHAI_INTTHAO"].ToString());
                        data.TTRANG_CH = int.Parse(dr["TTRANG_CH"].ToString());

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
