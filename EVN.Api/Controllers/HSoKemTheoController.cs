using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.PMIS;
using EVN.Core.Repository;
using EVN.Core.Utilities;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/hosokemtheo")]
    public class HSoKemTheoController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(HSoKemTheoController));

        [JwtAuthentication]
        [HttpGet]
        [Route("getListHSKT/{maDVQL}/{maYC}/{loai}")]
        public IHttpActionResult getListHSKT(string maDVQL,string maYC,int loai)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IHSKemTheoService service = IoC.Resolve<IHSKemTheoService>();
                var list = service.GetbyFilter(maDVQL, maYC, loai);         
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
        [HttpPost]
        [Route("GetByFilter")]
        public IHttpActionResult GetByFilter(HSoKemTheoFilter request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IHSKemTheoService service = IoC.Resolve<IHSKemTheoService>();
                var list = service.GetbyFilter(request.maDViQly, request.maYCau, request.loai);
                var listModel = new List<HoSoKemTheoModel>();
                foreach(var item in list)
                {
                    var model=new HoSoKemTheoModel(item);
                    model.Base64 = GetBase64(item.Data);
                    listModel.Add(model);
                }
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

        private string GetBase64(string path)
        {
            string fullPath = $"{ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"]}/{path}";

            //Check whether File exists.
            if (!System.IO.File.Exists(fullPath))
            {
                return "";
            }

            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int length = Convert.ToInt32(fs.Length);
                byte[] buff = new byte[length];
                fs.Read(buff, 0, length);
                string base64 = Convert.ToBase64String(buff);
                return base64;
            }
        }

        [JwtAuthentication]
        [HttpGet]
        [Route("GetByID/{id}")]
        public IHttpActionResult GetByID(int id)
        {
            try
            {
                IHSKemTheoService service = IoC.Resolve<IHSKemTheoService>();
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
        [Route("CreateHSKT")]
        public IHttpActionResult CreateHSKT()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string data = null;
                data = httpRequest.Form["data"];
                IHSKemTheoService hsktservice = IoC.Resolve<IHSKemTheoService>();
                
                var item = new HSKemTheo();
                HSKemTheo model = JsonConvert.DeserializeObject<HSKemTheo>(data);
                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                var cvyc = congVanYeuCauService.GetbyMaYCau(model.MaYeuCau);
                //Upload Image
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string fileFolder = $"//HSTK//";
                    string fileName = $"{model.MaYeuCau}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(postedFile.FileName)}";
                    string imagePath = FileUtils.SaveFilePDFAsync(postedFile, fileFolder, fileName);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return BadRequest();
                    }
                    item.Data = $"/{fileFolder}/{fileName}";                    
                }
                item.MaHoSo = model.MaHoSo;
                item.LoaiHoSo = model.LoaiHoSo;
                item.TenHoSo = model.TenHoSo; 
                item.TrangThai = model.TrangThai;
                item.MaDViQLy = model.MaDViQLy;
                item.MaYeuCau = model.MaYeuCau;
                item.Type = model.Type;
        
                hsktservice.CreateNew(item);
                hsktservice.CommitChanges();
                return Ok(item);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("UpdateHSKT")]
        public IHttpActionResult UpdateHSKT()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string data = null;
                data = httpRequest.Form["data"];
                IHSKemTheoService hdsktservice = IoC.Resolve<IHSKemTheoService>();
            
                HSKemTheo model = JsonConvert.DeserializeObject<HSKemTheo>(data);
                var item = hdsktservice.Getbykey(model.ID);

                item.LoaiHoSo = model.LoaiHoSo;
                item.TenHoSo = model.TenHoSo; ;
                item.TrangThai = model.TrangThai;

                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                var cvyc = congVanYeuCauService.GetbyMaYCau(item.MaYeuCau);

                //Upload Image
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string fileFolder = $"//HSTK//";
                    string fileName = $"{model.MaYeuCau}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(postedFile.FileName)}";
                    string imagePath = FileUtils.SaveFilePDFAsync(postedFile, fileFolder, fileName);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return BadRequest();
                    }
                    item.Data = $"/{fileFolder}/{fileName}";                    
                }
                hdsktservice.Update(item);
                hdsktservice.CommitChanges();
                return Ok(item);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpGet]
        [Route("delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {

                IHSKemTheoService service = IoC.Resolve<IHSKemTheoService>();
                service.Delete(id);
                service.CommitChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("UpLoadFileHSKT")]
        public IHttpActionResult UpLoadFileHSKT()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string data = httpRequest.Form["data"];
                IHSKemTheoService hdsktservice = IoC.Resolve<IHSKemTheoService>();
        
                HSKemTheo model = JsonConvert.DeserializeObject<HSKemTheo>(data);
                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                var cvyc = congVanYeuCauService.GetbyMaYCau(model.MaYeuCau);

                var item = new HSKemTheo();
                if (model.ID == 0)
                {
                    item.MaHoSo = model.MaHoSo;
                    item.LoaiHoSo = model.LoaiHoSo;
                    item.TenHoSo = model.TenHoSo; ;
                    item.TrangThai = model.TrangThai;
                    item.MaDViQLy = model.MaDViQLy;
                    item.MaYeuCau = model.MaYeuCau;
                    item.Type = model.Type;
                }
                else
                {
                    item = hdsktservice.Getbykey(model.ID);
                    item.LoaiHoSo = model.LoaiHoSo;
                    item.TenHoSo = model.TenHoSo;
                    item.TrangThai = model.TrangThai;

                }
                //Upload Image
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string fileFolder = $"//HSTK//";
                    string fileName = $"{model.MaYeuCau}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(postedFile.FileName)}";
                    string imagePath = FileUtils.SaveFile(postedFile, fileFolder, fileName);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return BadRequest();
                    }
                    item.Data = $"/{fileFolder}/{fileName}";                    
                }
                hdsktservice.Save(item);
                hdsktservice.CommitChanges();
                return Ok(item);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }
    }
}
