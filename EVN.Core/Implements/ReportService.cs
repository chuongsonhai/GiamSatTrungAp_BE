﻿using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
using EVN.Core.Utilities;
using FX.Core;
using FX.Data;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EVN.Core.Implements
{
    public class ReportService : BaseService<CongVanYeuCau, int>, IReportService
    {
        private ILog log = LogManager.GetLogger(typeof(ReportService));
        public ReportService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<ReportData> ListbyQuater(string maDViQLy, int quater, int year, int pageindex, int pagesize, out int total)
        {
            DateTime denNgay = DateTime.Now;
            DateTime tuNgay = CommonUtils.GetDate(year, quater, out denNgay);

            ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
            IBienBanKSService bbkssrv = IoC.Resolve<IBienBanKSService>();
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IYCauNghiemThuService ntservice = IoC.Resolve<IYCauNghiemThuService>();
            IBienBanNTService bbntsrv = IoC.Resolve<IBienBanNTService>();
            ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
            var cskhcfg = cfgservice.GetbyCode("TTRINH_SYNC");
            if (cskhcfg != null && cskhcfg.Value == "1")
                ntservice.Sync();

            IList<SQLParam> sqlParams = new List<SQLParam>();
            IList<ReportData> result = new List<ReportData>();

            var query = service.Query.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet < denNgay.AddDays(1) && p.TrangThai >= TrangThaiCongVan.BienBanKS);
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);

            total = query.Count();
            query = query.OrderByDescending(p => p.MaYeuCau);
            var yeucaus = new List<CongVanYeuCau>();
            //if (pagesize > 0)
            //    yeucaus = query.Skip(pageindex * pagesize).Take(pagesize).ToList();
            //else
            yeucaus = query.ToList();

            string[] maYCaus = yeucaus.Select(p => p.MaYeuCau).ToArray();

            //var listBBKS = bbkssrv.Query.Where(p => maYCaus.Contains(p.MaYeuCau)).ToList();
            //var listTTrinh = ttrinhsrv.Query.Where(p => maYCaus.Contains(p.MA_YCAU_KNAI)).ToList();
            //var listYCNT = ntservice.Query.Where(p => maYCaus.Contains(p.MaYeuCau)).ToList();
            //var listBBNT = bbntsrv.Query.Where(p => maYCaus.Contains(p.MaYeuCau)).ToList();

            foreach (var item in yeucaus)
            {
                ReportData data = new ReportData();
                data.TEN_NGUOIYCAU = !string.IsNullOrWhiteSpace(item.CoQuanChuQuan) ? item.CoQuanChuQuan : item.NguoiYeuCau;
                data.MA_YCAU_KNAI = item.MaYeuCau;
                data.TEN_DVIQLY = item.BenNhan;
                data.MA_DVIQLY = item.MaDViQLy;

                if (item.TrangThai == TrangThaiCongVan.BienBanKS)
                {
                    data.TRANGTHAIHOSO = "Biên bản khảo sát";
                }
                if (item.TrangThai == TrangThaiCongVan.DuThaoTTDN)
                {
                    data.TRANGTHAIHOSO = "Dự thảo thỏa thuận";
                }
                if (item.TrangThai == TrangThaiCongVan.KHKy)
                {
                    data.TRANGTHAIHOSO = "Gửi khách hàng";
                }
                if (item.TrangThai == TrangThaiCongVan.DuChuKy)
                {
                    data.TRANGTHAIHOSO = "Ký số thỏa thuận";
                }
                if (item.TrangThai == TrangThaiCongVan.HoanThanh)
                {
                    data.TRANGTHAIHOSO = "Hoàn thành TTDN";
                }
                if (item.TrangThai == TrangThaiCongVan.ChuyenTiep)
                {
                    data.TRANGTHAIHOSO = "Chuyển tiếp";
                }
                if (item.TrangThai == TrangThaiCongVan.Huy)
                {
                    data.TRANGTHAIHOSO = "Bị trả lại";
                }

                var ycnt = ntservice.GetbyMaYCau(item.MaYeuCau);
                if (ycnt != null)
                {
                    if (ycnt.TrangThai == TrangThaiNghiemThu.MoiTao)
                    {
                        data.TRANGTHAIHOSO = "Chưa tiếp nhận YCKT và NT";
                    }
                    if (ycnt.TrangThai == TrangThaiNghiemThu.TiepNhan)
                    {
                        data.TRANGTHAIHOSO = "Tiếp nhận YCKT và NT";
                    }
                    if (ycnt.TrangThai == TrangThaiNghiemThu.PhanCongKT)
                    {
                        data.TRANGTHAIHOSO = "Phân công kiểm tra";
                    }
                    if (ycnt.TrangThai == TrangThaiNghiemThu.GhiNhanKT)
                    {
                        data.TRANGTHAIHOSO = "Kết quả kiểm tra";
                    }
                    if (ycnt.TrangThai == TrangThaiNghiemThu.BienBanKT)
                    {
                        data.TRANGTHAIHOSO = "Biên bản kiểm tra";
                    }
                    if (ycnt.TrangThai == TrangThaiNghiemThu.DuThaoHD)
                    {
                        data.TRANGTHAIHOSO = "Dự thảo hợp đồng";
                    }
                    if (ycnt.TrangThai == TrangThaiNghiemThu.PhanCongTC)
                    {
                        data.TRANGTHAIHOSO = "Phân công thi công";
                    }
                    if (ycnt.TrangThai == TrangThaiNghiemThu.KetQuaTC)
                    {
                        data.TRANGTHAIHOSO = "Kết quả treo tháo";
                    }
                    if (ycnt.TrangThai == TrangThaiNghiemThu.BienBanTC)
                    {
                        data.TRANGTHAIHOSO = "Biên bản treo tháo";
                    }
                    if (ycnt.TrangThai == TrangThaiNghiemThu.NghiemThu)
                    {
                        data.TRANGTHAIHOSO = "Biên bản nghiệm thu";
                    }
                    if (ycnt.TrangThai == TrangThaiNghiemThu.HoanThanh)
                    {
                        data.TRANGTHAIHOSO = "Hoàn thành";
                    }
                    if (ycnt.TrangThai == TrangThaiNghiemThu.Huy)
                    {
                        data.TRANGTHAIHOSO = "Bị trả lại";
                    }
                }



                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                if (ttrinhs.Count() == 0) continue;
                var ttrinhTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                var ttrinhPK = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                var ttrinhKS = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                var ttrinhCH5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                var ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");
                if (ttrinhBDN == null)
                {
                    ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                }

                var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                var ttrinhTVB = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                var ttrinhKTR = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                var ttrinhMNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "MNT");
                var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");

                //Thời gian tiếp nhận
                if (ttrinhTN == null || !ttrinhTN.NGAY_KTHUC.HasValue) continue;

                int songayTN = CommonUtils.TotalDate(ttrinhTN.NGAY_BDAU.Date, ttrinhTN.NGAY_KTHUC.Value.Date);
                data.NGAY_TNHAN = data.NGAY_TNHANYCAU = ttrinhTN.NGAY_BDAU.ToString("dd/MM/yyyy");
                data.TGIAN_TNHAN = songayTN;
                data.NGAY_CHUYENKS = ttrinhTN.NGAY_KTHUC.Value.ToString("dd/MM/yyyy");
                data.TONG_TGIAN = songayTN;
                //Thời gian khảo sát
                if (ttrinhKS != null && ttrinhKS.NGAY_KTHUC.HasValue)
                {
                    if (ttrinhCH5 != null && ttrinhCH5.NGAY_KTHUC.HasValue)
                    {
                        int songayKS = CommonUtils.TotalDate(ttrinhKS.NGAY_BDAU.Date, ttrinhCH5.NGAY_BDAU.Date);
                        if (ttrinhKS.NGAY_BDAU.Date == ttrinhTN.NGAY_KTHUC.Value.Date)
                            songayKS -= 1;
                        data.NGAY_KSAT = ttrinhKS.NGAY_BDAU.ToString("dd/MM/yyyy");
                        data.TGIAN_KSAT = songayKS;
                        data.TONG_TGIAN += songayKS;
                    }
                }

                if (ttrinhBDN != null && ttrinhBDN.NGAY_KTHUC.HasValue)
                {
                    int songayBDN = CommonUtils.TotalDate(ttrinhBDN.NGAY_BDAU.Date, ttrinhBDN.NGAY_KTHUC.Value.Date);
                    data.NGAY_BDN = ttrinhBDN.NGAY_BDAU.ToString("dd/MM/yyyy");

                }

                if (ttrinhDDN != null && ttrinhDDN.NGAY_KTHUC.HasValue)
                {
                    int songayTTDN = CommonUtils.TotalDate(ttrinhDDN.NGAY_BDAU.Date, ttrinhDDN.NGAY_KTHUC.Value.Date);
                    if (ttrinhCH5 != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhCH5.NGAY_KTHUC.Value.Date)
                        songayTTDN -= 1;
                    data.TONG_TGIAN += songayTTDN;
                    data.NGAY_DT_TDN = ttrinhDDN.NGAY_BDAU.ToString("dd/MM/yyyy");
                    data.NGAY_TTX_TDN = ttrinhDDN.NGAY_KTHUC.Value.ToString("dd/MM/yyyy");
                    data.TGIAN_DNOI = songayTTDN;
                }

                //Thời gian khách hàng thi công
                if (ttrinhPC != null && ttrinhPC.NGAY_KTHUC.HasValue)
                {
                    if (ttrinhBTT != null && ttrinhBTT.NGAY_KTHUC.HasValue)
                    {
                        int songayTC = CommonUtils.TotalDate(ttrinhPC.NGAY_BDAU.Date, ttrinhBTT.NGAY_BDAU.Date);
                        data.TONG_TGIAN += songayTC;
                        data.NGAY_TH_TVB = ttrinhPC.NGAY_BDAU.ToString("dd/MM/yyyy");
                        data.NGAY_NV_TVB = ttrinhPC.NGAY_BDAU.ToString("dd/MM/yyyy");
                    }
                }

                //Thời gian ký hợp đồng, nghiệm thu
                if (ttrinhDHD != null && ttrinhDHD.NGAY_KTHUC.HasValue)
                {
                    if (ttrinhNT != null && ttrinhNT.NGAY_KTHUC.HasValue)
                    {
                        DateTime fromTime = ttrinhDHD.NGAY_BDAU.Date;
                        if (ttrinhBTT != null && fromTime < ttrinhBTT.NGAY_KTHUC.Value.Date)
                            fromTime = ttrinhBTT.NGAY_KTHUC.Value.Date;

                        int songayNT = CommonUtils.TotalDate(fromTime, ttrinhNT.NGAY_BDAU.Date);
                        if (ttrinhBTT != null && fromTime == ttrinhBTT.NGAY_KTHUC.Value.Date)
                            songayNT -= 1;
                        data.TONG_TGIAN += songayNT;
                        data.NGAY_NT_TVB = fromTime.ToString("dd/MM/yyyy");
                        data.NGAY_HTAT_YC = ttrinhNT.NGAY_BDAU.Date.ToString("dd/MM/yyyy");
                        data.TGIAN_NTHU = songayNT;
                    }
                }

                data.TONG_TGIAN_GIAIQUYET = data.TONG_TGIAN;


                result.Add(data);
            }

            //total = result.Count();
            //return result;

            if (pagesize > 0)
            {
                int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
                result = result.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
                total = pageindex * pagesize + result.Count;
                return result.Take(pagesize).ToList();
            }
            else
            {
                total = result.Count();
                return result;
            }
        }

        public IList<BaoCaoTongHopChiTietData> GetBaoCaoTongHop(DateTime tuNgay, DateTime denNgay, bool isHoanTat)
        {
            ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
            IBienBanKSService bbkssrv = IoC.Resolve<IBienBanKSService>();
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IYCauNghiemThuService ntservice = IoC.Resolve<IYCauNghiemThuService>();
            IBienBanNTService bbntsrv = IoC.Resolve<IBienBanNTService>();
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
            var cskhcfg = cfgservice.GetbyCode("TTRINH_SYNC");
            if (cskhcfg != null && cskhcfg.Value == "1")
                ntservice.Sync();

            IList<BaoCaoTongHopChiTietData> result = new List<BaoCaoTongHopChiTietData>();

            var thanghientai = new DateTime(denNgay.Year, denNgay.Month, 1);
            var listbbntluyke = service.Query.Where(p => p.NgayYeuCau < denNgay.AddDays(1) && p.NgayYeuCau >= tuNgay && p.TrangThai > TrangThaiCongVan.MoiTao);
            var listKT = ttrinhsrv.Query.Where(p => p.MA_CVIEC == "KT").Select(x => x.MA_YCAU_KNAI).ToList();

            if (isHoanTat)
            {
                listbbntluyke = listbbntluyke.Where(p => listKT.Contains(p.MaYeuCau));
            }
            else
            {
                listbbntluyke = listbbntluyke.Where(p => !listKT.Contains(p.MaYeuCau));
            }

            var listbbnttrongthang = listbbntluyke.Where(p => p.NgayLap < denNgay.AddDays(1) && p.NgayLap >= thanghientai);


            var listDonVi = organizationService.Query.Where(p => p.type != 1).ToList();

            foreach (var dv in listDonVi)
            {
                log.Error($"{dv.orgCode}");
                BaoCaoTongHopChiTietData detail = new BaoCaoTongHopChiTietData();
                detail.MaDonVi = dv.orgCode;
                detail.TenDonVi = dv.orgName;

                var listThang = listbbnttrongthang.Where(p => p.MaDViQLy == dv.orgCode);
                var listLuyKe = listbbntluyke.Where(p => p.MaDViQLy == dv.orgCode);

                detail.SoCTTrongThang = listThang.Count();
                detail.SoCTLuyKe = listLuyKe.Count();
                double tongTGTrongThang = 0;
                foreach (var bbnt in listThang)
                {
                    log.Error($"{bbnt.MaYeuCau}");
                    var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == bbnt.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                    if (ttrinhs.Count() == 0) continue;

                    var ttrinhTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                    var ttrinhPK = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                    var ttrinhKS = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                    var ttrinhCH5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                    var ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");
                    if (ttrinhBDN == null)
                    {
                        ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    }

                    var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                    var ttrinhTVB = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                    var ttrinhKTR = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                    var ttrinhMNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "MNT");
                    var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                    var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                    var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                    var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                    var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");

                    //Thời gian tiếp nhận
                    if (ttrinhTN == null || !ttrinhTN.NGAY_KTHUC.HasValue) continue;

                    int songayTN = CommonUtils.TotalDate(ttrinhTN.NGAY_BDAU.Date, ttrinhTN.NGAY_KTHUC.Value.Date);

                    tongTGTrongThang += songayTN;

                    //Thời gian khảo sát
                    if (ttrinhKS != null && ttrinhKS.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhCH5 != null && ttrinhCH5.NGAY_KTHUC.HasValue)
                        {
                            int songayKS = CommonUtils.TotalDate(ttrinhKS.NGAY_BDAU.Date, ttrinhCH5.NGAY_BDAU.Date);
                            if (ttrinhKS.NGAY_BDAU.Date == ttrinhTN.NGAY_KTHUC.Value.Date)
                                songayKS -= 1;

                            tongTGTrongThang += songayKS;
                        }
                    }

                    if (ttrinhBDN != null && ttrinhBDN.NGAY_KTHUC.HasValue)
                    {
                        int songayBDN = CommonUtils.TotalDate(ttrinhBDN.NGAY_BDAU.Date, ttrinhBDN.NGAY_KTHUC.Value.Date);
                        //tongTGTrongThang = tongTGTrongThang + songayBDN;
                    }

                    if (ttrinhDDN != null && ttrinhDDN.NGAY_KTHUC.HasValue)
                    {
                        int songayTTDN = CommonUtils.TotalDate(ttrinhDDN.NGAY_BDAU.Date, ttrinhDDN.NGAY_KTHUC.Value.Date);
                        if (ttrinhCH5 != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhCH5.NGAY_KTHUC.Value.Date)
                            songayTTDN -= 1;
                        tongTGTrongThang += songayTTDN;
                    }

                    //Thời gian khách hàng thi công
                    if (ttrinhPC != null && ttrinhPC.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhBTT != null && ttrinhBTT.NGAY_KTHUC.HasValue)
                        {
                            DateTime fromTime = ttrinhPC.NGAY_BDAU.Date;
                            if (ttrinhs.Any(p => p.MA_CVIEC == "TC" && !string.IsNullOrWhiteSpace(p.MA_TNGAI)))
                            {
                                fromTime = ttrinhTC.NGAY_BDAU.Date;
                                log.Error($"{bbnt.MaYeuCau} - ttrinhTC: {ttrinhTC.NGAY_BDAU} - {fromTime}");
                            }
                            int songayPC = CommonUtils.TotalDate(fromTime, ttrinhBTT.NGAY_BDAU.Date);
                            if (ttrinhDDN != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                songayPC -= 1;
                            tongTGTrongThang += songayPC;
                        }
                    }

                    //Thời gian ký hợp đồng, nghiệm thu
                    if (ttrinhDHD != null && ttrinhDHD.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhNT != null && ttrinhNT.NGAY_KTHUC.HasValue)
                        {
                            DateTime fromTime = ttrinhDHD.NGAY_BDAU.Date;
                            if (ttrinhBTT != null && fromTime < ttrinhBTT.NGAY_KTHUC.Value.Date)
                                fromTime = ttrinhBTT.NGAY_KTHUC.Value.Date;

                            int songayNT = CommonUtils.TotalDate(fromTime, ttrinhNT.NGAY_BDAU.Date);
                            if (ttrinhBTT != null && fromTime == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                songayNT -= 1;
                            tongTGTrongThang += songayNT;
                        }
                    }
                }
                double tongTGLuyKe = 0;
                foreach (var bbnt in listLuyKe)
                {
                    var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == bbnt.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                    if (ttrinhs.Count() == 0) continue;
                    var ttrinhTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                    var ttrinhPK = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                    var ttrinhKS = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                    var ttrinhCH5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                    var ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");
                    if (ttrinhBDN == null)
                    {
                        ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    }

                    var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                    var ttrinhTVB = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                    var ttrinhKTR = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                    var ttrinhMNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "MNT");
                    var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                    var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                    var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                    var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                    var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");

                    //Thời gian tiếp nhận
                    if (ttrinhTN == null || !ttrinhTN.NGAY_KTHUC.HasValue) continue;

                    int songayTN = CommonUtils.TotalDate(ttrinhTN.NGAY_BDAU.Date, ttrinhTN.NGAY_KTHUC.Value.Date);
                    tongTGLuyKe += songayTN;

                    //Thời gian khảo sát
                    if (ttrinhKS != null && ttrinhKS.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhCH5 != null && ttrinhCH5.NGAY_KTHUC.HasValue)
                        {
                            int songayKS = CommonUtils.TotalDate(ttrinhKS.NGAY_BDAU.Date, ttrinhCH5.NGAY_BDAU.Date);
                            if (ttrinhKS.NGAY_BDAU.Date == ttrinhTN.NGAY_KTHUC.Value.Date)
                                songayKS -= 1;
                            tongTGLuyKe += songayKS;
                        }
                    }

                    if (ttrinhBDN != null && ttrinhBDN.NGAY_KTHUC.HasValue)
                    {
                        int songayBDN = CommonUtils.TotalDate(ttrinhBDN.NGAY_BDAU.Date, ttrinhBDN.NGAY_KTHUC.Value.Date);
                        if (ttrinhBDN.NGAY_KTHUC.Value.Date != ttrinhCH5.NGAY_KTHUC.Value.Date)
                        {
                            if (ttrinhBDN.NGAY_BDAU.Date == ttrinhCH5.NGAY_BDAU.Date)
                            {
                                songayBDN = songayBDN - 1;
                            }
                        }
                        //tongTGLuyKe += songayBDN;
                    }

                    if (ttrinhDDN != null && ttrinhDDN.NGAY_KTHUC.HasValue)
                    {
                        int songayTTDN = CommonUtils.TotalDate(ttrinhDDN.NGAY_BDAU.Date, ttrinhDDN.NGAY_KTHUC.Value.Date);
                        if (ttrinhCH5 != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhCH5.NGAY_KTHUC.Value.Date)
                            songayTTDN -= 1;
                        tongTGLuyKe += songayTTDN;
                    }

                    //Thời gian khách hàng thi công
                    if (ttrinhPC != null && ttrinhPC.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhBTT != null && ttrinhBTT.NGAY_KTHUC.HasValue)
                        {
                            DateTime fromTime = ttrinhPC.NGAY_BDAU.Date;
                            if (ttrinhs.Any(p => p.MA_CVIEC == "TC" && !string.IsNullOrWhiteSpace(p.MA_TNGAI)))
                            {
                                fromTime = ttrinhTC.NGAY_BDAU.Date;
                                log.Error($"{bbnt.MaYeuCau} - ttrinhTC: {ttrinhTC.NGAY_BDAU} - {fromTime}");
                            }
                            int songayPC = CommonUtils.TotalDate(fromTime, ttrinhBTT.NGAY_BDAU.Date);
                            if (ttrinhDDN != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                songayPC -= 1;
                            tongTGLuyKe += songayPC;
                        }
                    }

                    //Thời gian ký hợp đồng, nghiệm thu
                    if (ttrinhDHD != null && ttrinhDHD.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhNT != null && ttrinhNT.NGAY_KTHUC.HasValue)
                        {
                            DateTime fromTime = ttrinhDHD.NGAY_BDAU.Date;
                            if (ttrinhBTT != null && fromTime < ttrinhBTT.NGAY_KTHUC.Value.Date)
                                fromTime = ttrinhBTT.NGAY_KTHUC.Value.Date;
                            int songayNT = CommonUtils.TotalDate(fromTime, ttrinhNT.NGAY_BDAU.Date);
                            if (ttrinhBTT != null && fromTime == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                songayNT -= 1;
                            tongTGLuyKe += songayNT;
                        }
                    }
                }

                if (listThang.Count() > 0)
                {
                    log.Error($"{dv.orgCode}: {tongTGTrongThang}");

                    detail.TGTrongThang = Math.Round(tongTGTrongThang / (listThang.Count()), 2, MidpointRounding.AwayFromZero);
                    detail.SoNgayTrongThang = tongTGTrongThang;
                }
                else
                {
                    detail.TGTrongThang = 0;
                    detail.SoNgayTrongThang = 0;
                }
                if (listLuyKe.Count() > 0)
                {
                    log.Error($"{dv.orgCode}: {tongTGLuyKe}");
                    detail.TGLuyKe = Math.Round(tongTGLuyKe / (listLuyKe.Count()), 2, MidpointRounding.AwayFromZero);
                    detail.SoNgayLuyKe = tongTGLuyKe;
                }
                else
                {
                    detail.TGLuyKe = 0;
                    detail.SoNgayLuyKe = 0;
                }

                result.Add(detail);
            }

            return result;
        }

        public IList<BaoCaoTongHopChiTietData> GetBaoCaoChiTiet(DateTime tuNgay, DateTime denNgay, bool isHoanTat)
        {
            ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
            IBienBanKSService bbkssrv = IoC.Resolve<IBienBanKSService>();
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IYCauNghiemThuService ntservice = IoC.Resolve<IYCauNghiemThuService>();
            IBienBanNTService bbntsrv = IoC.Resolve<IBienBanNTService>();
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
            var cskhcfg = cfgservice.GetbyCode("TTRINH_SYNC");
            if (cskhcfg != null && cskhcfg.Value == "1")
                ntservice.Sync();

            IList<BaoCaoTongHopChiTietData> result = new List<BaoCaoTongHopChiTietData>();

            var startDate = new DateTime(denNgay.Year, denNgay.Month, 1);

            var listtrongthang = service.Query.Where(p => p.NgayYeuCau < denNgay.AddDays(1) && p.NgayYeuCau >= startDate && p.TrangThai > TrangThaiCongVan.MoiTao);

            var listKT = ttrinhsrv.Query.Where(p => p.MA_CVIEC == "KT").Select(x => x.MA_YCAU_KNAI).ToList();

            if (isHoanTat)
            {
                listtrongthang = listtrongthang.Where(p => listKT.Contains(p.MaYeuCau));
            }
            else
            {
                listtrongthang = listtrongthang.Where(p => !listKT.Contains(p.MaYeuCau));
            }

            var listDonVi = organizationService.Query.Where(p => p.type != 1).ToList();
            foreach (var dv in listDonVi)
            {
                BaoCaoTongHopChiTietData detail = new BaoCaoTongHopChiTietData();
                detail.MaDonVi = dv.orgCode;
                detail.TenDonVi = dv.orgName;
                var listThang = listtrongthang.Where(p => p.MaDViQLy == dv.orgCode);

                detail.SoCTTrongThang = listThang.Count();

                int countKS = 0;
                int countTTDN = 0;
                int countNT = 0;

                double tongTGTrongThang = 0;
                double tongTGTNTrongThang = 0;
                double tongTGKSTrongThang = 0;
                double tongTGTTDNTrongThang = 0;
                double tongTGNTTrongThang = 0;

                foreach (var bbnt in listThang)
                {
                    log.Error($"{bbnt.MaYeuCau}");
                    var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == bbnt.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                    if (ttrinhs.Count() == 0) continue;

                    var ttrinhTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                    var ttrinhPK = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                    var ttrinhKS = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                    var ttrinhCH5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                    var ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");
                    if (ttrinhBDN == null)
                    {
                        ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    }

                    var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                    var ttrinhTVB = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                    var ttrinhKTR = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                    var ttrinhMNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "MNT");
                    var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                    var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                    var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                    var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                    var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");

                    if (ttrinhTN == null || !ttrinhTN.NGAY_KTHUC.HasValue) continue;

                    int songayTN = CommonUtils.TotalDate(ttrinhTN.NGAY_BDAU.Date, ttrinhTN.NGAY_KTHUC.Value.Date);
                    tongTGTNTrongThang += songayTN;
                    tongTGTrongThang += songayTN;

                    //Thời gian khảo sát
                    if (ttrinhKS != null && ttrinhKS.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhCH5 != null && ttrinhCH5.NGAY_KTHUC.HasValue)
                        {
                            int songayKS = CommonUtils.TotalDate(ttrinhKS.NGAY_BDAU.Date, ttrinhCH5.NGAY_BDAU.Date);
                            tongTGKSTrongThang += songayKS;
                            if (ttrinhKS.NGAY_BDAU.Date == ttrinhTN.NGAY_KTHUC.Value.Date)
                                songayKS -= 1;

                            tongTGTrongThang += songayKS;
                            detail.SLKSatTrongThang++;
                            countKS++;
                        }
                    }

                    //Thời gian thỏa thuận đấu nối
                    if (ttrinhDDN != null && ttrinhDDN.NGAY_KTHUC.HasValue)
                    {
                        int songayTTDN = CommonUtils.TotalDate(ttrinhDDN.NGAY_BDAU.Date, ttrinhDDN.NGAY_KTHUC.Value.Date);
                        tongTGTTDNTrongThang += songayTTDN;
                        if (ttrinhCH5 != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhCH5.NGAY_KTHUC.Value.Date)
                            songayTTDN -= 1;
                        tongTGTrongThang += songayTTDN;
                        detail.SLTTDNTrongThang++;
                        countTTDN++;
                    }

                    //Thời gian khách hàng thi công
                    if (ttrinhPC != null && ttrinhPC.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhBTT != null && ttrinhBTT.NGAY_KTHUC.HasValue)
                        {
                            DateTime fromTime = ttrinhPC.NGAY_BDAU.Date;
                            if (ttrinhs.Any(p => p.MA_CVIEC == "TC" && !string.IsNullOrWhiteSpace(p.MA_TNGAI)))
                                fromTime = ttrinhTC.NGAY_BDAU.Date;
                            int songayPC = CommonUtils.TotalDate(fromTime, ttrinhBTT.NGAY_BDAU.Date);
                            if (ttrinhDDN != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                songayPC -= 1;
                            tongTGTrongThang += songayPC;
                            tongTGNTTrongThang += songayPC;
                            countNT++;
                        }
                    }

                    //Thời gian ký hợp đồng, nghiệm thu
                    if (ttrinhDHD != null && ttrinhDHD.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhNT != null && ttrinhNT.NGAY_KTHUC.HasValue)
                        {
                            DateTime fromTime = ttrinhDHD.NGAY_BDAU.Date;
                            if (ttrinhBTT != null && fromTime < ttrinhBTT.NGAY_KTHUC.Value.Date)
                                fromTime = ttrinhBTT.NGAY_KTHUC.Value.Date;
                            int songayNT = CommonUtils.TotalDate(fromTime, ttrinhNT.NGAY_BDAU.Date);

                            if (ttrinhBTT != null && fromTime == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                songayNT -= 1;

                            tongTGNTTrongThang += songayNT;
                            tongTGTrongThang += songayNT;
                            if (countNT == 0)
                                countNT++;
                        }
                    }
                    detail.SLNTTrongThang = countNT;
                }
                log.Error($"{dv.orgCode}-tongTGTrongThang:{tongTGTrongThang}, tongTGTNTrongThang:{tongTGTNTrongThang}, tongTGKSTrongThang:{tongTGKSTrongThang}, tongTGTTDNTrongThang:{tongTGTTDNTrongThang}, tongTGNTTrongThang:{tongTGNTTrongThang}");
                if (listThang.Count() > 0)
                {
                    detail.TGTrongThang = Math.Round(tongTGTrongThang / (listThang.Count()), 2, MidpointRounding.AwayFromZero);
                    detail.SoNgayTrongThang = tongTGTrongThang;
                    detail.TGTiepNhanTrongThang = Math.Round(tongTGTNTrongThang / (listThang.Count()), 2, MidpointRounding.AwayFromZero);
                    detail.SoNgayTiepNhanTrongThang = tongTGTNTrongThang;
                    if (countKS > 0)
                    {
                        detail.TGKhaoSatTrongThang = Math.Round(tongTGKSTrongThang / (countKS), 2, MidpointRounding.AwayFromZero);
                        detail.SoNgayKhaoSatTrongThang = tongTGKSTrongThang;
                    }

                    if (countTTDN > 0)
                    {
                        detail.TGTTDNTrongThang = Math.Round(tongTGTTDNTrongThang / (countTTDN), 2, MidpointRounding.AwayFromZero);
                        detail.SoNgayTTDNTrongThang = tongTGTTDNTrongThang;
                    }

                    if (countNT > 0)
                    {
                        detail.TGNTTrongThang = Math.Round(tongTGNTTrongThang / (countNT), 2, MidpointRounding.AwayFromZero);
                        detail.SoNgayNTTrongThang = tongTGNTTrongThang;
                    }

                }
                result.Add(detail);
            }

            return result;
        }

        public IList<BaoCaoTongHopChiTietData> GetBaoCaoChiTietLuyKe(DateTime tuNgay, DateTime denNgay, bool isHoanTat)
        {
            ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
            IBienBanKSService bbkssrv = IoC.Resolve<IBienBanKSService>();
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IYCauNghiemThuService ntservice = IoC.Resolve<IYCauNghiemThuService>();
            IBienBanNTService bbntsrv = IoC.Resolve<IBienBanNTService>();
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
            var cskhcfg = cfgservice.GetbyCode("TTRINH_SYNC");
            if (cskhcfg != null && cskhcfg.Value == "1")
                ntservice.Sync();

            IList<BaoCaoTongHopChiTietData> result = new List<BaoCaoTongHopChiTietData>();

            var listbbntluyke = service.Query.Where(p => p.NgayYeuCau < denNgay.AddDays(1) && p.NgayYeuCau >= tuNgay.Date && p.TrangThai > TrangThaiCongVan.MoiTao);
            var listKT = ttrinhsrv.Query.Where(p => p.MA_CVIEC == "KT").Select(x => x.MA_YCAU_KNAI).ToList();

            if (isHoanTat)
            {
                listbbntluyke = listbbntluyke.Where(p => listKT.Contains(p.MaYeuCau));
            }
            else
            {
                listbbntluyke = listbbntluyke.Where(p => !listKT.Contains(p.MaYeuCau));
            }

            var listDonVi = organizationService.Query.Where(p => p.type != 1).ToList();
            foreach (var dv in listDonVi)
            {
                BaoCaoTongHopChiTietData detail = new BaoCaoTongHopChiTietData();
                detail.MaDonVi = dv.orgCode;
                detail.TenDonVi = dv.orgName;

                var listLuyKe = listbbntluyke.Where(p => p.MaDViQLy == dv.orgCode);

                detail.SoCTLuyKe = listLuyKe.Count();
                log.Error($"{dv.orgCode}-{listLuyKe.Count()}");

                int countKS = 0;
                int countTTDN = 0;
                int countNT = 0;

                double tongTGLuyKe = 0;
                double tongTGTNLuyKe = 0;
                double tongTGKSLuyKe = 0;
                double tongTGTTDNLuyKe = 0;
                double tongTGNTLuyKe = 0;
                foreach (var bbnt in listLuyKe)
                {
                    log.Error($"{bbnt.MaYeuCau}");
                    var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == bbnt.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                    if (ttrinhs.Count() == 0) continue;

                    var ttrinhTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                    var ttrinhPK = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                    var ttrinhKS = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                    var ttrinhCH5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                    var ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");
                    if (ttrinhBDN == null)
                    {
                        ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    }

                    var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                    var ttrinhTVB = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                    var ttrinhKTR = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                    var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                    var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                    var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                    var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                    var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");

                    if (ttrinhTN == null || !ttrinhTN.NGAY_KTHUC.HasValue) continue;

                    //Thời gian tiếp nhận
                    int songayTN = CommonUtils.TotalDate(ttrinhTN.NGAY_BDAU.Date, ttrinhTN.NGAY_KTHUC.Value.Date);

                    tongTGTNLuyKe += songayTN;
                    tongTGLuyKe += songayTN;

                    //Thời gian khảo sát
                    if (ttrinhKS != null && ttrinhKS.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhCH5 != null && ttrinhCH5.NGAY_KTHUC.HasValue)
                        {
                            int songayKS = CommonUtils.TotalDate(ttrinhKS.NGAY_BDAU.Date, ttrinhCH5.NGAY_BDAU.Date);
                            tongTGKSLuyKe += songayKS;
                            if (ttrinhKS.NGAY_BDAU.Date == ttrinhTN.NGAY_KTHUC.Value.Date)
                                songayKS -= 1;

                            tongTGLuyKe += songayKS;
                            detail.SLKSatLuyKe++;
                            countKS++;
                        }
                    }


                    if (ttrinhDDN != null && ttrinhDDN.NGAY_KTHUC.HasValue)
                    {
                        int songayTTDN = CommonUtils.TotalDate(ttrinhDDN.NGAY_BDAU.Date, ttrinhDDN.NGAY_KTHUC.Value.Date);
                        tongTGTTDNLuyKe += songayTTDN;
                        if (ttrinhCH5 != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhCH5.NGAY_KTHUC.Value.Date)
                            songayTTDN -= 1;
                        tongTGLuyKe += songayTTDN;
                        detail.SLTTDNLuyKe++;
                        countTTDN++;
                    }

                    //Thời gian khách hàng thi công
                    if (ttrinhPC != null && ttrinhPC.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhBTT != null && ttrinhBTT.NGAY_KTHUC.HasValue)
                        {
                            DateTime fromTime = ttrinhPC.NGAY_BDAU.Date;
                            if (ttrinhs.Any(p => p.MA_CVIEC == "TC" && !string.IsNullOrWhiteSpace(p.MA_TNGAI)))
                            {
                                fromTime = ttrinhTC.NGAY_BDAU.Date;
                                log.Error($"{bbnt.MaYeuCau} - ttrinhTC: {ttrinhTC.NGAY_BDAU} - {fromTime}");
                            }
                            int songayPC = CommonUtils.TotalDate(fromTime, ttrinhBTT.NGAY_BDAU.Date);
                            if (ttrinhDDN != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                songayPC -= 1;
                            tongTGLuyKe += songayPC;
                            tongTGNTLuyKe += songayPC;
                            log.Error($"songayPC: {songayPC}");
                            countNT++;
                        }
                    }

                    //Thời gian ký hợp đồng, nghiệm thu
                    if (ttrinhDHD != null && ttrinhDHD.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhNT != null && ttrinhNT.NGAY_KTHUC.HasValue)
                        {
                            DateTime fromTime = ttrinhDHD.NGAY_BDAU.Date;
                            if (ttrinhBTT != null && fromTime < ttrinhBTT.NGAY_KTHUC.Value.Date)
                                fromTime = ttrinhBTT.NGAY_KTHUC.Value.Date;
                            int songayNT = CommonUtils.TotalDate(fromTime, ttrinhNT.NGAY_BDAU.Date);
                            if (ttrinhBTT != null && fromTime == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                songayNT -= 1;

                            tongTGNTLuyKe += songayNT;
                            tongTGLuyKe += songayNT;
                            if (countNT == 0)
                                countNT++;
                        }
                    }
                    detail.SLNTLuyKe = countNT;
                }
                log.Error($"{dv.orgCode}-tongTGLuyKe:{tongTGLuyKe}, tongTGTNLuyKe:{tongTGTNLuyKe}, tongTGKSLuyKe:{tongTGKSLuyKe}, tongTGTTDNLuyKe:{tongTGTTDNLuyKe}, tongTGNTLuyKe:{tongTGNTLuyKe}");
                if (listLuyKe.Count() > 0)
                {
                    detail.TGLuyKe = Math.Round(tongTGLuyKe / (listLuyKe.Count()), 2, MidpointRounding.AwayFromZero);
                    detail.SoNgayLuyKe = tongTGLuyKe;
                    detail.TGTiepNhanLuyKe = Math.Round(tongTGTNLuyKe / (listLuyKe.Count()), 2, MidpointRounding.AwayFromZero);
                    detail.SoNgayTiepNhanLuyKe = tongTGTNLuyKe;
                    if (countKS > 0)
                    {
                        detail.TGKhaoSatLuyKe = Math.Round(tongTGKSLuyKe / (countKS), 2, MidpointRounding.AwayFromZero);
                        detail.SoNgayKhaoSatLuyKe = tongTGKSLuyKe;
                    }

                    if (countTTDN > 0)
                    {
                        detail.TGTTDNLuyKe = Math.Round(tongTGTTDNLuyKe / (countTTDN), 2, MidpointRounding.AwayFromZero);
                        detail.SoNgayTTDNLuyKe = tongTGTTDNLuyKe;
                    }

                    if (countNT > 0)
                    {
                        detail.TGNTLuyKe = Math.Round(tongTGNTLuyKe / (countNT), 2, MidpointRounding.AwayFromZero);
                        detail.SoNgayNTLuyKe = tongTGNTLuyKe;
                    }
                }
                result.Add(detail);
            }

            return result;
        }

        public IList<BaoCaoTTDN> GetListBaoCaoTTDN(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {
            IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IKetQuaKSService ketQuaKSService = IoC.Resolve<IKetQuaKSService>();
            ITroNgaiService troNgaiService = IoC.Resolve<ITroNgaiService>();
            var query = Query.Where(p => p.TrangThai > TrangThaiCongVan.MoiTao);
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);
            if (status > -1)
                query = query.Where(p => p.TrangThai == (TrangThaiCongVan)status);
            if (!string.IsNullOrWhiteSpace(khachhang))
                query = query.Where(p => p.CoQuanChuQuan.Contains(khachhang) || p.NguoiYeuCau.Contains(khachhang) || p.MaKHang.Contains(khachhang));
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.MaYeuCau.Contains(keyword) || p.DuAnDien.Contains(keyword));

            query = query.OrderByDescending(p => p.MaYeuCau);
            var ret = new List<CongVanYeuCau>();
            var result = new List<BaoCaoTTDN>();
            ret = query.ToList();

            foreach (var item in ret)
            {
                var detail = new BaoCaoTTDN();
                detail.MaDViQLy = item.MaDViQLy;
                detail.TenKH = item.TenKhachHang;
                detail.TenCT = item.DuAnDien;
                detail.DCCT = item.DiaChiDungDien;
                detail.MaYC = item.MaYeuCau;

                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                if (ttrinhs.Count() == 0) continue;
                var trongais = ttrinhs.Where(p => !string.IsNullOrWhiteSpace(p.MA_TNGAI)).ToList();

                var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                if (ttrinhtnhs == null || !ttrinhtnhs.NGAY_KTHUC.HasValue) continue;

                var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");

                int songayTN = CommonUtils.TotalDate(ttrinhtnhs.NGAY_BDAU.Date, ttrinhtnhs.NGAY_KTHUC.Value.Date);
                detail.ThoiGianTNHS = songayTN;
                detail.SoNgayTNHS = songayTN;
                detail.TongSoNgayTTDN += songayTN;

                var ttrinhpk = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                {
                    int songay = CommonUtils.TotalDate(ttrinhpk.NGAY_BDAU.Date, ttrinhpk.NGAY_KTHUC.Value.Date);

                    detail.ThoiGianChuyenHSSangKS = songay;
                }

                if (item.TrangThai == TrangThaiCongVan.Huy)
                {
                    detail.TroNgaiTNHS = item.LyDoHuy;
                    var ttrinhhuy = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HU");
                    if (ttrinhhuy != null && ttrinhhuy.NGAY_KTHUC.HasValue)
                    {
                        int songay = CommonUtils.TotalDate(ttrinhhuy.NGAY_BDAU.Date, ttrinhhuy.NGAY_KTHUC.Value.Date);

                        detail.ThoiGianTLHS = songay;
                    }
                }

                var bbks = bienBanKSService.GetbyYeuCau(item.MaYeuCau);

                if (bbks != null)
                {
                    detail.TongCS = bbks.TongCongSuat;
                    detail.TongChieuDaiDD = bbks.DayDan;

                    var ketquaks = ketQuaKSService.GetbyMaYCau(item.MaYeuCau);
                    if (ketquaks != null)
                    {
                        if (!string.IsNullOrWhiteSpace(ketquaks.MA_TNGAI))
                        {
                            var trongai = troNgaiService.Getbykey(ketquaks.MA_TNGAI);
                            if (trongai != null)
                            {
                                detail.TroNgaiKS = trongai.TEN_TNGAI;
                            }
                        }
                    }
                }

                if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                {
                    if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                    {
                        int songayKS = CommonUtils.TotalDate(ttrinhks.NGAY_BDAU.Date, ttrinhch5.NGAY_BDAU.Date);
                        detail.SoNgayThucHienKS = songayKS;
                        if (ttrinhks.NGAY_BDAU.Date == ttrinhtnhs.NGAY_KTHUC.Value.Date)
                            songayKS -= 1;
                        detail.TongSoNgayTTDN += songayKS;
                    }
                }

                var ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");
                if (ttrinhbdn == null)
                {
                    ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                }
                if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                {
                    int songayBDN = CommonUtils.TotalDate(ttrinhbdn.NGAY_BDAU.Date, ttrinhbdn.NGAY_KTHUC.Value.Date);
                    detail.SoNgayLapTTDN = songayBDN;
                }

                var ttrinhddn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                {
                    int songay = CommonUtils.TotalDate(ttrinhddn.NGAY_BDAU.Date, ttrinhddn.NGAY_KTHUC.Value.Date);
                    detail.ThoiGianLapVaDuyetTTDN = songay;
                    detail.ThoiGianChuyenBBTTDNDenKH = songay;
                    if (ttrinhch5 != null && ttrinhddn.NGAY_BDAU.Date == ttrinhch5.NGAY_KTHUC.Value.Date)
                        songay -= 1;
                    detail.TongSoNgayTTDN += songay;
                }

                detail.SoNgayLapBBTTDN += detail.ThoiGianLapVaDuyetTTDN;
                detail.TongSoNgayTTDN = detail.SoNgayLapBBTTDN + detail.SoNgayThucHienKS + detail.SoNgayTNHS;
                result.Add(detail);
            }
            if (pagesize > 0)
            {
                int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
                result = result.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
                total = pageindex * pagesize + result.Count;
                return result.Take(pagesize).ToList();
            }
            else
            {
                total = result.Count();
                return result;
            }
        }

        public IList<BaoCaoYCNT> GetListBaoCaoNT(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {
            IYCauNghiemThuService yCauNghiemThuService = IoC.Resolve<IYCauNghiemThuService>();
            ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
            IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
            IBienBanKTService bienBanKTService = IoC.Resolve<IBienBanKTService>();
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IKetQuaKTService ketQuaKTService = IoC.Resolve<IKetQuaKTService>();
            ITroNgaiService troNgaiService = IoC.Resolve<ITroNgaiService>();

            var query = yCauNghiemThuService.Query.Where(p => p.TrangThai > TrangThaiNghiemThu.MoiTao); ;
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);
            if (status > -1)
                query = query.Where(p => p.TrangThai == (TrangThaiNghiemThu)status);
            if (!string.IsNullOrWhiteSpace(khachhang))
                query = query.Where(p => p.CoQuanChuQuan.Contains(khachhang) || p.NguoiYeuCau.Contains(khachhang) || p.MaKHang.Contains(khachhang));
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.MaYeuCau.Contains(keyword) || p.DuAnDien.Contains(keyword));

            query = query.OrderByDescending(p => p.MaYeuCau);
            var ret = new List<YCauNghiemThu>();
            var result = new List<BaoCaoYCNT>();
            ret = query.ToList();

            foreach (var item in ret)
            {
                var detail = new BaoCaoYCNT();

                var cvyc = congVanYeuCauService.GetbyMaYCau(item.MaYeuCau);

                detail.MaDViQLy = item.MaDViQLy;
                detail.TenKH = cvyc.TenKhachHang;
                detail.TenCT = item.DuAnDien;
                detail.DCCT = item.DiaChiDungDien;
                detail.MaYC = item.MaYeuCau;

                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                if (ttrinhs.Count() == 0) continue;

                var ttrinhtnhskt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                if (ttrinhtnhskt == null || !ttrinhtnhskt.NGAY_KTHUC.HasValue) continue;

                var ttrinhktr = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");

                int songayTVB = CommonUtils.TotalDate(ttrinhtnhskt.NGAY_BDAU.Date, ttrinhtnhskt.NGAY_KTHUC.Value.Date);

                detail.ThoiGianTNHS = songayTVB;
                detail.SoNgayTNHS = songayTVB;
                detail.SoNgayKTDK = songayTVB;

                if (ttrinhktr != null && ttrinhktr.NGAY_KTHUC.HasValue)
                {
                    int songay = CommonUtils.TotalDate(ttrinhktr.NGAY_BDAU.Date, ttrinhtnhskt.NGAY_KTHUC.Value.Date);
                    detail.ThoiGianChuyenHSSangKT = songay;
                }

                if (ttrinhktr == null && item.TrangThai == TrangThaiNghiemThu.Huy)
                {
                    //detail.TroNgaiTNHS = item.LyDoHuy;
                    var ttrinhhuy = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HU");
                    if (ttrinhhuy != null && ttrinhhuy.NGAY_KTHUC.HasValue)
                    {
                        int songay = CommonUtils.TotalDate(ttrinhhuy.NGAY_BDAU.Date, ttrinhhuy.NGAY_KTHUC.Value.Date);
                        detail.ThoiGianTLHS = songay;
                    }
                }
                var bbks = bienBanKSService.GetbyYeuCau(item.MaYeuCau);

                if (bbks != null)
                {
                    detail.TongCS = bbks.TongCongSuat;
                    detail.TongChieuDaiDD = bbks.DayDan;
                }
                var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                if (bbkt != null)
                {
                    var ketquakt = ketQuaKTService.GetbyMaYCau(item.MaYeuCau);
                    if (ketquakt != null)
                    {
                        if (!string.IsNullOrWhiteSpace(ketquakt.MA_TNGAI))
                        {
                            var trongai = troNgaiService.Getbykey(ketquakt.MA_TNGAI);
                            if (ttrinhktr != null && ttrinhktr.NGAY_KTHUC.HasValue)
                            {
                                int songay = CommonUtils.TotalDate(ttrinhktr.NGAY_BDAU.Date, ttrinhktr.NGAY_KTHUC.Value.Date);
                                detail.ThoiGianYeuCauKHHoanThanhTonTai = songay;
                                if (trongai != null)
                                {
                                    detail.TroNgaiKT = trongai.TEN_TNGAI;
                                }
                            }
                        }
                    }

                    if (ttrinhktr != null && ttrinhktr.NGAY_KTHUC.HasValue)
                    {
                        int songay = CommonUtils.TotalDate(ttrinhktr.NGAY_BDAU.Date, ttrinhktr.NGAY_KTHUC.Value.Date);
                        detail.ThoiGianKTThucTe = songay;
                        detail.ThoiGianLapVaDuyetBBKT = songay;
                        detail.ThoiGianChuyenBBKTDenKH = songay;
                        detail.SoNgayKTDK += songay;
                    }
                }

                var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");

                var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");

                //Thời gian khách hàng thi công
                if (ttrinhPC != null && ttrinhPC.NGAY_KTHUC.HasValue)
                {
                    if (ttrinhBTT != null && ttrinhBTT.NGAY_KTHUC.HasValue)
                    {
                        DateTime fromTime = ttrinhPC.NGAY_BDAU.Date;
                        if (ttrinhs.Any(p => p.MA_CVIEC == "TC" && !string.IsNullOrWhiteSpace(p.MA_TNGAI)))
                            fromTime = ttrinhTC.NGAY_BDAU.Date;
                        int songayPC = CommonUtils.TotalDate(fromTime, ttrinhBTT.NGAY_BDAU.Date);
                        if (ttrinhDDN != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhBTT.NGAY_KTHUC.Value.Date)
                            songayPC -= 1;
                        detail.ThoiGianTNDNNT = songayPC;
                        detail.TongSoNgayYCNT = songayPC;
                    }
                }

                //Thời gian ký hợp đồng, nghiệm thu
                if (ttrinhDHD != null && ttrinhDHD.NGAY_KTHUC.HasValue)
                {
                    if (ttrinhNT != null && ttrinhNT.NGAY_KTHUC.HasValue)
                    {
                        DateTime fromTime = ttrinhDHD.NGAY_BDAU.Date;
                        if (ttrinhBTT != null && fromTime < ttrinhBTT.NGAY_KTHUC.Value.Date)
                            fromTime = ttrinhBTT.NGAY_KTHUC.Value.Date;
                        int songayNT = CommonUtils.TotalDate(fromTime, ttrinhNT.NGAY_BDAU.Date);

                        if (ttrinhBTT != null && fromTime == ttrinhBTT.NGAY_KTHUC.Value.Date)
                            songayNT -= 1;
                        detail.SoNgayNTVaKyHD = songayNT;
                        detail.ThoiGianNTDDVaKyHD = songayNT;
                        detail.TongSoNgayYCNT += songayNT;

                        log.Error($"songayNT: {songayNT}");
                    }
                }
                result.Add(detail);
            }
            if (pagesize > 0)
            {
                int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
                result = result.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
                total = pageindex * pagesize + result.Count;
                return result.Take(pagesize).ToList();
            }
            else
            {
                total = result.Count();
                return result;
            }
        }

        public IList<BaoCaoChiTietTCDN> GetListBaoCaoChiTietTCDN(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {

            IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IYCauNghiemThuService yCauNghiemThuService = IoC.Resolve<IYCauNghiemThuService>();
            IKetQuaKSService ketQuaKSService = IoC.Resolve<IKetQuaKSService>();
            IKetQuaKTService ketQuaKTService = IoC.Resolve<IKetQuaKTService>();
            ITroNgaiService troNgaiService = IoC.Resolve<ITroNgaiService>();
            IBienBanKTService bienBanKTService = IoC.Resolve<IBienBanKTService>();
            var query = Query.Where(p => p.TrangThai > TrangThaiCongVan.MoiTao);
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);
            if (status > -1)
                query = query.Where(p => p.TrangThai == (TrangThaiCongVan)status);
            if (!string.IsNullOrWhiteSpace(khachhang))
                query = query.Where(p => p.CoQuanChuQuan.Contains(khachhang) || p.NguoiYeuCau.Contains(khachhang) || p.MaKHang.Contains(khachhang));
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.MaYeuCau.Contains(keyword) || p.DuAnDien.Contains(keyword));

            query = query.OrderByDescending(p => p.MaYeuCau);
            var ret = new List<CongVanYeuCau>();
            var result = new List<BaoCaoChiTietTCDN>();
            ret = query.ToList();


            foreach (var item in ret)
            {
                var detail = new BaoCaoChiTietTCDN();
                detail.MaDViQLy = item.MaDViQLy;
                detail.TenKH = item.TenKhachHang;
                detail.TenCT = item.DuAnDien;
                detail.DCCT = item.DiaChiDungDien;
                detail.MaYC = item.MaYeuCau;

                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                if (ttrinhs.Count() == 0) continue;

                var ttrinhTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                if (ttrinhTN == null || !ttrinhTN.NGAY_KTHUC.HasValue) continue;

                var ttrinhPK = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                var ttrinhKS = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                var ttrinhCH5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                var ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");
                if (ttrinhBDN == null)
                {
                    ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                }

                var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                var ttrinhTVB = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                var ttrinhKTR = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");

                var bbks = bienBanKSService.GetbyYeuCau(item.MaYeuCau);

                if (bbks != null)
                {
                    detail.TongCS = bbks.TongCongSuat;
                    detail.TongChieuDaiDD = bbks.DayDan;

                    var ketquaks = ketQuaKSService.GetbyMaYCau(item.MaYeuCau);
                    if (ketquaks != null)
                    {
                        if (!string.IsNullOrWhiteSpace(ketquaks.MA_TNGAI))
                        {
                            var trongai = troNgaiService.Getbykey(ketquaks.MA_TNGAI);
                            if (trongai != null)
                            {
                                detail.TroNgaiTTDN = trongai.TEN_TNGAI;
                            }
                        }
                    }
                }

                //Thời gian tiếp nhận
                int songayTN = CommonUtils.TotalDate(ttrinhTN.NGAY_BDAU.Date, ttrinhTN.NGAY_KTHUC.Value.Date);

                detail.TongSoNgayTTDN += songayTN;
                detail.TongSoNgayTCDN += songayTN;

                log.Error($"songayTN: {songayTN}");
                //Thời gian khảo sát
                if (ttrinhKS != null && ttrinhKS.NGAY_KTHUC.HasValue)
                {
                    if (ttrinhCH5 != null && ttrinhCH5.NGAY_KTHUC.HasValue)
                    {
                        int songayKS = CommonUtils.TotalDate(ttrinhKS.NGAY_BDAU.Date, ttrinhCH5.NGAY_BDAU.Date);

                        if (ttrinhKS.NGAY_BDAU.Date == ttrinhTN.NGAY_KTHUC.Value.Date)
                            songayKS -= 1;

                        detail.TongSoNgayTTDN += songayKS;
                        detail.TongSoNgayTCDN += songayKS;
                    }
                }

                if (ttrinhDDN != null && ttrinhDDN.NGAY_KTHUC.HasValue)
                {
                    int songayTTDN = CommonUtils.TotalDate(ttrinhDDN.NGAY_BDAU.Date, ttrinhDDN.NGAY_KTHUC.Value.Date);
                    if (ttrinhCH5 != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhCH5.NGAY_KTHUC.Value.Date)
                        songayTTDN -= 1;
                    detail.TongSoNgayTTDN += songayTTDN;
                    detail.TongSoNgayTCDN += songayTTDN;
                }

                var ycnt = yCauNghiemThuService.GetbyMaYCau(item.MaYeuCau);
                if (ycnt != null)
                {
                    var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                    if (bbkt != null)
                    {
                        var ketquakt = ketQuaKTService.GetbyMaYCau(item.MaYeuCau);
                        if (ketquakt != null)
                        {
                            if (!string.IsNullOrWhiteSpace(ketquakt.MA_TNGAI))
                            {
                                var trongai = troNgaiService.Getbykey(ketquakt.MA_TNGAI);
                                if (trongai != null)
                                {
                                    detail.TroNgaiKTDK = trongai.TEN_TNGAI;
                                }
                            }
                        }
                        if (ttrinhTVB != null && ttrinhTVB.NGAY_KTHUC.HasValue)
                        {
                            int songayTVB = CommonUtils.TotalDate(ttrinhTVB.NGAY_BDAU.Date, ttrinhTVB.NGAY_KTHUC.Value.Date);
                            detail.TongSoNgayKTDK += songayTVB;

                            if (ttrinhKTR != null && ttrinhKTR.NGAY_KTHUC.HasValue)
                            {
                                int songayKTR = CommonUtils.TotalDate(ttrinhKTR.NGAY_BDAU.Date, ttrinhKTR.NGAY_KTHUC.Value.Date);
                                detail.TongSoNgayKTDK += songayKTR;
                            }
                        }
                    }

                    //Thời gian khách hàng thi công
                    if (ttrinhPC != null && ttrinhPC.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhBTT != null && ttrinhBTT.NGAY_KTHUC.HasValue)
                        {
                            DateTime fromTime = ttrinhPC.NGAY_BDAU.Date;
                            if (ttrinhs.Any(p => p.MA_CVIEC == "TC" && !string.IsNullOrWhiteSpace(p.MA_TNGAI)))
                                fromTime = ttrinhTC.NGAY_BDAU.Date;
                            int songayPC = CommonUtils.TotalDate(fromTime, ttrinhBTT.NGAY_BDAU.Date);
                            if (ttrinhDDN != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                songayPC -= 1;
                            detail.TongSoNgayNT += songayPC;
                            detail.TongSoNgayTCDN += songayPC;
                            log.Error($"songayPC: {songayPC}");
                        }
                    }

                    //Thời gian ký hợp đồng, nghiệm thu
                    if (ttrinhDHD != null && ttrinhDHD.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhNT != null && ttrinhNT.NGAY_KTHUC.HasValue)
                        {
                            DateTime fromTime = ttrinhDHD.NGAY_BDAU.Date;
                            if (ttrinhBTT != null && fromTime < ttrinhBTT.NGAY_KTHUC.Value.Date)
                                fromTime = ttrinhBTT.NGAY_KTHUC.Value.Date;
                            int songayNT = CommonUtils.TotalDate(fromTime, ttrinhNT.NGAY_BDAU.Date);
                            if (ttrinhBTT != null && fromTime == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                songayNT -= 1;
                            detail.TongSoNgayNT += songayNT;
                            detail.TongSoNgayTCDN += songayNT;
                            log.Error($"songayNT: {songayNT}");
                        }
                    }
                }

                if (detail.TongSoNgayTTDN > 2)
                {
                    detail.HangMucQuaHan = "Thoả thuận đấu nối";
                    detail.SoNgayQuaHan = detail.TongSoNgayTTDN - 2;
                }
                if (detail.TongSoNgayKTDK > 4)
                {
                    detail.HangMucQuaHan = "Kiểm tra điều kiện";
                    detail.SoNgayQuaHan = detail.TongSoNgayNT - 2;
                }
                if (detail.TongSoNgayNT > 1)
                {
                    detail.HangMucQuaHan = "Nghiệm thu";
                    detail.SoNgayQuaHan = detail.TongSoNgayNT - 1;
                }
                result.Add(detail);
            }
            if (pagesize > 0)
            {
                int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
                result = result.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
                total = pageindex * pagesize + result.Count;
                return result.Take(pagesize).ToList();
            }
            else
            {
                total = result.Count();
                return result;

            }
        }

        public IList<ThoiGianCapDienModel> GetThoigiancapdien(string donViQuanLy)
        {
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();

               var query = Query.Where(p => p.MaDViQLy == donViQuanLy);

            var result = new List<ThoiGianCapDienModel>();
            var listDonVi = organizationService.GetAll();
            if("-1" == donViQuanLy)
            {
                foreach (var dv in listDonVi)
                {
                    var detail = new ThoiGianCapDienModel();
                    detail.TenDV = dv.orgName;
                    var listCV = query.Where(x => x.MaDViQLy == dv.orgCode);
                    detail.TongSoCTTiepNhanTTDN = listCV.Count();
                   result.Add(detail);
                }

            }
            else
                {
                var detail = new ThoiGianCapDienModel();
                detail.TenDV = donViQuanLy;
                var listCV = query.Where(x => x.MaDViQLy == donViQuanLy);
                detail.TongSoCTTiepNhanTTDN = listCV.Count();
                result.Add(detail);
                }

            //var result = new SoLuongKhaoSatModel();
            ////Số lượng
            //result.SoLuongKhaoSat = query.Count();
            //result.SoLuongKhaoSatThanhCong = query.Count(x => x.KETQUA == "Thành công");
            //result.SoLuongKhaoSatThatBai = query.Count(x => x.KETQUA == "Thất bại");
            //return result;

            return result;
        }

        public IList<BaoCaoTHTCDN> GetListBaoCaoTHTCDN(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate)
        {
            IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IYCauNghiemThuService yCauNghiemThuService = IoC.Resolve<IYCauNghiemThuService>();
            IBienBanKTService bienBanKTService = IoC.Resolve<IBienBanKTService>();
            IKetQuaKSService ketQuaKSService = IoC.Resolve<IKetQuaKSService>();
            IKetQuaKTService ketQuaKTService = IoC.Resolve<IKetQuaKTService>();
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            var query = Query.Where(p => p.TrangThai > TrangThaiCongVan.MoiTao);
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);
            //if (status > -1)
            //    query = query.Where(p => p.TrangThai == (TrangThaiCongVan)status);
            if (!string.IsNullOrWhiteSpace(khachhang))
                query = query.Where(p => p.CoQuanChuQuan.Contains(khachhang) || p.NguoiYeuCau.Contains(khachhang) || p.MaKHang.Contains(khachhang));
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.MaYeuCau.Contains(keyword) || p.DuAnDien.Contains(keyword));

            query = query.OrderByDescending(p => p.MaYeuCau);

            var result = new List<BaoCaoTHTCDN>();
            var listDonVi = organizationService.GetAll();

            foreach (var dv in listDonVi)
            {
                var detail = new BaoCaoTHTCDN();
                detail.TenDV = dv.orgName;
                var listCV = query.Where(x => x.MaDViQLy == dv.orgCode);
                detail.TongSoCTTiepNhanTTDN = listCV.Count();

                int countTN = 0;
                int countHT = 0;
                int countHU = 0;
                int countTongKTDK = 0;
                int countTongNT = 0;
                int countTNKTDK = 0;
                int countHTKTDK = 0;
                int countHUKTKD = 0;
                int countHTNT = 0;
                double tongTGTTDN = 0;
                double tongTGKTDK = 0;
                double tongTGNT = 0;
                double tongTGTCDN = 0;

                int countQuaHanTTDN = 0;
                int ngayQuaHanTTDN = 0;
                int countQuaHanKTDK = 0;
                int ngayQuaHanKTDK = 0;
                int countQuaHanNT = 0;
                int ngayQuaHanNT = 0;
                int countQuaHan = 0;
                int ngayQuaHan = 0;

                foreach (var item in listCV)
                {
                    double detailTGTTDN = 0;
                    double detailTGNT = 0;
                    double detailKTDK = 0;
                    var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).ToList();

                    if (ttrinhs.Count() == 0) continue;

                    var ttrinhTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                    if (ttrinhTN == null || !ttrinhTN.NGAY_KTHUC.HasValue) continue;

                    var ttrinhPK = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                    var ttrinhKS = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                    var ttrinhCH5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                    var ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");
                    if (ttrinhBDN == null)
                    {
                        ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    }

                    var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                    var ttrinhTVB = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                    var ttrinhKTR = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                    var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                    var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                    var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                    var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                    var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");
                    if (item.TrangThai == TrangThaiCongVan.Huy)
                    {
                        countHU = countHU + 1;
                    }
                    var bbks = bienBanKSService.GetbyYeuCau(item.MaYeuCau);

                    if (bbks != null)
                    {
                        var ketquaks = ketQuaKSService.GetbyMaYCau(item.MaYeuCau);
                        if (ketquaks != null)
                        {
                            if (!string.IsNullOrWhiteSpace(ketquaks.MA_TNGAI))
                            {
                                countTN = countTN + 1;
                            }
                        }

                    }
                    //Thời gian tiếp nhận
                    int songayTN = CommonUtils.TotalDate(ttrinhTN.NGAY_BDAU.Date, ttrinhTN.NGAY_KTHUC.Value.Date);

                    tongTGTTDN += songayTN;
                    detailTGTTDN += songayTN;
                    tongTGTCDN = tongTGTCDN + songayTN;

                    log.Error($"songayTN: {songayTN}");
                    //Thời gian khảo sát
                    if (ttrinhKS != null && ttrinhKS.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhCH5 != null && ttrinhCH5.NGAY_KTHUC.HasValue)
                        {
                            int songayKS = CommonUtils.TotalDate(ttrinhKS.NGAY_BDAU.Date, ttrinhCH5.NGAY_BDAU.Date);
                            if (ttrinhKS.NGAY_BDAU.Date == ttrinhTN.NGAY_KTHUC.Value.Date)
                                songayKS -= 1;

                            tongTGTTDN += songayKS;
                            detailTGTTDN += songayKS;
                            tongTGTCDN = tongTGTCDN + songayKS;
                        }
                    }

                    if (ttrinhDDN != null && ttrinhDDN.NGAY_KTHUC.HasValue)
                    {
                        int songayTTDN = CommonUtils.TotalDate(ttrinhDDN.NGAY_BDAU.Date, ttrinhDDN.NGAY_KTHUC.Value.Date);
                        if (ttrinhCH5 != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhCH5.NGAY_KTHUC.Value.Date)
                            songayTTDN -= 1;
                        tongTGTTDN += songayTTDN;
                        detailTGTTDN += songayTTDN;
                        tongTGTCDN = tongTGTCDN + songayTTDN;
                        countHT++;
                    }

                    var ycnt = yCauNghiemThuService.GetbyMaYCau(item.MaYeuCau);
                    if (ycnt != null)
                    {
                        countTongKTDK = countTongKTDK + 1;

                        if (ycnt.TrangThai == TrangThaiNghiemThu.Huy)
                        {
                            countHUKTKD = countHUKTKD + 1;
                            if (ttrinhKTR == null)
                            {
                                countTNKTDK = countTNKTDK + 1;
                            }
                        }
                        var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                        if (bbkt != null)
                        {
                            var ketquakt = ketQuaKTService.GetbyMaYCau(item.MaYeuCau);
                            if (ketquakt != null)
                            {
                                if (!string.IsNullOrWhiteSpace(ketquakt.MA_TNGAI))
                                {
                                    countTNKTDK = countTNKTDK + 1;
                                }
                            }
                        }

                        if (ttrinhTVB != null && ttrinhTVB.NGAY_KTHUC.HasValue)
                        {
                            int songay = CommonUtils.TotalDate(ttrinhTVB.NGAY_BDAU.Date, ttrinhTVB.NGAY_KTHUC.Value.Date);
                            tongTGKTDK = tongTGKTDK + songay;
                            detailKTDK = detailKTDK + songay;

                            if (ttrinhKTR != null && ttrinhKTR.NGAY_KTHUC.HasValue)
                            {
                                int songayKTR = CommonUtils.TotalDate(ttrinhTVB.NGAY_BDAU.Date, ttrinhTVB.NGAY_KTHUC.Value.Date);
                                tongTGKTDK = tongTGKTDK + songayKTR;
                                detailKTDK = detailKTDK + songayKTR;
                                countHTKTDK = countHTKTDK + 1;
                            }
                        }

                        //Thời gian khách hàng thi công
                        if (ttrinhPC != null && ttrinhPC.NGAY_KTHUC.HasValue)
                        {
                            if (ttrinhBTT != null && ttrinhBTT.NGAY_KTHUC.HasValue)
                            {
                                DateTime fromTime = ttrinhPC.NGAY_BDAU.Date;
                                if (ttrinhs.Any(p => p.MA_CVIEC == "TC" && !string.IsNullOrWhiteSpace(p.MA_TNGAI)))
                                    fromTime = ttrinhTC.NGAY_BDAU.Date;
                                int songayPC = CommonUtils.TotalDate(fromTime, ttrinhBTT.NGAY_BDAU.Date);
                                if (ttrinhDDN != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                    songayPC -= 1;
                                tongTGNT = tongTGNT + songayPC;
                                detailTGNT = detailTGNT + songayPC;
                                tongTGTCDN = tongTGTCDN + songayPC;
                            }
                        }

                        //Thời gian ký hợp đồng, nghiệm thu
                        if (ttrinhDHD != null && ttrinhDHD.NGAY_KTHUC.HasValue)
                        {
                            if (ttrinhNT != null && ttrinhNT.NGAY_KTHUC.HasValue)
                            {
                                DateTime fromTime = ttrinhDHD.NGAY_BDAU.Date;
                                if (ttrinhBTT != null && fromTime < ttrinhBTT.NGAY_KTHUC.Value.Date)
                                    fromTime = ttrinhBTT.NGAY_KTHUC.Value.Date;
                                int songayNT = CommonUtils.TotalDate(fromTime, ttrinhNT.NGAY_BDAU.Date);

                                if (ttrinhBTT != null && fromTime == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                    songayNT -= 1;
                                tongTGNT = tongTGNT + songayNT;
                                detailTGNT = detailTGNT + songayNT;
                                countHTNT = countHTNT + 1;
                                tongTGTCDN = tongTGTCDN + songayNT;
                                log.Error($"songayNT: {songayNT}");
                            }
                        }
                    }
                    if (detailTGTTDN > 2 || detailTGNT > 1 || detailKTDK > 4)
                    {
                        countQuaHan = countQuaHan + 1;
                        if (detailTGTTDN > 2)
                        {
                            countQuaHanTTDN = countQuaHanTTDN + 1;
                            ngayQuaHanTTDN = ngayQuaHanTTDN + (int)(detailTGTTDN - 2);
                            ngayQuaHan = ngayQuaHan + (int)(detailTGTTDN - 2);
                        }
                        if (detailTGNT > 1)
                        {
                            countQuaHanNT = countQuaHanNT + 1;
                            ngayQuaHanNT = ngayQuaHanNT + (int)(detailTGNT - 1);
                            ngayQuaHan = ngayQuaHan + (int)(detailTGNT - 1);
                        }
                        if (detailKTDK > 4)
                        {
                            countQuaHanKTDK = countQuaHanKTDK + 1;
                            ngayQuaHanKTDK = ngayQuaHanKTDK + (int)(detailKTDK - 4);
                            ngayQuaHan = ngayQuaHan + (int)(detailKTDK - 2);
                        }
                    }
                }


                detail.TongSoCTCoTroNgaiTTDN = countTN;
                detail.TongSoCTDaHoanThanhTTDN = countHT;
                detail.TongSoCTChuaHoanThanhTTDN = listCV.Count() - countHT - countHU;
                detail.TongSoCTTiepNhanKTDK = countTongKTDK;
                detail.TongSoCTCoTroNgaiKTDK = countTNKTDK;
                detail.TongSoCTDaHoanThanhKTDK = countHTKTDK;
                detail.TongSoCTChuaHoanThanhKTDK = countTongKTDK - countHTKTDK - countHUKTKD;

                detail.TongSoCTTiepNhanNT = countTongNT;
                detail.TongSoCTDaHoanThanhNT = countHTNT;
                detail.TongSoCTChuaHoanThanhNT = countTongKTDK - countHTNT - countHUKTKD;

                detail.SoNgayQuaHanTTDN = countQuaHanTTDN;
                detail.SoNgayQuaHanTTDN = ngayQuaHanTTDN;
                detail.SoNgayQuaHanKTDK = countQuaHanKTDK;
                detail.SoNgayQuaHanKTDK = ngayQuaHanKTDK;
                detail.SoNgayQuaHanNT = countQuaHanNT;
                detail.SoNgayQuaHanNT = ngayQuaHanNT;
                detail.SoNgayQuaHan = countQuaHan;
                detail.SoNgayQuaHan = ngayQuaHan;

                if (listCV.Count() > 0)
                {
                    detail.SoNgayThucHienTBTTDN = Math.Round((decimal)(tongTGTTDN / listCV.Count()), 1, MidpointRounding.AwayFromZero);
                    if (countTongKTDK > 0)
                    {
                        detail.SoNgayThucHienTBKTDK = Math.Round((decimal)(tongTGKTDK / countTongKTDK), 1, MidpointRounding.AwayFromZero);
                        if (countHTNT > 0)
                        {
                            detail.SoNgayThucHienTBNT = Math.Round((decimal)(tongTGNT / countHTNT), 1, MidpointRounding.AwayFromZero);
                        }
                    }
                    detail.SoNgayThucHienTBTTDN = Math.Round((decimal)(tongTGTCDN / listCV.Count()), 1, MidpointRounding.AwayFromZero);
                }
                result.Add(detail);
            }
            return result;
        }

        public IList<BaoCaoChiTietTCDN> GetListBaoCaoChiTietQuaHan(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {
            IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IYCauNghiemThuService yCauNghiemThuService = IoC.Resolve<IYCauNghiemThuService>();
            IKetQuaKSService ketQuaKSService = IoC.Resolve<IKetQuaKSService>();
            IKetQuaKTService ketQuaKTService = IoC.Resolve<IKetQuaKTService>();
            ITroNgaiService troNgaiService = IoC.Resolve<ITroNgaiService>();
            IBienBanKTService bienBanKTService = IoC.Resolve<IBienBanKTService>();
            var query = Query.Where(p => p.TrangThai > TrangThaiCongVan.MoiTao);
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);
            if (status > -1)
                query = query.Where(p => p.TrangThai == (TrangThaiCongVan)status);
            if (!string.IsNullOrWhiteSpace(khachhang))
                query = query.Where(p => p.CoQuanChuQuan.Contains(khachhang) || p.NguoiYeuCau.Contains(khachhang) || p.MaKHang.Contains(khachhang));
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.MaYeuCau.Contains(keyword) || p.DuAnDien.Contains(keyword));

            query = query.OrderByDescending(p => p.MaYeuCau);
            var ret = new List<CongVanYeuCau>();
            var result = new List<BaoCaoChiTietTCDN>();
            ret = query.ToList();


            foreach (var item in ret)
            {
                var detail = new BaoCaoChiTietTCDN();
                detail.MaDViQLy = item.MaDViQLy;
                detail.TenKH = item.TenKhachHang;
                detail.TenCT = item.DuAnDien;
                detail.DCCT = item.DiaChiDungDien;
                detail.MaYC = item.MaYeuCau;

                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                if (ttrinhs.Count() == 0) continue;

                var ttrinhTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");

                if (ttrinhTN == null || !ttrinhTN.NGAY_KTHUC.HasValue) continue;

                var ttrinhPK = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                var ttrinhKS = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                var ttrinhCH5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                var ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");
                if (ttrinhBDN == null)
                {
                    ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                }

                var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                var ttrinhTVB = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                var ttrinhKTR = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");

                var bbks = bienBanKSService.GetbyYeuCau(item.MaYeuCau);

                if (bbks != null)
                {
                    detail.TongCS = bbks.TongCongSuat;
                    detail.TongChieuDaiDD = bbks.DayDan;

                    var ketquaks = ketQuaKSService.GetbyMaYCau(item.MaYeuCau);
                    if (ketquaks != null)
                    {
                        if (!string.IsNullOrWhiteSpace(ketquaks.MA_TNGAI))
                        {
                            var trongai = troNgaiService.Getbykey(ketquaks.MA_TNGAI);
                            if (trongai != null)
                            {
                                detail.TroNgaiTTDN = trongai.TEN_TNGAI;
                            }
                        }
                    }
                }

                //Thời gian tiếp nhận
                int songayTN = CommonUtils.TotalDate(ttrinhTN.NGAY_BDAU.Date, ttrinhTN.NGAY_KTHUC.Value.Date);

                detail.TongSoNgayTTDN += songayTN;
                detail.TongSoNgayTCDN += songayTN;

                log.Error($"songayTN: {songayTN}");
                //Thời gian khảo sát
                if (ttrinhKS != null && ttrinhKS.NGAY_KTHUC.HasValue)
                {
                    if (ttrinhCH5 != null && ttrinhCH5.NGAY_KTHUC.HasValue)
                    {
                        int songayKS = CommonUtils.TotalDate(ttrinhKS.NGAY_BDAU.Date, ttrinhCH5.NGAY_BDAU.Date);

                        if (ttrinhKS.NGAY_BDAU.Date == ttrinhTN.NGAY_KTHUC.Value.Date)
                            songayKS -= 1;

                        detail.TongSoNgayTTDN += songayKS;
                        detail.TongSoNgayTCDN += songayKS;
                    }
                }

                if (ttrinhDDN != null && ttrinhDDN.NGAY_KTHUC.HasValue)
                {
                    int songayTTDN = CommonUtils.TotalDate(ttrinhDDN.NGAY_BDAU.Date, ttrinhDDN.NGAY_KTHUC.Value.Date);
                    if (ttrinhCH5 != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhCH5.NGAY_KTHUC.Value.Date)
                        songayTTDN -= 1;
                    detail.TongSoNgayTTDN += songayTTDN;
                    detail.TongSoNgayTCDN += songayTTDN;
                }

                var ycnt = yCauNghiemThuService.GetbyMaYCau(item.MaYeuCau);
                if (ycnt != null)
                {
                    var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                    if (bbkt != null)
                    {
                        var ketquakt = ketQuaKTService.GetbyMaYCau(item.MaYeuCau);
                        if (ketquakt != null)
                        {
                            if (!string.IsNullOrWhiteSpace(ketquakt.MA_TNGAI))
                            {
                                var trongai = troNgaiService.Getbykey(ketquakt.MA_TNGAI);
                                if (trongai != null)
                                {
                                    detail.TroNgaiKTDK = trongai.TEN_TNGAI;
                                }
                            }
                        }

                        if (ttrinhTVB != null && ttrinhTVB.NGAY_KTHUC.HasValue)
                        {
                            int songayTVB = CommonUtils.TotalDate(ttrinhTVB.NGAY_BDAU.Date, ttrinhTVB.NGAY_KTHUC.Value.Date);
                            detail.TongSoNgayKTDK += songayTVB;

                            if (ttrinhKTR != null && ttrinhKTR.NGAY_KTHUC.HasValue)
                            {
                                int songayKTR = CommonUtils.TotalDate(ttrinhKTR.NGAY_BDAU.Date, ttrinhKTR.NGAY_KTHUC.Value.Date);
                                detail.TongSoNgayKTDK += songayKTR;

                            }
                        }
                    }

                    //Thời gian khách hàng thi công
                    if (ttrinhPC != null && ttrinhPC.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhBTT != null && ttrinhBTT.NGAY_KTHUC.HasValue)
                        {
                            int songayPC = CommonUtils.TotalDate(ttrinhPC.NGAY_BDAU.Date, ttrinhBTT.NGAY_BDAU.Date);
                            detail.TongSoNgayNT += songayPC;
                            detail.TongSoNgayTCDN += songayPC;
                            log.Error($"songayPC: {songayPC}");
                        }
                    }

                    //Thời gian ký hợp đồng, nghiệm thu
                    if (ttrinhDHD != null && ttrinhDHD.NGAY_KTHUC.HasValue)
                    {
                        if (ttrinhNT != null && ttrinhNT.NGAY_KTHUC.HasValue)
                        {
                            DateTime fromTime = ttrinhDHD.NGAY_BDAU.Date;
                            if (ttrinhBTT != null && fromTime < ttrinhBTT.NGAY_KTHUC.Value.Date)
                                fromTime = ttrinhBTT.NGAY_KTHUC.Value.Date;
                            int songayNT = CommonUtils.TotalDate(fromTime, ttrinhNT.NGAY_BDAU.Date);
                            if (ttrinhBTT != null && fromTime == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                songayNT -= 1;
                            detail.TongSoNgayNT += songayNT;
                            detail.TongSoNgayTCDN += songayNT;
                            log.Error($"songayNT: {songayNT}");
                        }
                    }
                }

                if (detail.TongSoNgayTTDN > 2)
                {
                    detail.HangMucQuaHan = "Thoả thuận đấu nối";
                    detail.SoNgayQuaHan = detail.TongSoNgayTTDN - 2;
                }
                if (detail.TongSoNgayKTDK > 4)
                {
                    detail.HangMucQuaHan = "Kiểm tra điều kiện";
                    detail.SoNgayQuaHan = detail.TongSoNgayNT - 2;
                }
                if (detail.TongSoNgayNT > 1)
                {
                    detail.HangMucQuaHan = "Nghiệm thu";
                    detail.SoNgayQuaHan = detail.TongSoNgayNT - 1;
                }
                result.Add(detail);
            }
            result = result.Where(x => x.SoNgayQuaHan > 0).ToList();
            if (pagesize > 0)
            {
                int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
                result = result.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
                total = pageindex * pagesize + result.Count;
                return result.Take(pagesize).ToList();
            }
            else
            {
                total = result.Count();
                return result;

            }
        }

        public IList<BaoCaoTHQuaHan> GetListBaoCaoTHQuaHan(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate)
        {
            var list = new List<BaoCaoTHQuaHan>();
            return list;
        }

        public IList<BaoCaoTTDN> GetListTiepNhan(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {
            IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IKetQuaKSService ketQuaKSService = IoC.Resolve<IKetQuaKSService>();
            ITroNgaiService troNgaiService = IoC.Resolve<ITroNgaiService>();
            var query = Query.Where(p => p.TrangThai > TrangThaiCongVan.MoiTao);
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);
            if (status > -1)
                query = query.Where(p => p.TrangThai == (TrangThaiCongVan)status);
            if (!string.IsNullOrWhiteSpace(khachhang))
                query = query.Where(p => p.CoQuanChuQuan.Contains(khachhang) || p.NguoiYeuCau.Contains(khachhang) || p.MaKHang.Contains(khachhang));
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.MaYeuCau.Contains(keyword) || p.DuAnDien.Contains(keyword));

            query = query.OrderByDescending(p => p.MaYeuCau);
            var ret = new List<CongVanYeuCau>();
            var result = new List<BaoCaoTTDN>();
            ret = query.ToList();

            foreach (var item in ret)
            {
                var detail = new BaoCaoTTDN();
                detail.MaDViQLy = item.MaDViQLy;
                detail.TenKH = item.TenKhachHang;
                detail.TenCT = item.DuAnDien;
                detail.DCCT = item.DiaChiDungDien;
                detail.MaYC = item.MaYeuCau;

                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                if (ttrinhs.Count() == 0) continue;
                var trongais = ttrinhs.Where(p => !string.IsNullOrWhiteSpace(p.MA_TNGAI)).ToList();

                var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                if (ttrinhtnhs == null || !ttrinhtnhs.NGAY_KTHUC.HasValue) continue;

                var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");

                int songayTN = CommonUtils.TotalDate(ttrinhtnhs.NGAY_BDAU.Date, ttrinhtnhs.NGAY_KTHUC.Value.Date);
                detail.ThoiGianTNHS = songayTN;
                detail.SoNgayTNHS = songayTN;
                detail.TongSoNgayTTDN += songayTN;

                result.Add(detail);
            }
            if (pagesize > 0)
            {
                int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
                result = result.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
                total = pageindex * pagesize + result.Count;
                return result.Take(pagesize).ToList();
            }
            else
            {
                total = result.Count();
                return result;
            }
        }
        public IList<BaoCaoTTDN> GetListKhaoSat(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {
            IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IKetQuaKSService ketQuaKSService = IoC.Resolve<IKetQuaKSService>();
            ITroNgaiService troNgaiService = IoC.Resolve<ITroNgaiService>();
            var query = Query.Where(p => p.TrangThai > TrangThaiCongVan.MoiTao);
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);
            if (status > -1)
                query = query.Where(p => p.TrangThai == (TrangThaiCongVan)status);
            if (!string.IsNullOrWhiteSpace(khachhang))
                query = query.Where(p => p.CoQuanChuQuan.Contains(khachhang) || p.NguoiYeuCau.Contains(khachhang) || p.MaKHang.Contains(khachhang));
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.MaYeuCau.Contains(keyword) || p.DuAnDien.Contains(keyword));

            query = query.OrderByDescending(p => p.MaYeuCau);
            var ret = new List<CongVanYeuCau>();
            var result = new List<BaoCaoTTDN>();
            ret = query.ToList();

            foreach (var item in ret)
            {
                var detail = new BaoCaoTTDN();
                detail.MaDViQLy = item.MaDViQLy;
                detail.TenKH = item.TenKhachHang;
                detail.TenCT = item.DuAnDien;
                detail.DCCT = item.DiaChiDungDien;
                detail.MaYC = item.MaYeuCau;

                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                if (ttrinhs.Count() == 0) continue;
                var trongais = ttrinhs.Where(p => !string.IsNullOrWhiteSpace(p.MA_TNGAI)).ToList();

                var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                if (ttrinhtnhs == null || !ttrinhtnhs.NGAY_KTHUC.HasValue) continue;

                var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");




                if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                {
                    if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                    {
                        int songayKS = CommonUtils.TotalDate(ttrinhks.NGAY_BDAU.Date, ttrinhch5.NGAY_BDAU.Date);
                        detail.SoNgayThucHienKS = songayKS;
                        if (ttrinhks.NGAY_BDAU.Date == ttrinhtnhs.NGAY_KTHUC.Value.Date)
                            songayKS -= 1;
                        detail.TongSoNgayTTDN += songayKS;
                    }
                }
                result.Add(detail);
            }
            if (pagesize > 0)
            {
                int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
                result = result.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
                total = pageindex * pagesize + result.Count;
                return result.Take(pagesize).ToList();
            }
            else
            {
                total = result.Count();
                return result;
            }
        }

        public IList<CongVanYeuCau> TinhThoiGian2()
        {
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IYCauNghiemThuService yCauNghiemThuService = IoC.Resolve<IYCauNghiemThuService>();
            INgayNghiLeService ngayNghiLeService = IoC.Resolve<INgayNghiLeService>();
            ILoaiCanhBaoService dLoaicanhbao = IoC.Resolve<ILoaiCanhBaoService>();

            var now = DateTime.Now;

            //setting ngày lễ value phải lưu dưới dạng ngày/tháng. Nhiều giá trị thì phải cách nhau dấu ,
            var data = ngayNghiLeService.GetNgayLe(Constants.KeyNgayLe);
            var ngayle = data.VALUE.Split(',').ToList(); //lấy được list ngày dạng day/month
            var ngayNghi = ConvertDate(ngayle);

            //ngày đầu năm
            var ngayDauNam = new DateTime(now.Year, 1, 1);
            //ngày cuối năm
            var ngayCuoiNam = new DateTime(now.Year, 12, 31);
            //lấy ngày
            var dayWeekends = DateExtensions.GetWeekendDates(ngayDauNam, ngayCuoiNam);
            //danh sách ngày nghỉ

            ngayNghi.AddRange(dayWeekends);


            var response = new List<CongVanYeuCau>();


            DateTime ngay = DateTime.ParseExact("01/04/2024", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.TrangThai >= TrangThaiCongVan.MoiTao && p.NgayLap >= ngay).ToList();
            foreach (var item in query)
            {
                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                var ttrinhTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                var ttrinhPK = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                var ttrinhKS = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                var ttrinhCH5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                var ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");
                if (ttrinhBDN == null)
                {
                    ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                }
                var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                var ttrinhTVB = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                var ttrinhKTR = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                var ttrinhMNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "MNT");
                var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");

                //Nếu ngày hiện tại là ngay nghỉ
                if (ngayNghi.Contains(now.Date) || DateTime.Now.Hour <= 8 || DateTime.Now.Hour >= 17)
                {

                }
                else
                {

                        TimeSpan ts = DateTime.Now - item.NgayLap;
                        if (ts.TotalMinutes > 30)
                        {
                            if (item.TrangThai == TrangThaiCongVan.MoiTao)
                            {
                                item.LoaiCanhBao = 1;
                                response.Add(item);
                            }
                        }
                    

                    // lập thỏa thuận đấu nối quá 48h
                    TimeSpan tsthoathuanDN = new TimeSpan();
                    if (ttrinhBDN == null)
                    {
                        if (ttrinhKS != null)
                        {

                            tsthoathuanDN = DateTime.Now - ttrinhKS.NGAY_TAO;
                        }

                    }
                    else
                    {
                        if (ttrinhKS != null && ttrinhBDN.NGAY_KTHUC.HasValue)
                        {
                            tsthoathuanDN = ttrinhBDN.NGAY_TAO - ttrinhKS.NGAY_TAO;
                        }
                    }
                        if (tsthoathuanDN.TotalMinutes > 30) 
                        {

                            if (item.TrangThai >= TrangThaiCongVan.MoiTao && item.TrangThai < TrangThaiCongVan.HoanThanh)
                            {
                                item.LoaiCanhBao = 2;
                                response.Add(item);
                            }

                        }
                    

                    // Thời gian tiếp nhận yêu cầu kiểm tra đóng điện và nghiệm thu quá 2h
                    var itemNT = yCauNghiemThuService.GetbyMaYCau(item.MaYeuCau);
                    if (itemNT != null)
                    {
                      
                            TimeSpan tsNT = DateTime.Now - itemNT.NgayLap;
                            if (tsNT.TotalMinutes > 30)
                            {
                                if (itemNT.TrangThai == TrangThaiNghiemThu.MoiTao)
                                {
                                    item.LoaiCanhBao = 3;
                                    response.Add(item);
                                }
                            }
                        


                        //Thời gian dự thảo và ký hợp đồng mua bán điện
                        //dự thảo và ký hợp đồng
                        TimeSpan tsKyHopDong = new TimeSpan();
                        if (ttrinhDHD == null)
                        {
                            if (ttrinhPC != null)
                            {
                                tsKyHopDong = DateTime.Now - ttrinhPC.NGAY_TAO;
                            }

                        }
                        else
                        {
                            if (ttrinhPC != null && ttrinhDHD.NGAY_KTHUC.HasValue)
                            {
                                tsKyHopDong = ttrinhDHD.NGAY_TAO - ttrinhPC.NGAY_TAO;
                            }

                        }

                            if (tsKyHopDong.TotalMinutes > 30) // TEST UAT
                            {
                                if (itemNT.TrangThai >= TrangThaiNghiemThu.PhanCongTC && itemNT.TrangThai <= TrangThaiNghiemThu.NghiemThu)
                                {
                                    item.LoaiCanhBao = 4;
                                    response.Add(item);
                                }
                            }
                        

                        // Thời gian thực hiện kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu

                        TimeSpan tsKyNghiemThu = new TimeSpan();
                        if (ttrinhNT == null)
                        {
                            if (ttrinhTVB != null)
                            {
                                tsKyNghiemThu = DateTime.Now - ttrinhTVB.NGAY_TAO;
                            }

                        }
                        else
                        {
                            if (ttrinhTVB != null && ttrinhNT.NGAY_KTHUC.HasValue)
                            {
                                tsKyNghiemThu = ttrinhNT.NGAY_TAO - ttrinhTVB.NGAY_TAO;
                            }

                        }


                            if (tsKyNghiemThu.TotalMinutes > 30)
                            {
                                if (itemNT.TrangThai >= TrangThaiNghiemThu.PhanCongKT && itemNT.TrangThai <= TrangThaiNghiemThu.HoanThanh)
                                {
                                    item.LoaiCanhBao = 5;
                                    response.Add(item);
                                }
                            }
                        
                        //Giám sát thời gian nghiệm thu yêu cầu cấp điện mới trung áp
                        TimeSpan tsGSNghiemThu = new TimeSpan();
                        if (ttrinhNT == null)
                        {
                            tsGSNghiemThu = DateTime.Now - itemNT.NgayLap;
                        }
                        else
                        {
                            if (ttrinhNT.NGAY_KTHUC.HasValue)
                            {
                                tsGSNghiemThu = ttrinhNT.NGAY_TAO - itemNT.NgayLap;
                            }

                        }

                 
                            if (tsKyNghiemThu.TotalMinutes > 30) //test UAT
                            {
                                if (itemNT.TrangThai >= TrangThaiNghiemThu.TiepNhan && itemNT.TrangThai <= TrangThaiNghiemThu.HoanThanh)
                                {
                                    item.LoaiCanhBao = 6;
                                    response.Add(item);
                                }
                            }
                        
                    }


                    //thời gian cảnh báo các bộ hồ sơ sắp hết hạn hiệu lực thỏa thuận đấu nối
                    TimeSpan tsCanhBaoHetHanTTDN = new TimeSpan();
                    if (ttrinhDDN != null && ttrinhDDN.NGAY_KTHUC.HasValue)
                    {
                        tsCanhBaoHetHanTTDN = DateTime.Now - ttrinhDDN.NGAY_TAO;
                    }

                        if (tsCanhBaoHetHanTTDN.TotalMinutes > 30) //TEST UAT
                        {

                            if (item.TrangThai == TrangThaiCongVan.HoanThanh)
                            {
                                item.LoaiCanhBao = 16;
                                response.Add(item);
                            }
                        
                    }


                    // Thời gian thực hiện cấp điện mới trung áp
                    var TongSoNgayTCDN = 0;
                    if (ttrinhTN != null && ttrinhTN.NGAY_KTHUC.HasValue)
                    {
                        int songayTN = CommonUtils.TotalDate(ttrinhTN.NGAY_BDAU.Date, ttrinhTN.NGAY_KTHUC.Value.Date);


                        TongSoNgayTCDN += songayTN;

                        log.Error($"songayTN: {songayTN}");
                        //Thời gian khảo sát
                        if (ttrinhKS != null && ttrinhKS.NGAY_KTHUC.HasValue)
                        {
                            if (ttrinhCH5 != null && ttrinhCH5.NGAY_KTHUC.HasValue)
                            {
                                int songayKS = CommonUtils.TotalDate(ttrinhKS.NGAY_BDAU.Date, ttrinhCH5.NGAY_BDAU.Date);

                                if (ttrinhKS.NGAY_BDAU.Date == ttrinhTN.NGAY_KTHUC.Value.Date)
                                    songayKS -= 1;


                                TongSoNgayTCDN += songayKS;
                            }
                        }

                        if (ttrinhDDN != null && ttrinhDDN.NGAY_KTHUC.HasValue)
                        {
                            int songayTTDN = CommonUtils.TotalDate(ttrinhDDN.NGAY_BDAU.Date, ttrinhDDN.NGAY_KTHUC.Value.Date);
                            if (ttrinhCH5 != null && ttrinhDDN.NGAY_BDAU.Date == ttrinhCH5.NGAY_KTHUC.Value.Date)
                                songayTTDN -= 1;

                            TongSoNgayTCDN += songayTTDN;
                        }

                        if (itemNT != null)
                        {
                            //Thời gian khách hàng thi công
                            if (ttrinhPC != null && ttrinhPC.NGAY_KTHUC.HasValue)
                            {
                                if (ttrinhBTT != null && ttrinhBTT.NGAY_KTHUC.HasValue)
                                {
                                    int songayPC = CommonUtils.TotalDate(ttrinhPC.NGAY_BDAU.Date, ttrinhBTT.NGAY_BDAU.Date);

                                    TongSoNgayTCDN += songayPC;
                                    log.Error($"songayPC: {songayPC}");
                                }
                            }

                            //Thời gian ký hợp đồng, nghiệm thu
                            if (ttrinhDHD != null && ttrinhDHD.NGAY_KTHUC.HasValue)
                            {
                                if (ttrinhNT != null && ttrinhNT.NGAY_KTHUC.HasValue)
                                {
                                    DateTime fromTime = ttrinhDHD.NGAY_BDAU.Date;
                                    if (ttrinhBTT != null && fromTime < ttrinhBTT.NGAY_KTHUC.Value.Date)
                                        fromTime = ttrinhBTT.NGAY_KTHUC.Value.Date;
                                    int songayNT = CommonUtils.TotalDate(fromTime, ttrinhNT.NGAY_BDAU.Date);
                                    if (ttrinhBTT != null && fromTime == ttrinhBTT.NGAY_KTHUC.Value.Date)
                                        songayNT -= 1;

                                    TongSoNgayTCDN += songayNT;
                                    log.Error($"songayNT: {songayNT}");
                                }
                            }

                        }
                            if (TongSoNgayTCDN > 4)
                            {
                                item.LoaiCanhBao = 15;
                                response.Add(item);
                            }
                        
                    }

                }
            }
            return response;
        }

        static bool IsWeekend_Thursday(DateTime date)
        {
            // Kiểm tra nếu ngày trong tuần là Thứ Năm (Thursday) 
            return date.DayOfWeek == DayOfWeek.Thursday;
        }

        static bool IsWeekend_Friday(DateTime date)
        {
            // Kiểm tra nếu ngày trong tuần là Thứ Sáu (Friday) 
            return date.DayOfWeek == DayOfWeek.Friday;
        }

        public IList<CongVanYeuCau> TinhThoiGian()
        {
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IYCauNghiemThuService yCauNghiemThuService = IoC.Resolve<IYCauNghiemThuService>();
            INgayNghiLeService ngayNghiLeService = IoC.Resolve<INgayNghiLeService>();
            ILoaiCanhBaoService dLoaicanhbao = IoC.Resolve<ILoaiCanhBaoService>();

            var now = DateTime.Now;

            //setting ngày lễ value phải lưu dưới dạng ngày/tháng. Nhiều giá trị thì phải cách nhau dấu ,
            var data = ngayNghiLeService.GetNgayLe(Constants.KeyNgayLe);
            var ngayle = data.VALUE.Split(',').ToList(); //lấy được list ngày dạng day/month
            var ngayNghi = ConvertDate(ngayle);

            //ngày đầu năm
            var ngayDauNam = new DateTime(now.Year, 1, 1);
            //ngày cuối năm
            var ngayCuoiNam = new DateTime(now.Year, 12, 31);
            //lấy ngày
            var dayWeekends = DateExtensions.GetWeekendDates(ngayDauNam, ngayCuoiNam);
            //danh sách ngày nghỉ

            ngayNghi.AddRange(dayWeekends);


            var response = new List<CongVanYeuCau>();
            DateTime ngay = DateTime.ParseExact("01/04/2024", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var query = Query.Where(p => p.TrangThai >= TrangThaiCongVan.MoiTao && p.NgayLap >= ngay).ToList();
            foreach (var item in query)
            {
                var ttrinhs = ttrinhsrv.Query.Where(p =>  p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).ToList();
             
                var ttrinhTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");// Tiếp nhận thỏa thuân đấu nối
                var ttrinhPK = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");// Phân công
                var ttrinhKS = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");// Khảo sát
                var ttrinhCH5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");// Khảo sát thuận lợi
                var ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");// Biên bản thỏa thuận đấu nối
                if (ttrinhBDN == null)
                {
                    ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");// KH ký thỏa thuận đấu nối
                }
                var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                var ttrinhKDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                var ttrinhTVB = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");// Tiếp nhận văn bản đề nghị kiểm tra
                var ttrinhKTR = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");// Biên bản kiểm tra
                var ttrinhDTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DTN");// Biên bản kiểm tra
                var ttrinhMNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "MNT");// Phát hành thư mời
                var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");// Phân công
                var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");// Thi công
                var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");// BB treo tháo
                var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");// Ngành điện ký
                var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");// Hoàn thành nghiệm thu

                //Nếu ngày hiện tại là ngay nghỉ
                if (ngayNghi.Contains(now.Date) || DateTime.Now.Hour <= 8 || DateTime.Now.Hour >= 17)
                {

                }
             
                else
                {
                    var lcanhbao = dLoaicanhbao.Query.Where(p => p.TRANGTHAI == 0);
                    var lcanhbao1 = lcanhbao.FirstOrDefault(p => p.ID == 1);
                    var lcanhbao2 = lcanhbao.FirstOrDefault(p => p.ID == 2);
                    var lcanhbao3 = lcanhbao.FirstOrDefault(p => p.ID == 3);
                    var lcanhbao4 = lcanhbao.FirstOrDefault(p => p.ID == 4);
                    var lcanhbao5 = lcanhbao.FirstOrDefault(p => p.ID == 5);
                    var lcanhbao6 = lcanhbao.FirstOrDefault(p => p.ID == 6);
                    var lcanhbao16 = lcanhbao.FirstOrDefault(p => p.ID == 16);
                    var lcanhbao15 = lcanhbao.FirstOrDefault(p => p.ID == 15);

                    if (lcanhbao1.ID == 1)
                    {     
                        TimeSpan ts = DateTime.Now - item.NgayLap;
                            if (ts.TotalHours >= lcanhbao1.CHUKYCANHBAO)
                            {

                                    if (item.TrangThai == TrangThaiCongVan.MoiTao)
                                {
                                    item.LoaiCanhBao = 1;
                                    response.Add(item);
                                    
                                }
                            }
                    }

                  
                        // lập thỏa thuận đấu nối quá 48h
                        TimeSpan tsthoathuanDN = new TimeSpan();
                        if (ttrinhBDN == null)
                        {
                            if (ttrinhKS != null)
                            {

                                tsthoathuanDN = DateTime.Now - ttrinhKS.NGAY_TAO;
                            }

                        }
                        else
                        {
                            if (ttrinhKS != null && ttrinhBDN.NGAY_KTHUC.HasValue)
                            {
                                tsthoathuanDN = ttrinhBDN.NGAY_TAO - ttrinhKS.NGAY_TAO;
                            }
                        }


                    if (lcanhbao2.ID == 2)
                    {

                        if(ttrinhDDN == null)
                        {
                            if (ttrinhKS != null)
                            {
                                if (IsWeekend_Friday(ttrinhKS.NGAY_TAO))
                                {
                                    if (tsthoathuanDN.TotalHours > lcanhbao2.CHUKYCANHBAO + 24)
                                    {

                                        if (item.TrangThai >= TrangThaiCongVan.MoiTao && item.TrangThai < TrangThaiCongVan.HoanThanh)
                                        {
                                            item.LoaiCanhBao = 2;
                                            response.Add(item);
                                        }

                                    }
                                }
                                else
                                {

                                    if (tsthoathuanDN.TotalHours > lcanhbao2.CHUKYCANHBAO)
                                    {

                                        if (item.TrangThai >= TrangThaiCongVan.MoiTao && item.TrangThai < TrangThaiCongVan.HoanThanh)
                                        {
                                            item.LoaiCanhBao = 2;
                                            response.Add(item);
                                        }

                                    }
                                }
                            }
                         
                        }
                    }

                        // Thời gian tiếp nhận yêu cầu kiểm tra đóng điện và nghiệm thu quá 2h
                    var itemNT = yCauNghiemThuService.GetbyMaYCau(item.MaYeuCau);
                    if (itemNT != null)
                    {
                        if (lcanhbao3.ID == 3)
                        {
                            if (ttrinhTVB == null) 
                            { 
                                TimeSpan tsNT = DateTime.Now - itemNT.NgayLap;
                                if (tsNT.TotalHours >= lcanhbao3.CHUKYCANHBAO)
                                {
                                    if (itemNT.TrangThai == TrangThaiNghiemThu.MoiTao)
                                    {
                                        item.LoaiCanhBao = 3;
                                        response.Add(item);
                                    }
                                }
                            }
                        }


                        //Thời gian dự thảo và ký hợp đồng mua bán điện
                        //dự thảo và ký hợp đồng
                        TimeSpan tsKyHopDong = new TimeSpan();

                        if (ttrinhDHD == null)
                        {
                            if (ttrinhPC != null)
                            {
                                tsKyHopDong = DateTime.Now - ttrinhPC.NGAY_TAO;
                            }

                            if (lcanhbao4.ID == 4)
                            {
                                if (tsKyHopDong.TotalHours > lcanhbao4.CHUKYCANHBAO)
                                {
                                    if (itemNT.TrangThai >= TrangThaiNghiemThu.PhanCongTC && itemNT.TrangThai <= TrangThaiNghiemThu.NghiemThu)
                                    {
                                        var clonedItem = new CongVanYeuCau
                                        {
                                            MaYeuCau = item.MaYeuCau,
                                            TrangThai = item.TrangThai,
                                            NgayLap = item.NgayLap,
                                            LoaiCanhBao = 4,
                                            MaDViQLy = item.MaDViQLy
                                        };
                                        response.Add(clonedItem);


                                    }
                                }
                            }
                        }


                        // Thời gian thực hiện kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu
                        TimeSpan tsKyNghiemThu = new TimeSpan();

                        if (ttrinhDTN == null)
                        {
                            if (ttrinhTVB != null)
                            {
                                tsKyNghiemThu = DateTime.Now - ttrinhTVB.NGAY_TAO;
                            }

                            if (lcanhbao5.ID == 5)
                            {

                                if (ttrinhTVB != null)
                                {
                                    if (IsWeekend_Friday(ttrinhTVB.NGAY_TAO))
                                    {
                                        if (tsKyNghiemThu.TotalHours > lcanhbao5.CHUKYCANHBAO + 24)
                                        {
                                            if (itemNT.TrangThai >= TrangThaiNghiemThu.TiepNhan && itemNT.TrangThai < TrangThaiNghiemThu.DuThaoHD)
                                            {
                                                item.LoaiCanhBao = 5;
                                                response.Add(item);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (tsKyNghiemThu.TotalHours > lcanhbao5.CHUKYCANHBAO)
                                        {
                                            if (itemNT.TrangThai >= TrangThaiNghiemThu.TiepNhan && itemNT.TrangThai < TrangThaiNghiemThu.DuThaoHD)
                                            {
                                                item.LoaiCanhBao = 5;
                                                response.Add(item);
                                            }
                                        }
                                    }
                                }
                            
                            }
                        }
                        
                        //Giám sát thời gian nghiệm thu yêu cầu cấp điện mới trung áp

                        TimeSpan tsGSNghiemThu = new TimeSpan();
                        if (ttrinhNT == null)
                        {
                            tsGSNghiemThu = DateTime.Now - itemNT.NgayLap;

                            if (lcanhbao6.ID == 6)
                            {
                                //if (tsKyNghiemThu.TotalHours > 48) // CODE GỐC
                                //{
                                if (itemNT != null)
                                {
                                    if (IsWeekend_Thursday(itemNT.NgayLap))
                                    {
                                        if (tsKyNghiemThu.TotalHours > lcanhbao6.CHUKYCANHBAO + 48) //test UAT
                                        {
                                            if (itemNT.TrangThai >= TrangThaiNghiemThu.TiepNhan && itemNT.TrangThai < TrangThaiNghiemThu.EVNKyHD)
                                            {
                                                item.LoaiCanhBao = 6;
                                                response.Add(item);
                                            }
                                        }
                                    }

                                    else if (IsWeekend_Friday(itemNT.NgayLap))
                                    {
                                        if (tsKyNghiemThu.TotalHours > lcanhbao6.CHUKYCANHBAO + 24) //test UAT
                                        {
                                            if (itemNT.TrangThai >= TrangThaiNghiemThu.TiepNhan && itemNT.TrangThai < TrangThaiNghiemThu.EVNKyHD)
                                            {
                                                item.LoaiCanhBao = 6;
                                                response.Add(item);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (tsKyNghiemThu.TotalHours > lcanhbao6.CHUKYCANHBAO) //test UAT
                                        {
                                            if (itemNT.TrangThai >= TrangThaiNghiemThu.TiepNhan && itemNT.TrangThai < TrangThaiNghiemThu.HoanThanh)
                                            {
                                                item.LoaiCanhBao = 6;
                                                response.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //thời gian cảnh báo các bộ hồ sơ sắp hết hạn hiệu lực thỏa thuận đấu nối

                    TimeSpan tsCanhBaoHetHanTTDN = new TimeSpan();
                    if (ttrinhKDN == null)
                    {
                        if (ttrinhTN != null && ttrinhTN.NGAY_KTHUC.HasValue)
                        {
                            tsCanhBaoHetHanTTDN = DateTime.Now - ttrinhTN.NGAY_TAO;
                        }
                        if (lcanhbao16.ID == 16)
                        {
                            //if (tsCanhBaoHetHanTTDN.TotalDays > 730) // CODE GỐC
                            //{
                            if (tsCanhBaoHetHanTTDN.TotalDays > lcanhbao16.CHUKYCANHBAO) //TEST UAT
                            {

                                if (item.TrangThai == TrangThaiCongVan.HoanThanh)
                                {
                                    item.LoaiCanhBao = 16;
                                    response.Add(item);
                                }
                            }
                        }
                    }
                    // Thời gian thực hiện cấp điện mới trung áp
                    TimeSpan tsNghiemThu = new TimeSpan();
                    if (ttrinhNT == null)
                    {
                            if (ttrinhTVB != null)
                            {
                                tsNghiemThu = DateTime.Now - ttrinhTVB.NGAY_TAO;
                            }

                            if (lcanhbao15.ID == 15)
                            {

                            if (tsNghiemThu.TotalHours > lcanhbao15.CHUKYCANHBAO)
                            {
                                if (itemNT.TrangThai >= TrangThaiNghiemThu.TiepNhan && itemNT.TrangThai < TrangThaiNghiemThu.HoanThanh)
                                {
                                    var clonedItem = new CongVanYeuCau
                                    {
                                        MaYeuCau = item.MaYeuCau,
                                        TrangThai = item.TrangThai,
                                        NgayLap = item.NgayLap,
                                        LoaiCanhBao = 15,
                                        MaDViQLy = item.MaDViQLy
                                    };
                                    response.Add(clonedItem);

                                }
                            }
                            }
                    }

                }
            }
            return response;
        }

        public IList<CongVanYeuCau> TinhThoiGianCanhBao()
        {
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IYCauNghiemThuService yCauNghiemThuService = IoC.Resolve<IYCauNghiemThuService>();
            INgayNghiLeService ngayNghiLeService = IoC.Resolve<INgayNghiLeService>();
            ILoaiCanhBaoService dLoaicanhbao = IoC.Resolve<ILoaiCanhBaoService>();

            var now = DateTime.Now;
            //setting ngày lễ value phải lưu dưới dạng ngày/tháng. Nhiều giá trị thì phải cách nhau dấu ,
            var data = ngayNghiLeService.GetNgayLe(Constants.KeyNgayLe);
            var ngayle = data.VALUE.Split(',').ToList(); //lấy được list ngày dạng day/month
            var ngayNghi = ConvertDate(ngayle);

            //ngày đầu năm
            var ngayDauNam = new DateTime(now.Year, 1, 1);
            //ngày cuối năm
            var ngayCuoiNam = new DateTime(now.Year, 12, 31);
            //lấy ngày
            var dayWeekends = DateExtensions.GetWeekendDates(ngayDauNam, ngayCuoiNam);
            //danh sách ngày nghỉ

            ngayNghi.AddRange(dayWeekends);

            CongVanYeuCau congVan = new CongVanYeuCau();

            var response = new List<CongVanYeuCau>();

            //Nếu ngày hiện tại là ngay nghỉ
            if (ngayNghi.Contains(now.Date) || DateTime.Now.Hour <= 8 || DateTime.Now.Hour >= 17)
            {
                return response;
            }

            DateTime ngay = DateTime.ParseExact("01/04/2024", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.TrangThai >= TrangThaiCongVan.MoiTao && p.NgayLap >= ngay && p.TrangThai != TrangThaiCongVan.Huy).ToList();

            TimeSpan ts = new TimeSpan();
            var chukycanhbao = 0;
            foreach (var item in query)
            {
                // Lấy thông tin bảng DVTIENTRINH
                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau).OrderByDescending(p => p.STT).ToList();
                var ttnt = yCauNghiemThuService.Query.Where(p => p.MaYeuCau == item.MaYeuCau).FirstOrDefault();
                if (ttrinhs.Count == 0)
                {
                    if (item.TrangThai == TrangThaiCongVan.MoiTao)
                    {
                        ts = DateTime.Now - item.NgayLap;
                        if (ts.TotalHours >= dLoaicanhbao.Query.Where(p => p.ID == 1).FirstOrDefault().CHUKYCANHBAO)
                        {
                            item.LoaiCanhBao = 1;
                            response.Add(item);
                        }

                    }
                }
                else
                {
                    var ttrinhTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                    var ttrinhPK = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                    var ttrinhKS = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                    var ttrinhCH5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                    var ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN1");
                    if (ttrinhBDN == null)
                    {
                        ttrinhBDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    }
                    var ttrinhDTN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DTN");
                    var ttrinhKDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    var ttrinhDDN = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                    var ttrinhTVB = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                    var ttrinhKTR = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                    var ttrinhMNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "MNT");
                    var ttrinhPC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                    var ttrinhTC = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                    var ttrinhBTT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                    var ttrinhDHD = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                    var ttrinhNT = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");

                    if (ttrinhKS != null && ttrinhTN != null && ttrinhCH5 == null && item.TrangThai >= TrangThaiCongVan.TiepNhan && item.TrangThai < TrangThaiCongVan.HoanThanh)
                    {
                        ts = DateTime.Now - ttrinhKS.NGAY_TAO;
                        if (IsWeekend_Friday(ttrinhKS.NGAY_TAO))
                        {
                            chukycanhbao = dLoaicanhbao.Query.FirstOrDefault(p => p.ID == 2).CHUKYCANHBAO + 24;
                        }
                        else
                        {
                            chukycanhbao = dLoaicanhbao.Query.FirstOrDefault(p => p.ID == 2).CHUKYCANHBAO;
                        }
                        if(ts.TotalHours >= chukycanhbao)
                        {
                            item.LoaiCanhBao = 2;
                            response.Add(item);
                        }
                    }
                    else if (ttrinhTVB == null && ttnt != null && ttnt.TrangThai == 0)
                    {
                        ts = DateTime.Now - ttnt.NgayLap;
                        chukycanhbao = dLoaicanhbao.Query.FirstOrDefault(p => p.ID == 3).CHUKYCANHBAO;
                        if( ts.TotalHours >= chukycanhbao)
                        {
                            item.LoaiCanhBao = 3;
                            response.Add(item);
                        }

                    }
                    else if (ttrinhTVB != null && ttnt != null && ttnt.TrangThai >= TrangThaiNghiemThu.TiepNhan && ttnt.TrangThai != TrangThaiNghiemThu.Huy)
                    {
                        ts = DateTime.Now - ttrinhTVB.NGAY_TAO;
                        var chukycanhbao5 = dLoaicanhbao.Query.FirstOrDefault(p => p.ID == 5).CHUKYCANHBAO;
                        var chukycanhbao6 = dLoaicanhbao.Query.FirstOrDefault(p => p.ID == 6).CHUKYCANHBAO;
                        var chukycanhbao15 = dLoaicanhbao.Query.FirstOrDefault(p => p.ID == 15).CHUKYCANHBAO * 24;
                        if (IsWeekend_Friday(ttrinhTVB.NGAY_TAO))
                        {
                            chukycanhbao6 += 48;
                            chukycanhbao15 += 48;
                            chukycanhbao5 += 24;
                        }
                        if (IsWeekend_Thursday(ttrinhTVB.NGAY_TAO))
                        {
                            chukycanhbao6 += 24;
                            chukycanhbao15 += 24;
                        }
                        if (ts.TotalHours >= chukycanhbao6 && ttrinhNT == null && ttnt != null && ttnt.TrangThai >= TrangThaiNghiemThu.TiepNhan && ttnt.TrangThai <= TrangThaiNghiemThu.HoanThanh)
                        {
                            if (ts.TotalHours <= chukycanhbao6 + 2)
                            {
                                item.LoaiCanhBao = 6;
                                response.Add(item);
                            }
                        }
                        else if (ts.TotalHours >= chukycanhbao15 && ttrinhNT == null && ttnt.TrangThai >= TrangThaiNghiemThu.TiepNhan && ttnt.TrangThai <= TrangThaiNghiemThu.EVNKyHD )
                        {
                            if (ts.TotalHours <= chukycanhbao15 + 2)
                            {
                                item.LoaiCanhBao = 15;
                                response.Add(item);
                            }
                        }

                        else if (ts.TotalHours >= chukycanhbao5 && ttrinhKTR == null && ttnt.TrangThai >= TrangThaiNghiemThu.TiepNhan && ttnt.TrangThai <= TrangThaiNghiemThu.BienBanKT)
                        {

                            if (ts.TotalHours <= chukycanhbao5 + 2)
                            {
                                item.LoaiCanhBao = 5;
                                response.Add(item);
                            }
                        }

                        else if (ts.TotalHours < chukycanhbao5 && ttrinhKTR != null && ttrinhDTN == null && ttnt.TrangThai >= TrangThaiNghiemThu.PhanCongTC && ttnt.TrangThai <= TrangThaiNghiemThu.DuThaoHD)
                        {
                            ts = DateTime.Now - ttrinhKTR.NGAY_TAO;
                            chukycanhbao = dLoaicanhbao.Query.FirstOrDefault(p => p.ID == 4).CHUKYCANHBAO;
                            if ( ts.TotalHours >= chukycanhbao && ts.TotalHours < chukycanhbao + 2)
                            {
                                item.LoaiCanhBao = 4;
                                response.Add(item);
                            }
                        }

                    }
                    //else if ( ttrinhTN != null && ttrinhKDN == null && item.TrangThai < TrangThaiCongVan.HoanThanh)
                    //{
                    //    ts = DateTime.Now - ttrinhTN.NGAY_TAO;
                    //    chukycanhbao = dLoaicanhbao.Query.FirstOrDefault(p => p.ID == 16).CHUKYCANHBAO;
                    //    if (ts.TotalDays > chukycanhbao)
                    //    {
                    //        item.LoaiCanhBao = 16;
                    //        response.Add(item);
                    //    }
                    //}
                } 
            }
            return response;
        }


        public List<DateTime> ConvertDate(List<string> ngayLe)
        {
            List<DateTime> result = new List<DateTime>();
            foreach (var item in ngayLe)
            {
                //fomat
                var date = DateTime.ParseExact($"{item}/{DateTime.Now.Year}", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                result.Add(date.Date);
            }
            return result;
        }
    }



}
