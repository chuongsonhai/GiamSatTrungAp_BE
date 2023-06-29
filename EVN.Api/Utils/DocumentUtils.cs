using EVN.Core;
using EVN.Core.CMIS;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using System;

namespace EVN.Api.Utils
{
    public class DocumentUtils
    {
        static ILog log = LogManager.GetLogger(typeof(DocumentUtils));
        public static bool UpdatePdf(string maYCau, string loaiHSo, byte[] pdfdata, string noiDungXuLy, bool thuanLoi = true, bool huy = false)
        {
            switch (loaiHSo)
            {
                case "56":
                    return UpdatePdfBBKS(maYCau, pdfdata, noiDungXuLy, thuanLoi, huy);
                case "57":
                    return UpdatePdfBBDN(maYCau, pdfdata, noiDungXuLy, thuanLoi, huy);
                case "60":
                    return UpdatePdfBBKT(maYCau, pdfdata, noiDungXuLy, thuanLoi, huy);
                case "PL_BDPT":
                case "PL_TB":
                    return UpdatePLuc(maYCau, pdfdata, loaiHSo, thuanLoi, huy);
                case "PL_TTDBao":
                    return UpdateTTDBao(maYCau, pdfdata, loaiHSo, thuanLoi, huy);
                case "PL_TTMBan":
                    return UpdateTTMBan(maYCau, pdfdata, loaiHSo, thuanLoi, huy);
                case "PL_TTCDut":
                    return UpdateTTCDut(maYCau, pdfdata, loaiHSo, thuanLoi, huy);
                default: return false;
            }
        }

        public static bool UpdateStatus(string maYCau, string loaiHSo, string noiDungXuLy, int trangThai = 0)
        {
            switch (loaiHSo)
            {
                case "HDNH":
                    return UpdateHDMB(maYCau, noiDungXuLy, trangThai);
                case "HDNSH":
                    return UpdateHDMB(maYCau, noiDungXuLy, trangThai);
                case "BBAN_TTHAO":
                    return UpdateBBTT(maYCau, noiDungXuLy, trangThai);
                default: return false;
            }
        }

        #region UpdatePDF        

