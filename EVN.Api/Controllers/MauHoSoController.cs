using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/mauhoso")]
    public class MauHoSoController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(MauHoSoController));

        [JwtAuthentication(Roles = "Admin")]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IMauHoSoService service = IoC.Resolve<IMauHoSoService>();
                var list = service.Query.ToList();
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

        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                IMauHoSoService service = IoC.Resolve<IMauHoSoService>();
                var item = service.Getbykey(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post(MauHoSo model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IMauHoSoService service = IoC.Resolve<IMauHoSoService>();
                var item = service.Getbykey(model.Code);
                if (item != null)
                {
                    result.success = false;
                    result.message = $"Mã hồ sơ {model.Code} đã có trên hệ thống.";
                    return Ok(result);
                }
                service.CreateNew(model);
                service.CommitChanges();
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("update")]
        public IHttpActionResult Update()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                IMauHoSoService service = IoC.Resolve<IMauHoSoService>();
                string data = null;
                data = httpRequest.Form["data"];

                MauHoSo model = JsonConvert.DeserializeObject<MauHoSo>(data);
                var item = service.Getbykey(model.Code);
                if (item == null)
                {
                    item = new MauHoSo();
                    item.Code = model.Code;
                }
                item.Name = model.Name;
                //Upload Image
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    IRepository repository = new FileStoreRepository();
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(postedFile.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                    }
                    string folder = $"MauHoSo/{model.Code}";
                    string loaiFile = Path.GetExtension(postedFile.FileName);
                    item.Data = repository.Store(folder, fileData, item.Data, loaiFile.Replace(".", ""));
                    log.Error(item.Data);
                }
                service.Save(item);
                service.CommitChanges();
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("xoamau/{id}")]
        public IHttpActionResult XoaMau(string id)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IMauHoSoService service = IoC.Resolve<IMauHoSoService>();
                var item = service.Getbykey(id);
                service.Delete(item);
                service.CommitChanges();
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("updatestatus/{id}")]
        public IHttpActionResult UpdateStatus(string id)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IMauHoSoService service = IoC.Resolve<IMauHoSoService>();
                var item = service.Getbykey(id);
                if (item.TrangThai == 0)
                    item.TrangThai = 1;
                else
                    item.TrangThai = 0;
                service.Save(item);
                service.CommitChanges();
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                return Ok(result);
            }
        }
    }
}
