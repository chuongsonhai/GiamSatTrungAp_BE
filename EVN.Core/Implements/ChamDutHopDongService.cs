using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.PMIS;
using EVN.Core.Repository;
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
    public class ChamDutHopDongService : FX.Data.BaseService<ChamDutHopDong, int>, IChamDutHopDongService
    {
        ILog log = LogManager.GetLogger(typeof(ThoaThuanTyLeService));
        public ChamDutHopDongService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public ChamDutHopDong GetbyCongvan(int congvanid)
        {
            return Get(p => p.CongVanID == congvanid);
        }
        public ChamDutHopDong CreateNew(ChamDutHopDong item, IList<HeThongDDChamDut> hethongdodem, out string message)
        {
            message = "";
            try
            {
                IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
                IHeThongDDChamDutService chiTietDamBaoService = IoC.Resolve<IHeThongDDChamDutService>();

                BeginTran();
                item.HeThongDDChamDut = hethongdodem;
                item.Data = item.GetPdf();
                CreateNew(item);
                foreach (var dodem in hethongdodem)
                {
                    dodem.ThoaThuanID = item.ID;
                    chiTietDamBaoService.CreateNew(dodem);
                }
                string maLoaiHSo = LoaiHSoCode.PL_HD_CD;
                var hoSo = hsgtservice.GetHoSoGiayTo(item.MaDViQLy, item.MaYeuCau, maLoaiHSo);
                if (hoSo == null) hoSo = new HoSoGiayTo();
                hoSo.TrangThai = 0;
                hoSo.MaYeuCau = item.MaYeuCau;
                hoSo.MaDViQLy = item.MaDViQLy;
                hoSo.LoaiHoSo = maLoaiHSo;
                hoSo.TenHoSo = "BIÊN BẢN CHẤM DỨT HỢP ĐỒNG MUA BÁN ĐIỆN";
                hoSo.NguoiKy = item.NguoiKyUQ;
                hoSo.ChucVu = item.ChucVu;
                hoSo.Data = item.Data;
                hoSo.TrinhKy = true;
                hsgtservice.Save(hoSo);
                CommitTran();
                return item;
            }
            catch (Exception ex)
            {
                RolbackTran();
                message = ex.Message;
                return null;
            }
        }
        public ChamDutHopDong Update(ChamDutHopDong item, IList<HeThongDDChamDut> hethongdodem, out string message)
        {
            message = "";
            try
            {
                IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
                IHeThongDDChamDutService chiTietDamBaoService = IoC.Resolve<IHeThongDDChamDutService>();

                BeginTran();
                foreach (HeThongDDChamDut pp in item.HeThongDDChamDut)
                {
                    chiTietDamBaoService.Delete(pp);
                }
                item.HeThongDDChamDut = hethongdodem;
                item.Data = item.GetPdf(true);
                Save(item);
                foreach (HeThongDDChamDut pp in hethongdodem)
                {
                    pp.ThoaThuanID = item.ID;
                    chiTietDamBaoService.CreateNew(pp);
                }

                string maLoaiHSo = LoaiHSoCode.PL_HD_CD;
                var hoSo = hsgtservice.GetHoSoGiayTo(item.MaDViQLy, item.MaYeuCau, maLoaiHSo);
                if (hoSo == null) hoSo = new HoSoGiayTo();
                hoSo.TrangThai = 0;
                hoSo.MaYeuCau = item.MaYeuCau;
                hoSo.MaDViQLy = item.MaDViQLy;
                hoSo.LoaiHoSo = maLoaiHSo;
                hoSo.TenHoSo = "BIÊN BẢN CHẤM DỨT HỢP ĐỒNG MUA BÁN ĐIỆN";
                hoSo.NguoiKy = item.NguoiKyUQ;
                hoSo.ChucVu = item.ChucVu;
                hoSo.Data = item.Data;
                hoSo.TrinhKy = true;
                hsgtservice.Save(hoSo);
                CommitTran();
                return item;
            }
            catch (Exception ex)
            {
                RolbackTran();
                message = ex.Message;
                return null;
            }
        }

        public bool Sign(ChamDutHopDong item)
        {
            IRepository repository = new FileStoreRepository();
            var pdfdata = repository.GetData(item.Data);
            if (pdfdata == null)
                throw new Exception("Không tìm thấy file path.");

            try
            {
                IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
                IOrganizationService orgSrv = IoC.Resolve<IOrganizationService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                var cvyc = congVanYeuCauService.GetbyMaYCau(item.MaYeuCau);
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                string maLoaiHSo = LoaiHSoCode.PL_HD_CD;
                var org = orgSrv.GetbyCode(item.MaDViQLy);
                string orgCode = org.compCode;
                string signName = org.daiDien;
              
                var result = PdfSignUtil.SignPdf(signName, orgCode, pdfdata, "BÊN BÁN ĐIỆN");
                if (!result.suc)
                    return false;

                BeginTran();
                string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";
                item.Data = repository.Store(folder, Convert.FromBase64String(result.data), item.Data);
                item.TrangThai = 1;
                Save(item);
                             
                var hoSo = hsgtservice.GetHoSoGiayTo(item.MaDViQLy, item.MaYeuCau, maLoaiHSo);
                hoSo.TrangThai = 2;
                hoSo.Data = item.Data;
                hsgtservice.Save(hoSo);

                CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                return false;
            }
        }

        public ChamDutHopDong GetbyMaYCau(string maYCau)
        {
            return Get(p => p.MaYeuCau == maYCau);
        }
    }
}