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
    public class KetQuaTCService : FX.Data.BaseService<KetQuaTC, int>, IKetQuaTCService
    {
        private ILog log = LogManager.GetLogger(typeof(KetQuaTCService));
        public KetQuaTCService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public KetQuaTC GetbyMaYCau(string maYCau)
        {
            return Get(p => p.MA_YCAU_KNAI == maYCau);
        }

        public bool SaveKetQua(BienBanDN bienbandn, KetQuaTC item, PhanCongTC phancongtc)
        {
            try
            {
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IPhanCongTCService pcongtcsrv = IoC.Resolve<IPhanCongTCService>();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();

                var congvan = congvansrv.GetbyMaYCau(bienbandn.MaYeuCau);
                string maCViecTruoc = congvan.MaCViec;
                var tthaiycau = ttycausrv.GetbyStatus((int)TrangThaiNghiemThu.BienBanTC, 1);
                if (tthaiycau != null)
                    maCViecTruoc = tthaiycau.CVIEC_TRUOC;
                var cviechtai = cauhinhsrv.Get(p => p.MA_LOAI_YCAU == congvan.MaLoaiYeuCau && p.MA_CVIEC_TRUOC == maCViecTruoc && p.MA_CVIEC_TIEP == item.MA_CVIEC);

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var cauhinhs = cauhinhsrv.GetbyMaCViec(congvan.MaLoaiYeuCau, item.MA_CVIEC);
                var cauhinh = cauhinhs.FirstOrDefault();

                if (maCViecTruoc == item.MA_CVIEC) cauhinh = cviechtai;
                long nextstep = tientrinhsrv.LastbyMaYCau(congvan.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                item.MA_YCAU_KNAI = congvan.MaYeuCau;
                item.MA_DDO_DDIEN = congvan.MaDDoDDien;
                item.MA_DVIQLY = congvan.MaDViQLy;

                item.MA_BPHAN_GIAO = userdata.maBPhan;
                item.MA_NVIEN_GIAO = userdata.maNVien;
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

                congvan.TrangThai = item.THUAN_LOI ? TrangThaiNghiemThu.BienBanTC : congvan.TrangThai;
                congvan.MaCViec = item.MA_CVIEC;
                if (cviechtai != null && cviechtai.TRANG_THAI_TIEP.HasValue)
                {
                    item.TRANG_THAI = 0;
                    congvan.MaCViec = cauhinh.MA_CVIEC_TIEP;
                    congvan.TrangThai = (TrangThaiNghiemThu)cviechtai.TRANG_THAI_TIEP.Value;
                    phancongtc.TRANG_THAI = 0;
                    pcongtcsrv.Save(phancongtc);
                }

                Save(item);
                congvansrv.Update(congvan);

                DvTienTrinh tientrinh = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, item.MA_CVIEC, 0);
                if (tientrinh == null)
                    tientrinh = new DvTienTrinh();
                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = item.MA_BPHAN_NHAN;
                tientrinh.MA_CVIEC = item.MA_CVIEC;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_DDO_DDIEN = congvan.MaDDoDDien;
                tientrinh.MA_DVIQLY = congvan.MaDViQLy;

                tientrinh.MA_NVIEN_NHAN = item.MA_NVIEN_NHAN;
                tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;
                tientrinh.NDUNG_XLY = item.NDUNG_XLY;

                tientrinh.NGAY_BDAU = DateTime.Today;
                //if (ttrinhtruoc != null)
                //    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.HasValue ? ttrinhtruoc.NGAY_KTHUC.Value : DateTime.Now;
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

                CommitTran();

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
