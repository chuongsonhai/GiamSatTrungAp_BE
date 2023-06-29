using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/ketquaks")]
    public class KetQuaKSController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(KetQuaKSController));

        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                IKetQuaKSService service = IoC.Resolve<IKetQuaKSService>();
                IPhanCongKSService pcksservice = IoC.Resolve<IPhanCongKSService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();
                var tthaiycau = ttycausrv.GetbyStatus((int)TrangThaiCongVan.GhiNhanKS, 0);

                var congvan = congvansrv.GetbyMaYCau(id);

                var item = service.GetbyMaYCau(congvan.MaYeuCau);
                if (item == null)
                {
                    item = new KetQuaKS();
                    item.MA_DVIQLY = congvan.MaDViQLy;

                    item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                    item.MA_YCAU_KNAI = congvan.MaYeuCau;
                    item.MA_DDO_DDIEN = congvan.MaDDoDDien;

                    item.NGAY_HEN = DateTime.Now;
                    item.NGAY_BDAU = DateTime.Today;

                    item.THUAN_LOI = true;
                    item.MA_CVIEC_TRUOC = "KS";
                }
                var pcks = pcksservice.GetbyMaYCau(congvan.MaLoaiYeuCau, congvan.MaYeuCau);
                KetQuaKSModel model = new KetQuaKSModel(item);
                model.GHI_NHAN_KQ = pcks != null && pcks.TRANG_THAI == 1;
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] KetQuaKSModel model)
        {
            ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
            IKetQuaKSService service = IoC.Resolve<IKetQuaKSService>();
            ICongViecService cvservice = IoC.Resolve<ICongViecService>();
            ITroNgaiService tngaisrv = IoC.Resolve<ITroNgaiService>();

            var yeucau = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);

            if (string.IsNullOrWhiteSpace(model.NDUNG_XLY))
            {
                var congviec = cvservice.Getbykey(model.MA_CVIEC);
                model.NDUNG_XLY = congviec.TEN_CVIEC;
                if (model.THUAN_LOI == 0)
                {
                    var tngai = tngaisrv.Getbykey(model.MA_TNGAI);
                    if (tngai != null) model.NDUNG_XLY = tngai.TEN_TNGAI;
                }
            }

            KetQuaKS item = service.GetbyMaYCau(yeucau.MaYeuCau);
            if (item == null) item = new KetQuaKS();

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
                return Ok(new KetQuaKSModel(item));
            }
            if (!service.SaveKetQua(yeucau, item))
                return BadRequest();
            KetQuaKSModel result = new KetQuaKSModel(item);
            return Ok(result);
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] KetQuaKSModel model)
        {
            ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
            IKetQuaKSService service = IoC.Resolve<IKetQuaKSService>();
            var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
            KetQuaKS item = service.Getbykey(model.ID);
            item = model.ToEntity(item);
            if (item.THUAN_LOI)
            {
                item.NGUYEN_NHAN = String.Empty;
                item.MA_TNGAI = string.Empty;
            }
            if (item.TRANG_THAI == 0)
            {
                service.Save(item);
                service.CommitChanges();
                return Ok(item);
            }
            if (!service.SaveKetQua(congvan, item))
                return BadRequest();
            KetQuaKSModel result = new KetQuaKSModel(item);
            return Ok(result);
        }
    }
}