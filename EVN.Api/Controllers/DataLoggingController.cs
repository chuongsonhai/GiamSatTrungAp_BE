using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/datalogging")]
    public class DataloggingController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(DataloggingController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        

        public IHttpActionResult Filter(DataLoggingFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;


                DateTime fromtime = DateTime.MinValue;
                DateTime totime = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.Filter.fromdate))
                    fromtime = DateTime.ParseExact(request.Filter.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.Filter.todate))
                    totime = DateTime.ParseExact(request.Filter.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                IDataLoggingService service = IoC.Resolve<IDataLoggingService>();
                var list = service.GetbyFilter(request.Filter.maDonVi, request.Filter.maYCau, request.Filter.userName, request.Filter.keyword,fromtime,totime, pageindex, request.Paginator.pageSize, out total);

                
                var listModel = new List<DataLoggingModel>();
                foreach (var item in list)
                {
                    var model = new DataLoggingModel(item);
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
    }
}
