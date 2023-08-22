using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using EVN.Core.Utilities;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        public IHttpActionResult Post()
        {
            ResponseFileResult result = new ResponseFileResult();
            var httpRequest = HttpContext.Current.Request;
            string data = httpRequest.Form["data"];
            IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
            PhanhoiTraodoiRequest model = JsonConvert.DeserializeObject<PhanhoiTraodoiRequest>(data);
            ILogCanhBaoService LogCBservice = IoC.Resolve<ILogCanhBaoService>();
            try
            {
                var item = new PhanhoiTraodoi();
                item.CANHBAO_ID = model.CANHBAO_ID;
                item.NOIDUNG_PHANHOI = model.NOIDUNG_PHANHOI;
                item.NGUOI_GUI = model.NGUOI_GUI;
                item.DONVI_QLY = model.DONVI_QLY;
                item.THOIGIAN_GUI = DateTime.Now;
                item.TRANGTHAI_XOA = 0;
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string fileFolder = $"//GSCD//";
                    string fileName = $"{model.CANHBAO_ID}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(postedFile.FileName)}";
                    string imagePath = FileUtils.SaveFile(postedFile, fileFolder, fileName);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return BadRequest();
                    }
                    item.FILE_DINHKEM = $"/{fileFolder}/{fileName}";
                }
                service.CreateNew(item);
                service.CommitChanges();
                LogCanhBao logCB = new LogCanhBao();
                logCB.CANHBAO_ID = item.CANHBAO_ID;
                logCB.DATA_MOI = JsonConvert.SerializeObject(item);
                logCB.NGUOITHUCHIEN = HttpContext.Current.User.Identity.Name;
                logCB.THOIGIAN = DateTime.Now;
                logCB.TRANGTHAI = 4;
                LogCBservice.CreateNew(logCB);
                LogCBservice.CommitChanges();


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
                item = service.GetbyPhanHoiId(Id);
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
        [Route("edit")]
        public IHttpActionResult UpdateById()
        {
            ResponseFileResult result = new ResponseFileResult();
            var httpRequest = HttpContext.Current.Request;
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
                string data = httpRequest.Form["data"];
                PhanhoiTraodoiRequest model = JsonConvert.DeserializeObject<PhanhoiTraodoiRequest>(data);
                var item = new PhanhoiTraodoi();
                item = service.GetbyPhanHoiId(model.ID);
                item.NOIDUNG_PHANHOI = model.NOIDUNG_PHANHOI;
                item.THOIGIAN_GUI = DateTime.Now;
                item.TRANGTHAI_XOA = 0;
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string fileFolder = $"//GSCD//";
                    string fileName = $"{model.CANHBAO_ID}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(postedFile.FileName)}";
                    string imagePath = FileUtils.SaveFile(postedFile, fileFolder, fileName);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return BadRequest();
                    }
                    item.FILE_DINHKEM = $"/{fileFolder}/{fileName}";
                }
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
        [Route("delete/{ID}")]
        public IHttpActionResult Delete([FromUri] int ID)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
                var item = new PhanhoiTraodoi();
                item = service.GetbyPhanHoiId(ID);
                item.TRANGTHAI_XOA = 1;
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

        



    }
}