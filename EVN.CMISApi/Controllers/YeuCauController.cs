using EVN.CMISApi.Models;
using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;

namespace EVN.CMISApi.Controllers
{
    [RoutePrefix("api/yeucau")]
    public class YeuCauController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(YeuCauController));

        [HttpPost]
        [Route("dongbo")]
        public IHttpActionResult DongBo(YeuCauRequest model)
        {
            var config = ConnectConfig.Instance;
            OracleConnection con = new OracleConnection(config.connectstring);
            try
            {
                OracleCommand cmd = new OracleCommand();
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select t.* from DICHVU.DV_TIEN_TNHAN t ");
                sql.AppendLine(" WHERE t.MA_YCAU_KNAI=:MA_YEU_CAU ");
                cmd.CommandText = sql.ToString();
                cmd.Parameters.Add(new OracleParameter("MA_YEU_CAU", model.maYCau));

                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    IList<TienTiepNhanData> result = new List<TienTiepNhanData>();
                    while (dr.Read())
                    {
                        TienTiepNhanData data = new TienTiepNhanData();
                        data.MaYeuCau = dr["MA_YCAU_KNAI"].ToString();
                        data.MaLoaiYeuCau = dr["MA_LOAI_YCAU"].ToString();

                        data.MaDViTNhan = dr["MA_DVI_TNHAN"].ToString();
                        data.MaDViQLy = dr["MA_DVIQLY"].ToString();
                        data.MaKHang = dr["MA_KHANG"].ToString();
                        data.NguoiYeuCau = dr["TEN_NGUOIYCAU"].ToString();
                        data.DChiNguoiYeuCau = dr["DCHI_NGUOIYCAU"].ToString();
                        data.TenKhachHang = dr["TEN_KHANG"].ToString();
                        data.CoQuanChuQuan = dr["CQUAN_CHUQUAN"].ToString();
                        data.DiaChiCoQuan = dr["DCHI_CQUANCQ"].ToString();
                        data.MST = dr["MST"].ToString();
                        data.DienThoai = dr["DTHOAI"].ToString();

                        data.Email = dr["EMAIL"].ToString();
                        data.Fax = dr["FAX"].ToString();
                        data.SoTaiKhoan = dr["SO_TKHOAN"].ToString();
                        data.MaNHang = dr["MA_NHANG"].ToString();
                        data.SoNha = dr["SO_NHA"].ToString();
                        data.DuongPho = dr["DUONG_PHO"].ToString();
                        data.DiaChinhID = dr["ID_DIA_CHINH"].ToString();
                        data.SoPha = int.Parse(dr["SO_PHA"].ToString());
                        int diensinhhoat = int.Parse(dr["MDICH_SHOAT"].ToString());
                        data.DienSinhHoat = diensinhhoat == 1;
                        data.NgayHen = dr["NGAY_HEN"].ToString();
                        data.NgayHenKhaoSat = dr["NGAYHEN_KSAT"].ToString();
                        data.NoiDungYeuCau = dr["NOI_DUNG_YCAU"].ToString();
                        data.TinhTrang = int.Parse(dr["TINH_TRANG"].ToString());
                        data.MaHinhThuc = dr["MA_HTHUC"].ToString();

                        data.NgaySua = dr["NGAY_SUA"].ToString();

                        result.Add(data);
                    }
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
            finally
            {
                con.Close();
            }
        }

        [HttpGet]
        [Route("getinfo/{maYCau}")]
        public IHttpActionResult GetInfo(string maYCau)
        {
            var config = ConnectConfig.Instance;
            OracleConnection con = new OracleConnection(config.connectstring);
            try
            {
                OracleCommand cmd = new OracleCommand();
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select t.*,d.MA_DDO_DDIEN from DICHVU.DV_TIEN_TNHAN t INNER JOIN DICHVU.CD_DDO_DDIEN d ON t.MA_YCAU_KNAI = d.MA_YCAU_KNAI");
                sql.AppendLine(" WHERE t.MA_YCAU_KNAI=:MA_YEU_CAU AND t.TINH_TRANG = 1");
                cmd.CommandText = sql.ToString();
                cmd.Parameters.Add(new OracleParameter("MA_YEU_CAU", maYCau));

                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    IList<TienTiepNhanData> result = new List<TienTiepNhanData>();
                    while (dr.Read())
                    {
                        TienTiepNhanData data = new TienTiepNhanData();
                        data.MaDDoDDien = dr["MA_DDO_DDIEN"] != null ? dr["MA_DDO_DDIEN"].ToString() : "";

                        data.MaYeuCau = dr["MA_YCAU_KNAI"].ToString();
                        data.MaLoaiYeuCau = dr["MA_LOAI_YCAU"].ToString();

                        data.MaDViTNhan = dr["MA_DVI_TNHAN"].ToString();
                        data.MaDViQLy = dr["MA_DVIQLY"].ToString();
                        data.MaKHang = dr["MA_KHANG"].ToString();
                        data.NguoiYeuCau = dr["TEN_NGUOIYCAU"].ToString();
                        data.DChiNguoiYeuCau = dr["DCHI_NGUOIYCAU"].ToString();
                        data.TenKhachHang = dr["TEN_KHANG"].ToString();
                        data.CoQuanChuQuan = dr["CQUAN_CHUQUAN"].ToString();
                        data.DiaChiCoQuan = dr["DCHI_CQUANCQ"].ToString();
                        data.MST = dr["MST"].ToString();
                        data.DienThoai = dr["DTHOAI"].ToString();

                        data.Email = dr["EMAIL"].ToString();
                        data.Fax = dr["FAX"].ToString();
                        data.SoTaiKhoan = dr["SO_TKHOAN"].ToString();
                        data.MaNHang = dr["MA_NHANG"].ToString();
                        data.SoNha = dr["SO_NHA"].ToString();
                        data.DuongPho = dr["DUONG_PHO"].ToString();
                        data.DiaChinhID = dr["ID_DIA_CHINH"].ToString();
                        data.SoPha = int.Parse(dr["SO_PHA"].ToString());
                        int diensinhhoat = int.Parse(dr["MDICH_SHOAT"].ToString());
                        data.DienSinhHoat = diensinhhoat == 1;
                        data.NgayHen = dr["NGAY_HEN"].ToString();
                        data.NgayHenKhaoSat = dr["NGAYHEN_KSAT"].ToString();
                        data.NoiDungYeuCau = dr["NOI_DUNG_YCAU"].ToString();
                        data.TinhTrang = int.Parse(dr["TINH_TRANG"].ToString());
                        data.MaHinhThuc = dr["MA_HTHUC"].ToString();

                        data.NgaySua = dr["NGAY_SUA"].ToString();

                        result.Add(data);
                    }
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
            finally
            {
                con.Close();
            }
        }


        [HttpPost]
        [Route("getdiemdo")]
        public IHttpActionResult GetDiemDo(DiemDoRequest request)
        {
            var config = ConnectConfig.Instance;
            OracleConnection con = new OracleConnection(config.connectstring);
            try
            {
                OracleCommand cmd = new OracleCommand();
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select * from DICHVU.CD_DIEM_DO");
                sql.AppendLine(" WHERE MA_DVIQLY=:MA_DVIQLY AND MA_DDO_DDIEN=:MA_DDO_DDIEN");
                cmd.CommandText = sql.ToString();
                cmd.Parameters.Add(new OracleParameter("MA_DVIQLY", request.maDViQLy));
                cmd.Parameters.Add(new OracleParameter("MA_DDO_DDIEN", request.maDDoDDien));

                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    IList<DiemDoData> result = new List<DiemDoData>();
                    while (dr.Read())
                    {
                        DiemDoData data = new DiemDoData();
                        data.MA_DVIQLY = dr["MA_DVIQLY"].ToString();

                        data.MA_DDO = dr["MA_DDO"].ToString();
                        data.MA_DDO_DDIEN = dr["MA_DDO_DDIEN"].ToString();

                        data.MA_HDONG = dr["MA_HDONG"].ToString();
                        data.MA_CAPDA = dr["MA_CAPDA"].ToString();
                        data.DIA_CHI = dr["DIA_CHI"].ToString();
                        data.MUC_DICH = dr["MUC_DICH"].ToString();
                        data.SOHUU_LUOI = dr["SOHUU_LUOI"].ToString();
                        data.LOAI_DDO = dr["LOAI_DDO"].ToString();
                        data.SO_HO = dr["SO_HO"].ToString();
                        data.TTRANG_TREOTHAO = dr["TTRANG_TREOTHAO"].ToString();
                        data.MA_TRAM = dr["MA_TRAM"].ToString();
                        data.MA_LO = dr["MA_LO"].ToString();

                        data.MA_TO = dr["MA_TO"].ToString();
                        data.PHA = dr["PHA"].ToString();
                        data.SO_COT = dr["SO_COT"].ToString();
                        data.SO_HOP = dr["SO_HOP"].ToString();
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

    public class DiemDoRequest
    {
        public string maDViQLy { get; set; }
        public string maDDoDDien { get; set; }
    }

    public class YeuCauRequest
    {
        public string maYCau { get; set; }        
    }
}
