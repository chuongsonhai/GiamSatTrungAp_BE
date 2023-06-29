using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/thoathuantyle")]
    public class ThoaThuanTyLeController : ApiController
    {
        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                IThoaThuanTyLeService service = IoC.Resolve<IThoaThuanTyLeService>();
                IThietBiService thietBiService = IoC.Resolve<IThietBiService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
                IOrganizationService orgsrv = IoC.Resolve<IOrganizationService>();

                var yeucau = congvansrv.Getbykey(id);
                var ttdn = ttdnservice.GetbyNo(yeucau.SoThoaThuanDN, yeucau.MaYeuCau);
                var item = service.GetbyCongvan(id);
                if (item == null)
                    item = new ThoaThuanTyLe();

                var org = orgsrv.GetbyCode(yeucau.MaDViQLy);
                item.CongVanID = yeucau.ID;
                if (string.IsNullOrWhiteSpace(item.DonVi))
                    item.DonVi = org.orgName;
                if (string.IsNullOrWhiteSpace(item.DaiDien))
                    item.DaiDien = org.daiDien;
                if (string.IsNullOrWhiteSpace(item.ChucVu))
                    item.ChucVu = org.chucVu;
                if (string.IsNullOrWhiteSpace(item.DiaDiem))
                    item.DiaDiem = org.address;
                if (string.IsNullOrWhiteSpace(item.SoTaiKhoan))
                    item.SoTaiKhoan = org.soTaiKhoan;
                if (string.IsNullOrWhiteSpace(item.NganHang))
                    item.NganHang = org.nganHang;
                if (string.IsNullOrWhiteSpace(item.DienThoai))
                    item.DienThoai = org.phone;
                if (string.IsNullOrWhiteSpace(item.MaSoThue))
                    item.MaSoThue = org.taxCode;
                if (string.IsNullOrWhiteSpace(item.Email))
                    item.Email = org.email;
                if (string.IsNullOrWhiteSpace(item.Fax))
                    item.Fax = org.fax;

                if (string.IsNullOrWhiteSpace(item.KHChucVu))
                    item.KHChucVu = ttdn.KHChucDanh;
                if (string.IsNullOrWhiteSpace(item.KHDaiDien))
                    item.KHDaiDien = ttdn.KHDaiDien;
                if (string.IsNullOrWhiteSpace(item.KHDiaChiDungDien))
                    item.KHDiaChiDungDien = ttdn.KHDiaChi;
                if (string.IsNullOrWhiteSpace(item.KHDienThoai))
                    item.KHDienThoai = ttdn.KHDienThoai;
                if (string.IsNullOrWhiteSpace(item.KHMa))
                    item.KHMa = ttdn.MaKH;
                if (string.IsNullOrWhiteSpace(item.KHMaSoThue))
                    item.KHMaSoThue = ttdn.KHMaSoThue;
                if (string.IsNullOrWhiteSpace(item.KHTen))
                    item.KHTen = ttdn.KHTen;

                item.MaDViQLy = ttdn.MaDViQLy;
                item.MaYeuCau = ttdn.MaYeuCau;
                item.NgayLap = DateTime.Now;
                item.Data = item.GetPdf();

                if (item != null && string.IsNullOrWhiteSpace(item.Data))
                    item.Data = item.GetPdf();

                item.MucDichThucTeSDD.Clear();
                IList<ThietBi> thietBis = thietBiService.GetByFilter(yeucau.MaDViQLy, yeucau.MaYeuCau);
                if (thietBis.Count > 0)
                {
                    foreach (var tb in thietBis)
                    {
                        MucDichThucTeSDD mucDichThucTeSDD = new MucDichThucTeSDD();
                        mucDichThucTeSDD.TenThietBi = tb.Ten;
                        mucDichThucTeSDD.CongSuat = tb.CongSuat;
                        mucDichThucTeSDD.SoGio = tb.SoGio;
                        mucDichThucTeSDD.SoNgay = tb.SoNgay;
                        mucDichThucTeSDD.TongCongSuatSuDung = tb.TongCongSuat;
                        mucDichThucTeSDD.DienNangSuDung = tb.DienNangSD;
                        mucDichThucTeSDD.MucDichSDD = tb.MucDichSD;
                        item.MucDichThucTeSDD.Add(mucDichThucTeSDD);
                    }
                }
                ThoaThuanTyLeModel model = new ThoaThuanTyLeModel(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }


        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] ThoaThuanTyLeModel model)
        {
            IThoaThuanTyLeService service = IoC.Resolve<IThoaThuanTyLeService>();
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            IHoSoGiayToService hsgtservice = IoC.Resolve<IHoSoGiayToService>();
            var congvan = congvansrv.Getbykey(model.CongVanID);

            var item = new ThoaThuanTyLe();
            item = model.ToEntity(item);
            item.CongVanID = model.CongVanID;
            item.MaYeuCau = congvan.MaYeuCau;
            item.MaDViQLy = congvan.MaDViQLy;

            string message = "";
            item.Data = string.Empty;

            IList<MucDichThucTeSDD> listChiTietMucDichTT = new List<MucDichThucTeSDD>();
            foreach (var chitiet in model.MucDichThucTeSDD)
            {
                MucDichThucTeSDD detail = new MucDichThucTeSDD();
                detail.TenThietBi = chitiet.TenThietBi;
                detail.MucDichSDD = chitiet.MucDichSDD;
                detail.SoNgay = chitiet.SoNgay;
                detail.SoLuong = chitiet.SoLuong;
                detail.TongCongSuatSuDung = chitiet.TongCongSuatSuDung;
                detail.CongSuat = chitiet.CongSuat;
                detail.DienNangSuDung = chitiet.DienNangSuDung;
                detail.SoGio = chitiet.SoGio;
                detail.HeSoDongThoi = chitiet.HeSoDongThoi;
                listChiTietMucDichTT.Add(detail);
            }
            IList<GiaDienTheoMucDich> listChiTietGiaDien = new List<GiaDienTheoMucDich>();
            foreach (var chitiet in model.GiaDienTheoMucDich)
            {
                GiaDienTheoMucDich detail = new GiaDienTheoMucDich();
                detail.SoCongTo = chitiet.SoCongTo;
                detail.ApDungTuChiSo = chitiet.ApDungTuChiSo;
                detail.MucDichSuDung = chitiet.MucDichSuDung;
                detail.TyLe = chitiet.TyLe;
                detail.MaGhiChiSo = chitiet.MaGhiChiSo;
                detail.GDGioBT = chitiet.GDGioBT;
                detail.GDGioCD = chitiet.GDGioCD;
                detail.GDGioTD = chitiet.GDGioTD;
                detail.GDKhongTheoTG = chitiet.GDKhongTheoTG;
                listChiTietGiaDien.Add(detail);
            }
            item = service.CreateNew(item, listChiTietMucDichTT, listChiTietGiaDien, out message);
            if (item == null)
                return BadRequest(message);
            return Ok(new ThoaThuanTyLeModel(item));
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] ThoaThuanTyLeModel model)
        {
            IThoaThuanTyLeService service = IoC.Resolve<IThoaThuanTyLeService>();
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            var congvan = congvansrv.Getbykey(model.CongVanID);

            var item = service.Getbykey(model.ID);
            item = model.ToEntity(item);
            item.CongVanID = model.CongVanID;
            item.MaYeuCau = congvan.MaYeuCau;
            item.MaDViQLy = congvan.MaDViQLy;
            item.Data = string.Empty;
            IList<MucDichThucTeSDD> listChiTietMucDichTT = new List<MucDichThucTeSDD>();
            foreach (var chitiet in model.MucDichThucTeSDD)
            {
                MucDichThucTeSDD detail = new MucDichThucTeSDD();
                detail.TenThietBi = chitiet.TenThietBi;
                detail.MucDichSDD = chitiet.MucDichSDD;
                detail.SoNgay = chitiet.SoNgay;
                detail.SoLuong = chitiet.SoLuong;
                detail.TongCongSuatSuDung = chitiet.TongCongSuatSuDung;
                detail.CongSuat = chitiet.CongSuat;
                detail.DienNangSuDung = chitiet.DienNangSuDung;
                detail.SoGio = chitiet.SoGio;
                detail.HeSoDongThoi = chitiet.HeSoDongThoi;
                listChiTietMucDichTT.Add(detail);
            }
            IList<GiaDienTheoMucDich> listChiTietGiaDien = new List<GiaDienTheoMucDich>();
            foreach (var chitiet in model.GiaDienTheoMucDich)
            {
                GiaDienTheoMucDich detail = new GiaDienTheoMucDich();
                detail.SoCongTo = chitiet.SoCongTo;
                detail.ApDungTuChiSo = chitiet.ApDungTuChiSo;
                detail.MucDichSuDung = chitiet.MucDichSuDung;
                detail.TyLe = chitiet.TyLe;
                detail.MaGhiChiSo = chitiet.MaGhiChiSo;
                detail.GDGioBT = chitiet.GDGioBT;
                detail.GDGioCD = chitiet.GDGioCD;
                detail.GDGioTD = chitiet.GDGioTD;
                detail.GDKhongTheoTG = chitiet.GDKhongTheoTG;
                listChiTietGiaDien.Add(detail);
            }

            string message = "";
            item = service.Update(item, listChiTietMucDichTT, listChiTietGiaDien, out message);
                       
            if (item == null)
                return BadRequest(message);
            return Ok(new ThoaThuanTyLeModel(item));
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("sign")]
        public IHttpActionResult Sign([FromBody] SignModel model)
        {
            try
            {
                IThoaThuanTyLeService service = IoC.Resolve<IThoaThuanTyLeService>();
                var item = service.Getbykey(model.id);
                if (string.IsNullOrWhiteSpace(item.Data))
                    item.Data = item.GetPdf();

                if (!service.Sign(item))
                {
                    return BadRequest();
                }
                var result = new ThoaThuanTyLeModel(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}