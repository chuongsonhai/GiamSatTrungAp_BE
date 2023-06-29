using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Linq;
using System.Web;

namespace EVN.Core.Implements
{
    public class KetQuaKSService : FX.Data.BaseService<KetQuaKS, int>, IKetQuaKSService
    {
        private ILog log = LogManager.GetLogger(typeof(KetQuaKSService));
        public KetQuaKSService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public bool SaveKetQua(CongVanYeuCau congvan, KetQuaKS item)
        {
            try
            {
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

                BeginTran();
                var ttrinhtruoc = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, maCViecTruoc, 0);
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = item.MA_CVIEC;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }

                congvan.TrangThai = TrangThaiCongVan.BienBanKS;
                congvan.MaCViec = item.MA_CVIEC;                
                if (cauhinh.TRANG_THAI_TIEP.HasValue)
                {
                    item.TRANG_THAI = 0;
                    congvan.MaCViec = cauhinh.MA_CVIEC_TIEP;
                    congvan.TrangThai = (TrangThaiCongVan)cauhinh.TRANG_THAI_TIEP.Value;
                }

                congvansrv.Update(congvan);

                item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                item.MA_YCAU_KNAI = congvan.MaYeuCau;
                item.MA_DDO_DDIEN = congvan.MaDDoDDien;
                item.MA_DVIQLY = congvan.MaDViQLy;

                item.MA_BPHAN_GIAO = userdata.maBPhan;
                item.MA_NVIEN_GIAO = userdata.maNVien;
                item.MA_CVIEC_TRUOC = maCViecTruoc;
                Save(item);

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
                tientrinh.MA_TNGAI = item.MA_TNGAI;

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

        public KetQuaKS GetbyMaYCau(string maYCau)
        {
            return Get(p => p.MA_YCAU_KNAI == maYCau);
        }
    }
}
