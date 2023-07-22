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
                var list = service.GetbyFilter(request.Filter.TenLoaiCanhbao, pageindex, request.Paginator.pageSize, out total);
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
        public IHttpActionResult Post([FromBody] LoaiCanhBaoDataRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();

                var item = new DanhMucLoaiCanhBao();
                item.TenLoaiCanhBao = model.TenLoaiCanhBao;
                item.ChuKyGui = model.ChuKyGui;
                item.PhanLoai = model.PhanLoai;
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
                return Ok(result);
            }
        }

      
        [HttpPost]
        [Route("getlistcanhbao")]
        public IHttpActionResult GetListCanhBao()
        {
            ResponseResult result = new ResponseResult();
            try
            {
               
                IReportService service = IoC.Resolve<IReportService>();
                var list = service.TinhThoiGian();
                result.data = list;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

    }
}
