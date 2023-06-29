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
    public class ChuyenTiepService : FX.Data.BaseService<ChuyenTiep, int>, IChuyenTiepService
    {
        ILog log = LogManager.GetLogger(typeof(ChuyenTiepService));
        public ChuyenTiepService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public bool SaveChuyenTiep(BienBanDN bienbanDN, ChuyenTiep item)
        {
            try
            {
                IThoaThuanDNChiTietService chitietsrv = IoC.Resolve<IThoaThuanDNChiTietService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();

                var congvan = congvansrv.GetbyMaYCau(bienbanDN.MaYeuCau);
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                string cvTruoc = "KDN";
                string maCViec = "TVB";
                var ttrinhtruoc = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, cvTruoc, 0);

                item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                item.MA_YCAU_KNAI = congvan.MaYeuCau;
                item.MA_DDO_DDIEN = congvan.MaDDoDDien;
                item.MA_DVIQLY = congvan.MaDViQLy;

                item.MA_BPHAN_GIAO = userdata.maBPhan;
                item.MA_NVIEN_GIAO = userdata.maNVien;
                item.MA_CVIEC_TRUOC = cvTruoc;
                item.MA_CVIEC = maCViec;

                BeginTran();
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;                    
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }

                Save(item);
                congvan.TrangThai = TrangThaiCongVan.HoanThanh;
                //congvan.MaCViec = item.MA_CVIEC;
                congvansrv.Update(congvan);

                ThoaThuanDNChiTiet history = new ThoaThuanDNChiTiet();
                history.MaYeuCau = congvan.MaYeuCau;
                history.NgayYeuCau = congvan.NgayYeuCau;
                history.DuAnDien = congvan.DuAnDien;
                history.DiaChiDungDien = congvan.DiaChiDungDien;
                history.Type = DoingBusinessType.KhaoSat;
                history.MoTa = "Hoàn thành thỏa thuận đấu nối: " + userdata.maNVien;
                history.NgayThucHien = DateTime.Now;
                history.UpdateDate = DateTime.Now;
                chitietsrv.CreateNew(history);
                CommitTran();

                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                var sendmail = new SendMail();
                sendmail.EMAIL = congvan.Email;
                sendmail.TIEUDE = "Hoàn thành thỏa thuận đấu nối vào lưới điện trung áp";
                sendmail.MA_DVIQLY = congvan.MaDViQLy;
                sendmail.MA_YCAU_KNAI = congvan.MaYeuCau;
                Dictionary<string, string> bodyParams = new Dictionary<string, string>();
                bodyParams.Add("$khachHang", congvan.TenKhachHang ?? congvan.NguoiYeuCau);
                bodyParams.Add("$maYCau", congvan.MaYeuCau);
                bodyParams.Add("$ngayYCau", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                bodyParams.Add("$ngayDuyet", DateTime.Now.ToString("dd/MM/yyyy"));
                bodyParams.Add("$soThoaThuan", bienbanDN.SoBienBan);
                bodyParams.Add("$ngayThoaThuan", bienbanDN.NgayLap.ToString("dd/MM/yyyy"));
                sendmailsrv.Process(sendmail, "HoanThanhThoaThuan", bodyParams);

                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(congvan.MaDViQLy, congvan.MaYeuCau);
                deliver.Deliver(congvan.MaYeuCau);

                try
                {
                    var sendmailnv = new SendMail();
                    var nhanVienNhan = userdatasrv.Query.Where(x => x.maNVien == item.MA_NVIEN_NHAN).FirstOrDefault();

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
                            bodyParamsNV.Add("$maYCau", congvan.MaYeuCau);
                            bodyParamsNV.Add("$donVi", congvan.MaDViQLy);
                            bodyParamsNV.Add("$duAnDien", congvan.DuAnDien);
                            bodyParamsNV.Add("$khuVuc", congvan.DiaChiDungDien);
                            bodyParamsNV.Add("$ngaytiepnhan", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                            bodyParamsNV.Add("$nhanvien", nhanVienNhan.fullName ?? nhanVienNhan.username);
                            bodyParamsNV.Add("$buochientai", "Hoàn thành thoả thuận đấu nối");
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
                RolbackTran();
                return false;
            }
        }
        
        public ChuyenTiep GetbyMaYCau(string loaiYCau, string maYCau)
        {
            return Get(p => p.MA_LOAI_YCAU == loaiYCau && p.MA_YCAU_KNAI == maYCau);
        }
    }
}
