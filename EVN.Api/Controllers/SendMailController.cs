using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/sendmail")]
    public class SendMailController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(SendMailController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(SendMailFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;

                DateTime synctime = DateTime.Today;
                ISendMailService service = IoC.Resolve<ISendMailService>();
                DateTime fromtime = DateTime.MinValue;
                DateTime totime = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.Filter.fromdate))
                    fromtime = DateTime.ParseExact(request.Filter.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.Filter.todate))
                    totime = DateTime.ParseExact(request.Filter.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var list = service.GetbyFilter(request.Filter.maYCau,request.Filter.keyword, request.Filter.status, fromtime, totime, pageindex, request.Paginator.pageSize, out total);
                var listModel = new List<SendMailData>();
                foreach (var item in list)
                {
                    var model = new SendMailData(item);
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
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                ISendMailService service = IoC.Resolve<ISendMailService>();
                var item = service.Getbykey(id);
                var model = new SendMailData(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] SendMailData model)
        {
            try
            {
                ISendMailService service = IoC.Resolve<ISendMailService>();

                var entity = service.Getbykey(model.ID);
                entity = model.ToEntity(entity);
                entity.TRANG_THAI = 3;
                service.Save(entity);
                service.CommitChanges();
                IDeliverService deliver = new DeliverService();
                deliver.Deliver(entity.MA_YCAU_KNAI);
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }
    }
}
