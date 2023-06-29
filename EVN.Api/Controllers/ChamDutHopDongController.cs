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
    [RoutePrefix("api/chamduthopdong")]
    public class ChamDutHopDongController : ApiController
    {
        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                IChamDutHopDongService service = IoC.Resolve<IChamDutHopDongService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
                IOrganizationService orgsrv = IoC.Resolve<IOrganizationService>();
                var congvan = congvansrv.Getbykey(id);
                var ttdn = ttdnservice.GetbyNo(congvan.SoThoaThuanDN, congvan.MaYeuCau);
                var item = service.GetbyCongvan(id);
                if (item == null)
                {
                    item = new ChamDutHopDong();

                    var org = orgsrv.GetbyCode(congvan.MaDViQLy);
                    item.CongVanID = congvan.ID;
                    item.DonVi = org.orgName;
                    item.DaiDien = org.daiDien;
                    item.ChucVu = org.chucVu;
                    item.DiaDiem = org.address;
                    item.SoTaiKhoan = org.soTaiKhoan;
                    item.NganHang = org.nganHang;
                    item.DienThoai = org.phone;
                    item.MaSoThue = org.taxCode;
                    item.Email = org.email;
                    item.Fax = org.fax;


                    item.KHChucVu = ttdn.KHChucDanh;
                    item.KHDaiDien = ttdn.KHDaiDien;
                    item.KHDienThoai = ttdn.KHDienThoai;
                    item.KHMa = ttdn.MaKH;
                    item.KHMaSoThue = ttdn.KHMaSoThue;
                    item.KHTen = ttdn.KHTen;
                    item.MaDViQLy = ttdn.MaDViQLy;
                    item.MaYeuCau = ttdn.MaYeuCau;
                    item.NgayLap = DateTime.Now;
                    item.Data = item.GetPdf();
                }
                if (item != null && string.IsNullOrWhiteSpace(item.Data))
                {
                    item.Data = item.GetPdf();
                }

                ChamDutHopDongModel model = new ChamDutHopDongModel(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }


        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] ChamDutHopDongModel model)
        {
            IChamDutHopDongService service = IoC.Resolve<IChamDutHopDongService>();
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            var yeucau = congvansrv.Getbykey(model.CongVanID);

            var item = new ChamDutHopDong();
            item = model.ToEntity(item);
            item.CongVanID = model.CongVanID;
            item.MaYeuCau = yeucau.MaYeuCau;
            item.MaDViQLy = yeucau.MaDViQLy;

            string message = "";
            item.Data = string.Empty;
            IList<HeThongDDChamDut> listChiTiet = new List<HeThongDDChamDut>();
            foreach (var chitiet in model.HeThongDDChamDut)
            {
                HeThongDDChamDut detail = new HeThongDDChamDut();
                detail.DiemDo = chitiet.DiemDo;
                detail.SoCongTo = chitiet.SoCongTo;
                detail.Loai = chitiet.Loai;
                detail.TI = chitiet.TI;
                detail.TU = chitiet.TU;
                detail.HeSoNhan = chitiet.HeSoNhan;
                detail.ChiSoChot = chitiet.ChiSoChot;
                listChiTiet.Add(detail);
            }
            item = service.CreateNew(item, listChiTiet, out message);
                       
            if (item == null)
                return BadRequest(message);
            return Ok(new ChamDutHopDongModel(item));
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] ChamDutHopDongModel model)
        {
            IChamDutHopDongService service = IoC.Resolve<IChamDutHopDongService>();
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            var yeucau = congvansrv.Getbykey(model.CongVanID);

            var item = service.Getbykey(model.ID);
            item = model.ToEntity(item);
            item.CongVanID = model.CongVanID;
            item.MaYeuCau = yeucau.MaYeuCau;
            item.MaDViQLy = yeucau.MaDViQLy;
            item.Data = string.Empty;
            IList<HeThongDDChamDut> listChiTiet = new List<HeThongDDChamDut>();
            foreach (var chitiet in model.HeThongDDChamDut)
            {
                HeThongDDChamDut detail = new HeThongDDChamDut();
                detail.DiemDo = chitiet.DiemDo;
                detail.SoCongTo = chitiet.SoCongTo;
                detail.Loai = chitiet.Loai;
                detail.TI = chitiet.TI;
                detail.TU = chitiet.TU;
                detail.HeSoNhan = chitiet.HeSoNhan;
                detail.ChiSoChot = chitiet.ChiSoChot;
                listChiTiet.Add(detail);
            }

            string message = "";
            item = service.Update(item, listChiTiet, out message);
            
            if (item == null)
                return BadRequest(message);
            return Ok(new ChamDutHopDongModel(item));
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("sign")]
        public IHttpActionResult Sign([FromBody] SignModel model)
        {
            try
            {
                IChamDutHopDongService service = IoC.Resolve<IChamDutHopDongService>();
                var item = service.Getbykey(model.id);
                if (string.IsNullOrWhiteSpace(item.Data))
                    item.Data = item.GetPdf();

                if (!service.Sign(item))
                {
                    return BadRequest();
                }
                var result = new ChamDutHopDongModel(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}