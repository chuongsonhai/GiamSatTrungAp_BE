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
    [RoutePrefix("api/cauhinhcanhbao")]
    public class CauhinhcanhbaoController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(CanhBaoController));

        //2.10	(GET) /cauhinhcanhbao/filter
        //[JwtAuthentication]
        [HttpPost]
        [Route("log/filter")]
        public IHttpActionResult FilterLog([FromBody] CauHinhCanhBaoLogFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                //DateTime synctime = DateTime.Today;
                //var fromDate = DateTime.MinValue;
                //var toDate = DateTime.MaxValue;
                //if (!string.IsNullOrWhiteSpace(request.Filter.tuNgay))
                //    fromDate = DateTime.ParseExact(request.Filter.tuNgay, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                //if (!string.IsNullOrWhiteSpace(request.Filter.denNgay))
                //    toDate = DateTime.ParseExact(request.Filter.denNgay, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                ILogCanhBaoService service = IoC.Resolve<ILogCanhBaoService>();
                ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
                List<object> resultList = new List<object>();
                var listCanhBao = canhBaoService.Filter1(request.Filter.fromDate, request.Filter.toDate, request.Filter.maLoaiCanhBao, request.Filter.trangThai, request.Filter.donViQuanLy, pageindex, request.Paginator.pageSize, out total);
                foreach (var canhbao in listCanhBao)
                {
                    IList<LogCanhBao> listLog = service.Filter(canhbao.ID);
                    foreach (var log in listLog)
                    {
                        resultList.Add(new { log.ID, canhbao.LOAI_CANHBAO_ID, canhbao.NOIDUNG, log.DATA_CU, log.NGUOITHUCHIEN, log.THOIGIAN, canhbao.DONVI_DIENLUC, canhbao.TRANGTHAI_CANHBAO });
                    }
                }
                result.total = total;
                result.data = resultList;
                result.success = true;
                if (result.total == 0)
                {
                    result.message = "Không có dữ liệu";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.data = null;
                return Ok(result);
            }
        }
        //2.10	(GET) /cauhinhcanhbao/filter
        //[JwtAuthentication]
        [HttpPost]
        [Route("cauhinhcanhbao/filter")]
        public IHttpActionResult cauhinhcanhbao(CauhinhcanhbaoFilterRequest request)
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
        public IHttpActionResult cauhinhcanhbaoadd([FromBody] Cauhinhcanhbao model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();

                var item = new DanhMucLoaiCanhBao();

                item.TENLOAICANHBAO = model.tenLoaiCanhbao;
                item.CHUKYCANHBAO = model.chuKy;
                item.TRANGTHAI = 0;
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

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult cauhinhcanhbaoedit([FromUri] int ID)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();
                var item = new DanhMucLoaiCanhBao();
                item = service.GetbyNo(ID);
                Cauhinhcanhbao obj = new Cauhinhcanhbao(item);
                // item.ID = model.MALOAICANHBAO;
                result.data = obj;
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


        //2.12	(POST) /cauhinhcanhbao/edit
        //[JwtAuthentication]
        [HttpPost]
        [Route("edit")]
        public IHttpActionResult cauhinhcanhbaoedit()
        {
            ResponseFileResult result = new ResponseFileResult();
            var httpRequest = HttpContext.Current.Request;
            try
            {
                string data = httpRequest.Form["data"];
                Cauhinhcanhbao model = JsonConvert.DeserializeObject<Cauhinhcanhbao>(data);
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();
                var item = new DanhMucLoaiCanhBao();
                item = service.GetbyNo(model.maLoaiCanhBao);
                // item.ID = model.MALOAICANHBAO;
                item.TENLOAICANHBAO = model.tenLoaiCanhbao;
                item.CHUKYCANHBAO = model.chuKy;
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
        [Route("log/filter2")]
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
