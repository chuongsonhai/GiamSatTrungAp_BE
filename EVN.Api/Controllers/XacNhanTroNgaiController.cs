﻿using EVN.Api.Jwt;
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
    [RoutePrefix("api/KhaoSat")]
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
                    //var fromDate = DateTime.MinValue.ToString();
                    //var toDate = DateTime.MaxValue.ToString();
                    //if (!string.IsNullOrWhiteSpace(request.FilterKH.tuNgay))
                    //    fromDate = DateTime.ParseExact(request.FilterKH.tuNgay, DateTimeParse.Format, null, DateTimeStyles.None);
                    //if (!string.IsNullOrWhiteSpace(request.FilterKH.denNgay))
                    //    toDate = DateTime.ParseExact(request.FilterKH.denNgay, DateTimeParse.Format, null, DateTimeStyles.None);
                    ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
                    IGiamSatCongVanCanhbaoidService serviceyeucau = IoC.Resolve<IGiamSatCongVanCanhbaoidService>();
                    var resultList = new List<object>();
                    var list = canhBaoService.GetbykhachhangFilter(request.FilterKH.tuNgay, request.FilterKH.denNgay, request.FilterKH.maLoaiCanhBao,
                        request.FilterKH.donViQuanLy, pageindex, request.Paginator.pageSize, out total);
                    foreach (var canhbao in list)
                    {
                        var listCongVan = serviceyeucau.Filterkhaosat(canhbao.MA_YC);
                        foreach (var congvan in listCongVan)
                        {
                        string textTrangThai = "";
                        if (congvan.TrangThai == TrangThaiCongVan.MoiTao)
                        {
                            textTrangThai = "Mới tạo";
                        }
                        else if (congvan.TrangThai == TrangThaiCongVan.TiepNhan)
                        {
                            textTrangThai = "Tiếp Nhận";
                        }
                        else if (congvan.TrangThai == TrangThaiCongVan.PhanCongKS)
                        {
                            textTrangThai = "Phân Công Khảo Sát";
                        }
                        else if (congvan.TrangThai  == TrangThaiCongVan.GhiNhanKS)
                        {
                            textTrangThai = "Ghi Nhận Khảo Sát";
                        }
                        else if (congvan.TrangThai == TrangThaiCongVan.BienBanKS)
                        {
                            textTrangThai = "Biên Bản Khảo sát";
                        }
                        else if (congvan.TrangThai == TrangThaiCongVan.DuThaoTTDN)
                        {
                            textTrangThai = "Dự thảo thỏa thuận đấu nối";
                        }else if(congvan.TrangThai == TrangThaiCongVan.KHKy)
                        {
                            textTrangThai = "Khách Hàng Ký";
                        }
                        else if (congvan.TrangThai == TrangThaiCongVan.DuChuKy)
                        {
                            textTrangThai = "Đủ Chữ Ký";
                        }else if(congvan.TrangThai == TrangThaiCongVan.HoanThanh)
                        {
                            textTrangThai = "Hoàn Thành";
                        }else if(congvan.TrangThai == TrangThaiCongVan.ChuyenTiep)
                        {
                            textTrangThai = "Chuyển Tiếp";
                        }
                        else if (congvan.TrangThai == TrangThaiCongVan.Huy)
                        {
                            textTrangThai = "Hủy";
                        }
                        resultList.Add(new { congvan.MaYeuCau, congvan.TenKhachHang, TrangThai = textTrangThai });
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
        [Route("{id}")]
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
                var textTrangThaiKhaoSat = "";
                var textTrangThaiYeuCau = "";

                //chuyển trạng thái cảnh báo sang kiểu text
                if (khaosat.TRANGTHAI == 1)
                {
                    textTrangThaiKhaoSat = "Mới tạo danh sách";
                }
                else if (khaosat.TRANGTHAI == 2)
                {
                    textTrangThaiKhaoSat = "Tạo phiếu khảo sát";
                }
                else if (khaosat.TRANGTHAI == 3)
                {
                    textTrangThaiKhaoSat = "Cập nhật kết quả khảo sát";
                }
                else if (khaosat.TRANGTHAI == 4)
                {
                    textTrangThaiKhaoSat = "Chuyển đơn vị";
                }
                else if (khaosat.TRANGTHAI == 5)
                {
                    textTrangThaiKhaoSat = "Đơn vị cập nhật giải trình";
                }
                else if (khaosat.TRANGTHAI == 6)
                {
                    textTrangThaiKhaoSat = "Kết thúc khảo sát";
                }



                //chuyển trạng thái công văn sang kiểu text
                if (ThongTinYeuCau.TrangThai == TrangThaiCongVan.MoiTao)
                {
                    textTrangThaiYeuCau = "Mới tạo";
                }
                else if (ThongTinYeuCau.TrangThai == TrangThaiCongVan.TiepNhan)
                {
                    textTrangThaiYeuCau = "Tiếp nhận";
                }
                else if (ThongTinYeuCau.TrangThai == TrangThaiCongVan.PhanCongKS)
                {
                    textTrangThaiYeuCau = "Phân công khảo sát";
                }
                else if (ThongTinYeuCau.TrangThai == TrangThaiCongVan.GhiNhanKS)
                {
                    textTrangThaiYeuCau = "Ghi nhận khảo sát";
                }
                else if (ThongTinYeuCau.TrangThai == TrangThaiCongVan.BienBanKS)
                {
                    textTrangThaiYeuCau = "Biên bản khảo sát";
                }
                else if (ThongTinYeuCau.TrangThai == TrangThaiCongVan.DuThaoTTDN)
                {
                    textTrangThaiYeuCau = "Dự thảo thỏa thuận đấu nối";
                }
                else if (ThongTinYeuCau.TrangThai == TrangThaiCongVan.KHKy)
                {
                    textTrangThaiYeuCau = "Khách hàng ký";
                }
                else if (ThongTinYeuCau.TrangThai == TrangThaiCongVan.DuChuKy)
                {
                    textTrangThaiYeuCau = "Đủ chữ ký";
                }
                else if (ThongTinYeuCau.TrangThai == TrangThaiCongVan.HoanThanh)
                {
                    textTrangThaiYeuCau = "Hoàn thành";
                }
                else if (ThongTinYeuCau.TrangThai == TrangThaiCongVan.ChuyenTiep)
                {
                    textTrangThaiYeuCau = "Chuyển tiếp";
                }
                else if (ThongTinYeuCau.TrangThai == TrangThaiCongVan.Huy)
                {
                    textTrangThaiYeuCau = "Hủy";
                }


                var oj1 = new
                { 
                        maYeuCau = ThongTinYeuCau.MaYeuCau,
                        ketQuaKhaoSat = khaosat.KETQUA,
                        trangThaiYeuCau = textTrangThaiYeuCau,
                        trangThaiKhaoSat = textTrangThaiKhaoSat,
                        tenKhachHang = ThongTinYeuCau.TenKhachHang,
                        nguoiKhaoSat = HttpContext.Current.User.Identity.Name,
                        thoiGianKhaoSat = khaosat.THOIGIAN_KHAOSAT   
                };
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

        //2.5 (POST) /khaosat/add
        //[JwtAuthentication]
        //thêm mới khảo sát 
        [HttpPost]
        [Route("add")]
        public IHttpActionResult Post([FromBody] XacNhanTroNgaikhaosatadd model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                IUserdataService userdataService = IoC.Resolve<IUserdataService>();
                var item = new XacNhanTroNgai();
        
                item.NOIDUNG_CAUHOI = model.noiDungKhaoSat;
                item.PHANHOI_KH = model.khachHangPhanHoi;
                item.KETQUA = model.ketQuaKhaoSat;
                item.NGUOI_KS = HttpContext.Current.User.Identity.Name;
                item.TRANGTHAI = 3;
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
        //sửa khảo sát
        [Route("{id}")]
        [HttpPost]
        public IHttpActionResult UpdateById([FromBody] XacNhanTroNgakhaosatid model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                var item = new XacNhanTroNgai();

                //sửa nội dung khảo sát
                var khaosat = service.GetKhaoSat(model.idKhaoSat);
                khaosat.NOIDUNG_CAUHOI = model.noiDungKhaoSat;
                khaosat.PHANHOI_KH = model.khachHangPhanHoi;
                khaosat.KETQUA = model.ketQuaKhaoSat;
                khaosat.NGUOI_KS = HttpContext.Current.User.Identity.Name;
                khaosat.TRANGTHAI = 3;
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
        // thêm mới phản hồi
        [HttpPost]
        [Route("phanhoi/add")]
        public IHttpActionResult Post([FromBody] PhanhoiTraodoiRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();

                var item = new PhanhoiTraodoi();
            
                item.NOIDUNG_PHANHOI = model.NOIDUNG_PHANHOI;
                item.NGUOI_GUI = HttpContext.Current.User.Identity.Name;
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
        // sửa nội dung phản hồi
        [HttpPost]
        [Route("phanhoi/{id}")]
        public IHttpActionResult UpdateById([FromUri] int id, [FromBody] PhanhoiTraodoiRequest model )
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
               // var item = new PhanhoiTraodoi();
               // service.Updatephanhoiid(model.ID);

                // lấy phản hồi trao đổi bằng ID 
                var phanhoi = service.GetbyPhanHoiId(id);

                //cập nhật nội dung phản hồi trao đổi
                phanhoi.NOIDUNG_PHANHOI = model.NOIDUNG_PHANHOI;
                phanhoi.NGUOI_GUI = HttpContext.Current.User.Identity.Name;
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
        [HttpGet]
        [Route("phanhoi/{id}")]
        public IHttpActionResult Filter([FromUri] int id)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int total = 0;
                DateTime synctime = DateTime.Today;
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
                var list = service.FilterByID(id);
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
        [Route("filter")]
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
                // lọc cảnh báo theo thời gian, mã đơn vị quản lý 
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
                        var textTrangThaiKhaoSat = "";
                        if (khaosat.TRANGTHAI == 1)
                        {
                            textTrangThaiKhaoSat = "Mới tạo danh sách";
                        }
                        else if (khaosat.TRANGTHAI == 2)
                        {
                            textTrangThaiKhaoSat = "Tạo phiếu khảo sát";
                        }
                        else if (khaosat.TRANGTHAI == 3)
                        {
                            textTrangThaiKhaoSat = "Cập nhật kết quả khảo sát";
                        }
                        else if (khaosat.TRANGTHAI == 4)
                        {
                            textTrangThaiKhaoSat = "Chuyển đơn vị";
                        }
                        else if (khaosat.TRANGTHAI == 5)
                        {
                            textTrangThaiKhaoSat = "Đơn vị cập nhật giải trình";
                        }
                        else if (khaosat.TRANGTHAI == 6)
                        {
                            textTrangThaiKhaoSat = "Kết thúc khảo sát";
                        }
                        listKhaoSatFilter.Add(new { khaosat.ID, TrangThaiKhaoSat = textTrangThaiKhaoSat, khaosat.THOIGIAN_KHAOSAT, khaosat.KETQUA });
                    }

                    //lọc ra tên khác hàng, trạng thái yêu cầu ứng với mã yêu cầu
                    var congVanYeuCau = congVanYeuCauService.GetbyMaYCau(canhbao.MA_YC);
                    string textTrangThai = "";
                    if (congVanYeuCau.TrangThai == TrangThaiCongVan.MoiTao)
                    {
                        textTrangThai = "Mới tạo";
                    }
                    else if (congVanYeuCau.TrangThai == TrangThaiCongVan.TiepNhan)
                    {
                        textTrangThai = "Tiếp Nhận";
                    }
                    else if (congVanYeuCau.TrangThai == TrangThaiCongVan.PhanCongKS)
                    {
                        textTrangThai = "Phân Công Khảo Sát";
                    }
                    else if (congVanYeuCau.TrangThai == TrangThaiCongVan.GhiNhanKS)
                    {
                        textTrangThai = "Ghi Nhận Khảo Sát";
                    }
                    else if (congVanYeuCau.TrangThai == TrangThaiCongVan.BienBanKS)
                    {
                        textTrangThai = "Biên Bản Khảo sát";
                    }
                    else if (congVanYeuCau.TrangThai == TrangThaiCongVan.DuThaoTTDN)
                    {
                        textTrangThai = "Dự thảo thỏa thuận đấu nối";
                    }
                    else if (congVanYeuCau.TrangThai == TrangThaiCongVan.KHKy)
                    {
                        textTrangThai = "Khách Hàng Ký";
                    }
                    else if (congVanYeuCau.TrangThai == TrangThaiCongVan.DuChuKy)
                    {
                        textTrangThai = "Đủ Chữ Ký";
                    }
                    else if (congVanYeuCau.TrangThai == TrangThaiCongVan.HoanThanh)
                    {
                        textTrangThai = "Hoàn Thành";
                    }
                    else if (congVanYeuCau.TrangThai == TrangThaiCongVan.ChuyenTiep)
                    {
                        textTrangThai = "Chuyển Tiếp";
                    }
                    else if (congVanYeuCau.TrangThai == TrangThaiCongVan.Huy)
                    {
                        textTrangThai = "Hủy";
                    }
                    //tạo ra response API
                    var obj = new { congVanYeuCau.MaYeuCau, congVanYeuCau.TenKhachHang, DanhSachKhaoSat =  listKhaoSatFilter, TrangThaiCongVan = textTrangThai };
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
        [Route("log/filter")]
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

                    var obj = new { canhbao.DONVI_DIENLUC, canhbao.TRANGTHAI_CANHBAO, DanhSachKhaoSat = listKhaoSatFilter, DanhSachLogCanhBao = listLogCanhBao };
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