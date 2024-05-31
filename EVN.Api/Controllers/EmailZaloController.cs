using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/EmailZalo")]
    public class EmailZaloController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(DonViController));
        [HttpPost]
        [Route("email/add")]
        public IHttpActionResult AddEmail([FromBody] EmailModelFilter request)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IEmailService service = IoC.Resolve<IEmailService>();
                Email email = new Email();
                email.ID = request.Filter.ID;
                email.MA_DVIQLY = request.Filter.MA_DVIQLY;
                email.MA_DVU = request.Filter.MA_DVU;
                email.MA_KHANG = request.Filter.MA_KHANG;
                email.NOI_DUNG = request.Filter.NOI_DUNG;
                email.NGAY_SUA = request.Filter.NGAY_SUA;
                email.NGUOI_SUA = request.Filter.NGUOI_SUA;
                email.NGAY_TAO = request.Filter.NGAY_TAO;
                email.NGUOI_TAO = request.Filter.NGUOI_TAO;
                email.TIEU_DE = request.Filter.TIEU_DE;
                email.TINH_TRANG = request.Filter.TINH_TRANG;
                email.ID_HDON = request.Filter.ID_HDON;
                email.EMAIL = request.Filter.EMAIL;
                service.CreateNew(email);
                service.CommitChanges();
                result.success = true;
                result.message = "Thêm mới thành công";
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

        //user_nhan_canhbao
        [HttpPost]
        [Route("user_nhan_canhbao/add")]
        public IHttpActionResult usernhancanhbao([FromBody] UserNhanCanhBaoid request)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IUserNhanCanhBaoService userNhanCanhBaoService = IoC.Resolve<IUserNhanCanhBaoService>();
                UserNhanCanhBao user = new UserNhanCanhBao();
                user.MA_DVIQLY = request.MA_DVIQLY;
                user.USER_ID = request.USER_ID;
                userNhanCanhBaoService.CreateNew(user);
                userNhanCanhBaoService.CommitChanges();
                result.success = true;
                result.message = "Thêm mới thành công";
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
        [Route("edit")]
        public IHttpActionResult usernhancanhbaoedit()
        {
            ResponseFileResult result = new ResponseFileResult();
            var httpRequest = HttpContext.Current.Request;
            try
            {
                string data = httpRequest.Form["data"];
                UserNhanCanhBaoid model = JsonConvert.DeserializeObject<UserNhanCanhBaoid>(data);
                IUserNhanCanhBaoService userNhanCanhBaoService = IoC.Resolve<IUserNhanCanhBaoService>();
                var item = new UserNhanCanhBao();
                item = userNhanCanhBaoService.GetbyNo(model.ID);
                item.MA_DVIQLY = model.MA_DVIQLY;
                item.USER_ID = model.USER_ID;
                userNhanCanhBaoService.Update(item);
                userNhanCanhBaoService.CommitChanges();
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


        [HttpGet]
        [Route("sendnotification")]
        public async Task<IHttpActionResult> Send()
        {
            ResponseFileResult result = new ResponseFileResult();
            ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
            IEmailService service = IoC.Resolve<IEmailService>();
            IZaloService zaloservice = IoC.Resolve<IZaloService>();
            IUserNhanCanhBaoService userNhanCanhBaoService = IoC.Resolve<IUserNhanCanhBaoService>();
            IUserdataService userdataService = IoC.Resolve<IUserdataService>();
            try
            {
                

                var listCB = CBservice.Query.Where(p => p.TRANGTHAI_CANHBAO == 1).ToList();
                var listItemExistEmail = new List<EmailZaloCountModel>();
                var listItemExistZalo = new List<EmailZaloCountModel>();
                foreach (var item in listCB) 
                {

                    IList<UserNhanCanhBao> listNguoiNhan = userNhanCanhBaoService.GetbyMaDviQly(item.DONVI_DIENLUC);
                    IList<Userdata> listNguoiNhanzalo = userdataService.GetbyMaDviQly(item.DONVI_DIENLUC);


                    //Email
                    foreach (var nguoiNhan in listNguoiNhan)
                    { 
                      if (item.MA_YC == null)
                        {
                            continue;
                        }
                      else 
                        { 
                        var existEmail = await GetExits(listItemExistEmail, item.NOIDUNG);// check nội udng trùng r
                        var point = await GetPoint(existEmail);
                        var user = userdataService.Getbykey(nguoiNhan.USER_ID);
                        Email email = new Email();
                        email.MA_DVIQLY = nguoiNhan.MA_DVIQLY;
                        email.MA_DVU = "TA";
                        email.NOI_DUNG = item.NOIDUNG + point;
                        email.NGAY_TAO = DateTime.Now;
                        email.NGUOI_TAO = "admin";
                        email.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                        email.TINH_TRANG = 1;
                        email.EMAIL = user.email;
                            if (email.EMAIL == null)
                            {

                            }
                            else
                            {
                                service.CreateNew(email);
                            }
                            
                        }
                    }
              
                    ////Zalo
                    //foreach (var nguoiNhan2 in listNguoiNhanzalo)
                    //{
                    //    if (item.MA_YC == null)
                    //    {
                    //        continue;
                    //    }
                    //    else
                    //    {
                    //        //  var user = userdataService.Getbysdt(nguoiNhan.phoneNumber);
                    //        ZaloClient za = new ZaloClient();
                    //        var existZalo = await GetExits(listItemExistZalo, item.NOIDUNG);
                    //        var pointZalo = await GetPoint(existZalo);
                    //        var idzalo = za.get_idzalo(nguoiNhan2.phoneNumber); // Lay thong tin idzalo tu sdt
                    //        Zalo zalo = new Zalo(); ;
                    //        zalo.MA_DVIQLY = nguoiNhan2.maDViQLy;
                    //        zalo.MA_DVU = "TA";
                    //        zalo.NOI_DUNG = item.NOIDUNG + pointZalo;
                    //        zalo.NGAY_TAO = DateTime.Now;
                    //        zalo.NGUOI_TAO = "admin";
                    //        zalo.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                    //        zalo.TINH_TRANG = 1;
                    //        zalo.ID_ZALO = idzalo;
                    //        if (idzalo == "-1")
                    //        {

                    //        }
                    //        else
                    //        {
                    //            zaloservice.CreateNew(zalo);
                    //        }
                    //    }
                    //}

               
                    item.TRANGTHAI_CANHBAO = 2;
                    CBservice.Update(item);
                }

                
                service.CommitChanges();
                result.success = true;
                result.message = "Thêm mới thành công";
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
        [Route("filter")]
        public IHttpActionResult Filter(UserNhanCanhBaoFilterRequest filter)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                // int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                // int total = 0;
                DateTime synctime = DateTime.Today;
                IUserNhanCanhBaoService service = IoC.Resolve<IUserNhanCanhBaoService>();
                var list = service.Getid(filter.Filter.maDViQLy);

                IList<UserNhanCanhBao> data = new List<UserNhanCanhBao>();

                
                // result.total = list.Count();
                result.data = list;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new List<UserNhanCanhBao>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        [HttpPost]
        [Route("filternguoinhan")]
        public IHttpActionResult Filternguoinhan(UserNhanCanhBaoFilterRequest filter)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                // int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                // int total = 0;
                DateTime synctime = DateTime.Today;
                IUserNhanCanhBaoService service = IoC.Resolve<IUserNhanCanhBaoService>();
                IUserdataService serviceuser = IoC.Resolve<IUserdataService>();

                var listModel = new List<UserDataNHANdata>();
                var ht = serviceuser.GetMadvi(filter.Filter.maDViQLy);


                foreach (var item in ht)
                {

        
                    var model = new UserDataNHANdata(item);
                    var usernhan = service.GetMaDviQly(item.maDViQLy);
                    var userdata = serviceuser.GetMaDviQly(item.maDViQLy);
                    if (usernhan != null && userdata != null && usernhan.USER_ID == userdata.userId)
                    {
                
                        listModel.Add(model);
                    }
                    else
                    {
                        continue;
                    }

                }
                // result.total = list.Count();
                result.data = listModel;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new List<UserNhanCanhBao>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
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
                IUserNhanCanhBaoService service = IoC.Resolve<IUserNhanCanhBaoService>();
                var item = new UserNhanCanhBao();
                item = service.GetbyNo(ID);
                UserNhanCanhBaoid obj = new UserNhanCanhBaoid(item);
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



        //[JwtAuthentication]
        [HttpGet]
        [Route("delete/{ID}")]
        public IHttpActionResult Delete([FromUri] int ID)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IUserNhanCanhBaoService service = IoC.Resolve<IUserNhanCanhBaoService>();
                var item = new UserNhanCanhBao();
                item = service.GetbyNo(ID);
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

        private async Task<int> GetExits(List<EmailZaloCountModel> list, string noi_dung)
        {
            var exists = list.FirstOrDefault(x => x.NOI_DUNG == noi_dung);
            if(exists == null)
            {
                list.Add(new EmailZaloCountModel()
                {
                    NOI_DUNG = noi_dung,
                    SO_LAN = 0
                });
                return 0;
            }
            else
            {
                var count = exists.SO_LAN;
                list.Remove(exists);
                list.Add(new EmailZaloCountModel()
                {
                    SO_LAN = count + 1,
                    NOI_DUNG = noi_dung
                });
                return count + 1;
            }
        }
        private async Task<string> GetPoint(int so_lan)
        {
            string result = "";
            for (int i = 0; i < so_lan; i++)
            {
                result += " ";
            }
            return result;
        }

        //[JwtAuthentication]
        [HttpGet]
        [Route("download/apk")]
        public IHttpActionResult DownloadApk()
            {
                try
                {

                var filePath = AppDomain.CurrentDomain.BaseDirectory + @"Templates\app-release.apk";
                var fileName = "app-release.apk";
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);

                    var content = new ByteArrayContent(fileBytes);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.android.package-archive");

                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = content
                    };

                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };

                    return ResponseMessage(response);
                }
                catch (Exception ex)
                {
                    // Log or handle the exception appropriately
                    return InternalServerError(ex);
                }
            }
        


    }

}
