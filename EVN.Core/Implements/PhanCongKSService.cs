using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Core.Implements
{
    public class PhanCongKSService : FX.Data.BaseService<PhanCongKS, int>, IPhanCongKSService
    {
        private ILog log = LogManager.GetLogger(typeof(PhanCongKSService));
        public PhanCongKSService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public bool SavePhanCong(CongVanYeuCau congvan, PhanCongKS item)
        {
            try
            {
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                string maCViecTruoc = item.MA_CVIEC_TRUOC;
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

                item.MA_BPHAN_NHAN = item.MA_BPHAN_GIAO;
                item.MA_NVIEN_NHAN = item.MA_NVIEN_KS;

                BeginTran();

                congvan.TrangThai = TrangThaiCongVan.GhiNhanKS;
                congvan.MaCViec = item.MA_CVIEC;
                congvansrv.Update(congvan);

                var ttrinhtruoc = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, maCViecTruoc, 0);
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = item.MA_CVIEC;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }
                Save(item);

                DvTienTrinh tientrinh = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, item.MA_CVIEC, 0);
                if (tientrinh == null)
                    tientrinh = new DvTienTrinh();
                tientrinh.MA_DVIQLY = congvan.MaDViQLy;

                tientrinh.MA_CVIEC = item.MA_CVIEC;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_DDO_DDIEN = congvan.MaDDoDDien;

                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = item.MA_BPHAN_NHAN;
                tientrinh.MA_NVIEN_NHAN = item.MA_NVIEN_KS;

                tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;
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
                tbao.NgayHen = item.NGAY_HEN;
                tbao.MaCViec = congvan.MaCViec;
                tbao.Loai = LoaiThongBao.KhaoSat;
                tbao.TrangThai = TThaiThongBao.Moi;
                tbao.NoiDung = $"Thực hiện khảo sát lưới điện, ngày hẹn: {tbao.NgayHen.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = item.MA_NVIEN_KS;
                tbao.BPhanNhan = item.MA_BPHAN_NHAN;
                tbao.CongViec = item.NDUNG_XLY;
                tbao.DuAnDien = congvan.DuAnDien;
                tbao.KhachHang = congvan.TenKhachHang;
                service.CreateNew(tbao);
                CommitTran();

                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                IDeliverService deliver = new DeliverService();                
                try
                {
                    INhanVienService nviensrv = IoC.Resolve<INhanVienService>();

                    var nvien = nviensrv.GetbyCode(item.MA_DVIQLY, item.MA_NVIEN_KS);
                    var sendmail = new SendMail();
                    sendmail.EMAIL = congvan.Email;
                    sendmail.TIEUDE = "Thực hiện khảo sát lưới điện";
                    sendmail.MA_DVIQLY = congvan.MaDViQLy;
                    sendmail.MA_YCAU_KNAI = congvan.MaYeuCau;
                    Dictionary<string, string> bodyParams = new Dictionary<string, string>();
                    bodyParams.Add("$khachHang", congvan.TenKhachHang ?? congvan.NguoiYeuCau);
                    bodyParams.Add("$maYCau", congvan.MaYeuCau);
                    bodyParams.Add("$lienHe", $"{nvien.TEN_NVIEN}, Số điện thoại: {nvien.DIEN_THOAI}");
                    bodyParams.Add("$ngayYCau", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                    bodyParams.Add("$ngayHen", item.NGAY_HEN.ToString("dd/MM/yyyy"));
                    sendmailsrv.Process(sendmail, "KhaoSatLuoiDien", bodyParams);
                    deliver.Deliver(congvan.MaYeuCau);

                    var sendmailnv = new SendMail();
                    var nhanVienNhan = userdatasrv.Query.Where(x => x.maNVien == item.MA_NVIEN_KS).FirstOrDefault();
                    if (nhanVienNhan != null)
                    {
                        if (!string.IsNullOrWhiteSpace(nhanVienNhan.email))
                        {
                            sendmailnv.EMAIL = nhanVienNhan.email;
                            sendmailnv.TIEUDE = "Thông báo về yêu cầu cần xử lý";
                            sendmailnv.MA_DVIQLY = congvan.MaDViQLy;
                            sendmailnv.MA_YCAU_KNAI = congvan.MaYeuCau;
                            Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                            bodyParamsNV.Add("$khachHang", congvan.TenKhachHang ?? congvan.NguoiYeuCau);
                            bodyParamsNV.Add("$donVi", congvan.MaDViQLy);
                            bodyParamsNV.Add("$duAnDien", congvan.DuAnDien);
                            bodyParamsNV.Add("$khuVuc", congvan.DiaChiDungDien);
                            bodyParamsNV.Add("$maYCau", congvan.MaYeuCau);
                            bodyParamsNV.Add("$ngaytiepnhan", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                            bodyParamsNV.Add("$nhanvien", nhanVienNhan.fullName ?? nhanVienNhan.username);
                            bodyParamsNV.Add("$buochientai", "Phân công khảo sát");
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

        public PhanCongKS GetbyMaYCau(string loaiYCau, string maYCau)
        {
            return Get(p => p.MA_LOAI_YCAU == loaiYCau && p.MA_YCAU_KNAI == maYCau);
        }

        /// <summary>
        /// Hủy phân công khảo sát:
        /// - Cập nhật trạng thái của phân công khảo sát về  = 0
        /// - Cập nhật trạng thái của công văn yêu cầu về trạng thái trước đó
        /// </summary>
        public bool Cancel(CongVanYeuCau congvan, PhanCongKS item)
        {
            try
            {
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                BeginTran();
                item.TRANG_THAI = 0;
                Save(item);
                var ttrinhPK = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, "PK", -1);
                if (ttrinhPK != null && ttrinhPK.TRANG_THAI != 1)
                {
                    ttrinhPK.TRANG_THAI = 0;
                    tientrinhsrv.Save(ttrinhPK);
                }

                var thongbao = service.GetbyYCau(congvan.MaDViQLy, congvan.MaYeuCau, LoaiThongBao.KhaoSat);
                if (thongbao != null)
                {
                    service.Delete(thongbao);
                }
                if (congvan.TrangThai > TrangThaiCongVan.PhanCongKS)
                {
                    congvan.TrangThai = TrangThaiCongVan.PhanCongKS;
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
    }
}
