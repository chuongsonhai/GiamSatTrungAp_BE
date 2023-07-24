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
    [RoutePrefix("api/PhanhoiTraodoi")]
    public class PhanhoiTraodoiController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(PhanhoiTraodoiController));

        //[JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(PhanhoiTraodoiFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                DateTime synctime = DateTime.Today;
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
                var list = service.GetbyFilter(request.Filter.CANHBAO_ID, pageindex, request.Paginator.pageSize, out total);
                var listModel = new List<PhanhoiTraodoiRequest>();
                foreach (var item in list)
                {
                    var model = new PhanhoiTraodoiRequest(item);
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
                result.data = new List<PhanhoiTraodoiRequest>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        //[JwtAuthentication]
        [HttpPost]
        [Route("add")]
        public IHttpActionResult Post([FromBody] PhanhoiTraodoiRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();

                var item = new PhanhoiTraodoi();
                item.CANHBAO_ID = model.CANHBAO_ID;
                item.NOIDUNG_PHANHOI = model.NOIDUNG_PHANHOI;
                item.NGUOI_GUI = model.NGUOI_GUI;
                item.DONVI_QLY = model.DONVI_QLY;
                item.THOIGIAN_GUI = model.THOIGIAN_GUI;
                item.TRANGTHAI_XOA = model.TRANGTHAI_XOA;
                item.PHANHOI_TRAODOI_ID = model.PHANHOI_TRAODOI_ID;
                item.FILE_DINHKEM = model.FILE_DINHKEM;
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
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
                var item = new PhanhoiTraodoi();
                item = service.Getbykey(Id);
                result.data = item;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new PhanhoiTraodoiRequest();
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        //[JwtAuthentication]
        [HttpPost]
        public IHttpActionResult UpdateById([FromBody] PhanhoiTraodoiRequest model, [FromUri] int Id)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
                var item = new PhanhoiTraodoi();
                item.ID = Id;
                item.CANHBAO_ID = model.CANHBAO_ID;
                item.NOIDUNG_PHANHOI = model.NOIDUNG_PHANHOI;
                item.NGUOI_GUI = model.NGUOI_GUI;
                item.DONVI_QLY = model.DONVI_QLY;
                item.THOIGIAN_GUI = model.THOIGIAN_GUI;
                item.TRANGTHAI_XOA = model.TRANGTHAI_XOA;
                item.PHANHOI_TRAODOI_ID = model.PHANHOI_TRAODOI_ID;
                item.FILE_DINHKEM = model.FILE_DINHKEM;
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
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
                var item = new PhanhoiTraodoi();
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