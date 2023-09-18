using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
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

        [HttpGet]
        [Route("sendnotification")]
        public IHttpActionResult Send()
        {
            ResponseFileResult result = new ResponseFileResult();
            ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
            IEmailService service = IoC.Resolve<IEmailService>();
            IEmailService service1 = IoC.Resolve<IEmailService>();
            IZaloService zaloservice = IoC.Resolve<IZaloService>();
            IZaloService zaloservice1 = IoC.Resolve<IZaloService>();
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
                    IList<UserNhanCanhBao> listNguoiNhanx3 = userNhanCanhBaoService.GetbyMaDviQly("X0206");
                    IList<UserNhanCanhBao> listNguoiNhanb09 = userNhanCanhBaoService.GetbyMaDviQly("PD");
                    IList<Userdata> listNguoiNhanzalo = userdataService.GetbyMaDviQly(item.DONVI_DIENLUC);
                    IList<Userdata> listNguoiNhanx3zalo = userdataService.GetbyMaDviQly("X0206");
                    IList<Userdata> listNguoiNhanB09zalo = userdataService.GetbyMaDviQly("PD");
                    //Email
                    foreach (var nguoiNhan in listNguoiNhan)
                    {
                      
                        var existEmail = GetExits(listItemExistEmail, item.NOIDUNG);
                        var point = GetPoint(existEmail);
                        var user = userdataService.Getbykey(nguoiNhan.USER_ID);
                        Email email = new Email();
                        email.MA_DVIQLY = item.DONVI_DIENLUC;
                        email.MA_DVU = "TA";
                        email.NOI_DUNG = item.NOIDUNG + point;
                        email.NGAY_TAO = DateTime.Now;
                        email.NGUOI_TAO = "admin";
                        email.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                        email.TINH_TRANG = 1;
                        email.EMAIL = user.email;
                        service.CreateNew(email);
                    }
              

                //EMAIL_X3
                    foreach (var nguoiNhan1 in listNguoiNhanx3)
                    {
                        var existEmailX3 = GetExits(listItemExistEmail, item.NOIDUNG);
                        var pointX3 = GetPoint(existEmailX3);
                        var user1 = userdataService.Getbykey(nguoiNhan1.USER_ID);
                        Email email1 = new Email();
                        email1.MA_DVIQLY = "X03";
                        email1.MA_DVU = "TA";
                        email1.NOI_DUNG = item.NOIDUNG + pointX3;
                        email1.NGAY_TAO = DateTime.Now;
                        email1.NGUOI_TAO = "admin";
                        email1.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                        email1.TINH_TRANG = 1;
                       
                        email1.EMAIL = user1.email;
                        service1.CreateNew(email1);

                    }

                    //EMAIL_B09
                    foreach (var nguoiNhanB09 in listNguoiNhanb09)
                    {
                        var existEmailB09 = GetExits(listItemExistEmail, item.NOIDUNG);
                        var pointB09 = GetPoint(existEmailB09);
                        var userB09 = userdataService.Getbykey(nguoiNhanB09.USER_ID);
                        Email emailB09 = new Email();
                        emailB09.MA_DVIQLY = "B09";
                        emailB09.MA_DVU = "TA";
                        emailB09.NOI_DUNG = item.NOIDUNG + pointB09;
                        emailB09.NGAY_TAO = DateTime.Now;
                        emailB09.NGUOI_TAO = "admin";
                        emailB09.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                        emailB09.TINH_TRANG = 1;

                        emailB09.EMAIL = userB09.email;
                        service1.CreateNew(emailB09);

                    }


                    //Zalo
                    foreach (var nguoiNhan2 in listNguoiNhanzalo)
                    {
                      //  var user = userdataService.Getbysdt(nguoiNhan.phoneNumber);
                        ZaloClient za = new ZaloClient();
                        var existZalo = GetExits(listItemExistZalo, item.NOIDUNG);
                        var pointZalo = GetPoint(existZalo);
                        var idzalo = za.get_idzalo(nguoiNhan2.phoneNumber); // Lay thong tin idzalo tu sdt
                        Zalo zalo = new Zalo(); ;
                        zalo.MA_DVIQLY = item.DONVI_DIENLUC;
                        zalo.MA_DVU = "TA";
                        zalo.NOI_DUNG = item.NOIDUNG + pointZalo;
                        zalo.NGAY_TAO = DateTime.Now;
                        zalo.NGUOI_TAO = "admin";
                        zalo.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                        zalo.TINH_TRANG = 1;
                        zalo.ID_ZALO = idzalo;
                            if (idzalo == "-1")
                            {

                            }
                            else
                            {
                                zaloservice.CreateNew(zalo);
                            }
                    }

                    //Zalo_X3
                    foreach (var nguoiNhan3 in listNguoiNhanx3zalo)
                    {
                        //  var user = userdataService.Getbysdt(nguoiNhan.phoneNumber);
                        ZaloClient za1 = new ZaloClient();
                        var existZaloX3 = GetExits(listItemExistZalo, item.NOIDUNG);
                        var pointZaloX3 = GetPoint(existZaloX3);
                        var idzalo1 = za1.get_idzalo(nguoiNhan3.phoneNumber); // Lay thong tin idzalo tu sdt
                        Zalo zalo1 = new Zalo(); ;
                        zalo1.MA_DVIQLY = "X03";
                        zalo1.MA_DVU = "TA";
                        zalo1.NOI_DUNG = item.NOIDUNG + pointZaloX3;
                        zalo1.NGAY_TAO = DateTime.Now;
                        zalo1.NGUOI_TAO = "admin";
                        zalo1.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                        zalo1.TINH_TRANG = 1;
                        zalo1.ID_ZALO = idzalo1;

                            if (idzalo1 == "-1")
                            {

                            }
                            else
                            {
                                zaloservice1.CreateNew(zalo1);
                            }
                    }


                    //Zalo_X3
                    foreach (var nguoiNhanzalob09 in listNguoiNhanB09zalo)
                    {
                        //  var user = userdataService.Getbysdt(nguoiNhan.phoneNumber);
                        ZaloClient za1 = new ZaloClient();
                        var existZalob09 = GetExits(listItemExistZalo, item.NOIDUNG);
                        var pointZalob09 = GetPoint(existZalob09);
                        var idzalo1 = za1.get_idzalo(nguoiNhanzalob09.phoneNumber); // Lay thong tin idzalo tu sdt
                        Zalo zaloB09 = new Zalo(); ;
                        zaloB09.MA_DVIQLY = "B09";
                        zaloB09.MA_DVU = "TA";
                        zaloB09.NOI_DUNG = item.NOIDUNG + pointZalob09;
                        zaloB09.NGAY_TAO = DateTime.Now;
                        zaloB09.NGUOI_TAO = "admin";
                        zaloB09.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                        zaloB09.TINH_TRANG = 1;
                        zaloB09.ID_ZALO = idzalo1;

                        if (idzalo1 == "-1")
                        {

                        }
                        else
                        {
                            zaloservice1.CreateNew(zaloB09);
                        }
                    }
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
                var list = service.GetbyMaDviQly(filter.Filter.maDViQLy);
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

        private int GetExits(List<EmailZaloCountModel> list, string noi_dung)
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
        private string GetPoint(int so_lan)
        {
            string result = "";
            for (int i = 0; i < so_lan; i++)
            {
                result += " ";
            }
            return result;
        }
    }
    
}
