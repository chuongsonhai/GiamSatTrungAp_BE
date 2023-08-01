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
    //[RoutePrefix("api/dashboard")]
    public class CanhBaoController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(CanhBaoController));

        //1.(GET) dashboard/canhbao
        //[JwtAuthentication]
        [HttpPost]
        [Route("dashboard/canhbao")]
        public IHttpActionResult GetCanhbao(CanhBaoFilterRequestdashboardcanhbao model)
        {

            ResponseResult result = new ResponseResult();
            try
            {
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                
                IList<CanhbaoModel> data = new List<CanhbaoModel>();
                var list = service.GetbyCanhbao(model.Filterdashboardcanhbao.fromdate, model.Filterdashboardcanhbao.todate);
                foreach (var item in list)
                {
                  data.Add(new CanhbaoModel(item));
              
                }
                result.total = list.Count();
                result.data = data;
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

        //3.(GET) dashboard/thoigiancapdien
        [HttpPost]
        [Route("dashboard/thoigiancapdien")]
        public IHttpActionResult GetThoigiancapdien(string donViQuanLy, DateTime tungay, DateTime denngay)
        {
            ResponseResult result = new ResponseResult();
            try
            {

                DateTime synctime = DateTime.Today;
                IReportService service = IoC.Resolve<IReportService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
               

                var list = service.GetThoigiancapdien(donViQuanLy, tungay, denngay);
             
                var listModel = new Thoigiancapdien(list);


                result.data = listModel;
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

        //2.1	(GET) /canhbao/filter
        //[JwtAuthentication]
        [HttpPost]
        [Route("canhbao/filter")]
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
        public IHttpActionResult GetById(CanhBao model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                var item = new CanhBao();
                // item = service.Getbykey(Id);
                item.ID = model.ID;
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
        public IHttpActionResult GetBycanhbaoId(int id)
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

        //2.9	(POST) /canhbao/{id}
        //[JwtAuthentication]
        [HttpPost]
        [Route("canhbao/id")]
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
        [Route("canhbao/phanhoi/add")]
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
                item.TRANGTHAI_XOA = 0;
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

        //2.6	(POST) / canhbao/phanhoi/delete
        //[JwtAuthentication]
        [HttpPost]
        [Route("canhbao/phanhoi/delete")]
        public IHttpActionResult Delete(PhanhoiTraodoiRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
                var item = new PhanhoiTraodoi();
                item.ID = model.ID;
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

        //2.14/canhbao/add
        [HttpGet]
        [Route("createCanhBao")]
        public IHttpActionResult GetListCanhBao()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IReportService service = IoC.Resolve<IReportService>();
                ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
                var list = service.TinhThoiGian();
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
                            canhbao.NOIDUNG = "Thời gian tiếp nhận yêu cầu cấp điện lập thỏa thuận đấu nối của khách hàng quá 02 giờ; Mã Yêu cầu:" + item.MaYeuCau + ";Tên KH:" + item.NguoiYeuCau + ";SDT:" + item.DienThoai;
                            break;
                        case 2:
                            canhbao.NOIDUNG = "Thời gian thực hiện lập thỏa thuận đấu nối quá 02 ngày; Mã Yêu cầu:" + item.MaYeuCau + ";Tên KH:" + item.NguoiYeuCau + ";SDT:" + item.DienThoai;
                            break;
                        case 3:
                            canhbao.NOIDUNG = "Thời gian tiếp nhận yêu cầu kiểm tra đóng điện và nghiệm thu; Mã Yêu cầu:" + item.MaYeuCau + ";Tên KH:" + item.NguoiYeuCau + ";SDT:" + item.DienThoai;
                            break;
                        case 4:
                            canhbao.NOIDUNG = "Thời gian dự thảo và ký hợp đồng mua bán điện; Mã Yêu cầu:" + item.MaYeuCau + ";Tên KH:" + item.NguoiYeuCau + ";SDT:" + item.DienThoai;
                            break;
                        case 5:
                            canhbao.NOIDUNG = "Thời gian thực hiện kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu; Mã Yêu cầu:" + item.MaYeuCau + ";Tên KH:" + item.NguoiYeuCau + ";SDT:" + item.DienThoai;
                            break;
                        case 6:
                            canhbao.NOIDUNG = "Giám sát thời gian nghiệm thu yêu cầu cấp điện mới trung áp; Mã Yêu cầu:" + item.MaYeuCau + ";Tên KH:" + item.NguoiYeuCau + ";SDT:" + item.DienThoai;
                            break;
                        case 7:
                            canhbao.NOIDUNG = "Cảnh báo các bộ hồ sơ sắp hết hạn hiệu lực thỏa thuận đấu nối; Mã Yêu cầu:" + item.MaYeuCau + ";Tên KH:" + item.NguoiYeuCau + ";SDT:" + item.DienThoai;
                            break;
                        case 8:
                            canhbao.NOIDUNG = "Thời gian thực hiện cấp điện mới trung áp; Mã Yêu cầu:" + item.MaYeuCau + ";Tên KH:" + item.NguoiYeuCau + ";SDT:" + item.DienThoai;
                            break;
                    }
                    CBservice.CreateNew(canhbao);
                }
                service.CommitChanges();
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

    }
}
