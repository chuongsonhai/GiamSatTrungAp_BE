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
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/canhbao")]
    public class GiamSatCapDienController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(GiamSatCapDienController));

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
                var list = service.Filter(filter.Filter.fromdate, filter.Filter.todate, filter.Filter.maLoaiCanhBao, filter.Filter.trangThai, filter.Filter.maDViQLy);
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
        [Route("canhbao/finnish")]
        public IHttpActionResult GetById([FromUri] int Id)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                var item = new CanhBao();
                item = service.Getbykey(Id);
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
        [Route("canhbao/id")]
        public IHttpActionResult GetBycanhbaoId([FromUri] int id)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IGiamSatCanhBaoCanhbaoidService servicecanhbao = IoC.Resolve<IGiamSatCanhBaoCanhbaoidService>();
                IGiamSatPhanhoiCanhbaoidService servicephanhoi = IoC.Resolve<IGiamSatPhanhoiCanhbaoidService>();
                IGiamSatCongVanCanhbaoidService serviceyeucau = IoC.Resolve<IGiamSatCongVanCanhbaoidService>();
                var ThongTinCanhBao = servicecanhbao.Getbyid(id);  
                var ThongTinYeuCau = serviceyeucau.GetbyMaYCau(ThongTinCanhBao.MA_YC);
                var DanhSachPhanHoi = servicephanhoi.Getbyid(id);
                var oj = new { ThongTinCanhBao, ThongTinYeuCau, DanhSachPhanHoi };
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

        //2.4	(POST) / canhbao/phanhoi/add
        //[JwtAuthentication]
        [HttpPost]
        [Route("canhbao/phanhoi/add")]
        public IHttpActionResult Post([FromBody] PhanhoiTraodoiRequest model)
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
                item.PHANHOI_TRAODOI_ID = model.PHANHOI_TRAODOI_ID;
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
        [Route("canhbao/phanhoi/edit")]
        public IHttpActionResult UpdateById([FromBody] giamSatCapDien model, [FromUri] int Id)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IgiamSatCapDienService service = IoC.Resolve<IgiamSatCapDienService>();
                var item = new giamSatCapDien();
                item.ID = Id;
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

        //2.6	(POST) / canhbao/phanhoi/delete
        //[JwtAuthentication]
        [HttpPost]
        [Route("canhbao/phanhoi/delete")]
        public IHttpActionResult Delete([FromUri] int ID)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
                var item = new PhanhoiTraodoi();
                item.ID = ID;
                //item.TenLoaiCanhBao = model.TenLoaiCanhBao;
                //item.ChuKyGui = model.ChuKyGui;
                //item.PhanLoai = model.PhanLoai;
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

        //2.9	(POST) /canhbao/{id}
        //[JwtAuthentication]
        [HttpPost]
        [Route("canhbao/id")]
        public IHttpActionResult PostCanhbao([FromBody] GiamsatcapdienCanhBaoid model, [FromUri] int Id)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IGiamsatcapdienCanhBaoidService service = IoC.Resolve<IGiamsatcapdienCanhBaoidService>();

                var item = new GiamsatcapdienCanhBaoid();
                item.ID = Id;
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

        //2.10	(GET) /cauhinhcanhbao/filter
        //[JwtAuthentication]
        [HttpGet]
        [Route("cauhinhcanhbao/filter")]
        public IHttpActionResult cauhinhcanhbao(int maLoaiCanhBao)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                // int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                // int total = 0;
                DateTime synctime = DateTime.Today;
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();
                var list = service.Filter( maLoaiCanhBao);
                IList<Cauhinhcanhbao> data = new List<Cauhinhcanhbao>();

                foreach (var item in list)
                {
                    data.Add(new Cauhinhcanhbao(item));

                }
                // result.total = list.Count();
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new List<Cauhinhcanhbao>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }


        //2.11	(POST) /cauhinhcanhbao/add
        //[JwtAuthentication]
        [HttpPost]
        [Route("cauhinhcanhbao/add")]
        public IHttpActionResult cauhinhcanhbaoadd([FromBody] LoaiCanhBaoDataRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();

                var item = new DanhMucLoaiCanhBao();

                item.TENLOAICANHBAO = model.TENLOAICANHBAO;
                item.CHUKYCANHBAO = model.CHUKYCANHBAO;
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

        //2.12	(POST) /cauhinhcanhbao/edit
        //[JwtAuthentication]
        [HttpPost]
        [Route("cauhinhcanhbao/edit")]
        public IHttpActionResult cauhinhcanhbaoedit([FromBody] LoaiCanhBaoDataRequest model, [FromUri] int Id)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();
                var item = new DanhMucLoaiCanhBao();
                item.ID = Id;
               // item.ID = model.MALOAICANHBAO;
                item.TENLOAICANHBAO = model.TENLOAICANHBAO;
                item.CHUKYCANHBAO = model.CHUKYCANHBAO;
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
        [Route("cauhinhcanhbao/log/filter")]
        public IHttpActionResult logFilter(int canhbaoID, int trangThai, string datacu, string datamoi,
            string tungay, string denngay, string nguoithuchien)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                // int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                // int total = 0;
                DateTime synctime = DateTime.Today;
                ILogCanhBaoService service = IoC.Resolve<ILogCanhBaoService>();
                var list = service.GetbyFilter( canhbaoID,  trangThai,  datacu,  datamoi, tungay,  denngay,  nguoithuchien);
                IList<LogCanhBaoRequest> data = new List<LogCanhBaoRequest>();

                foreach (var item in list)
                {
                    data.Add(new LogCanhBaoRequest(item));

                }
                // result.total = list.Count();
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new List<LogCanhBao>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

    }
}
