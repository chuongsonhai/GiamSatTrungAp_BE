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
        //[JwtAuthentication]
        [HttpPost]
        [Route("khaosat/filter")]
        public IHttpActionResult Filter(XacNhanTroNgaiFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                DateTime synctime = DateTime.Today;
                IXacNhanTroNgaiService khaosatService = IoC.Resolve<IXacNhanTroNgaiService>();
                IGiamSatCongVanCanhbaoidService serviceyeucau = IoC.Resolve<IGiamSatCongVanCanhbaoidService>();
                List<object> resultList = new List<object>();
                //var list = canhBaoService.GetbykhachhangFilter(request.Filter.tuNgay, request.Filter.denNgay, request.Filter.trangThaiKhaoSat,
                //    request.Filter.maYeuCau, request.Filter.donViQuanLy, pageindex, request.Paginator.pageSize, out total);
                var list = khaosatService.khaosatfilter(request.Filter.tuNgay, request.Filter.denNgay, request.Filter.trangThaiKhaoSat,
                  request.Filter.donViQuanLy, pageindex, request.Paginator.pageSize, out total);
                foreach (var canhbao in list)
                {
                   // var listLog = serviceyeucau.Filterkhaosat(canhbao.ID);
                    //foreach (var kh in listLog)
                    //{
                    //    resultList.Add(new { kh.MaYeuCau, kh.TenKhachHang, kh.TrangThai });
                    //}
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
                var ThongTinCanhBao = servicecanhbao.Getbyid(id);
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
                item.ID = model.idKhaoSat;
                item.NOIDUNG_CAUHOI = model.noiDungKhaoSat;
                item.PHANHOI_KH = model.khachHangPhanHoi;        
                item.KETQUA = model.ketQuaKhaoSat;
                item.NGUOI_KS = model.nguoiKhaoSat;
                item.TRANGTHAI = model.trangThaiKhaoSat;
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


        //2.2 (GET)/dashboard/khaosat
        //[JwtAuthentication]
        [HttpPost]
        [Route("dashboard/khaosat")]
        public IHttpActionResult GetKhaosat(XacNhanTroNgaiFilterkhaosatRequest Request)
        {

            ResponseResult result = new ResponseResult();
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                var list = service.GetSoLuongKhaoSat(Request.Filterdashboadkhaosat.tuNgay, Request.Filterdashboadkhaosat.denNgay);

                //  result.total = list.Count();
                result.data = list;
                result.success = true;
                return Ok(result);

            }
            catch (Exception ex)
            {
                result.success = false;
                var mess = ex.Message;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
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
                var item = new PhanhoiTraodoi();
                item.ID = model.ID;
                item.NOIDUNG_PHANHOI = model.NOIDUNG_PHANHOI;
                item.NGUOI_GUI = model.NGUOI_GUI;
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

    }
}