        static bool UpdatePdfBBKS(string maYCau, byte[] pdfdata, string noiDungXuLy, bool thuanLoi = true, bool huy = false)
        {
            try
            {
                IBienBanKSService bbksservice = IoC.Resolve<IBienBanKSService>();
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                IKetQuaKSService ketquasrv = IoC.Resolve<IKetQuaKSService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();

                var yeucau = service.GetbyMaYCau(maYCau);
                var item = bbksservice.GetbyYeuCau(yeucau.MaYeuCau);
                log.Error($"{item.MaYeuCau}, ?thuanloi: {thuanLoi}");
                if (!thuanLoi)
                {
                    if (huy)
                        return bbksservice.Cancel(yeucau, item, noiDungXuLy);
                    else
                    {
                        try
                        {
                            log.Error($"{item.MaYeuCau}: {noiDungXuLy}");
                            string maLoaiHSo = LoaiHSoCode.BB_KS;
                            item.TrangThai = (int)TrangThaiBienBan.MoiTao;
                            item.TroNgai = noiDungXuLy;
                            var ketquaks = ketquasrv.GetbyMaYCau(yeucau.MaYeuCau);
                            var hoso = hsoservice.Get(p => p.MaDViQLy == item.MaDViQLy && p.MaYeuCau == item.MaYeuCau && p.LoaiHoSo == maLoaiHSo);
                            bbksservice.BeginTran();
                            if (hoso != null)
                            {
                                hsoservice.Delete(hoso);
                            }
                            ketquaks.TRANG_THAI = 0;
                            ketquaks.NDUNG_XLY = noiDungXuLy;
                            ketquasrv.Save(ketquaks);

                            yeucau.TrangThai = TrangThaiCongVan.GhiNhanKS;
                            service.Save(yeucau);

                            bbksservice.Save(item);
                            bbksservice.CommitTran();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            bbksservice.RolbackTran();
                            return false;
                        }
                    }
                }
                if (item.TrangThai == (int)TrangThaiBienBan.DaDuyet && thuanLoi && pdfdata != null)
                {
                    log.Error($"Update pdf: {item.MaYeuCau}");
                    return bbksservice.Confirm(item, pdfdata);
                }                    
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        static bool UpdatePdfBBDN(string maYCau, byte[] pdfdata, string noiDungXuLy, bool thuanLoi = true, bool huy = false)
        {
            try
            {
                IBienBanDNService service = IoC.Resolve<IBienBanDNService>();
                var item = service.GetbyMaYeuCau(maYCau);
                if (item.TrangThai == (int)TrangThaiBienBan.DuThao)
                {
                    if (thuanLoi)
                    {
                        string maLoaiHSo = LoaiHSoCode.BB_DN;
                        IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
                        var hoso = hsoservice.GetHoSoGiayTo(item.MaDViQLy, item.MaYeuCau, maLoaiHSo);
                        try
                        {
                            service.BeginTran();
                            hoso.TrangThai = 7;
                            hsoservice.Save(hoso);

                            item.KHXacNhan = true;
                            item.TrangThai = (int)TrangThaiBienBan.MoiTao;
                            service.Save(item);
                            service.CommitTran();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            service.RolbackTran();
                            return false;
                        }
                    }
                    if (huy) return service.Cancel(item);
                    return service.Adjust(item, noiDungXuLy);
                }
                if (!thuanLoi)
                {
                    if (huy)
                        return service.Cancel(item);
                    return service.Adjust(item, noiDungXuLy);
                }
                if (item.TrangThai == (int)TrangThaiBienBan.DaDuyet && thuanLoi && pdfdata != null)
                    return service.Confirm(item, pdfdata);
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        static bool UpdatePdfBBKT(string maYCau, byte[] pdfdata, string noiDungXuLy, bool thuanLoi = true, bool huy = false)
        {
            try
            {
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();
                IKetQuaKTService ketquasrv = IoC.Resolve<IKetQuaKTService>();
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();

                var item = service.GetbyMaYCau(maYCau);
                var yeucau = cvservice.GetbyMaYCau(maYCau);

                log.Error($"{item.MaYeuCau}, ?thuanloi: {thuanLoi}");
                if (!thuanLoi)
                {
                    if (huy)
                        return service.Cancel(yeucau, item, noiDungXuLy);
                    else
                    {
                        try
                        {
                            log.Error($"{item.MaYeuCau}: {noiDungXuLy}");
                            string maLoaiHSo = LoaiHSoCode.BB_KT;
                            item.TrangThai = (int)TrangThaiBienBan.MoiTao;
                            item.TroNgai = noiDungXuLy;

                            var ketquaks = ketquasrv.GetbyMaYCau(yeucau.MaYeuCau);
                            var hoso = hsoservice.Get(p => p.MaDViQLy == item.MaDViQLy && p.MaYeuCau == item.MaYeuCau && p.LoaiHoSo == maLoaiHSo);
                            service.BeginTran();
                            if (hoso != null)
                            {
                                hsoservice.Delete(hoso);
                            }
                            ketquaks.TRANG_THAI = 0;
                            ketquaks.NDUNG_XLY = item.TroNgai;
                            ketquasrv.Save(ketquaks);

                            yeucau.TrangThai = TrangThaiNghiemThu.GhiNhanKT;
                            cvservice.Save(yeucau);

                            service.Save(item);
                            service.CommitTran();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            service.RolbackTran();
                            return false;
                        }
                    }                    
                }
                if (item.TrangThai == (int)TrangThaiBienBan.DaDuyet && thuanLoi && pdfdata != null)
                {
                    log.Error($"Update pdf: {item.MaYeuCau}");
                    return service.Confirm(item, pdfdata);
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        static bool UpdatePLuc(string maYCau, byte[] pdfdata, string loaiHoSo, bool thuanLoi = true, bool huy = false)
        {
            try
            {
                IRepository repository = new FileStoreRepository();

                IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();

                var yeucau = cvservice.GetbyMaYCau(maYCau);
                var hoSo = hsgtservice.GetHoSoGiayTo(yeucau.MaDViQLy, yeucau.MaYeuCau, loaiHoSo);
                if (thuanLoi)
                {
                    string folder = $"{yeucau.MaDViQLy}/{yeucau.MaYeuCau}";
                    hoSo.TrangThai = 1;
                    hoSo.Data = repository.Store(folder, pdfdata, hoSo.Data);
                    hsgtservice.Save(hoSo);
                    hsgtservice.CommitChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }
        
        static bool UpdateTTDBao(string maYCau, byte[] pdfdata, string loaiHoSo, bool thuanLoi = true, bool huy = false)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                IThoaThuanDamBaoService service = IoC.Resolve<IThoaThuanDamBaoService>();
                IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();

                var item = service.GetbyMaYCau(maYCau);
                var yeucau = cvservice.GetbyMaYCau(maYCau);
                var hoSo = hsgtservice.GetHoSoGiayTo(yeucau.MaDViQLy, yeucau.MaYeuCau, loaiHoSo);
                if (thuanLoi)
                {
                    try
                    {
                        string folder = $"{yeucau.MaDViQLy}/{yeucau.MaYeuCau}";
                        item.Data = repository.Store(folder, pdfdata, item.Data);
                        item.TrangThai = (int)TrangThaiBienBan.KhachHangKy;

                        service.BeginTran();
                        service.Save(item);

                        hoSo.TrangThai = 1;
                        hoSo.Data = item.Data;
                        hsgtservice.Save(hoSo);
                        service.CommitTran();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        service.RolbackTran();
                        log.Error(ex);
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        static bool UpdateTTMBan(string maYCau, byte[] pdfdata, string loaiHoSo, bool thuanLoi = true, bool huy = false)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                IThoaThuanTyLeService service = IoC.Resolve<IThoaThuanTyLeService>();
                IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();

                var item = service.GetbyMaYCau(maYCau);
                var yeucau = cvservice.GetbyMaYCau(maYCau);
                var hoSo = hsgtservice.GetHoSoGiayTo(yeucau.MaDViQLy, yeucau.MaYeuCau, loaiHoSo);
                if (thuanLoi)
                {
                    try
                    {
                        string folder = $"{yeucau.MaDViQLy}/{yeucau.MaYeuCau}";
                        item.Data = repository.Store(folder, pdfdata, item.Data);
                        item.TrangThai = (int)TrangThaiBienBan.KhachHangKy;

                        service.BeginTran();
                        service.Save(item);

                        hoSo.TrangThai = 1;
                        hoSo.Data = item.Data;
                        hsgtservice.Save(hoSo);
                        service.CommitTran();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        service.RolbackTran();
                        log.Error(ex);
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        static bool UpdateTTCDut(string maYCau, byte[] pdfdata, string loaiHoSo, bool thuanLoi = true, bool huy = false)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                IChamDutHopDongService service = IoC.Resolve<IChamDutHopDongService>();
                IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();

                var item = service.GetbyMaYCau(maYCau);
                var yeucau = cvservice.GetbyMaYCau(maYCau);
                var hoSo = hsgtservice.GetHoSoGiayTo(yeucau.MaDViQLy, yeucau.MaYeuCau, loaiHoSo);
                if (thuanLoi)
                {
                    try
                    {
                        string folder = $"{yeucau.MaDViQLy}/{yeucau.MaYeuCau}";
                        item.Data = repository.Store(folder, pdfdata, item.Data);
                        item.TrangThai = (int)TrangThaiBienBan.KhachHangKy;

                        service.BeginTran();
                        service.Save(item);

                        hoSo.TrangThai = 1;
                        hoSo.Data = item.Data;
                        hsgtservice.Save(hoSo);
                        service.CommitTran();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        service.RolbackTran();
                        log.Error(ex);
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        #endregion

        #region UpdateStatus
        static bool UpdateHDMB(string maYCau, string noiDungXuLy, int trangThai = 0)
        {
            try
            {
                log.Error($"{maYCau}-trangThai:{trangThai}");
                IHopDongService hdservice = IoC.Resolve<IHopDongService>();
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();

                var congvan = service.GetbyMaYCau(maYCau);
                var item = hdservice.GetbyMaYCau(maYCau);
                if (trangThai > 0)
                {
                    ttrinhsrv.DongBoTienDo(congvan);
                    if (trangThai == 2)
                        return hdservice.Cancel(item);

                    item.TrangThai = (int)TrangThaiBienBan.MoiTao;
                    item.NoiDungXuLy = noiDungXuLy;
                    hdservice.Save(item);
                    hdservice.CommitChanges();
                    return true;
                }
                string maLoaiHSo = LoaiHSoCode.HD_NSH;
                ICmisProcessService cmisProcess = new CmisProcessService();
                byte[] pdfdata = cmisProcess.GetData(item.MaDViQLy, maYCau, maLoaiHSo);
                if (pdfdata == null || pdfdata.Length == 0)
                {
                    maLoaiHSo = LoaiHSoCode.HD_NH;
                    pdfdata = cmisProcess.GetData(item.MaDViQLy, maYCau, maLoaiHSo);
                }
                if (pdfdata != null && pdfdata.Length > 0)
                    hdservice.UpdatebyCMIS(item, pdfdata);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        static bool UpdateBBTT(string maYCau, string noiDungXuLy, int trangThai = 0)
        {
            IBienBanTTService bbttservice = IoC.Resolve<IBienBanTTService>();
            IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
            IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
            try
            {
                var yeucau = service.GetbyMaYCau(maYCau);
                var item = bbttservice.GetbyMaYCau(maYCau);
                if (trangThai > 0)
                {
                    if (trangThai == 2)
                        return bbttservice.Cancel(item);

                    item.TRANG_THAI = 0;
                    item.NoiDungXuLy = noiDungXuLy;
                    bbttservice.Save(item);
                    return true;
                }
                string maLoaiHSo = LoaiHSoCode.BB_TT;
                var hoso = hsoservice.Get(p => p.MaDViQLy == yeucau.MaDViQLy && p.MaYeuCau == maYCau && p.LoaiHoSo == maLoaiHSo);
                IRepository repository = new FileStoreRepository();
                string folder = $"{yeucau.MaDViQLy}/{yeucau.MaYeuCau}";

                bbttservice.BeginTran();
                item.TRANG_THAI = (int)TrangThaiBienBan.KhachHangKy;
                hoso.TrangThai = 1;
                hsoservice.Save(hoso);
                bbttservice.Save(item);
                bbttservice.CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                bbttservice.RolbackTran();
                log.Error(ex);
                return false;
            }
        }
        #endregion
    }
}