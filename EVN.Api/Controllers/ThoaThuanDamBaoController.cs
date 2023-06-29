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
    [RoutePrefix("api/thoathuandambao")]
    public class ThoaThuanDamBaoController : ApiController
    {
        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                IThoaThuanDamBaoService service = IoC.Resolve<IThoaThuanDamBaoService>();
                IOrganizationService orgsrv = IoC.Resolve<IOrganizationService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
                var congvan = congvansrv.Getbykey(id);
                var ttdn = ttdnservice.GetbyNo(congvan.SoThoaThuanDN, congvan.MaYeuCau);
                var item = service.GetbyCongvan(id);
                if (item == null)
                    item = new ThoaThuanDamBao();
                var org = orgsrv.GetbyCode(congvan.MaDViQLy);
                item.CongVanID = congvan.ID;

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

                if (string.IsNullOrWhiteSpace(item.KHChucVu))
                    item.KHChucVu = ttdn.KHChucDanh;
                if (string.IsNullOrWhiteSpace(item.KHDaiDien))
                    item.KHDaiDien = ttdn.KHDaiDien;
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
                {
                    item.Data = item.GetPdf();
                }

                ThoaThuanDamBaoModel model = new ThoaThuanDamBaoModel(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }


        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] ThoaThuanDamBaoModel model)
        {
            IThoaThuanDamBaoService service = IoC.Resolve<IThoaThuanDamBaoService>();
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            
            var yeucau = congvansrv.Getbykey(model.CongVanID);

            var item = new ThoaThuanDamBao();
            item = model.ToEntity(item);
            item.CongVanID = model.CongVanID;
            item.MaYeuCau = yeucau.MaYeuCau;
            item.MaDViQLy = yeucau.MaDViQLy;

            string message = "";
            item.Data = string.Empty;
            IList<ChiTietDamBao> listChiTiet = new List<ChiTietDamBao>();
            foreach (var chitiet in model.GiaTriDamBao)
            {
                ChiTietDamBao detail = new ChiTietDamBao();
                detail.GiaBanDien = chitiet.GiaBanDien;
                detail.MucDich = chitiet.MucDich;
                detail.SLTrungBinh = chitiet.SLTrungBinh;
                detail.SoNgayDamBao = chitiet.SoNgayDamBao;
                detail.ThanhTien = chitiet.ThanhTien;
                listChiTiet.Add(detail);
            }
            item = service.CreateNew(item, listChiTiet, out message);
            if (item == null)
                return BadRequest(message);
            return Ok(new ThoaThuanDamBaoModel(item));
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] ThoaThuanDamBaoModel model)
        {
            IThoaThuanDamBaoService service = IoC.Resolve<IThoaThuanDamBaoService>();
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            var yeucau = congvansrv.Getbykey(model.CongVanID);

            var item = service.Getbykey(model.ID);
            item = model.ToEntity(item);
            item.CongVanID = model.CongVanID;
            item.MaYeuCau = yeucau.MaYeuCau;
            item.MaDViQLy = yeucau.MaDViQLy;
            item.Data = string.Empty;
            IList<ChiTietDamBao> listChiTiet = new List<ChiTietDamBao>();
            foreach (var chitiet in model.GiaTriDamBao)
            {
                ChiTietDamBao detail = new ChiTietDamBao();
                detail.GiaBanDien = chitiet.GiaBanDien;
                detail.MucDich = chitiet.MucDich;
                detail.SLTrungBinh = chitiet.SLTrungBinh;
                detail.SoNgayDamBao = chitiet.SoNgayDamBao;
                detail.ThanhTien = chitiet.ThanhTien;
                listChiTiet.Add(detail);
            }

            string message = "";            
            item = service.Update(item, listChiTiet, out message);
            if (item == null)
                return BadRequest(message);
            return Ok(new ThoaThuanDamBaoModel(item));
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("sign")]
        public IHttpActionResult Sign([FromBody] SignModel model)
        {
            try
            {
                IThoaThuanDamBaoService service = IoC.Resolve<IThoaThuanDamBaoService>();
                var item = service.Getbykey(model.id);
                if (string.IsNullOrWhiteSpace(item.Data))
                    item.Data = item.GetPdf();

                if (!service.Sign(item))
                {
                    return BadRequest();
                }
                var result = new ThoaThuanDamBaoModel(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}