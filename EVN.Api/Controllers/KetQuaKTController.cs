using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using System;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/ketquakt")]
    public class KetQuaKTController : ApiController
    {
        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                IKetQuaKTService service = IoC.Resolve<IKetQuaKTService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();

                var congvan = congvansrv.GetbyMaYCau(id);
                
                var item = service.GetbyMaYCau(congvan.MaYeuCau);

                if (item == null)
                {
                    var ttrinhtruoc = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, congvan.MaCViec, -1);
                    item = new KetQuaKT();
                    item.MA_DVIQLY = congvan.MaDViQLy;

                    item.MA_YCAU_KNAI = congvan.MaYeuCau;
                    item.MA_DDO_DDIEN = congvan.MaDDoDDien;

                    item.NGAY_HEN = DateTime.Now;
                    item.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.HasValue ? ttrinhtruoc.NGAY_KTHUC.Value : DateTime.Today;
                    item.NGAY_HEN = DateTime.Now;

                    item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                    item.MA_CVIEC_TRUOC = "KTR";
                }
                KetQuaKTModel model = new KetQuaKTModel(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] KetQuaKTModel model)
        {
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            IKetQuaKTService service = IoC.Resolve<IKetQuaKTService>();
            ICongViecService cvservice = IoC.Resolve<ICongViecService>();
            ITroNgaiService tngaisrv = IoC.Resolve<ITroNgaiService>();

            var yeucau = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);

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
            if (item == null) item = new KetQuaKT();

            item = model.ToEntity(item);
            item.MA_LOAI_YCAU = yeucau.MaLoaiYeuCau;
            item.MA_YCAU_KNAI = yeucau.MaYeuCau;
            item.MA_DVIQLY = yeucau.MaDViQLy;

            if (item.THUAN_LOI)
            {
                item.NGUYEN_NHAN = string.Empty;
                item.MA_TNGAI = string.Empty;
            }

            if (item.TRANG_THAI == 0)
            {
                service.Save(item);
                service.CommitChanges();
                return Ok(new KetQuaKTModel(item));
            }
            if (!service.SaveKetQua(yeucau, item))
                return BadRequest();
            KetQuaKTModel result = new KetQuaKTModel(item);
            return Ok(result);
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] KetQuaKTModel model)
        {
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            IKetQuaKTService service = IoC.Resolve<IKetQuaKTService>();
            ICongViecService cvservice = IoC.Resolve<ICongViecService>();
            ITroNgaiService tngaisrv = IoC.Resolve<ITroNgaiService>();

            var yeucau = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
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

            KetQuaKT item = service.Getbykey(model.ID);
            item = model.ToEntity(item);
            item.MA_LOAI_YCAU = yeucau.MaLoaiYeuCau;
            item.MA_YCAU_KNAI = yeucau.MaYeuCau;
            item.MA_DVIQLY = yeucau.MaDViQLy;
            item.MA_DDO_DDIEN = yeucau.MaDDoDDien;

            if (item.THUAN_LOI)
            {
                item.NGUYEN_NHAN = string.Empty;
                item.MA_TNGAI = string.Empty;
            }

            if (item.TRANG_THAI == 0)
            {
                service.Save(item);
                service.CommitChanges();
                return Ok(new KetQuaKTModel(item));
            }
            if (!service.SaveKetQua(yeucau, item))
                return BadRequest();
            KetQuaKTModel result = new KetQuaKTModel(item);
            return Ok(result);
        }
    }
}
