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
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/canhbao")]
    public class CanhBaoController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(CanhBaoController));

        [HttpGet]
        [Route("createCanhBao")]
        public async Task<IHttpActionResult> GetListCanhBao()
        {
            ResponseResult result = new ResponseResult();
            try
            {

                IReportService service = IoC.Resolve<IReportService>();
                ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
                ILogCanhBaoService LogCBservice = IoC.Resolve<ILogCanhBaoService>();
                var list = service.TinhThoiGian();
                var listCanhBao = new List<CanhBao>();
                foreach (var item in list)
                {
                    var canhbao = new CanhBao();
                    canhbao.LOAI_CANHBAO_ID = item.LoaiCanhBao;
                    canhbao.LOAI_SOLANGUI = 1;
                    canhbao.MA_YC = item.MaYeuCau;
                    canhbao.THOIGIANGUI = DateTime.Now;
                    canhbao.TRANGTHAI_CANHBAO = 1;
                    canhbao.DONVI_DIENLUC = item.MaDViQLy;
                    switch (item.LoaiCanhBao)
                    {
                        case 1:
                            canhbao.NOIDUNG = "Cảnh báo lần…("+ canhbao.LOAI_SOLANGUI + ") <br>KH " + item.TenKhachHang+", SĐT "+ item.DienThoai+", ĐC: "+ item.DiaChiDungDien +", MaYC "+ canhbao.MA_YC +", ngày tiếp nhận: "+ item.NgayLap +"Đơn vị: "+ item.MaDViQLy+ " Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu cấp điện, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 2:
                            canhbao.NOIDUNG = "Cảnh báo lần…(" + canhbao.LOAI_SOLANGUI + ") <br>KH " + item.TenKhachHang + ", SĐT " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + "Đơn vị: " + item.MaDViQLy + " Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu cấp điện đơn vị chưa thực hiện lập thỏa thuận đấu nối trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 3:
                            canhbao.NOIDUNG = "Cảnh báo lần…(" + canhbao.LOAI_SOLANGUI + ") <br>KH " + item.TenKhachHang + ", SĐT " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + "Đơn vị: " + item.MaDViQLy + " Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu nghiệm thu đóng điện, đơn vị chưa thực hiện tiếp nhận yêu cầu nghiệm thu trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 4:
                            canhbao.NOIDUNG = "Cảnh báo lần…(" + canhbao.LOAI_SOLANGUI + ") <br>KH " + item.TenKhachHang + ", SĐT " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + "Đơn vị: " + item.MaDViQLy + " Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu dự thảo và ký hợp đồng mua bán điện, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 5:
                            canhbao.NOIDUNG = "Cảnh báo lần…(" + canhbao.LOAI_SOLANGUI + ") <br>KH " + item.TenKhachHang + ", SĐT " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + "Đơn vị: " + item.MaDViQLy + " Đã quá 02 ngày kể từ khi tiếp nhận yêu kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 6:
                            canhbao.NOIDUNG = "Cảnh báo lần…(" + canhbao.LOAI_SOLANGUI + ") <br>KH " + item.TenKhachHang + ", SĐT " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + "Đơn vị: " + item.MaDViQLy + " Đã quá 04 ngày kể từ khi tiếp nhận yêu cầu cấp điện mới trung áp, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 7:
                            canhbao.NOIDUNG = "Cảnh báo lần…(" + canhbao.LOAI_SOLANGUI + ") <br>KH " + item.TenKhachHang + ", SĐT " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + "Đơn vị: " + item.MaDViQLy + " Đã gặp trở ngại trong quá trình tiếp nhận yêu cầu của khách, đơn vị hãy xử lý yêu cầu cấp điện/thỏa thuận đấu nối trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 8:
                            canhbao.NOIDUNG = "Cảnh báo lần…(" + canhbao.LOAI_SOLANGUI + ") <br>KH " + item.TenKhachHang + ", SĐT" + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + "Đơn vị: " + item.MaDViQLy + " Đã gặp trở ngại trong quá trình khảo sát khách hàng, đơn vị hãy xử lý yêu cầu lập thỏa thuận đấu nối trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                    }

                    listCanhBao.Add(canhbao);

                }
                string message = "";
             

                if (listCanhBao.Any())
                {
                    // group data theo MA_YC và TRANGTHAI_CANHBAO
                    var canhBaoGroup = listCanhBao.GroupBy(x => new { x.MA_YC})
                        .Select(x => new { key = x.Key, value = x.Select(d => d) })
                        .ToList();
                    // Lấy các bản ghi duy nhất theo MA_YC và TRANGTHAI_CANHBAO
                    var danhSachCanhBaoMoi = canhBaoGroup.Where(x => x.value.Count() == 1).SelectMany(x => x.value).ToList();
                    // lặp danh sách để thêm mới 
                    foreach (var newCanhBao in danhSachCanhBaoMoi)
                    {
                        var checkTonTai = await CBservice.CheckExits(newCanhBao.MA_YC);
                        // nếu ko tồn tại thì insert
 
                        if (!checkTonTai)
                        {
                            //CBservice.CreateNew(newCanhBao);
                            CBservice.CreateCanhBao(newCanhBao, out message);
                            if (string.IsNullOrEmpty(message))
                            {
                                LogCanhBao logCB = new LogCanhBao();
                                // cần ins cả vào đây
                                logCB.CANHBAO_ID = newCanhBao.ID;
                                logCB.DATA_MOI = JsonConvert.SerializeObject(newCanhBao);
                                logCB.NGUOITHUCHIEN = "HeThong";
                                logCB.THOIGIAN = DateTime.Now;
                                logCB.TRANGTHAI = 1;
                                LogCBservice.CreateNew(logCB);
                                LogCBservice.CommitChanges();
                            }
                            else
                            {
                                throw new Exception(message);
                            }
                        }
                        else
                            //nếu tồn tại thì bỏ qua tiếp tục
                            // có tồn tại rồi thì nhảy vào case này

                            continue;
                    }
                    service.CommitChanges();
                }
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;

                result.message = ex.Message;

                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";

                return Ok(result);
            }
        }





       

        //2.1	(GET) /canhbao/filter
        //[JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(CanhBaoFilterRequest filter)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                // int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                // int total = 0;
                DateTime synctime = DateTime.Today;
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                var list = service.Filter(filter.Filter.fromdate, filter.Filter.todate, filter.Filter.maLoaiCanhBao, filter.Filter.trangThai, filter.Filter.maDViQLy, filter.Filter.SoLanGui, filter.Filter.keyword);
                IList<CanhBaoRequest> data = new List<CanhBaoRequest>();

                foreach (var item in list)
                {
                    data.Add(new CanhBaoRequest(item));
                }
                // result.total = list.Count();
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new List<CanhBaoRequest>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }
        //2.2	(POST) /canhbao/finnish
        //[JwtAuthentication]
        [HttpPost]
        [Route("finnish")]
        public IHttpActionResult GetById(int Id)

        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                var item = new CanhBao();
                // item = service.Getbykey(Id);
                //   result.data = item;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new CanhBaoRequest();
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        //2.3	(GET) /canhbao/{id}
        //[JwtAuthentication]
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetBycanhbaoId([FromUri] int id)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IGiamSatCanhBaoCanhbaoidService servicecanhbao = IoC.Resolve<IGiamSatCanhBaoCanhbaoidService>();
                IGiamSatPhanhoiCanhbaoidService servicephanhoi = IoC.Resolve<IGiamSatPhanhoiCanhbaoidService>();
                IGiamSatCongVanCanhbaoidService serviceyeucau = IoC.Resolve<IGiamSatCongVanCanhbaoidService>();
                ILogCanhBaoService LogCanhBaoservice = IoC.Resolve<ILogCanhBaoService>();
                var ThongTinCanhBao = servicecanhbao.Getbyid(id);
                // mới chỉ lấy dc trạng thái của TTDN, chưa lấy dc của nghiệm thu
                var ThongTinYeuCau = serviceyeucau.GetbyMaYCau(ThongTinCanhBao.MA_YC);
                var DanhSachPhanHoi = servicephanhoi.Getbyid(id);
                var DanhSachTuongTac = LogCanhBaoservice.Filter(id);
                var oj = new { ThongTinCanhBao, ThongTinYeuCau, DanhSachPhanHoi, DanhSachTuongTac };
                result.data = oj;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new CanhBaoRequest();
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        //2.9	(POST) /canhbao/{id}
        //[JwtAuthentication]
        [HttpPost]
        [Route("{id}")]
        public IHttpActionResult PostCanhbao(GiamsatcapdienCanhBaoid model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IGiamsatcapdienCanhBaoidService service = IoC.Resolve<IGiamsatcapdienCanhBaoidService>();

                var item = new GiamsatcapdienCanhBaoid();
                item.ID = model.ID;
                item.TRANGTHAI_CANHBAO = 1;
                item.NOIDUNG = model.NOIDUNG;

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
                result.success = false;
                return Ok(result);
            }
        }

        //2.4	(POST) / canhbao/phanhoi/add
        //[JwtAuthentication]
        [HttpPost]
        [Route("phanhoi/add")]
        public IHttpActionResult Post(PhanhoiTraodoiRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();

                var item = new PhanhoiTraodoi();
                item.CANHBAO_ID = model.CANHBAO_ID;
                item.NOIDUNG_PHANHOI = model.NOIDUNG_PHANHOI;
                item.NGUOI_GUI = model.NGUOI_GUI;
                item.DONVI_QLY = model.DONVI_QLY;
                item.THOIGIAN_GUI = model.THOIGIAN_GUI;
                item.TRANGTHAI_XOA = model.TRANGTHAI_XOA;

                item.FILE_DINHKEM = model.FILE_DINHKEM;
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

        //2.5	(POST) / canhbao/phanhoi/edit
        //[JwtAuthentication]
        [HttpPost]
        [Route("phanhoi/edit")]
        public IHttpActionResult UpdateById(giamSatCapDien model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IgiamSatCapDienService service = IoC.Resolve<IgiamSatCapDienService>();
                var item = new giamSatCapDien();
                item.ID = model.ID;
                item.NOIDUNG_PHANHOI = model.NOIDUNG_PHANHOI;
                item.PHANHOI_TRAODOI_ID = model.PHANHOI_TRAODOI_ID;
                item.NGUOI_GUI = model.NGUOI_GUI;
                item.DONVI_QLY = model.DONVI_QLY;
                item.PHANHOI_TRAODOI_ID = 1;
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

        [HttpGet]
        [Route("updateStatus/{ID}/{Status}")]
        public IHttpActionResult updateStatus([FromUri] int ID, [FromUri] int Status, [FromUri] int NGUYENHHAN_CANHBAO)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                ILogCanhBaoService LogCBservice = IoC.Resolve<ILogCanhBaoService>();
                var item = new CanhBao();
                item = service.Getbyid(ID);
                item.TRANGTHAI_CANHBAO = Status;
                service.Update(item);
                service.CommitChanges();

                item.NGUYENHHAN_CANHBAO = NGUYENHHAN_CANHBAO;
                service.CreateNew(item);
                service.CommitChanges();

                LogCanhBao logCB = new LogCanhBao();
                // cần ins cả vào đây
                logCB.CANHBAO_ID = ID;
                logCB.DATA_MOI = JsonConvert.SerializeObject(item);
                logCB.NGUOITHUCHIEN = HttpContext.Current.User.Identity.Name;
                logCB.THOIGIAN = DateTime.Now;
                logCB.TRANGTHAI = 1;
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
                return Ok(result);
            }
        }

        



    }
}
