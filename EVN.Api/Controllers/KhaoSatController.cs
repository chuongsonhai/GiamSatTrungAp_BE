using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/khaosat")]
    public class KhaoSatController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(KhaoSatController));

        //[JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(KhaoSatFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                DateTime synctime = DateTime.Today;
                IKhaoSatService service = IoC.Resolve<IKhaoSatService>();
                var list = service.GetbyFilter(request.Filter.CANHBAO_ID, pageindex, request.Paginator.pageSize, out total);
                var listModel = new List<KhaoSatRequest>();
                foreach (var item in list)
                {
                    var model = new KhaoSatRequest(item);
                    listModel.Add(model);
                }
                result.total = total;
                result.data = listModel;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new List<KhaoSatRequest>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        //[JwtAuthentication]
        [HttpPost]
        [Route("add")]
        public IHttpActionResult Post([FromBody] KhaoSatRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IKhaoSatService service = IoC.Resolve<IKhaoSatService>();

                var item = new KhaoSat();
                item.CANHBAO_ID = model.CANHBAO_ID;
                item.NOIDUNG_CAUHOI = model.NOIDUNG_CAUHOI;
                item.PHANHOI_KH = model.PHANHOI_KH;
                item.THOIGIAN_KHAOSAT = model.THOIGIAN_KHAOSAT.Date;
                item.NGUOI_KS = model.NGUOI_KS;
                item.KETQUA = model.KETQUA;
                item.TINHTRANG_KT_CB = model.TINHTRANG_KT_CB;
                item.TRANGTHAI_XOA_KHAOSAT = model.TRANGTHAI_XOA_KHAOSAT;
                item.DONVI_QLY = model.DONVI_QLY;
                service.CreateNew(item);
                service.CommitChanges();
                result.success = true;
                return Ok(result);

            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = ex.Message;
                result.success = false;
                return Ok(result);
            }
        }
        //[JwtAuthentication]
        [HttpGet]
        public IHttpActionResult GetById([FromUri] int Id)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IKhaoSatService service = IoC.Resolve<IKhaoSatService>();
                var item = new KhaoSat();
                item = service.Getbykey(Id);
                result.data = item;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new KhaoSat();
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        //[JwtAuthentication]
        [HttpPost]
        public IHttpActionResult UpdateById([FromBody] KhaoSatRequest model, [FromUri] int Id)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IKhaoSatService service = IoC.Resolve<IKhaoSatService>();
                var item = new KhaoSat();
                item.ID = Id;
                item.CANHBAO_ID = model.CANHBAO_ID;
                item.NOIDUNG_CAUHOI = model.NOIDUNG_CAUHOI;
                item.PHANHOI_KH = model.PHANHOI_KH;
                item.THOIGIAN_KHAOSAT = model.THOIGIAN_KHAOSAT;
                item.NGUOI_KS = model.NGUOI_KS;
                item.KETQUA = model.KETQUA;
                item.TINHTRANG_KT_CB = model.TINHTRANG_KT_CB;
                item.TRANGTHAI_XOA_KHAOSAT = model.TRANGTHAI_XOA_KHAOSAT;
                item.DONVI_QLY = model.DONVI_QLY;
                service.Update(item);
                service.CommitChanges();
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        //[JwtAuthentication]
        [HttpGet]
        [Route("delete")]
        public IHttpActionResult Delete([FromUri] int ID)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IKhaoSatService service = IoC.Resolve<IKhaoSatService>();
                var item = new KhaoSat();
                item.ID = ID;
                //item.TenLoaiCanhBao = model.TenLoaiCanhBao;
                //item.ChuKyGui = model.ChuKyGui;
                //item.PhanLoai = model.PhanLoai;
                service.Delete(item);
                service.CommitChanges();
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }



    }
}