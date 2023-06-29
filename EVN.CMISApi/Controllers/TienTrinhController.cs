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
    [RoutePrefix("api/tientrinh")]
    public class TienTrinhController : ApiController
    {
        [HttpGet]
        [Route("filter/{maYCau}")]
        public IHttpActionResult Filter(string maYCau)
        {
            var config = ConnectConfig.Instance;
            OracleConnection con = new OracleConnection(config.connectstring);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select * from DICHVU.DV_TIEN_TRINH WHERE MA_YCAU_KNAI=:MA_YCAU_KNAI";
                cmd.Parameters.Add(new OracleParameter("MA_YCAU_KNAI", maYCau));

                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    IList<DvTienTrinhData> result = new List<DvTienTrinhData>();
                    while (dr.Read())
                    {
                        DvTienTrinhData data = new DvTienTrinhData();
                        data.MA_BPHAN_GIAO = dr["MA_BPHAN_GIAO"].ToString();
                        data.MA_BPHAN_NHAN = dr["MA_BPHAN_NHAN"].ToString();

                        data.MA_CVIEC = dr["MA_CVIEC"].ToString();
                        data.MA_CVIECTIEP = dr["MA_CVIECTIEP"].ToString();
                        data.MA_DDO_DDIEN = dr["MA_DDO_DDIEN"].ToString();
                        data.MA_DVIQLY = dr["MA_DVIQLY"].ToString();
                        data.MA_NVIEN_GIAO = dr["MA_NVIEN_GIAO"].ToString();
                        data.MA_NVIEN_NHAN = dr["MA_NVIEN_NHAN"].ToString();
                        data.MA_TNGAI = dr["MA_TNGAI"].ToString();
                        data.MA_YCAU_KNAI = dr["MA_YCAU_KNAI"].ToString();
                        data.NDUNG_XLY = dr["NDUNG_XLY"].ToString();
                        data.NGUYEN_NHAN = dr["NGUYEN_NHAN"].ToString();
                        data.SO_NGAY_LVIEC = dr["SO_NGAY_LVIEC"].ToString();

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

        [HttpGet]
        [Route("getcurrent/{maYCau}")]
        public IHttpActionResult GetCurrent(string maYCau)
        {
            var config = ConnectConfig.Instance;
            OracleConnection con = new OracleConnection(config.connectstring);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select * from DICHVU.dv_tien_trinh where ma_ycau_knai=:MA_YCAU_KNAI ORDER BY kqua_id_buoc desc FETCH FIRST 1 ROWS ONLY;";
                cmd.Parameters.Add(new OracleParameter("MA_YCAU_KNAI", maYCau));

                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DvTienTrinhData data = new DvTienTrinhData();
                        data.MA_BPHAN_GIAO = dr["MA_BPHAN_GIAO"].ToString();
                        data.MA_BPHAN_NHAN = dr["MA_BPHAN_NHAN"].ToString();

                        data.MA_CVIEC = dr["MA_CVIEC"].ToString();
                        data.MA_CVIECTIEP = dr["MA_CVIECTIEP"].ToString();
                        data.MA_DDO_DDIEN = dr["MA_DDO_DDIEN"].ToString();
                        data.MA_DVIQLY = dr["MA_DVIQLY"].ToString();
                        data.MA_NVIEN_GIAO = dr["MA_NVIEN_GIAO"].ToString();
                        data.MA_NVIEN_NHAN = dr["MA_NVIEN_NHAN"].ToString();
                        data.MA_TNGAI = dr["MA_TNGAI"].ToString();
                        data.MA_YCAU_KNAI = dr["MA_YCAU_KNAI"].ToString();
                        data.NDUNG_XLY = dr["NDUNG_XLY"].ToString();
                        data.NGUYEN_NHAN = dr["NGUYEN_NHAN"].ToString();
                        data.SO_NGAY_LVIEC = dr["SO_NGAY_LVIEC"].ToString();
                        return Ok(data);
                    }
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
