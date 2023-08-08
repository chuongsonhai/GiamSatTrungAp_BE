using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
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
    //[RoutePrefix("khaosat")]
    public class XacNhanTroNgaiController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(XacNhanTroNgaiController));

        //[JwtAuthentication]
        [HttpPost]
        [Route("khaosat/khachhang/filter")]
        public IHttpActionResult khachhangFilter(XacNhanTroNgaiFilterkhRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                DateTime synctime = DateTime.Today;
                //var fromDate = DateTime.MinValue.ToString();
                //var toDate = DateTime.MaxValue.ToString();
                //if (!string.IsNullOrWhiteSpace(request.FilterKH.tuNgay))
                //    fromDate = DateTime.ParseExact(request.FilterKH.tuNgay, DateTimeParse.Format, null, DateTimeStyles.None);
                //if (!string.IsNullOrWhiteSpace(request.FilterKH.denNgay))
                //    toDate = DateTime.ParseExact(request.FilterKH.denNgay, DateTimeParse.Format, null, DateTimeStyles.None);
                ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
                IGiamSatCongVanCanhbaoidService serviceyeucau = IoC.Resolve<IGiamSatCongVanCanhbaoidService>();
                List<object> resultList = new List<object>();
                var list = canhBaoService.GetbykhachhangFilter(request.FilterKH.tuNgay, request.FilterKH.denNgay, request.FilterKH.maLoaiCanhBao,
                    request.FilterKH.donViQuanLy, pageindex, request.Paginator.pageSize, out total);
                foreach (var canhbao in list)
                {
                    var listLog = serviceyeucau.Filterkhaosat(canhbao.MA_YC);
                    foreach (var kh in listLog)
                    {
                        resultList.Add(new { kh.MaYeuCau, kh.TenKhachHang, kh.TrangThai });
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
                result.data = new List<XacNhanTroNgaiRequest>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }
     

        //2.3 (GET) /khaosat/{id}
        //[JwtAuthentication]
        [HttpGet]
        [Route("khaosat/id")]
        //public IHttpActionResult GetById(XacNhanTroNgakhaosatid model)
        //{
            public IHttpActionResult GetBykhaosatId(int id)
            {
                ResponseResult result = new ResponseResult();
                try
                {
                IXacNhanTroNgaiService khaosatService = IoC.Resolve<IXacNhanTroNgaiService>();
                IGiamSatCongVanCanhbaoidService serviceyeucau = IoC.Resolve<IGiamSatCongVanCanhbaoidService>();
                IGiamSatCanhBaoCanhbaoidService servicecanhbao = IoC.Resolve<IGiamSatCanhBaoCanhbaoidService>();
                var khaosat = khaosatService.GetKhaoSat(id);
                var ThongTinCanhBao = servicecanhbao.Getbyid(khaosat.CANHBAO_ID);
                var ThongTinYeuCau = serviceyeucau.GetbyMaYCau(ThongTinCanhBao.MA_YC);
               // var oj = new { khaosat, ThongTinYeuCau };
                var oj1 = new
                {
                    khaosat = new
                    {
                        maYeuCau = ThongTinYeuCau.MaYeuCau,
                        ketQuaKhaoSat = khaosat.KETQUA,
                        trangThaiYeuCau = ThongTinYeuCau.TrangThai,
                        trangThaiKhaoSat = khaosat.TRANGTHAI,
                        tenKhachHang = ThongTinYeuCau.TenKhachHang,
                        nguoiKhaoSat = khaosat.NGUOI_KS,
                        thoiGianKhaoSat = khaosat.THOIGIAN_KHAOSAT
                    }
                };
                //var oj = new
                //{
                //    data1 = new
                //    {
                //        maLoaiCanhBao = ThongTinCanhBao.maLoaiCanhBao,
                //        noiDungCanhBao = ThongTinCanhBao.noidungCanhBao,
                //    },
                //    data3 = new
                //    {
                //        maLoaiCanhBao = ThongTinCanhBao.maLoaiCanhBao,
                //        noiDungCanhBao = ThongTinCanhBao.noidungCanhBao,
                //    }
                //};
                result.data = oj1;
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
       // }

        //2.5 (POST) /khaosat/add
        //[JwtAuthentication]
        [HttpPost]
        [Route("khaosat/add")]
        public IHttpActionResult Post([FromBody] XacNhanTroNgaikhaosatadd model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();

                var item = new XacNhanTroNgai();
        
                item.NOIDUNG_CAUHOI = model.noiDungKhaoSat;
                item.PHANHOI_KH = model.khachHangPhanHoi;
                item.KETQUA = model.ketQuaKhaoSat;
                item.NGUOI_KS = model.nguoiKhaoSat;  
                item.TRANGTHAI = model.trangThaiKhaoSat;
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

        //2.4 (POST) /khaosat/{id}
        //[JwtAuthentication]
        [Route("khaosat/id")]
        [HttpPost]
        public IHttpActionResult UpdateById([FromBody] XacNhanTroNgakhaosatid model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                var item = new XacNhanTroNgai();
               
                service.UpdateKhaoid(model.idKhaoSat);
                var khaosat = service.UpdateKhaoid(model.idKhaoSat);
                khaosat.ID = model.idKhaoSat;
                khaosat.NOIDUNG_CAUHOI = model.noiDungKhaoSat;
                khaosat.PHANHOI_KH = model.khachHangPhanHoi;
                khaosat.KETQUA = model.ketQuaKhaoSat;
                khaosat.NGUOI_KS = model.nguoiKhaoSat;
                khaosat.TRANGTHAI = model.trangThaiKhaoSat;
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


     

        //2.6 (POST) /khaosat/phanhoi/add
        //[JwtAuthentication]
        [HttpPost]
        [Route("khaosat/phanhoi/add")]
        public IHttpActionResult Post([FromBody] PhanhoiTraodoiRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();

                var item = new PhanhoiTraodoi();
            
                item.NOIDUNG_PHANHOI = model.NOIDUNG_PHANHOI;
                item.NGUOI_GUI = model.NGUOI_GUI;
                item.TRANGTHAI_XOA = 0;

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

        //2.7 (POST) /khaosat/phanhoi/{id}
        //[JwtAuthentication]
        [HttpPost]
        [Route("khaosat/phanhoi/id")]
        public IHttpActionResult UpdateById([FromBody] PhanhoiTraodoiRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
               // var item = new PhanhoiTraodoi();
               // service.Updatephanhoiid(model.ID);
                var phanhoi = service.Updatephanhoiid(model.ID);
                phanhoi.NOIDUNG_PHANHOI = model.NOIDUNG_PHANHOI;
                phanhoi.NGUOI_GUI = model.NGUOI_GUI;
                phanhoi.TRANGTHAI_XOA = 1;
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

        //2.8 (GET) /khaosat/phanhoi/id
        //[JwtAuthentication]
        [HttpPost]
        [Route("khaosat/phanhoi/id2.8")]
        public IHttpActionResult Filter(XacnhantrongaiFilterRequestid request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                DateTime synctime = DateTime.Today;
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
                var list = service.GetbyFilter(request.Filter.ID, pageindex, request.Paginator.pageSize, out total);
                var listModel = new List<PhanhoiTraodoiRequestid>();
                foreach (var item in list)
                {
                    var model = new PhanhoiTraodoiRequestid(item);
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

        //2.2 (GET) /khaosat/filter
        [HttpPost]
        [Route("khaosat/filter")]
        public IHttpActionResult Filter(FilterKhaoSatByCanhBaoRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                //var fromDate = DateTime.MinValue;
                //var toDate = DateTime.MaxValue;
                //if (!string.IsNullOrWhiteSpace(request.tuNgay))
                //    fromDate = DateTime.ParseExact(request.tuNgay, DateTimeParse.Format, null, DateTimeStyles.None);
                //if (!string.IsNullOrWhiteSpace(request.denNgay))
                //    toDate = DateTime.ParseExact(request.denNgay, DateTimeParse.Format, null, DateTimeStyles.None);
                IXacNhanTroNgaiService xacMinhTroNgaiService = IoC.Resolve<IXacNhanTroNgaiService>();
                ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                // lọc cảnh báo theo thời gian, mã đơn vị quản lý, trạng thái 
                var list = canhBaoService.FilterByMaYCauAndDViQuanLy(request.tuNgay, request.denNgay, request.MaYeuCau, request.donViQuanLy);

                //danh sách kết quả
                var resultList = new List<object>();


                foreach (var canhbao in list)
                {
                    //lọc ra các thông tin liên quan đến khảo sát
                    var listKhaoSat = xacMinhTroNgaiService.FilterByCanhBaoIDAndTrangThai(canhbao.ID, request.TrangThaiKhaoSat);
                    var listKhaoSatFilter = new List<object>();
                    foreach (var khaosat in listKhaoSat)
                    {
                        listKhaoSatFilter.Add(new { khaosat.ID, khaosat.TRANGTHAI, khaosat.THOIGIAN_KHAOSAT, khaosat.KETQUA });
                    }

                    //lọc ra tên khác hàng, trạng thái yêu cầu ứng với mã yêu cầu
                    var congVanYeuCau = congVanYeuCauService.GetbyMaYCau(canhbao.MA_YC);

                    //tạo ra response API
                    var obj = new { congVanYeuCau.MaYeuCau, congVanYeuCau.TenKhachHang, listKhaoSatFilter, congVanYeuCau.TrangThai };
                    resultList.Add(obj);
                }
                result.data = resultList;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new List<XacNhanTroNgai>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        //2.9 (GET) /khaosat/log/filter
        //[JwtAuthentication]
        [HttpPost]
        [Route("khaosat/log/filter")]
        public IHttpActionResult FilterLog([FromBody] FilterKhaoSatByCanhBaologRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                //var fromDate = DateTime.MinValue;
                //var toDate = DateTime.MaxValue;
                //if (!string.IsNullOrWhiteSpace(request.tuNgay))
                //    fromDate = DateTime.ParseExact(request.tuNgay, DateTimeParse.Format, null, DateTimeStyles.None);
                //if (!string.IsNullOrWhiteSpace(request.denNgay))
                //    toDate = DateTime.ParseExact(request.denNgay, DateTimeParse.Format, null, DateTimeStyles.None);
                IXacNhanTroNgaiService xacMinhTroNgaiService = IoC.Resolve<IXacNhanTroNgaiService>();
                ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
                ILogCanhBaoService logCanhBaoService = IoC.Resolve<ILogCanhBaoService>();
                var list = canhBaoService.FilterBytrangThaiAndDViQuanLy(request.tuNgay, request.denNgay, request.trangThai, request.donViQuanLy);
                var resultList = new List<object>();
                foreach (var canhbao in list)
                {
                    //lay ra danh sach khao sat ung voi moi canh bao va add vao list khao sat filter
                    var listKhaoSat = xacMinhTroNgaiService.FilterByCanhBaoID(canhbao.ID);
                    var listKhaoSatFilter = new List<object>();
                    foreach (var khaosat in listKhaoSat)
                    {
                        listKhaoSatFilter.Add(new { khaosat.ID, khaosat.NOIDUNG_CAUHOI });
                    }

                    //lay ra danh sach Log canh bao ung voi moi canh bao va add vao list Log canh bao filter
                    var listLog = logCanhBaoService.GetByMaCanhBao(canhbao.ID);
                    var listLogCanhBao = new List<object>();
                    foreach (var log in listLog)
                    {
                        listLogCanhBao.Add(new { log.DATA_CU, log.NGUOITHUCHIEN, log.THOIGIAN });
                    }

                    var obj = new { canhbao.DONVI_DIENLUC, canhbao.TRANGTHAI_CANHBAO, listKhaoSatFilter, listLogCanhBao };
                    resultList.Add(obj);
                }
                result.data = resultList;
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