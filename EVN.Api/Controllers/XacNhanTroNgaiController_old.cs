﻿using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using EVN.Core.Utilities;
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
using System.IO;
using EVN.Core.Models;
using System.Data;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/KhaoSat")]
    public class XacNhanTroNgaiController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(XacNhanTroNgaiController));
        DataProvide_Oracle cnn = new DataProvide_Oracle();
        //[JwtAuthentication]

        public IList<MayCmis> ConvertDataTableToList(DataTable dt2)
        {
            var result = dt2.AsEnumerable().Select(row => new MayCmis
            {
                //MA_DVIQLY = row.Field<string>("MA_DVIQLY "),
                MA_YCAU_KNAI = row.Field<string>("MA_YCAU_KNAI"),

                TEN_KHANG = row.Field<string>("TEN_KHANG"),
                DU_AN_DIEN = row.Field<string>("DU_AN_DIEN"),

                NGAY_HTHANH = row.Field<DateTime>("NGAY_HTHANH"),
                EMAIL = row.Field<string>("EMAIL"),

            }).ToList();

            return result;
        }

        [HttpPost]
        [Route("khachhang/filter")]
        public IHttpActionResult khachhangFilter(XacNhanTroNgaiFilterkhRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total1 = 0;
                int total2 = 0;
                int total = 0;
                DateTime synctime = DateTime.Today;
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanKTService bienBanKTService = IoC.Resolve<IBienBanKTService>();
                IXacNhanTroNgaiService svkhaosat = IoC.Resolve<IXacNhanTroNgaiService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.Filter.fromdate))
                    fromDate = DateTime.ParseExact(request.Filter.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.Filter.todate))
                    toDate = DateTime.ParseExact(request.Filter.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                request.Filter.keyword = !string.IsNullOrWhiteSpace(request.Filter.keyword) ? request.Filter.keyword.Trim() : request.Filter.keyword;
                var listModel = new List<YeuCauNghiemThuData>();
                var HTlist = service.GetbyFilter(request.Filter.maDViQLy, request.Filter.keyword, "", (int)TrangThaiNghiemThu.HoanThanh, fromDate, toDate, pageindex, request.Paginator.pageSize, out total1);
                var HUlist = service.GetbyFilter(request.Filter.maDViQLy, request.Filter.keyword, "", (int)TrangThaiNghiemThu.Huy, fromDate, toDate, pageindex, request.Paginator.pageSize, out total2);

                DataTable dt = new DataTable();

                DataTable dtHU = new DataTable();
                DataTable dtHT = new DataTable();
            
                dtHU = cnn.Get_mayc_cmis_HUY(request.Filter.maDViQLy, fromDate, toDate);
                dtHT = cnn.Get_mayc_cmis_HT(request.Filter.maDViQLy, fromDate, toDate);
                IList<MayCmis> mayCmisListHU = ConvertDataTableToList(dtHU);
                IList<MayCmis> mayCmisListHT = ConvertDataTableToList(dtHT);
                if (request.Filter.trangthai_khaosat == "Chưa khảo sát") //chưa khảo sát
                {
                    if (request.Filter.mucdo_hailong == "1") //Rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis();
                          
                           // IList<MayCmis> mayCmisListHU = ConvertDataTableToList(dtHU);
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    //model.sdt_cmis = sdt_cmis_data;
                                    //model.TroNgai = bbkt.TroNgai;
                                    //model.TrangThaiText = "Hủy";
                                    //model.TrangThai_khaosat = "Chưa khảo sát";
                                    //listModel.Add(model);
                                    listModel.AddRange(HUlist.Select(x => new YeuCauNghiemThuData()
                                    {
                                        MaYeuCau = x.MaYeuCau,
                                        CoQuanChuQuan = x.CoQuanChuQuan,
                                        DuAnDien = x.DuAnDien,
                                        TrangThaiText = "Hủy",
                                        TrangThai_khaosat = "Chưa khảo sát",
                                        DienThoai = x.DienThoai,
                                        Email = x.Email,
                                        sdt_cmis = sdt_cmis_data,
                                }).ToList());

                                    //2
                                    listModel.AddRange(mayCmisListHU.Select(x => new YeuCauNghiemThuData()
                                    {
                                        MaYeuCau = x.MA_YCAU_KNAI,
                                        CoQuanChuQuan = x.TEN_KHANG,
                                        DuAnDien = x.DU_AN_DIEN,
                                        TrangThaiText = "Hủy",
                                        TrangThai_khaosat = "Chưa khảo sát",
                                        DienThoai = x.DTHOAI_YCAU,
                                        Email = x.EMAIL,
                                        sdt_cmis = sdt_cmis_data,


                                    }).ToList());
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "2") //không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "3") //bình thường
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "4") //hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "5") //hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                          //  IList<MayCmis> mayCmisListHU = ConvertDataTableToList(dtHU);
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    //model.sdt_cmis = sdt_cmis_data;
                                    //model.TroNgai = bbkt.TroNgai;
                                    //model.TrangThaiText = "Hủy";
                                    //model.TrangThai_khaosat = "Chưa khảo sát";
                                    //listModel.Add(model);
                                    listModel.AddRange(HUlist.Select(x => new YeuCauNghiemThuData()
                                    {
                                        MaYeuCau = x.MaYeuCau,
                                        CoQuanChuQuan = x.CoQuanChuQuan,
                                        DuAnDien = x.DuAnDien,
                                        TrangThaiText = "Hủy",
                                        TrangThai_khaosat = "Chưa khảo sát",
                                        DienThoai = x.DienThoai,
                                        Email = x.Email,
                                        sdt_cmis = sdt_cmis_data,
                                    }).ToList());

                                    //2
                                    listModel.AddRange(mayCmisListHU.Select(x => new YeuCauNghiemThuData()
                                    {
                                        MaYeuCau = x.MA_YCAU_KNAI,
                                        CoQuanChuQuan = x.TEN_KHANG,
                                        DuAnDien = x.DU_AN_DIEN,
                                        TrangThaiText = "Hủy",
                                        TrangThai_khaosat = "Chưa khảo sát",
                                        DienThoai = x.DTHOAI_YCAU,
                                        Email = x.EMAIL,
                                        sdt_cmis = sdt_cmis_data,


                                    }).ToList());
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    //model.sdt_cmis = sdt_cmis_data;
                                    //model.TroNgai = bbkt.TroNgai;
                                    //model.TrangThaiText = "Hoàn thành";
                                    //model.TrangThai_khaosat = "Chưa khảo sát";
                                    //listModel.Add(model);
                                    listModel.AddRange(HUlist.Select(x => new YeuCauNghiemThuData()
                                    {
                                        MaYeuCau = x.MaYeuCau,
                                        CoQuanChuQuan = x.CoQuanChuQuan,
                                        DuAnDien = x.DuAnDien,
                                        TrangThaiText = "Hủy",
                                        TrangThai_khaosat = "Chưa khảo sát",
                                        DienThoai = x.DienThoai,
                                        Email = x.Email,
                                        sdt_cmis = sdt_cmis_data,
                                    }).ToList());

                                    //2
                                    listModel.AddRange(mayCmisListHT.Select(x => new YeuCauNghiemThuData()
                                    {
                                        MaYeuCau = x.MA_YCAU_KNAI,
                                        CoQuanChuQuan = x.TEN_KHANG,
                                        DuAnDien = x.DU_AN_DIEN,
                                        TrangThaiText = "Hủy",
                                        TrangThai_khaosat = "Chưa khảo sát",
                                        DienThoai = x.DTHOAI_YCAU,
                                        Email = x.EMAIL,
                                        sdt_cmis = sdt_cmis_data,


                                    }).ToList());
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "-1") //all
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý

                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    model.sdt_cmis = sdt_cmis_data;
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh == null || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa khảo sát";
                                    model.sdt_cmis = sdt_cmis_data;
                                    listModel.Add(model);
                                }

                            }
                        }
                    }


                }
                //end

                if (request.Filter.trangthai_khaosat == "Đang khảo sát")   //Đang khảo sát
                {
                    if (request.Filter.mucdo_hailong == "1") //Rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Rất không hài lòng";
                                    listModel.Add(model);
                                }

                            }
                        }
                        if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Rất không hài lòng";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Rất không hài lòng";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Rất không hài lòng";
                                    listModel.Add(model);
                                }

                            }
                        }

                    }
                    //end

                    if (request.Filter.mucdo_hailong == "2") //Không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Không hài lòng";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Không hài lòng";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Không hài lòng";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau) 
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Không hài lòng";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    //end



                    if (request.Filter.mucdo_hailong == "3") //Bình thường
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Bình thường";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Bình thường";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Bình thường";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Bình thường";
                                    listModel.Add(model);
                                }

                            }

                        }
                    }

                    //end



                    if (request.Filter.mucdo_hailong == "4") //Hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Hài lòng";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Hài lòng";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Hài lòng";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Hài lòng";
                                    listModel.Add(model);
                                }


                            }
                        }
                    }

                    //end

                    if (request.Filter.mucdo_hailong == "5") //Rất hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Hài lòng";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Rất hài lòng";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Rất hài lòng";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    model.mucdo_hailong = "Rất hài lòng";
                                    listModel.Add(model);
                                }


                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "-1") //all hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đang khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                    }
                }
                //end
                if (request.Filter.trangthai_khaosat == "Kết thúc khảo sát")   //Kết thúc khảo sát
                {

                    if (request.Filter.mucdo_hailong == "1") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "2") //all
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }



                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "3") //all
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "4") //all
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "5") //all
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }
                    if (request.Filter.mucdo_hailong == "-1") //all
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 6 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                }
                //end

                if (request.Filter.trangthai_khaosat == "Dừng ngang cuộc gọi")   //Dừng ngang cuộc gọi
                {

                    if (request.Filter.mucdo_hailong == "1") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "2") // không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "3") //bình thường
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "4") //hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "5") //rất hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }


                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 2 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Dừng ngang cuộc gọi";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                }
                //end

                else if (request.Filter.trangthai_khaosat == "Chưa gọi được")   //Chưa gọi được
                {
                    if (request.Filter.mucdo_hailong == "-1") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;

                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);

                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "1") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }


                    if (request.Filter.mucdo_hailong == "2") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "3") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "4") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "5") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 1 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chưa gọi được";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }
                }

                //end


                else if (request.Filter.trangthai_khaosat == "Đã gọi thành công")   //Đã gọi thành công
                {
                    if (request.Filter.mucdo_hailong == "-1") //rất không hài lòng
                    {

                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "1") //rất không hài lòng
                    {

                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "2") //rất không hài lòng
                    {

                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "3") //rất không hài lòng
                    {

                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "4") //rất không hài lòng
                    {

                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "5") //rất không hài lòng
                    {

                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI_GOI == 0 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Đã gọi thành công";
                                    listModel.Add(model);
                                }

                            }
                        }
                    }

                }

                //end

                else if (request.Filter.trangthai_khaosat == "Chuyển đơn vị")   //Chuyển đơn vị
                {
                    if (request.Filter.mucdo_hailong == "-1") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }

                    }

                    if (request.Filter.mucdo_hailong == "1") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }

                    }

                    if (request.Filter.mucdo_hailong == "2") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }

                    }

                    if (request.Filter.mucdo_hailong == "3") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }

                    }

                    if (request.Filter.mucdo_hailong == "4") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }

                    }

                    if (request.Filter.mucdo_hailong == "5") //rất không hài lòng
                    {
                        if (request.Filter.trangthai_ycau == "Hoàn thành")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }
                        else if (request.Filter.trangthai_ycau == "Hủy")
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }

                        }
                        else
                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.TRANGTHAI == 3 && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    model.TrangThai_khaosat = "Chuyển đơn vị";
                                    listModel.Add(model);
                                }

                            }
                        }

                    }
                }

                //end
                else if (request.Filter.trangthai_khaosat == "-1")
                {
                    if (request.Filter.trangthai_ycau == "Hoàn thành")
                    {
                        dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                        int rowIndex = 0;
                        foreach (var item in HTlist)
                        {
                    
                            var model = new YeuCauNghiemThuData(item);
                            var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                            var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                            if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                            var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                            var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                            rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                            if (bbkt != null && kskh != null || mayc_cmis_data == item.MaYeuCau)
                            {
                                //model.sdt_cmis = sdt_cmis_data;
                                //model.TroNgai = bbkt.TroNgai;
                                //model.TrangThaiText = "Hoàn thành";
                                listModel.AddRange(HTlist.Select(x => new YeuCauNghiemThuData()
                                {
                                    MaYeuCau = x.MaYeuCau,
                                    CoQuanChuQuan = x.CoQuanChuQuan,
                                    DuAnDien = x.DuAnDien,
                                    TrangThaiText = "Hoàn thành",
                                    TrangThai_khaosat = kskh.TRANGTHAI == 6 ? "Kết thúc khảo sát" :
                                    (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5) ? "Đang khảo sát" : "Chưa khảo sát",
                                    DienThoai = x.DienThoai,
                                    Email = x.Email,
                                    sdt_cmis = sdt_cmis_data,
                                }

                                ).ToList()); 

                                //2
                                listModel.AddRange(mayCmisListHT.Select(x => new YeuCauNghiemThuData()
                                {
                                    MaYeuCau = x.MA_YCAU_KNAI,
                                    CoQuanChuQuan = x.TEN_KHANG,
                                    DuAnDien = x.DU_AN_DIEN,
                                    TrangThaiText = "Hoàn thành",
                                    TrangThai_khaosat = kskh.TRANGTHAI == 6 ? "Kết thúc khảo sát" :
                                    (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5) ? "Đang khảo sát" : "Chưa khảo sát",
                                    DienThoai = x.DTHOAI_YCAU,
                                    Email = x.EMAIL,
                                    sdt_cmis = x.DTHOAI_KH,


                                }).ToList());
                                //if (kskh.TRANGTHAI == 6)
                                //{
                                //    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                //}
                                //if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                //{
                                //    model.TrangThai_khaosat = "Đang khảo sát";
                                //}
                                //listModel.Add(model);

                            }
                            else if (kskh == null)
                            {
                                //model.sdt_cmis = sdt_cmis_data;
                                //model.TrangThaiText = "Hoàn thành";
                                //model.TrangThai_khaosat = "Chưa khảo sát";
                                //listModel.Add(model);
                                listModel.AddRange(HTlist.Select(x => new YeuCauNghiemThuData()
                                {
                                    MaYeuCau = x.MaYeuCau,
                                    CoQuanChuQuan = x.CoQuanChuQuan,
                                    DuAnDien = x.DuAnDien,
                                    TrangThaiText = "Hoàn thành",
                                    TrangThai_khaosat = "Chưa khảo sát",
                                    DienThoai = x.DienThoai,
                                    Email = x.Email,
                                    sdt_cmis = sdt_cmis_data,
                                }

                              ).ToList());

                                //2
                                listModel.AddRange(mayCmisListHT.Select(x => new YeuCauNghiemThuData()
                                {
                                    MaYeuCau = x.MA_YCAU_KNAI,
                                    CoQuanChuQuan = x.TEN_KHANG,
                                    DuAnDien = x.DU_AN_DIEN,
                                    TrangThaiText = "Hoàn thành",
                                    TrangThai_khaosat = "Chưa khảo sát",
                                    DienThoai = x.DTHOAI_YCAU,
                                    Email = x.EMAIL,
                                    sdt_cmis = x.DTHOAI_KH,


                                }).ToList());
                            }


                        }
                    }
                    else if (request.Filter.trangthai_ycau == "Hủy")
                    {
                        dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                        int rowIndex = 0;
                        foreach (var item in HUlist)
                        {
                            var model = new YeuCauNghiemThuData(item);
                            var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                            var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                            if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                            var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                            var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                            rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                            if (bbkt != null && kskh != null || mayc_cmis_data == item.MaYeuCau)
                            {
                                //model.sdt_cmis = sdt_cmis_data;
                                //model.TroNgai = bbkt.TroNgai;
                                //model.TrangThaiText = "Hủy";
                                //if (kskh.TRANGTHAI == 6)
                                //{
                                //    model.TrangThai_khaosat = "Kết thúc khảo sát";
                                //}
                                //if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                //{
                                //    model.TrangThai_khaosat = "Đang khảo sát";
                                //}
                                //listModel.Add(model);
                                listModel.AddRange(HUlist.Select(x => new YeuCauNghiemThuData()
                                {
                                    MaYeuCau = x.MaYeuCau,
                                    CoQuanChuQuan = x.CoQuanChuQuan,
                                    DuAnDien = x.DuAnDien,
                                    TrangThaiText = "Hủy",
                                    TrangThai_khaosat = kskh.TRANGTHAI == 6 ? "Kết thúc khảo sát" :
                                    (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5) ? "Đang khảo sát" : "Chưa khảo sát",
                                    DienThoai = x.DienThoai,
                                    Email = x.Email,
                                    sdt_cmis = sdt_cmis_data,
                                }

                                ).ToList());

                                //2
                                listModel.AddRange(mayCmisListHU.Select(x => new YeuCauNghiemThuData()
                                {
                                    MaYeuCau = x.MA_YCAU_KNAI,
                                    CoQuanChuQuan = x.TEN_KHANG,
                                    DuAnDien = x.DU_AN_DIEN,
                                    TrangThaiText = "Hủy",
                                    TrangThai_khaosat = kskh.TRANGTHAI == 6 ? "Kết thúc khảo sát" :
                                    (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5) ? "Đang khảo sát" : "Chưa khảo sát",
                                    DienThoai = x.DTHOAI_YCAU,
                                    Email = x.EMAIL,
                                    sdt_cmis = x.DTHOAI_KH,


                                }).ToList());
                            }
                            else
                            {
                                //model.sdt_cmis = sdt_cmis_data;
                                //model.TrangThaiText = "Hủy";
                                //model.TrangThai_khaosat = "Chưa khảo sát";
                                //listModel.Add(model);

                                listModel.AddRange(HTlist.Select(x => new YeuCauNghiemThuData()
                                {
                                    MaYeuCau = x.MaYeuCau,
                                    CoQuanChuQuan = x.CoQuanChuQuan,
                                    DuAnDien = x.DuAnDien,
                                    TrangThaiText = "Hủy",
                                    TrangThai_khaosat = "Chưa khảo sát",
                                    DienThoai = x.DienThoai,
                                    Email = x.Email,
                                    sdt_cmis = sdt_cmis_data,
                                }

                              ).ToList());

                                //2
                                listModel.AddRange(mayCmisListHT.Select(x => new YeuCauNghiemThuData()
                                {
                                    MaYeuCau = x.MA_YCAU_KNAI,
                                    CoQuanChuQuan = x.TEN_KHANG,
                                    DuAnDien = x.DU_AN_DIEN,
                                    TrangThaiText = "Hủy",
                                    TrangThai_khaosat = "Chưa khảo sát",
                                    DienThoai = x.DTHOAI_YCAU,
                                    Email = x.EMAIL,
                                    sdt_cmis = x.DTHOAI_KH,


                                }).ToList());
                            }

                        }

                    }

                    if (request.Filter.mucdo_hailong == "-1")
                    {
                        if (request.Filter.trangthai_ycau == "-1")
                        {
                            foreach (var item in HUlist)
                            {
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);

                                if (kskh != null)
                                {
                                    if (kskh.TRANGTHAI == 6 || (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5))
                                    {
                                        var filteredList = mayCmisListHU.Where(x => !addedMaYeuCau.Contains(x.MA_YCAU_KNAI)).ToList();
                                        if (kskh.MA_YCAU != null)
                                        {
                                            foreach (var x in filteredList)
                                            {
                                                addedMaYeuCau.Add(x.MA_YCAU_KNAI); // Add to HashSet to track duplicates
                                                listModel.Add(new YeuCauNghiemThuData()
                                                {
                                                    MaYeuCau = x.MA_YCAU_KNAI,
                                                    CoQuanChuQuan = x.TEN_KHANG,
                                                    DuAnDien = x.DU_AN_DIEN,
                                                    TrangThaiText = "Hủy",
                                                    TrangThai_khaosat = kskh.TRANGTHAI == 6 ? "Kết thúc khảo sát" : "Đang khảo sát",
                                                    DienThoai = x.DTHOAI_YCAU,
                                                    Email = x.EMAIL,
                                                    sdt_cmis = x.DTHOAI_KH,
                                                });
                                            }
                                        }


                                    }

                                }
                                else
                                {
                                    var filteredList = mayCmisListHU.Where(x => !addedMaYeuCau.Contains(x.MA_YCAU_KNAI)).ToList();
                                    foreach (var x in filteredList)
                                    {
                                        addedMaYeuCau.Add(x.MA_YCAU_KNAI); // Add to HashSet to track duplicates
                                        listModel.Add(new YeuCauNghiemThuData()
                                        {
                                            MaYeuCau = x.MA_YCAU_KNAI,
                                            CoQuanChuQuan = x.TEN_KHANG,
                                            DuAnDien = x.DU_AN_DIEN,
                                            TrangThaiText = "Hủy",
                                            TrangThai_khaosat = "Chưa khảo sát",
                                            DienThoai = x.DTHOAI_YCAU,
                                            Email = x.EMAIL,
                                            sdt_cmis = x.DTHOAI_KH,
                                        });
                                    }
                                }
                            }

                            foreach (var item in mayCmisListHT)
                            {
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MA_YCAU_KNAI);

                                if (kskh != null)
                                {
                                    if (kskh.TRANGTHAI == 6 || (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5))
                                    {
                                        var filteredList = mayCmisListHT
                                       .Where(x => !addedMaYeuCau.Contains(x.MA_YCAU_KNAI) && (kskh.MA_YCAU == x.MA_YCAU_KNAI)).ToList();

                                        foreach (var x in filteredList)
                                        {
                                            addedMaYeuCau.Add(x.MA_YCAU_KNAI); // Add to HashSet to track duplicates
                                            listModel.Add(new YeuCauNghiemThuData()
                                            {
                                                MaYeuCau = x.MA_YCAU_KNAI,
                                                CoQuanChuQuan = x.TEN_KHANG,
                                                DuAnDien = x.DU_AN_DIEN,
                                                TrangThaiText = "Hoàn thành",
                                                TrangThai_khaosat = kskh.TRANGTHAI == 6 ? "Kết thúc khảo sát" : "Đang khảo sát",
                                                DienThoai = x.DTHOAI_YCAU,
                                                Email = x.EMAIL,
                                                sdt_cmis = x.DTHOAI_KH,
                                            });

                                        }
                                    }

                                }
                                else
                                {
                                    var filteredList = mayCmisListHT.Where(x => !addedMaYeuCau.Contains(x.MA_YCAU_KNAI)).ToList();
                                    foreach (var x in filteredList)
                                    {
                                        addedMaYeuCau.Add(x.MA_YCAU_KNAI); // Add to HashSet to track duplicates
                                        listModel.Add(new YeuCauNghiemThuData()
                                        {
                                            MaYeuCau = x.MA_YCAU_KNAI,
                                            CoQuanChuQuan = x.TEN_KHANG,
                                            DuAnDien = x.DU_AN_DIEN,
                                            TrangThaiText = "Hoàn thành",
                                            TrangThai_khaosat = "Chưa khảo sát",
                                            DienThoai = x.DTHOAI_YCAU,
                                            Email = x.EMAIL,
                                            sdt_cmis = x.DTHOAI_KH,
                                        });
                                    }

                                }
                            }
                        }
                    }





                    if (request.Filter.mucdo_hailong == "1") //all hài lòng
                    {

                        if (request.Filter.trangthai_ycau == "-1")

                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    if (kskh.TRANGTHAI == 6)
                                    {
                                        model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    }
                                    if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                    {
                                        model.TrangThai_khaosat = "Đang khảo sát";
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                listModel.Add(model);
                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.DGHL_CAPDIEN == 1 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    if (kskh.TRANGTHAI == 6)
                                    {
                                        model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    }
                                    if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                    {
                                        model.TrangThai_khaosat = "Đang khảo sát";
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                listModel.Add(model);
                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "2") //all hài lòng
                    {

                        if (request.Filter.trangthai_ycau == "-1")

                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    if (kskh.TRANGTHAI == 6)
                                    {
                                        model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    }
                                    if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                    {
                                        model.TrangThai_khaosat = "Đang khảo sát";
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                listModel.Add(model);
                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.DGHL_CAPDIEN == 2 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    if (kskh.TRANGTHAI == 6)
                                    {
                                        model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    }
                                    if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                    {
                                        model.TrangThai_khaosat = "Đang khảo sát";
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                listModel.Add(model);
                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "3") //all hài lòng
                    {

                        if (request.Filter.trangthai_ycau == "-1")

                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    if (kskh.TRANGTHAI == 6)
                                    {
                                        model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    }
                                    if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                    {
                                        model.TrangThai_khaosat = "Đang khảo sát";
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                listModel.Add(model);
                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.DGHL_CAPDIEN == 3 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    if (kskh.TRANGTHAI == 6)
                                    {
                                        model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    }
                                    if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                    {
                                        model.TrangThai_khaosat = "Đang khảo sát";
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                listModel.Add(model);
                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "4") //all hài lòng
                    {

                        if (request.Filter.trangthai_ycau == "-1")

                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    if (kskh.TRANGTHAI == 6)
                                    {
                                        model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    }
                                    if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                    {
                                        model.TrangThai_khaosat = "Đang khảo sát";
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                listModel.Add(model);
                            }
                            foreach (var item in HTlist)
                            {
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.DGHL_CAPDIEN == 4 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    if (kskh.TRANGTHAI == 6)
                                    {
                                        model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    }
                                    if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                    {
                                        model.TrangThai_khaosat = "Đang khảo sát";
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                listModel.Add(model);
                            }
                        }
                    }

                    if (request.Filter.mucdo_hailong == "5") //all hài lòng
                    {

                        if (request.Filter.trangthai_ycau == "-1")

                        {
                            dt = cnn.Get_sdt_cmis(); // Lấy toàn bộ dữ liệu một lần
                            int rowIndex = 0;
                            foreach (var item in HUlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);
                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {
                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hủy";
                                    if (kskh.TRANGTHAI == 6)
                                    {
                                        model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    }
                                    if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                    {
                                        model.TrangThai_khaosat = "Đang khảo sát";
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                listModel.Add(model);
                            }
                            foreach (var item in HTlist)
                            {
                                if (rowIndex >= dt.Rows.Count) break; // Kiểm tra xem có vượt quá số dòng dữ liệu hay không

                                var sdt_cmis_data = dt.Rows[rowIndex]["DTHOAI_DD"].ToString();
                                var mayc_cmis_data = dt.Rows[rowIndex]["ma_ycau_knai"].ToString();
                                rowIndex++; // Tăng chỉ số dòng sau mỗi lần xử lý
                                var model = new YeuCauNghiemThuData(item);
                                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                                var kskh = svkhaosat.FilterByMaYeuCau(item.MaYeuCau);

                                if (bbkt != null && kskh != null && bbkt.MaYeuCau == kskh.MA_YCAU && kskh.DGHL_CAPDIEN == 5 || mayc_cmis_data == item.MaYeuCau)
                                {

                                    model.sdt_cmis = sdt_cmis_data;
                                    model.TroNgai = bbkt.TroNgai;
                                    model.TrangThaiText = "Hoàn thành";
                                    if (kskh.TRANGTHAI == 6)
                                    {
                                        model.TrangThai_khaosat = "Kết thúc khảo sát";
                                    }
                                    if (kskh.TRANGTHAI >= 0 && kskh.TRANGTHAI <= 5)
                                    {
                                        model.TrangThai_khaosat = "Đang khảo sát";
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                listModel.Add(model);
                            }
                        }
                    }
                }


                total = total1 + total2;
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
                //var ThongTinCanhBao = servicecanhbao.Getbyid(khaosat.CANHBAO_ID);
                //var ThongTinYeuCau = serviceyeucau.GetbyMaYCau(ThongTinCanhBao.MA_YC);
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




                var oj1 = new
                {
                    //maYeuCau = ThongTinYeuCau.MaYeuCau,
                    //KETQUA = khaosat.KETQUA,
                    // trangThaiYeuCau = textTrangThaiYeuCau,
                    // trangThaiKhaoSat = textTrangThaiKhaoSat,
                    // tenKhachHang = ThongTinYeuCau.TenKhachHang,
                    // nguoiKhaoSat = HttpContext.Current.User.Identity.Name,
                    // thoiGianKhaoSat = khaosat.THOIGIAN_KHAOSAT,
                    // CANHBAO_ID = ThongTinCanhBao.idCanhBao,
                    // NOIDUNG_CAUHOI = khaosat.NOIDUNG_CAUHOI,
                    // PHANHOI_KH = khaosat.PHANHOI_KH,
                    // PHANHOI_DV = khaosat.PHANHOI_DV

                };
                result.data = khaosat;
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
        public async Task<IHttpActionResult> Post()
        {
            ResponseFileResult result = new ResponseFileResult();
            var httpRequest = HttpContext.Current.Request;
            string data = httpRequest.Form["data"];
            XacNhanTroNgaikhaosatadd model = JsonConvert.DeserializeObject<XacNhanTroNgaikhaosatadd>(data);
            ILogKhaoSatService LogKhaoSatservice = IoC.Resolve<ILogKhaoSatService>();
            IYCauNghiemThuService NTservice = IoC.Resolve<IYCauNghiemThuService>();
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                IUserdataService userdataService = IoC.Resolve<IUserdataService>();
                ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();

                YCauNghiemThu YCNT = NTservice.GetbyMaYCau(model.MA_YCAU);
                var canhbao = canhBaoService.GetByMaYeuCau(model.MA_YCAU);
                var item = new XacNhanTroNgai();

                string mucdichsd = "";
                if (YCNT.DienSinhHoat)
                {
                    mucdichsd = "Sinh hoạt";
                }
                else { mucdichsd = "Ngoài sinh hoạt"; }
                item.MA_DVI = YCNT.MaDViQLy;
                item.MA_YCAU = model.MA_YCAU;
                item.MA_KH = YCNT.MaKHang;
                item.TEN_KH = YCNT.CoQuanChuQuan;
                item.DIA_CHI = YCNT.DiaChiDungDien;
                item.DIEN_THOAI = YCNT.DienThoai;
                item.MUCDICH_SD_DIEN = mucdichsd;
                item.SO_NGAY_CT = model.SO_NGAY_CT;
                item.SO_NGAY_TH_ND = model.SO_NGAY_TH_ND;
                item.TRANGTHAI_GQ = model.TRANGTHAI_GQ;
                item.TONG_CONGSUAT_CD = model.TONG_CONGSUAT_CD;
                item.DGCD_KH_PHANHOI = model.DGCD_KH_PHANHOI;
                item.DGCD_TH_CHUONGTRINH = model.DGCD_TH_CHUONGTRINH;
                item.DGCD_TH_DANGKY = model.DGCD_TH_DANGKY;
                item.CHENH_LECH = model.DGCD_TH_DANGKY - model.DGCD_KH_PHANHOI;
                item.DGYC_DK_DEDANG = model.DGYC_DK_DEDANG;
                item.DGYC_XACNHAN_NCHONG_KTHOI = model.DGYC_XACNHAN_NCHONG_KTHOI;
                item.DGYC_THAIDO_CNGHIEP = model.DGYC_THAIDO_CNGHIEP;
                item.DGKS_TDO_KSAT = model.DGKS_TDO_KSAT;
                item.DGKS_MINH_BACH = model.DGKS_MINH_BACH;
                item.DGKS_CHU_DAO = model.DGKS_CHU_DAO;
                item.DGNT_THUAN_TIEN = model.DGNT_THUAN_TIEN;
                item.DGNT_MINH_BACH = model.DGNT_MINH_BACH;
                item.DGNT_CHU_DAO = model.DGNT_CHU_DAO;
                item.KSAT_CHI_PHI = model.KSAT_CHI_PHI;
                item.DGHL_CAPDIEN = model.DGHL_CAPDIEN;
                item.TRANGTHAI_GOI = model.TRANGTHAI_GOI;
                item.NGAY = DateTime.Now;
                item.NGUOI_KSAT = model.NGUOI_KSAT;
                item.Y_KIEN_KH = model.Y_KIEN_KH;
                item.NOIDUNG = model.NOIDUNG;
                item.PHAN_HOI = model.PHAN_HOI;
                item.GHI_CHU = model.GHI_CHU;
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string fileFolder = $"//GSCD//";
                    string fileName = $"{model.CANHBAO_ID}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(postedFile.FileName)}";
                    string imagePath = FileUtils.SaveFile(postedFile, fileFolder, fileName);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return BadRequest();
                    }
                    item.FILE_DINHKEM = $"/{fileFolder}/{fileName}";
                }

                item.HANGMUC_KHAOSAT = model.HANGMUC_KHAOSAT;
                item.TRANGTHAI = 1;
                if (model.TRANGTHAI_GOI == 0 && model.DGHL_CAPDIEN != null)
                {
                    service.CreateNew(item);
                    service.CommitChanges();

                    var logKS = new LogKhaoSat();
                    logKS.KHAOSAT_ID = item.ID;
                    logKS.DATA_MOI = JsonConvert.SerializeObject(item);
                    logKS.TRANGTHAI = 1;
                    logKS.THOIGIAN = DateTime.Now;
                    logKS.NGUOITHUCHIEN = model.NGUOI_KSAT;
                    LogKhaoSatservice.CreateNew(logKS);
                    LogKhaoSatservice.CommitChanges();
                    result.success = true;
                }
                if (model.TRANGTHAI_GOI == 1 && model.DGHL_CAPDIEN == null)
                {
                    service.CreateNew(item);
                    service.CommitChanges();

                    var logKS = new LogKhaoSat();
                    logKS.KHAOSAT_ID = item.ID;
                    logKS.DATA_MOI = JsonConvert.SerializeObject(item);
                    logKS.TRANGTHAI = 1;
                    logKS.THOIGIAN = DateTime.Now;
                    logKS.NGUOITHUCHIEN = model.NGUOI_KSAT;
                    LogKhaoSatservice.CreateNew(logKS);
                    LogKhaoSatservice.CommitChanges();
                    result.success = true;
                }
                if (model.TRANGTHAI_GOI == 2 && model.DGHL_CAPDIEN == null)
                {
                    service.CreateNew(item);
                    service.CommitChanges();

                    var logKS = new LogKhaoSat();
                    logKS.KHAOSAT_ID = item.ID;
                    logKS.DATA_MOI = JsonConvert.SerializeObject(item);
                    logKS.TRANGTHAI = 1;
                    logKS.THOIGIAN = DateTime.Now;
                    logKS.NGUOITHUCHIEN = model.NGUOI_KSAT;
                    LogKhaoSatservice.CreateNew(logKS);
                    LogKhaoSatservice.CommitChanges();
                    result.success = true;
                }
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
        [Route("edit")]
        [HttpPost]
        public IHttpActionResult UpdateById()
        {
            ResponseFileResult result = new ResponseFileResult();
            var httpRequest = HttpContext.Current.Request;
            string data = httpRequest.Form["data"];
            XacNhanTroNgaikhaosatadd model = JsonConvert.DeserializeObject<XacNhanTroNgaikhaosatadd>(data);
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                ILogKhaoSatService LogKhaoSatservice = IoC.Resolve<ILogKhaoSatService>();
                ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                var item = new XacNhanTroNgai();

                //sửa nội dung khảo sát
                var khaosat = service.GetKhaoSat(model.ID);
                var canhbao = canhBaoService.GetByMaYeuCau(khaosat.MA_YCAU);
                var datacu = JsonConvert.SerializeObject(khaosat);
                khaosat.MA_YCAU = model.MA_YCAU;
                khaosat.NGAY_TIEPNHAN = model.NGAY_TIEPNHAN;
                khaosat.NGAY_HOANTHANH = model.NGAY_HOANTHANH;
                khaosat.TRANGTHAI_GQ = model.TRANGTHAI_GQ;
                khaosat.TONG_CONGSUAT_CD = model.TONG_CONGSUAT_CD;
                khaosat.DGCD_KH_PHANHOI = model.DGCD_KH_PHANHOI;
                khaosat.DGYC_DK_DEDANG = model.DGYC_DK_DEDANG;
                khaosat.DGYC_XACNHAN_NCHONG_KTHOI = model.DGYC_XACNHAN_NCHONG_KTHOI;
                khaosat.DGYC_THAIDO_CNGHIEP = model.DGYC_THAIDO_CNGHIEP;
                khaosat.DGKS_TDO_KSAT = model.DGKS_TDO_KSAT;
                khaosat.DGKS_MINH_BACH = model.DGKS_MINH_BACH;
                khaosat.DGKS_CHU_DAO = model.DGKS_CHU_DAO;
                khaosat.DGNT_THUAN_TIEN = model.DGNT_THUAN_TIEN;
                khaosat.DGNT_MINH_BACH = model.DGNT_MINH_BACH;
                khaosat.DGNT_CHU_DAO = model.DGNT_CHU_DAO;
                khaosat.KSAT_CHI_PHI = model.KSAT_CHI_PHI;
                khaosat.DGHL_CAPDIEN = model.DGHL_CAPDIEN;
                khaosat.TRANGTHAI_GOI = model.TRANGTHAI_GOI;
                khaosat.NGAY = DateTime.Now;
                khaosat.Y_KIEN_KH = model.Y_KIEN_KH;
                khaosat.NOIDUNG = model.NOIDUNG;
                khaosat.PHAN_HOI = model.PHAN_HOI;
                khaosat.GHI_CHU = model.GHI_CHU;
                //khaosat.DGCD_TH_CHUONGTRINH = (khaosat.NGAY - canhbao.THOIGIANGUI).Hours;
                //khaosat.DGCD_TH_DANGKY = (DateTime.Now - canhbao.THOIGIANGUI).Hours;

                if (string.IsNullOrEmpty(model.PHAN_HOI))
                {
                    khaosat.TRANGTHAI = 3;
                }
                else
                {
                    khaosat.TRANGTHAI = 5;
                }
                service.CommitChanges();

                var logKS = new LogKhaoSat();
                logKS.KHAOSAT_ID = khaosat.ID;
                logKS.DATA_MOI = JsonConvert.SerializeObject(khaosat);
                logKS.DATA_CU = datacu;
                logKS.TRANGTHAI = khaosat.TRANGTHAI;
                logKS.THOIGIAN = DateTime.Now;
                logKS.NGUOITHUCHIEN = khaosat.NGUOI_KSAT;
                LogKhaoSatservice.CreateNew(logKS);
                LogKhaoSatservice.CommitChanges();
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
        public IHttpActionResult UpdateById([FromUri] int id, [FromBody] PhanhoiTraodoiRequest model)
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
                IXacNhanTroNgaiService xacMinhTroNgaiService = IoC.Resolve<IXacNhanTroNgaiService>();
                ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                IYCauNghiemThuService NTservice = IoC.Resolve<IYCauNghiemThuService>();

                //lọc ra các thông tin liên quan đến khảo sát
                YCauNghiemThu YCNT = NTservice.GetbyMaYCau(request.IdYeuCau);
                var listKhaoSat = xacMinhTroNgaiService.FilterByCanhBaoIDAndTrangThai2(request.IdYeuCau, request.TrangThaiKhaoSat, request.mucdo_hailong);

                //lọc ra tên khác hàng, trạng thái yêu cầu ứng với mã yêu cầu

                //tạo ra response API
                var obj = new
                {
                    YCNT.MaYeuCau,
                    TenKhachHang = YCNT.CoQuanChuQuan,
                    DienThoai = YCNT.DienThoai,
                    Email = YCNT.Email,
                    DanhSachKhaoSat = listKhaoSat,
                    DONVI_DIENLUC = YCNT.MaDViQLy

                };
                result.data = obj;
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

        //(GET) /Filterngay
        [HttpPost]
        [Route("Filterngay")]
        public async Task<IHttpActionResult> Filterngay(FilterKhaoSatByCanhBaoRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IXacNhanTroNgaiService xacMinhTroNgaiService = IoC.Resolve<IXacNhanTroNgaiService>();
                ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                IYCauNghiemThuService NTservice = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService DVTIENTRINH = IoC.Resolve<IDvTienTrinhService>();
                //lọc ra các thông tin liên quan đến khảo sát
                YCauNghiemThu YCNT = NTservice.GetbyMaYCau(request.IdYeuCau);
                var listKhaoSat = xacMinhTroNgaiService.FilterByCanhBaoIDAndTrangThai(request.IdYeuCau);
                //lọc ra tên khác hàng, trạng thái yêu cầu ứng với mã yêu cầu

                //lấy mã ycau
                var dvtientrinh = DVTIENTRINH.FilterByMaYeuCau(YCNT.MaYeuCau);
                var tientrinh = DVTIENTRINH.myeutop1(YCNT.MaYeuCau);
                var khaosat = xacMinhTroNgaiService.FilterByMaYeuCau(YCNT.MaYeuCau);
                var checkTonTai1 = await xacMinhTroNgaiService.CheckExits(YCNT.MaYeuCau);
                var tientrinhend = DVTIENTRINH.myeutopend(YCNT.MaYeuCau);
                var item = new XacNhanTroNgai();
                if (!checkTonTai1)
                {
                    //tạo ra response API
                    var obj = new
                    {
                        DGCD_TH_CHUONGTRINH = (int)(tientrinhend.NGAY_TAO - tientrinh.NGAY_TAO).Days,
                        DGCD_TH_DANGKY = (int)(DateTime.Now - tientrinh.NGAY_TAO).Days,
                        TEN_KH = YCNT.CoQuanChuQuan,
                        DIA_CHI = YCNT.DiaChi,
                        SDT = YCNT.DienThoai

                    };
                    result.data = obj;
                    result.success = true;
                }
                else
                {
                    result.success = false;
                    result.message = "Mã yêu cầu đã khảo sát";
                }
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
        //[Route("log/filter")]
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
                    var listKhaoSat = xacMinhTroNgaiService.FilterByCanhBaoIDAndTrangThai("tet");
                    var listKhaoSatFilter = new List<object>();
                    foreach (var khaosat in listKhaoSat)
                    {
                        //1 listKhaoSatFilter.Add(new { khaosat.ID, khaosat.NOIDUNG_CAUHOI });
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

        [HttpPost]
        //[Route("filter")]
        [Route("log/filter")]
        public IHttpActionResult KhaoSatLogFilter(FilterLogKhaoSatRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;

                ILogKhaoSatService logKhaoSatService = IoC.Resolve<ILogKhaoSatService>();

                //lấy danh sách log khảo sát
                var listLog = logKhaoSatService.Filter(request.Filter.fromdate, request.Filter.todate, request.Filter.IdKhaoSat, pageindex, request.Paginator.pageSize, out total);

                if (total == 0)
                {
                    result.message = "Không có dữ liệu";
                }

                result.total = total;
                result.data = listLog;
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

        [HttpGet]
        [Route("updateStatus/{ID}/{Status}")]
        public IHttpActionResult updateStatus([FromUri] int ID, [FromUri] int Status)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                //ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                ILogKhaoSatService LogKhaoSatservice = IoC.Resolve<ILogKhaoSatService>();
                var item = new XacNhanTroNgai();

                //sửa nội dung khảo sát
                var khaosat = service.GetKhaoSat(ID);
                var datacu = JsonConvert.SerializeObject(khaosat);
                khaosat.TRANGTHAI = Status;
                service.Update(khaosat);
                service.CommitChanges();

                var logKS = new LogKhaoSat();
                logKS.KHAOSAT_ID = khaosat.ID;
                logKS.DATA_CU = datacu;
                logKS.DATA_MOI = JsonConvert.SerializeObject(khaosat);
                logKS.TRANGTHAI = Status;
                logKS.THOIGIAN = DateTime.Now;
                logKS.NGUOITHUCHIEN = khaosat.NGUOI_KSAT;
                LogKhaoSatservice.CreateNew(logKS);
                LogKhaoSatservice.CommitChanges();
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