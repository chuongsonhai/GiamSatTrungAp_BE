using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    //[RoutePrefix("api/dashboard")]
    public class CauhinhcanhbaoController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(CanhBaoController));

        //2.10	(GET) /cauhinhcanhbao/filter
        //[JwtAuthentication]
        [HttpPost]
        [Route("cauhinhcanhbao/filter")]
        public IHttpActionResult cauhinhcanhbao(LoaiCanhBaoFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                // int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                // int total = 0;
                DateTime synctime = DateTime.Today;
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();
                var list = service.Filter(request.Filter.maLoaiCanhBao);
                IList<Cauhinhcanhbao> data = new List<Cauhinhcanhbao>();

                foreach (var item in list)
                {
                    data.Add(new Cauhinhcanhbao(item));

                }
                // result.total = list.Count();
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new List<Cauhinhcanhbao>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }


        //2.11	(POST) /cauhinhcanhbao/add
        //[JwtAuthentication]
        [HttpPost]
        [Route("cauhinhcanhbao/add")]
        public IHttpActionResult cauhinhcanhbaoadd([FromBody] LoaiCanhBaoDataRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();

                var item = new DanhMucLoaiCanhBao();

                item.TENLOAICANHBAO = model.TENLOAICANHBAO;
                item.CHUKYCANHBAO = model.CHUKYCANHBAO;
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

        //2.12	(POST) /cauhinhcanhbao/edit
        //[JwtAuthentication]
        [HttpPost]
        [Route("cauhinhcanhbao/edit")]
        public IHttpActionResult cauhinhcanhbaoedit([FromBody] LoaiCanhBaoDataRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();
                var item = new DanhMucLoaiCanhBao();
                item.ID = model.ID;
                // item.ID = model.MALOAICANHBAO;
                item.TENLOAICANHBAO = model.TENLOAICANHBAO;
                item.CHUKYCANHBAO = model.CHUKYCANHBAO;
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
        [HttpPost]
        [Route("cauhinhcanhbao/log/filter")]
        public IHttpActionResult logFilter(LogCanhBaofilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                // int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                // int total = 0;
                DateTime synctime = DateTime.Today;
                ILogCanhBaoService service = IoC.Resolve<ILogCanhBaoService>();
                var list = service.GetbyFilter(request.Filter.canhbaoID, request.Filter.trangThai, request.Filter.datacu,
                    request.Filter.datamoi, request.Filter.tungay, request.Filter.denngay, request.Filter.nguoithuchien);
                IList<LogCanhBaoRequest> data = new List<LogCanhBaoRequest>();

                foreach (var item in list)
                {
                    data.Add(new LogCanhBaoRequest(item));

                }
                // result.total = list.Count();
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new List<LogCanhBao>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

    }
}
