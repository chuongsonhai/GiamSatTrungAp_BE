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
        [Route("khachhang/filter")]
        public IHttpActionResult khachhangFilter(XacNhanTroNgaiFilterkhRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                DateTime synctime = DateTime.Today;
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                var list = service.GetbykhachhangFilter(request.FilterKH.tuNgay, request.FilterKH.denNgay, request.FilterKH.maLoaiCanhBao,
                    request.FilterKH.donViQuanLy, pageindex, request.Paginator.pageSize, out total);
                var listModel = new List<XacNhanTroNgaiRequest>();
                foreach (var item in list)
                {
                    var model = new XacNhanTroNgaiRequest(item);
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
                result.data = new List<XacNhanTroNgaiRequest>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }
        //[JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(XacNhanTroNgaiFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                DateTime synctime = DateTime.Today;
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                var list = service.GetbyFilter(request.Filter.tuNgay, request.Filter.denNgay, request.Filter.trangThaiKhaoSat, 
                    request.Filter.maYeuCau, request.Filter.donViQuanLy, pageindex, request.Paginator.pageSize, out total);
                var listModel = new List<XacNhanTroNgaiRequest>();
                foreach (var item in list)
                {
                    var model = new XacNhanTroNgaiRequest(item);
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
        public IHttpActionResult GetById(XacNhanTroNgakhaosatid model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                var item = new XacNhanTroNgai();
                item.ID = model.idKhaoSat;

                result.data = item;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new XacNhanTroNgai();
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

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




        //2.7	(POST) / canhbao/xacnhantrongai/add
        //[JwtAuthentication]
        [HttpPost]
        [Route("canhbao/xacnhantrongai/add")]
        public IHttpActionResult XacnhantrongaiAdd([FromBody] XacNhanTroNgaiRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();

                var item = new XacNhanTroNgai();
                item.CANHBAO_ID = model.CANHBAO_ID;   
                item.TRANGTHAI= 0;
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

        //2.8	(POST) / canhbao/xacnhantrongai/edit
        //[JwtAuthentication]
        [HttpPost]
        [Route("canhbao/xacnhantrongai/edit")]
        public IHttpActionResult XacnhantrongaiEdit([FromBody] XacNhanTroNgaiRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                var item = new XacNhanTroNgai();
                item.ID = model.ID;
                item.CANHBAO_ID = model.CANHBAO_ID;
                item.TRANGTHAI = 1;
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

    }
}