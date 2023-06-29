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
    public class ThoaThuanDamBaoService : FX.Data.BaseService<ThoaThuanDamBao, int>, IThoaThuanDamBaoService
    {
        ILog log = LogManager.GetLogger(typeof(ThoaThuanDamBaoService));
        public ThoaThuanDamBaoService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public ThoaThuanDamBao GetbyCongvan(int congvanid)
        {
            return Get(p => p.CongVanID == congvanid);
        }
        public ThoaThuanDamBao CreateNew(ThoaThuanDamBao thoaThuanDamBao, IList<ChiTietDamBao> chiTietDamBaos, out string message)
        {
            message = "";
            try
            {
                IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
                IChiTietDamBaoService chiTietDamBaoService = IoC.Resolve<IChiTietDamBaoService>();
                thoaThuanDamBao.GiaTriDamBao = chiTietDamBaos;
                thoaThuanDamBao.Data = thoaThuanDamBao.GetPdf();
                BeginTran();
                CreateNew(thoaThuanDamBao);
                foreach (var item in chiTietDamBaos)
                {
                    item.ThoaThuanID = thoaThuanDamBao.ID;
                    chiTietDamBaoService.CreateNew(item);
                }

                string maLoaiHSo = LoaiHSoCode.PL_HD_DB;
                var hoSo = hsgtservice.GetHoSoGiayTo(thoaThuanDamBao.MaDViQLy, thoaThuanDamBao.MaYeuCau, maLoaiHSo);
                if (hoSo == null) hoSo = new HoSoGiayTo();
                hoSo.TrangThai = 0;
                hoSo.MaYeuCau = thoaThuanDamBao.MaYeuCau;
                hoSo.MaDViQLy = thoaThuanDamBao.MaDViQLy;
                hoSo.LoaiHoSo = maLoaiHSo;
                hoSo.TenHoSo = "THỎA THUẬN BẢO ĐẢM THỰC HIỆN HỢP ĐỒNG MUA BÁN ĐIỆN";
                hoSo.NguoiKy = thoaThuanDamBao.NguoiKyUQ;
                hoSo.ChucVu = thoaThuanDamBao.ChucVu;
                hoSo.Data = thoaThuanDamBao.Data;
                hoSo.TrinhKy = true;
                hsgtservice.Save(hoSo);
                CommitTran();
                return thoaThuanDamBao;
            }
            catch (Exception ex)
            {
                RolbackTran();
                message = ex.Message;
                return null;
            }
        }
        public ThoaThuanDamBao Update(ThoaThuanDamBao thoaThuanDamBao, IList<ChiTietDamBao> chiTietDamBaos, out string message)
        {
            message = "";
            try
            {
                IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
                IChiTietDamBaoService chiTietDamBaoService = IoC.Resolve<IChiTietDamBaoService>();

                BeginTran();
                foreach (ChiTietDamBao pp in thoaThuanDamBao.GiaTriDamBao)
                {
                    chiTietDamBaoService.Delete(pp);
                }
                thoaThuanDamBao.GiaTriDamBao = chiTietDamBaos;
                thoaThuanDamBao.Data = thoaThuanDamBao.GetPdf(true);
                Save(thoaThuanDamBao);
                foreach (ChiTietDamBao pp in chiTietDamBaos)
                {
                    pp.ThoaThuanID = thoaThuanDamBao.ID;
                    chiTietDamBaoService.CreateNew(pp);
                }

                string maLoaiHSo = LoaiHSoCode.PL_HD_DB;
                var hoSo = hsgtservice.GetHoSoGiayTo(thoaThuanDamBao.MaDViQLy, thoaThuanDamBao.MaYeuCau, maLoaiHSo);
                if (hoSo == null) hoSo = new HoSoGiayTo();
                hoSo.TrangThai = 0;
                hoSo.MaYeuCau = thoaThuanDamBao.MaYeuCau;
                hoSo.MaDViQLy = thoaThuanDamBao.MaDViQLy;
                hoSo.LoaiHoSo = maLoaiHSo;
                hoSo.TenHoSo = "THỎA THUẬN BẢO ĐẢM THỰC HIỆN HỢP ĐỒNG MUA BÁN ĐIỆN";
                hoSo.NguoiKy = thoaThuanDamBao.NguoiKyUQ;
                hoSo.ChucVu = thoaThuanDamBao.ChucVu;
                hoSo.Data = thoaThuanDamBao.Data;
                hoSo.TrinhKy = true;
                hsgtservice.Save(hoSo);
                CommitTran();
                return thoaThuanDamBao;
            }
            catch (Exception ex)
            {
                RolbackTran();
                message = ex.Message;
                return null;
            }
        }

        public bool Sign(ThoaThuanDamBao item)
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
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                var cvyc = congVanYeuCauService.GetbyMaYCau(item.MaYeuCau);

                string maLoaiHSo = LoaiHSoCode.PL_HD_DB;
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

        public ThoaThuanDamBao GetbyMaYCau(string maYCau)
        {
            return Get(p => p.MaYeuCau == maYCau);
        }
    }
}