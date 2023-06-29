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
    public class ThoaThuanTyLeService : FX.Data.BaseService<ThoaThuanTyLe, int>, IThoaThuanTyLeService
    {
        ILog log = LogManager.GetLogger(typeof(ThoaThuanTyLeService));
        public ThoaThuanTyLeService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public ThoaThuanTyLe GetbyCongvan(int congvanid)
        {
            return Get(p => p.CongVanID == congvanid);
        }
        public ThoaThuanTyLe CreateNew(ThoaThuanTyLe thoaThuanTyLe, IList<MucDichThucTeSDD> mucDichs, IList<GiaDienTheoMucDich> giaDiens, out string message)
        {
            message = "";
            try
            {
                IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
                IMucDichThucTeSDDService mucDichThucTeSDDService = IoC.Resolve<IMucDichThucTeSDDService>();
                IGiaDienTheoMucDichService giaDienTheoMucDichService = IoC.Resolve<IGiaDienTheoMucDichService>();

                BeginTran();
                thoaThuanTyLe.GiaDienTheoMucDich = giaDiens;
                thoaThuanTyLe.MucDichThucTeSDD = mucDichs;
                thoaThuanTyLe.Data = thoaThuanTyLe.GetPdf();
                CreateNew(thoaThuanTyLe);
                foreach (var item in mucDichs)
                {
                    item.ThoaThuanID = thoaThuanTyLe.ID;
                    mucDichThucTeSDDService.CreateNew(item);
                }
                foreach (var item in giaDiens)
                {
                    item.ThoaThuanID = thoaThuanTyLe.ID;
                    giaDienTheoMucDichService.CreateNew(item);
                }

                string maLoaiHSo = LoaiHSoCode.PL_HD_MB;
                var hoSo = hsgtservice.GetHoSoGiayTo(thoaThuanTyLe.MaDViQLy, thoaThuanTyLe.MaYeuCau, maLoaiHSo);
                if (hoSo == null) hoSo = new HoSoGiayTo();
                hoSo.TrangThai = 0;
                hoSo.MaYeuCau = thoaThuanTyLe.MaYeuCau;
                hoSo.MaDViQLy = thoaThuanTyLe.MaDViQLy;
                hoSo.LoaiHoSo = maLoaiHSo;
                hoSo.TenHoSo = "BIÊN BẢN THỎA THUẬN TỶ LỆ MỤC ĐÍCH SỬ DỤNG ĐIỆN";
                hoSo.NguoiKy = thoaThuanTyLe.NguoiKyUQ;
                hoSo.ChucVu = thoaThuanTyLe.ChucVu;
                hoSo.Data = thoaThuanTyLe.Data;
                hoSo.TrinhKy = true;
                hsgtservice.Save(hoSo);
                CommitTran();

                return thoaThuanTyLe;
            }
            catch (Exception ex)
            {
                RolbackTran();
                message = ex.Message;
                return null;
            }
        }
        public ThoaThuanTyLe Update(ThoaThuanTyLe thoaThuanTyLe, IList<MucDichThucTeSDD> mucDichs, IList<GiaDienTheoMucDich> giaDiens, out string message)
        {
            message = "";
            try
            {
                IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
                IMucDichThucTeSDDService mucDichThucTeSDDService = IoC.Resolve<IMucDichThucTeSDDService>();
                IGiaDienTheoMucDichService giaDienTheoMucDichService = IoC.Resolve<IGiaDienTheoMucDichService>();

                BeginTran();
                foreach (var item in thoaThuanTyLe.MucDichThucTeSDD)
                {
                    mucDichThucTeSDDService.Delete(item);
                }

                foreach (var item in thoaThuanTyLe.GiaDienTheoMucDich)
                {
                    giaDienTheoMucDichService.Delete(item);
                }

                thoaThuanTyLe.GiaDienTheoMucDich = giaDiens;
                thoaThuanTyLe.MucDichThucTeSDD = mucDichs;
                thoaThuanTyLe.Data = thoaThuanTyLe.GetPdf(true);
                Save(thoaThuanTyLe);
                
                foreach (var item in mucDichs)
                {
                    item.ThoaThuanID = thoaThuanTyLe.ID;
                    mucDichThucTeSDDService.CreateNew(item);
                }
                
                foreach (var item in giaDiens)
                {
                    item.ThoaThuanID = thoaThuanTyLe.ID;
                    giaDienTheoMucDichService.CreateNew(item);
                }

                string maLoaiHSo = LoaiHSoCode.PL_HD_MB;
                var hoSo = hsgtservice.GetHoSoGiayTo(thoaThuanTyLe.MaDViQLy, thoaThuanTyLe.MaYeuCau, maLoaiHSo);
                if (hoSo == null) hoSo = new HoSoGiayTo();
                hoSo.TrangThai = 0;
                hoSo.MaYeuCau = thoaThuanTyLe.MaYeuCau;
                hoSo.MaDViQLy = thoaThuanTyLe.MaDViQLy;
                hoSo.LoaiHoSo = maLoaiHSo;
                hoSo.TenHoSo = "BIÊN BẢN THỎA THUẬN TỶ LỆ MỤC ĐÍCH SỬ DỤNG ĐIỆN";
                hoSo.NguoiKy = thoaThuanTyLe.NguoiKyUQ;
                hoSo.ChucVu = thoaThuanTyLe.ChucVu;
                hoSo.Data = thoaThuanTyLe.Data;
                hoSo.TrinhKy = true;
                hsgtservice.Save(hoSo);
                CommitTran();
                return thoaThuanTyLe;
            }
            catch (Exception ex)
            {
                RolbackTran();
                message = ex.Message;
                return null;
            }
        }

        public bool Sign(ThoaThuanTyLe item)
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

                string maLoaiHSo = LoaiHSoCode.PL_HD_MB;
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

        public ThoaThuanTyLe GetbyMaYCau(string maYCau)
        {
            return Get(p => p.MaYeuCau == maYCau);
        }
    }
}