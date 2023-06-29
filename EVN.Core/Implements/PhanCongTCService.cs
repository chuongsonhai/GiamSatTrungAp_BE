using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EVN.Core.Implements
{
    public class PhanCongTCService : FX.Data.BaseService<PhanCongTC, int>, IPhanCongTCService
    {
        private ILog log = LogManager.GetLogger(typeof(PhanCongTCService));
        public PhanCongTCService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        /// <summary>
        /// Hủy phân công kiểm tra:
        /// - Cập nhật trạng thái của phân công kiểm tra về  = 0
        /// - Cập nhật trạng thái của công văn yêu cầu nghiệm thu về trạng thái trước đó
        /// </summary>
        public bool CancelKiemTra(YCauNghiemThu congvan, PhanCongTC item)
        {
            try
            {
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                BeginTran();
                item.TRANG_THAI = 0;
                Save(item);

                var thongbao = service.GetbyYCau(congvan.MaDViQLy, congvan.MaYeuCau, LoaiThongBao.KiemTra);
                if (thongbao != null)
                {
                    service.Delete(thongbao);
                }
                if (congvan.TrangThai > TrangThaiNghiemThu.PhanCongKT)
                {
                    congvan.TrangThai = TrangThaiNghiemThu.PhanCongKT;
                    congvansrv.Update(congvan);
                }
                CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Hủy phân công thi công:
        /// - Cập nhật trạng thái của phân công thi công về  = 0
        /// - Cập nhật trạng thái của công văn yêu cầu nghiệm thu về trạng thái trước đó
        /// </summary>
        public bool CancelThiCong(YCauNghiemThu congvan, PhanCongTC item)
        {
            try
            {
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                BeginTran();
                item.TRANG_THAI = 0;
                Save(item);
                var thongbao = service.GetbyYCau(congvan.MaDViQLy, congvan.MaYeuCau, LoaiThongBao.TreoThao);
                if (thongbao != null)
                {
                    service.Delete(thongbao);
                }
                if (congvan.TrangThai > TrangThaiNghiemThu.PhanCongTC)
                {
                    congvan.TrangThai = TrangThaiNghiemThu.PhanCongTC;
                    congvansrv.Update(congvan);
                }
                CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        public PhanCongTC GetbyMaYCau(string loaiYCau, string maYCau, int loai = 1)
        {
            return Get(p => p.MA_LOAI_YCAU == loaiYCau && p.MA_YCAU_KNAI == maYCau && p.LOAI == loai);
        }

        public bool SavePhanCong(BienBanDN bienbandn, PhanCongTC item)
        {
            try
            {
                IThongBaoService tbaosrv = IoC.Resolve<IThongBaoService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();

                var congvan = congvansrv.GetbyMaYCau(bienbandn.MaYeuCau);
                var tthaiycau = ttycausrv.GetbyStatus((int)TrangThaiNghiemThu.KetQuaTC, 1);
                string maCViecTruoc = congvan.MaCViec;
                if (tthaiycau != null)
                    maCViecTruoc = tthaiycau.CVIEC_TRUOC;

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var cauhinhs = cauhinhsrv.GetbyMaCViec(congvan.MaLoaiYeuCau, item.MA_CVIEC);
                var cauhinh = cauhinhs.FirstOrDefault();
                long nextstep = tientrinhsrv.LastbyMaYCau(congvan.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                item.MA_YCAU_KNAI = congvan.MaYeuCau;
                item.MA_DDO_DDIEN = congvan.MaDDoDDien;
                item.MA_DVIQLY = congvan.MaDViQLy;

                item.MA_BPHAN_GIAO = userdata.maBPhan;
                item.MA_NVIEN_GIAO = userdata.maNVien;

                item.MA_NVIEN_NHAN = item.MA_NVIEN_KS;
                item.MA_CVIEC_TRUOC = maCViecTruoc;
                
                BeginTran();
                var ttrinhtruoc = tientrinhsrv.GetbyYCau(bienbandn.MaYeuCau, maCViecTruoc, 0);
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = item.MA_CVIEC;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);                    
                }

                Save(item);
                congvan.TrangThai = TrangThaiNghiemThu.KetQuaTC;
                if (item.LOAI == 0)
                    congvan.TrangThai = TrangThaiNghiemThu.GhiNhanKT;

                congvan.MaCViec = item.MA_CVIEC;
                congvansrv.Update(congvan);

                DvTienTrinh tientrinh = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, item.MA_CVIEC, 0);
                if (tientrinh == null)
                    tientrinh = new DvTienTrinh();
                tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;

                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = item.MA_BPHAN_NHAN;
                tientrinh.MA_CVIEC = item.MA_CVIEC;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_DDO_DDIEN = congvan.MaDDoDDien;
                tientrinh.MA_DVIQLY = congvan.MaDViQLy;

                tientrinh.MA_NVIEN_NHAN = item.MA_NVIEN_KS;

                tientrinh.NDUNG_XLY = item.NDUNG_XLY;

                tientrinh.NGAY_BDAU = DateTime.Today;
                if (ttrinhtruoc != null)
                    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;

                tientrinh.NGAY_KTHUC = DateTime.Now;
                tientrinh.NGAY_HEN = item.NGAY_HEN;
                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                tientrinh.NGUOI_TAO = userdata.maNVien;
                tientrinh.NGUOI_SUA = userdata.maNVien;
                if (tientrinh.STT == 0)
                    tientrinh.STT = nextstep;

                tientrinhsrv.Save(tientrinh);                

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = congvan.MaYeuCau;
                tbao.MaDViQLy = congvan.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = congvan.MaCViec;                
                tbao.TrangThai = TThaiThongBao.Moi;
                tbao.Loai = LoaiThongBao.KiemTra;
                tbao.DuAnDien = congvan.DuAnDien;
                tbao.KhachHang = congvan.NguoiYeuCau;
                string noidung = $"Kiểm tra điều kiện đóng điện điểm đấu nối, ngày hẹn: {item.NGAY_HEN.ToString("dd/MM/yyyy")}";
                if (item.LOAI == 1)
                {
                    tbao.Loai = LoaiThongBao.TreoThao;
                    noidung = $"Thi công lắp đặt, ngày hẹn: {item.NGAY_HEN.ToString("dd/MM/yyyy")}";
                }

                tbao.NoiDung = noidung;
                tbao.NguoiNhan = item.MA_NVIEN_KS;
                tbao.BPhanNhan = userdata.maBPhan;
                tbao.CongViec = item.NDUNG_XLY;
                tbaosrv.CreateNew(tbao);
                CommitTran();

                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(congvan.MaDViQLy, congvan.MaYeuCau);
                try
                {                 
                    var sendmailnv = new SendMail();
                    var nhanVienNhan = userdatasrv.Query.Where(x => x.maNVien == item.MA_NVIEN_KS).FirstOrDefault();
                    if (nhanVienNhan != null)
                    {
                        if (string.IsNullOrWhiteSpace(nhanVienNhan.email))
                        {
                            sendmailnv.EMAIL = nhanVienNhan.email;
                            sendmailnv.TIEUDE = "Thông báo về yêu cầu cần xử lý";
                            sendmailnv.MA_DVIQLY = congvan.MaDViQLy;
                            sendmailnv.MA_YCAU_KNAI = congvan.MaYeuCau;
                            Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                            bodyParamsNV.Add("$khachHang", congvan.NguoiYeuCau);
                            bodyParamsNV.Add("$donVi", congvan.MaDViQLy);
                            bodyParamsNV.Add("$duAnDien", congvan.DuAnDien);
                            bodyParamsNV.Add("$khuVuc", congvan.DiaChiDungDien);
                            bodyParamsNV.Add("$maYCau", congvan.MaYeuCau);
                            bodyParamsNV.Add("$ngaytiepnhan", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                            bodyParamsNV.Add("$nhanvien", nhanVienNhan.fullName ?? nhanVienNhan.username);
                            bodyParamsNV.Add("$buochientai", "Phân công kiểm tra");
                            if (item.LOAI == 1)
                            {
                                bodyParamsNV.Add("$buochientai", "Phân công thi công");
                            }
                            sendmailsrv.Process(sendmailnv, "CanhBaoNV", bodyParamsNV);

                            deliver.Deliver(congvan.MaYeuCau);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }                
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                RolbackTran();
                return false;
            }
        }
    }
}
