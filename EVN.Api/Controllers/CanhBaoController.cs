﻿using EVN.Api.Jwt;
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
                IReportService service1 = IoC.Resolve<IReportService>();
                ICanhBaoService CBservice1 = IoC.Resolve<ICanhBaoService>();
                ILogCanhBaoService LogCBservice = IoC.Resolve<ILogCanhBaoService>();
                var list = service1.TinhThoiGian();
                //var listCanhBao = new List<CanhBao>();
                foreach (var item in list)
                {
                    var checkTonTai1 = await CBservice1.CheckExits(item.MaYeuCau, item.LoaiCanhBao);

                    if (!checkTonTai1)
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
                                canhbao.NOIDUNG = "Loại cảnh báo 1 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu cấp điện của khách hàng, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                break;
                            case 2:
                                canhbao.NOIDUNG = "Loại cảnh báo 2 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 02 ngày kể từ khi tiếp nhận đầy đủ hồ sơ thỏa thuận đấu nối của khách hàng, đơn vị chưa hoàn thành thỏa thuận đấu nối trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                break;
                            case 3:
                                canhbao.NOIDUNG = "Loại cảnh báo 3 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu kiểm tra điểm đóng điện và nghiệm thu của khách hàng của khách hàng đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                break;
                            case 4:
                                canhbao.NOIDUNG = "Loại cảnh báo 4 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 02 giờ kể từ khi có thông báo lập Hợp đồng mua bán điện đơn vị chưa xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                break;
                            case 5:
                                canhbao.NOIDUNG = "Loại cảnh báo 5 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 01 ngày kể từ khi tiếp nhận đầy đủ hồ sơ kiểm tra điểm đóng điện và nghiệm thu của khách hàng, đơn vị chưa hoàn thành kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                break;
                            case 6:
                                canhbao.NOIDUNG = "Loại cảnh báo 6 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 04 ngày kể từ khi tiếp nhận đầy đủ hồ sơ của khách hàng, đơn vị chưa hoàn thành cấp điện trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                break;
                            case 16:
                                canhbao.NOIDUNG = "Loại cảnh báo 16 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận:" + item.NgayLap + " ĐV: " + item.MaDViQLy + "<br> Đã quá thời gian 2 năm kể từ khi tiếp nhận đầy đủ hồ sơ thoản thuận đấu nối, hồ sơ thỏa thuận đấu nối của khách hàng sắp hết hiệu lực. Đơn vị liên hệ xác nhận với khách hàng để xử lý hồ sơ đúng qui định.";
                                break;
                            case 15:
                                canhbao.NOIDUNG = "Loại cảnh báo 15 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận:" + item.NgayLap + " ĐV: " + item.MaDViQLy + "<br>  Đã quá thời gian 04 ngày kể từ khi tiếp nhận đầy đủ hồ sơ kiểm tra điểm đóng điện và nghiệm thu của khách hàng, đơn vị chưa hoàn thành nghiệm thu và đóng điện trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                break;
                        }
                        //listCanhBao.Add(canhbao);

                        string message1 = "";
                        CBservice1.CreateCanhBao(canhbao, out message1);
                        if (string.IsNullOrEmpty(message1))
                        {
                            LogCanhBao logCB = new LogCanhBao();
                            // cần ins cả vào đây
                            logCB.CANHBAO_ID = item.ID;
                            logCB.DATA_MOI = JsonConvert.SerializeObject(item);
                            logCB.NGUOITHUCHIEN = "HeThong";
                            logCB.THOIGIAN = DateTime.Now;
                            logCB.TRANGTHAI = 1;
                            LogCBservice.CreateNew(logCB);
                            LogCBservice.CommitChanges();
                        }
                        else
                        {
                            throw new Exception(message1);
                        }
                    }
                    service1.CommitChanges();
                }

                var list1 = service1.TinhThoiGian2();
                //var listCanhBao = new List<CanhBao>();
                foreach (var item1 in list1)
                {
                    var checkTonTai11 = await CBservice1.CheckExits(item1.MaYeuCau, item1.LoaiCanhBao);

                    if (checkTonTai11)
                    {

                        string message1 = "";

                        var check_tontai_mycau1 = CBservice1.GetByMaYeuCautontai(item1.MaYeuCau, item1.LoaiCanhBao);
                        if (check_tontai_mycau1.TRANGTHAI_CANHBAO == 6)
                            continue;

                        if (check_tontai_mycau1.LOAI_SOLANGUI > 2)
                        {
                            continue;
                        }
                        else
                        {


                            TimeSpan ts = DateTime.Now - check_tontai_mycau1.THOIGIANGUI;
                            //cb1
                            if (ts.TotalMinutes > 30)
                            {

                                var canhbao12 = new CanhBao();


                                switch (item1.LoaiCanhBao)
                                {
                                    case 1:
                                        canhbao12.LOAI_CANHBAO_ID = item1.LoaiCanhBao;
                                        canhbao12.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                                        canhbao12.MA_YC = item1.MaYeuCau;
                                        canhbao12.THOIGIANGUI = DateTime.Now;
                                        canhbao12.TRANGTHAI_CANHBAO = 1;
                                        canhbao12.DONVI_DIENLUC = item1.MaDViQLy;
                                        canhbao12.NOIDUNG = "Loại cảnh báo 1 - lần " + canhbao12.LOAI_SOLANGUI + " <br>KH: " + item1.TenKhachHang + ", SĐT: " + item1.DienThoai + ", địa chỉ: " + item1.DiaChiDungDien + ", maYC: " + canhbao12.MA_YC + ", ngày tiếp nhận: " + item1.NgayLap + ", đơn vị: " + item1.MaDViQLy + "<br> Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu cấp điện của khách hàng, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                        CBservice1.CreateCanhBao(canhbao12, out message1);
                                        service1.CommitChanges();
                                        break;
                                }


                                if (string.IsNullOrEmpty(message1))
                                {
                                    LogCanhBao logCB = new LogCanhBao();
                                    // cần ins cả vào đây
                                    logCB.CANHBAO_ID = item1.ID;
                                    logCB.DATA_MOI = JsonConvert.SerializeObject(item1);
                                    logCB.NGUOITHUCHIEN = "HeThong";
                                    logCB.THOIGIAN = DateTime.Now;
                                    logCB.TRANGTHAI = 1;
                                    LogCBservice.CreateNew(logCB);
                                    LogCBservice.CommitChanges();
                                }
                                else
                                {
                                    throw new Exception(message1);
                                }

                            }


                            //cb2
                            if (ts.TotalMinutes > 30)
                            {

                                var canhbao122 = new CanhBao();


                                switch (item1.LoaiCanhBao)
                                {

                                    case 2:
                                        canhbao122.LOAI_CANHBAO_ID = item1.LoaiCanhBao;
                                        canhbao122.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                                        canhbao122.MA_YC = item1.MaYeuCau;
                                        canhbao122.THOIGIANGUI = DateTime.Now;
                                        canhbao122.TRANGTHAI_CANHBAO = 1;
                                        canhbao122.DONVI_DIENLUC = item1.MaDViQLy;
                                        canhbao122.NOIDUNG = "Loại cảnh báo 2 - lần " + canhbao122.LOAI_SOLANGUI + " <br>KH: " + item1.TenKhachHang + ", SĐT: " + item1.DienThoai + ", địa chỉ: " + item1.DiaChiDungDien + ", maYC: " + canhbao122.MA_YC + ", ngày tiếp nhận: " + item1.NgayLap + ", đơn vị: " + item1.MaDViQLy + "<br> Đã quá 02 ngày kể từ khi tiếp nhận đầy đủ hồ sơ thỏa thuận đấu nối của khách hàng, đơn vị chưa hoàn thành thỏa thuận đấu nối trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                        CBservice1.CreateCanhBao(canhbao122, out message1);
                                        service1.CommitChanges();
                                        break;

                                        if (string.IsNullOrEmpty(message1))
                                        {
                                            LogCanhBao logCB = new LogCanhBao();
                                            // cần ins cả vào đây
                                            logCB.CANHBAO_ID = item1.ID;
                                            logCB.DATA_MOI = JsonConvert.SerializeObject(item1);
                                            logCB.NGUOITHUCHIEN = "HeThong";
                                            logCB.THOIGIAN = DateTime.Now;
                                            logCB.TRANGTHAI = 1;
                                            LogCBservice.CreateNew(logCB);
                                            LogCBservice.CommitChanges();
                                        }
                                        else
                                        {
                                            throw new Exception(message1);
                                        }

                                }


                            }


                            //cb3
                            if (ts.TotalMinutes > 30)
                            {

                                var canhbao123 = new CanhBao();


                                switch (item1.LoaiCanhBao)
                                {
                                    case 3:
                                        canhbao123.LOAI_CANHBAO_ID = item1.LoaiCanhBao;
                                        canhbao123.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                                        canhbao123.MA_YC = item1.MaYeuCau;
                                        canhbao123.THOIGIANGUI = DateTime.Now;
                                        canhbao123.TRANGTHAI_CANHBAO = 1;
                                        canhbao123.DONVI_DIENLUC = item1.MaDViQLy;
                                        canhbao123.NOIDUNG = "Loại cảnh báo 3 - lần " + canhbao123.LOAI_SOLANGUI + " <br>KH: " + item1.TenKhachHang + ", SĐT: " + item1.DienThoai + ", địa chỉ: " + item1.DiaChiDungDien + ", maYC: " + canhbao123.MA_YC + ", ngày tiếp nhận: " + item1.NgayLap + ", đơn vị: " + item1.MaDViQLy + "<br> Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu kiểm tra điểm đóng điện và nghiệm thu của khách hàng của khách hàng đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                        CBservice1.CreateCanhBao(canhbao123, out message1);
                                        service1.CommitChanges();
                                        break;

                                        if (string.IsNullOrEmpty(message1))
                                        {
                                            LogCanhBao logCB = new LogCanhBao();
                                            // cần ins cả vào đây
                                            logCB.CANHBAO_ID = item1.ID;
                                            logCB.DATA_MOI = JsonConvert.SerializeObject(item1);
                                            logCB.NGUOITHUCHIEN = "HeThong";
                                            logCB.THOIGIAN = DateTime.Now;
                                            logCB.TRANGTHAI = 1;
                                            LogCBservice.CreateNew(logCB);
                                            LogCBservice.CommitChanges();
                                        }
                                        else
                                        {
                                            throw new Exception(message1);
                                        }

                                }

                            }


                            //cb4
                            if (ts.TotalMinutes > 30)
                            {

                                var canhbao124 = new CanhBao();


                                switch (item1.LoaiCanhBao)
                                {

                                    case 4:
                                        canhbao124.LOAI_CANHBAO_ID = item1.LoaiCanhBao;
                                        canhbao124.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                                        canhbao124.MA_YC = item1.MaYeuCau;
                                        canhbao124.THOIGIANGUI = DateTime.Now;
                                        canhbao124.TRANGTHAI_CANHBAO = 1;
                                        canhbao124.DONVI_DIENLUC = item1.MaDViQLy;
                                        canhbao124.NOIDUNG = "Loại cảnh báo 4 - lần " + canhbao124.LOAI_SOLANGUI + " <br>KH: " + item1.TenKhachHang + ", SĐT: " + item1.DienThoai + ", địa chỉ: " + item1.DiaChiDungDien + ", maYC: " + canhbao124.MA_YC + ", ngày tiếp nhận: " + item1.NgayLap + ", đơn vị: " + item1.MaDViQLy + "<br> Đã quá thời gian 02 giờ kể từ khi có thông báo lập Hợp đồng mua bán điện đơn vị chưa xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                        CBservice1.CreateCanhBao(canhbao124, out message1);
                                        service1.CommitChanges();
                                        break;

                                        if (string.IsNullOrEmpty(message1))
                                        {
                                            LogCanhBao logCB = new LogCanhBao();
                                            // cần ins cả vào đây
                                            logCB.CANHBAO_ID = item1.ID;
                                            logCB.DATA_MOI = JsonConvert.SerializeObject(item1);
                                            logCB.NGUOITHUCHIEN = "HeThong";
                                            logCB.THOIGIAN = DateTime.Now;
                                            logCB.TRANGTHAI = 1;
                                            LogCBservice.CreateNew(logCB);
                                            LogCBservice.CommitChanges();
                                        }
                                        else
                                        {
                                            throw new Exception(message1);
                                        }

                                }


                            }


                            //cb5
                            if (ts.TotalMinutes > 30)
                            {

                                var canhbao125 = new CanhBao();


                                switch (item1.LoaiCanhBao)
                                {

                                    case 5:
                                        canhbao125.LOAI_CANHBAO_ID = item1.LoaiCanhBao;
                                        canhbao125.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                                        canhbao125.MA_YC = item1.MaYeuCau;
                                        canhbao125.THOIGIANGUI = DateTime.Now;
                                        canhbao125.TRANGTHAI_CANHBAO = 1;
                                        canhbao125.DONVI_DIENLUC = item1.MaDViQLy;
                                        canhbao125.NOIDUNG = "Loại cảnh báo 5 - lần " + canhbao125.LOAI_SOLANGUI + " <br>KH: " + item1.TenKhachHang + ", SĐT: " + item1.DienThoai + ", địa chỉ: " + item1.DiaChiDungDien + ", maYC: " + canhbao125.MA_YC + ", ngày tiếp nhận: " + item1.NgayLap + " ,đơn vị: " + item1.MaDViQLy + "<br> Đã quá 01 ngày kể từ khi tiếp nhận đầy đủ hồ sơ kiểm tra điểm đóng điện và nghiệm thu của khách hàng, đơn vị chưa hoàn thành kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                        CBservice1.CreateCanhBao(canhbao125, out message1);
                                        service1.CommitChanges();
                                        break;

                                        if (string.IsNullOrEmpty(message1))
                                        {
                                            LogCanhBao logCB = new LogCanhBao();
                                            // cần ins cả vào đây
                                            logCB.CANHBAO_ID = item1.ID;
                                            logCB.DATA_MOI = JsonConvert.SerializeObject(item1);
                                            logCB.NGUOITHUCHIEN = "HeThong";
                                            logCB.THOIGIAN = DateTime.Now;
                                            logCB.TRANGTHAI = 1;
                                            LogCBservice.CreateNew(logCB);
                                            LogCBservice.CommitChanges();
                                        }
                                        else
                                        {
                                            throw new Exception(message1);
                                        }

                                }
                            }

                            //cb6
                            if (ts.TotalMinutes > 30)
                            {

                                var canhbao126 = new CanhBao();


                                switch (item1.LoaiCanhBao)
                                {

                                    case 6:
                                        canhbao126.LOAI_CANHBAO_ID = item1.LoaiCanhBao;
                                        canhbao126.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                                        canhbao126.MA_YC = item1.MaYeuCau;
                                        canhbao126.THOIGIANGUI = DateTime.Now;
                                        canhbao126.TRANGTHAI_CANHBAO = 1;
                                        canhbao126.DONVI_DIENLUC = item1.MaDViQLy;
                                        canhbao126.NOIDUNG = "Loại cảnh báo 6 - lần " + canhbao126.LOAI_SOLANGUI + " <br>KH: " + item1.TenKhachHang + ", SĐT: " + item1.DienThoai + ", địa chỉ: " + item1.DiaChiDungDien + ", maYC: " + canhbao126.MA_YC + ", ngày tiếp nhận: " + item1.NgayLap + " đơn vị: " + item1.MaDViQLy + "<br> Đã quá 04 ngày kể từ khi tiếp nhận đầy đủ hồ sơ của khách hàng, đơn vị chưa hoàn thành cấp điện trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                        CBservice1.CreateCanhBao(canhbao126, out message1);
                                        service1.CommitChanges();
                                        break;

                                        if (string.IsNullOrEmpty(message1))
                                        {
                                            LogCanhBao logCB = new LogCanhBao();
                                            // cần ins cả vào đây
                                            logCB.CANHBAO_ID = item1.ID;
                                            logCB.DATA_MOI = JsonConvert.SerializeObject(item1);
                                            logCB.NGUOITHUCHIEN = "HeThong";
                                            logCB.THOIGIAN = DateTime.Now;
                                            logCB.TRANGTHAI = 1;
                                            LogCBservice.CreateNew(logCB);
                                            LogCBservice.CommitChanges();
                                        }
                                        else
                                        {
                                            throw new Exception(message1);
                                        }

                                }


                            }

                            //cb16
                            if (ts.TotalMinutes > 30)
                            {

                                var canhbao127 = new CanhBao();


                                switch (item1.LoaiCanhBao)
                                {

                                    case 16:
                                        canhbao127.LOAI_CANHBAO_ID = item1.LoaiCanhBao;
                                        canhbao127.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                                        canhbao127.MA_YC = item1.MaYeuCau;
                                        canhbao127.THOIGIANGUI = DateTime.Now;
                                        canhbao127.TRANGTHAI_CANHBAO = 1;
                                        canhbao127.DONVI_DIENLUC = item1.MaDViQLy;
                                        
                                        canhbao127.NOIDUNG = "Loại cảnh báo 16 - lần " + canhbao127.LOAI_SOLANGUI + " <br>KH: " + item1.TenKhachHang + ", SĐT: " + item1.DienThoai + ", ĐC: " + item1.DiaChiDungDien + ", MaYC: " + canhbao127.MA_YC + ", ngày tiếp nhận:" + item1.NgayLap + " ĐV: " + item1.MaDViQLy + "<br> Đã quá thời gian 2 năm kể từ khi tiếp nhận đầy đủ hồ sơ thoản thuận đấu nối, hồ sơ thỏa thuận đấu nối của khách hàng sắp hết hiệu lực. Đơn vị liên hệ xác nhận với khách hàng để xử lý hồ sơ đúng qui định.";
                                        break;
                                        CBservice1.CreateCanhBao(canhbao127, out message1);
                                        service1.CommitChanges();
                                        if (string.IsNullOrEmpty(message1))
                                        {
                                            LogCanhBao logCB = new LogCanhBao();
                                            // cần ins cả vào đây
                                            logCB.CANHBAO_ID = item1.ID;
                                            logCB.DATA_MOI = JsonConvert.SerializeObject(item1);
                                            logCB.NGUOITHUCHIEN = "HeThong";
                                            logCB.THOIGIAN = DateTime.Now;
                                            logCB.TRANGTHAI = 1;
                                            LogCBservice.CreateNew(logCB);
                                            LogCBservice.CommitChanges();
                                        }
                                        else
                                        {
                                            throw new Exception(message1);
                                        }

                                }


                            }


                            //cb15
                            if (ts.TotalMinutes > 30)
                            {

                                var canhbao128 = new CanhBao();


                                switch (item1.LoaiCanhBao)
                                {
                                    case 15:
                                        canhbao128.LOAI_CANHBAO_ID = item1.LoaiCanhBao;
                                        canhbao128.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                                        canhbao128.MA_YC = item1.MaYeuCau;
                                        canhbao128.THOIGIANGUI = DateTime.Now;
                                        canhbao128.TRANGTHAI_CANHBAO = 1;
                                        canhbao128.DONVI_DIENLUC = item1.MaDViQLy;
                                        
                                        canhbao128.NOIDUNG = "Loại cảnh báo 15 - lần " + canhbao128.LOAI_SOLANGUI + " <br>KH: " + item1.TenKhachHang + ", SĐT: " + item1.DienThoai + ", ĐC: " + item1.DiaChiDungDien + ", MaYC: " + canhbao128.MA_YC + ", ngày tiếp nhận:" + item1.NgayLap + " ĐV: " + item1.MaDViQLy + "<br>  Đã quá thời gian 04 ngày kể từ khi tiếp nhận đầy đủ hồ sơ kiểm tra điểm đóng điện và nghiệm thu của khách hàng, đơn vị chưa hoàn thành nghiệm thu và đóng điện trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                                        CBservice1.CreateCanhBao(canhbao128, out message1);
                                        service1.CommitChanges();
                                        break;

                                        if (string.IsNullOrEmpty(message1))
                                        {
                                            LogCanhBao logCB = new LogCanhBao();
                                            // cần ins cả vào đây
                                            logCB.CANHBAO_ID = item1.ID;
                                            logCB.DATA_MOI = JsonConvert.SerializeObject(item1);
                                            logCB.NGUOITHUCHIEN = "HeThong";
                                            logCB.THOIGIAN = DateTime.Now;
                                            logCB.TRANGTHAI = 1;
                                            LogCBservice.CreateNew(logCB);
                                            LogCBservice.CommitChanges();
                                        }
                                        else
                                        {
                                            throw new Exception(message1);
                                        }

                                }


                            }
                        }
                            //end

                        }

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
        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(CanhBaoFilterRequest filter)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = filter.Paginator.page > 0 ? filter.Paginator.page - 1 : 0;
                int total = 0;
                DateTime synctime = DateTime.Today;
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                var list = service.Filter(filter.Filter.fromdate, filter.Filter.todate, filter.Filter.maLoaiCanhBao, filter.Filter.trangThai, filter.Filter.maDViQLy, filter.Filter.SoLanGui, filter.Filter.keyword, pageindex, filter.Paginator.pageSize, out total);
                IList<CanhBaoRequest> data = new List<CanhBaoRequest>();

                foreach (var item in list)
                {
                    data.Add(new CanhBaoRequest(item));
                }
                result.total = total;
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
                ICanhBaoService CBservice1 = IoC.Resolve<ICanhBaoService>();
                var ThongTinCanhBao = servicecanhbao.Getbyid(id);
                var viewnguyennhan_canhbao = CBservice1.Getbyid(id);
                // mới chỉ lấy dc trạng thái của TTDN, chưa lấy dc của nghiệm thu
                var ThongTinYeuCau = serviceyeucau.GetbyMaYCau(ThongTinCanhBao.MA_YC);
                var DanhSachPhanHoi = servicephanhoi.Getbyid(id);
                var DanhSachTuongTac = LogCanhBaoservice.Filter(id);
                var oj = new { viewnguyennhan_canhbao, ThongTinCanhBao, ThongTinYeuCau, DanhSachPhanHoi, DanhSachTuongTac };
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
        [Route("updateStatus/{ID}/{Status}/{NGUYENHHAN_CANHBAO}/{KETQUA_GIAMSAT}")]
        public IHttpActionResult updateStatus([FromUri] int ID, [FromUri] int Status, [FromUri] int NGUYENHHAN_CANHBAO, [FromUri] string KETQUA_GIAMSAT)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                ILogCanhBaoService LogCBservice = IoC.Resolve<ILogCanhBaoService>();
                var item = new CanhBao();
                item = service.Getbyid(ID);
                item.TRANGTHAI_CANHBAO = Status;
                item.NGUYENHHAN_CANHBAO = NGUYENHHAN_CANHBAO;
                item.KETQUA_GIAMSAT = KETQUA_GIAMSAT;
                service.Update(item);
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


        [HttpPost]
        [Route("UpdateStatusModel")]
        public IHttpActionResult UpdateStatusModel([FromBody] UpdateStatusModel model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {

                ICanhbaoUpdateStatusService service = IoC.Resolve<ICanhbaoUpdateStatusService>();
                ILogCanhBaoService LogCBservice = IoC.Resolve<ILogCanhBaoService>();
                var item = new cbUpdateStatusModel();
                item.ID = model.ID;
                item.TRANGTHAI_CANHBAO = 6;
                item.NGUYENHHAN_CANHBAO = model.NGUYENHHAN_CANHBAO;
                item.KETQUA_GIAMSAT = model.KETQUA_GIAMSAT;
                item.NGUOI_GIAMSAT = model.NGUOI_GIAMSAT;

                service.Update(item);
                service.CommitChanges();

                LogCanhBao logCB = new LogCanhBao();
                logCB.CANHBAO_ID = model.ID;
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

        [HttpGet]
        [Route("createCanhBaonew")]
        public async Task<IHttpActionResult> GetListCanhBaonew()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IReportService service1 = IoC.Resolve<IReportService>();
                ICanhBaoService CBservice1 = IoC.Resolve<ICanhBaoService>();
                ILogCanhBaoService LogCBservice = IoC.Resolve<ILogCanhBaoService>();
                IUserNhanCanhBaoService userNhanCanhBaoService = IoC.Resolve<IUserNhanCanhBaoService>();
                IUserdataService userdataService = IoC.Resolve<IUserdataService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IEmailService serviceemail = IoC.Resolve<IEmailService>();

                // lấy danh sách yêu cầu chậm tiến độ
                var list = service1.TinhThoiGianCanhBao();

                // duyệt yêu cầu trong danh sách
                // Ghi dữ liệu vào bảng GSCD_CANH_BAO
                // Ghi log vào bảng GSCD_LOG_CANHBAO
                foreach (var item in list)
                {
                    // kiểm tra đã có cảnh bảo chưa
                    var checkTonTai1 = await CBservice1.CheckExits(item.MaYeuCau, item.LoaiCanhBao);
                    var solangui = 1;

                    // nếu đã tồn tại cảnh báo
                    if (checkTonTai1)
                    {
                        // lấy cảnh báo gần nhất
                        var check_tontai_mycau1 = CBservice1.GetByMaYeuCautontai(item.MaYeuCau, item.LoaiCanhBao);

                        // tính thời gian từ cảnh báo gần nhất đến thời điểm hiện tại
                        TimeSpan ts = DateTime.Now - check_tontai_mycau1.THOIGIANGUI;

                        // kiểm tra các trường hợp bỏ qua không tạo cảnh báo
                        if (check_tontai_mycau1.TRANGTHAI_CANHBAO == 6 || check_tontai_mycau1.LOAI_SOLANGUI > 2 || ts.TotalMinutes < 30)
                            continue;

                        // nếu đạt điều kiện gửi cảnh báo: tăng số lần gửi lên 1.
                        solangui = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                    }

                    var canhbao = new CanhBao();
                    canhbao.LOAI_CANHBAO_ID = item.LoaiCanhBao;
                    canhbao.LOAI_SOLANGUI = solangui;
                    canhbao.MA_YC = item.MaYeuCau;
                    canhbao.THOIGIANGUI = DateTime.Now;
                    canhbao.TRANGTHAI_CANHBAO = 1;
                    canhbao.DONVI_DIENLUC = item.MaDViQLy;
                    switch (item.LoaiCanhBao)
                    {
                        case 1:
                            canhbao.NOIDUNG = "Loại cảnh báo 1 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu cấp điện của khách hàng, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 2:
                            canhbao.NOIDUNG = "Loại cảnh báo 2 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 02 ngày kể từ khi tiếp nhận đầy đủ hồ sơ thỏa thuận đấu nối của khách hàng, đơn vị chưa hoàn thành thỏa thuận đấu nối trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 3:
                            canhbao.NOIDUNG = "Loại cảnh báo 3 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu kiểm tra điểm đóng điện và nghiệm thu của khách hàng của khách hàng đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 4:
                            canhbao.NOIDUNG = "Loại cảnh báo 4 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 02 giờ kể từ khi có thông báo lập Hợp đồng mua bán điện đơn vị chưa xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 5:
                            canhbao.NOIDUNG = "Loại cảnh báo 5 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 01 ngày kể từ khi tiếp nhận đầy đủ hồ sơ kiểm tra điểm đóng điện và nghiệm thu của khách hàng, đơn vị chưa hoàn thành kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 6:
                            canhbao.NOIDUNG = "Loại cảnh báo 6 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Đã quá 04 ngày kể từ khi tiếp nhận đầy đủ hồ sơ của khách hàng, đơn vị chưa hoàn thành cấp điện trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                        case 16:
                            canhbao.NOIDUNG = "Loại cảnh báo 16 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận:" + item.NgayLap + " ĐV: " + item.MaDViQLy + "<br> Đã quá thời gian 2 năm kể từ khi tiếp nhận đầy đủ hồ sơ thoản thuận đấu nối, hồ sơ thỏa thuận đấu nối của khách hàng sắp hết hiệu lực. Đơn vị liên hệ xác nhận với khách hàng để xử lý hồ sơ đúng qui định.";
                            break;
                        case 15:
                            canhbao.NOIDUNG = "Loại cảnh báo 15 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận:" + item.NgayLap + " ĐV: " + item.MaDViQLy + "<br>  Đã quá thời gian 02 ngày kể từ khi tiếp nhận đầy đủ hồ sơ kiểm tra điểm đóng điện và nghiệm thu của khách hàng, đơn vị chưa hoàn thành nghiệm thu và đóng điện trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                            break;
                    }

                    string message1 = "";
                    CBservice1.CreateCanhBao(canhbao, out message1);

                    ///Nếu Lưu cảnh báo thành công (message1 = null)
                    ///ghi log
                    if (string.IsNullOrEmpty(message1))
                    {
                        LogCanhBao logCB = new LogCanhBao();
                        // cần ins cả vào đây
                        logCB.CANHBAO_ID = item.ID;
                        logCB.DATA_MOI = JsonConvert.SerializeObject(item);
                        logCB.NGUOITHUCHIEN = "HeThong";
                        logCB.THOIGIAN = DateTime.Now;
                        logCB.TRANGTHAI = 1;
                        LogCBservice.CreateNew(logCB);
                        //LogCBservice.CommitChanges();


                        //// Ghi dữ liệu vào bảng GSCD_DATA_EMAIL_ALL_TEST
                        //// lấy danh sách user nhận cảnh báo
                        //IList<UserNhanCanhBao> listNguoiNhan = userNhanCanhBaoService.GetbyMaDviQly(item.MaDViQLy);

                        //foreach (var nguoiNhan in listNguoiNhan)
                        //{
                        //    var user = userdataService.Getbykey(nguoiNhan.USER_ID);
                        //    if (user.email == null)
                        //        continue;
                        //    if (nguoiNhan.TRANGTHAI != 0)
                        //    {
                        //        var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).FirstOrDefault();
                        //        var manv_thuc_hien = ttrinhs.MA_NVIEN_NHAN;

                        //        if (string.IsNullOrEmpty(manv_thuc_hien) || manv_thuc_hien != user.maNVien)
                        //            continue;
                        //    }
                        //    Email email = new Email();
                        //    email.MA_DVIQLY = nguoiNhan.MA_DVIQLY;
                        //    email.MA_DVU = "TA";
                        //    email.NOI_DUNG = canhbao.NOIDUNG;
                        //    email.NGAY_TAO = DateTime.Now;
                        //    email.NGUOI_TAO = "admin";
                        //    email.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                        //    email.TINH_TRANG = 1;
                        //    email.EMAIL = user.email;
                        //    serviceemail.CreateNew(email);
                        //}
                        //canhbao.TRANGTHAI_CANHBAO = 2;
                        //CBservice1.Update(canhbao);
                    }
                }
                CBservice1.CommitChanges();

                // Ghi dữ liệu vào bảng GSCD_DATA_EMAIL_ALL_TEST
                //Lấy danh sách cảnh báo
                var cb = CBservice1.Query.Where(p => p.TRANGTHAI_CANHBAO == 1).ToList();
                foreach(var itemcb in cb)
                {
                    // lấy ds người nhận cảnh báo
                    IList<UserNhanCanhBao> listNguoiNhan = userNhanCanhBaoService.GetbyMaDviQly(itemcb.DONVI_DIENLUC);

                    foreach (var nguoiNhan in listNguoiNhan)
                    {
                        var user = userdataService.Getbykey(nguoiNhan.USER_ID);
                        if (user.email == null)
                            continue;
                        if (nguoiNhan.TRANGTHAI != 0)
                        {
                            var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == itemcb.MA_YC).OrderByDescending(p => p.STT).FirstOrDefault();
                            var manv_thuc_hien = ttrinhs.MA_NVIEN_NHAN;

                            if (string.IsNullOrEmpty(manv_thuc_hien) || manv_thuc_hien != user.maNVien)
                                continue;
                        }
                        var checktontaiemail = serviceemail.Query.Where(p => p.EMAIL == user.email && p.NOI_DUNG == itemcb.NOIDUNG).FirstOrDefault();
                        if(checktontaiemail != null)
                        {
                            continue;
                        }

                        Email email = new Email();
                        email.MA_DVIQLY = nguoiNhan.MA_DVIQLY;
                        email.MA_DVU = "TA";
                        email.NOI_DUNG = itemcb.NOIDUNG;
                        email.NGAY_TAO = DateTime.Now;
                        email.NGUOI_TAO = "admin";
                        email.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                        email.TINH_TRANG = 1;
                        email.EMAIL = user.email;
                        serviceemail.CreateNew(email);
                    }
                    itemcb.TRANGTHAI_CANHBAO = 2;
                    CBservice1.Update(itemcb);
                }
                CBservice1.CommitChanges();
                serviceemail.CommitChanges();
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = ex.Message;
                log.Error($"Ngày: {DateTime.Now}; Ex: {ex.Message} ");
                return Ok(result);
            }
        }

    }
}
