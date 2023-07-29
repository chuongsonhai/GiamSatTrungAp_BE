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
    [RoutePrefix("api/dmucLoaiCanhBao")]
    public class DanhMucLoaiCanhBaoController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(DanhMucLoaiCanhBaoController));

        //[JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(LoaiCanhBaoFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                DateTime synctime = DateTime.Today;
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();
                var list = service.GetbyFilter(request.Filter.TenLoaiCanhbao, request.Filter.maLoaiCanhBao, pageindex, request.Paginator.pageSize, out total);
                var listModel = new List<LoaiCanhBaoDataRequest>();
                foreach (var item in list)
                {
                    var model = new LoaiCanhBaoDataRequest(item);
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
                result.data = new List<LoaiCanhBaoDataRequest>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        //[JwtAuthentication]
        [HttpPost]
        [Route("add")]
        public IHttpActionResult Post([FromBody] LoaiCanhBaoDataRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();

                var item = new DanhMucLoaiCanhBao();
                item.TENLOAICANHBAO = model.TENLOAICANHBAO;
               // item.ID = model.MALOAICANHBAO;
                item.CHUKYCANHBAO = model.CHUKYCANHBAO;
                item.THOIGIANCHAYCUOI = model.THOIGIANCHAYCUOI;
                item.TRANGTHAI = model.TRANGTHAI;
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

                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();
                var item = new DanhMucLoaiCanhBao();
                item = service.Getbykey(Id);
                result.data = item;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new LoaiCanhBaoDataRequest();
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }


        //[JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Updatebyid([FromBody] LoaiCanhBaoDataRequest model, [FromUri] int Id)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();
                var item = new DanhMucLoaiCanhBao();
                item.ID = Id;
                item.TENLOAICANHBAO = model.TENLOAICANHBAO;
                // item.ID = model.MALOAICANHBAO;
                item.CHUKYCANHBAO = model.CHUKYCANHBAO;
                item.THOIGIANCHAYCUOI = model.THOIGIANCHAYCUOI;
                item.TRANGTHAI = model.TRANGTHAI;
                service.Update(item);
                service.CommitChanges();
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                //result.data = new LoaiCanhBaoDataRequest();
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
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();
                var item = new DanhMucLoaiCanhBao();
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