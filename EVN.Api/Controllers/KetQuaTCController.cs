﻿using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using System;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/ketquatc")]
    public class KetQuaTCController : ApiController
    {
        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                IKetQuaTCService service = IoC.Resolve<IKetQuaTCService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();

                var congvan = congvansrv.GetbyMaYCau(id);
                var item = service.GetbyMaYCau(congvan.MaYeuCau);

                if (item == null)
                {
                    item = new KetQuaTC();
                    item.MA_DVIQLY = congvan.MaDViQLy;

                    item.MA_YCAU_KNAI = congvan.MaYeuCau;
                    item.MA_DDO_DDIEN = congvan.MaDDoDDien;

                    item.NGAY_HEN = DateTime.Now;

                    item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                    item.MA_CVIEC_TRUOC = congvan.MaCViec;
                    item.THUAN_LOI = true;
                }
                KetQuaTCModel model = new KetQuaTCModel(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

       // [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] KetQuaTCModel model)
        {
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            IKetQuaTCService service = IoC.Resolve<IKetQuaTCService>();
            IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
            IPhanCongTCService pcongtcsrv = IoC.Resolve<IPhanCongTCService>();
            ICongViecService cvservice = IoC.Resolve<ICongViecService>();
            ITroNgaiService tngaisrv = IoC.Resolve<ITroNgaiService>();

            var yeucau = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
            var ttdn = ttdnservice.GetbyNo(yeucau.SoThoaThuanDN, yeucau.MaYeuCau);

            var congviec = cvservice.Getbykey(model.MA_CVIEC);
            if (string.IsNullOrWhiteSpace(model.NDUNG_XLY))
            {
                model.NDUNG_XLY = congviec.TEN_CVIEC;
                if (model.THUAN_LOI == 0)
                {
                    var tngai = tngaisrv.Getbykey(model.MA_TNGAI);
                    if (tngai != null) model.NDUNG_XLY = tngai.TEN_TNGAI;
                }
            }

            var item = service.GetbyMaYCau(yeucau.MaYeuCau);
            if (item == null) item = new KetQuaTC();

            item = model.ToEntity(item);
            item.MA_LOAI_YCAU = yeucau.MaLoaiYeuCau;
            item.MA_YCAU_KNAI = yeucau.MaYeuCau;
            item.MA_DVIQLY = yeucau.MaDViQLy;

            if (item.THUAN_LOI)
            {
                item.NGUYEN_NHAN = string.Empty;
                item.MA_TNGAI = string.Empty;
            }
            else
            {
                item.NGUYEN_NHAN = model.NGUYEN_NHAN;
                item.MA_TNGAI = model.MA_TNGAI;
            }

            if (item.TRANG_THAI == 0)
            {
                service.Save(item);
                service.CommitChanges();
                //return Ok(new KetQuaTCModel(item));
            }
            var pcongtc = pcongtcsrv.GetbyMaYCau(yeucau.MaLoaiYeuCau, yeucau.MaYeuCau);
            if (!service.SaveKetQua(ttdn, item, pcongtc))
                return BadRequest();
            else
            {
                if (service.SaveKetQua(ttdn, item, pcongtc)) ;
            }
            return Ok(new KetQuaTCModel(item));
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] KetQuaTCModel model)
        {
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            IKetQuaTCService service = IoC.Resolve<IKetQuaTCService>();
            IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
            IPhanCongTCService pcongtcsrv = IoC.Resolve<IPhanCongTCService>();
            ICongViecService cvservice = IoC.Resolve<ICongViecService>();
            ITroNgaiService tngaisrv = IoC.Resolve<ITroNgaiService>();

            var yeucau = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
            var ttdn = ttdnservice.GetbyNo(yeucau.SoThoaThuanDN, yeucau.MaYeuCau);
            var congviec = cvservice.Getbykey(model.MA_CVIEC);
            if (string.IsNullOrWhiteSpace(model.NDUNG_XLY))
            {
                model.NDUNG_XLY = congviec.TEN_CVIEC;
                if (model.THUAN_LOI == 0)
                {
                    var tngai = tngaisrv.Getbykey(model.MA_TNGAI);
                    if (tngai != null) model.NDUNG_XLY = tngai.TEN_TNGAI;
                }
            }

            var item = service.Getbykey(model.ID);
            item = model.ToEntity(item);
            item.MA_LOAI_YCAU = yeucau.MaLoaiYeuCau;
            item.MA_YCAU_KNAI = yeucau.MaYeuCau;
            item.MA_DVIQLY = yeucau.MaDViQLy;

            if (item.THUAN_LOI)
            {
                item.NGUYEN_NHAN = string.Empty;
                item.MA_TNGAI = string.Empty;
            }
            else
            {
                item.NGUYEN_NHAN = model.NGUYEN_NHAN;
                item.MA_TNGAI = model.MA_TNGAI;
            }
            if (item.TRANG_THAI == 0)
            {
                service.Save(item);
                service.CommitChanges();
                return Ok(new KetQuaTCModel(item));
            }
            var pcongtc = pcongtcsrv.GetbyMaYCau(yeucau.MaLoaiYeuCau, yeucau.MaYeuCau);
            if (!service.SaveKetQua(ttdn, item, pcongtc))
                return BadRequest();
            return Ok(new KetQuaTCModel(item));
        }
    }
}