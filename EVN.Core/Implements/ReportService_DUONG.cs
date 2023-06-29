using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
using FX.Core;
using FX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class ReportService : BaseService<CongVanYeuCau, int>, IReportService
    {
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

                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau);
                if (ttrinhs.Count() > 0)
                {
                    var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                    if (ttrinhtnhs != null)
                    {
                        data.NGAY_TNHAN = data.NGAY_TNHANYCAU = ttrinhtnhs.NGAY_BDAU.ToString("dd/MM/yyyy");
                        if (ttrinhtnhs.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                            var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                            if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                            {
                                songaytnhs = 1;
                            }

                            data.TGIAN_TNHAN = songaytnhs;

                        }

                    }

                    var ttrinhpk = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                    if (ttrinhpk != null)
                    {
                        data.NGAY_CHUYENKS = ttrinhpk.NGAY_BDAU.ToString("dd/MM/yyyy");

                    }
                    var bbks = bbkssrv.GetbyYeuCau(item.MaYeuCau);
                    var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                    var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                    if (bbks != null)
                    {
                        if (ttrinhks != null)
                        {
                            data.NGAY_KSAT = ttrinhks.NGAY_BDAU.ToString("dd/MM/yyyy");
                        }

                        if (ttrinhch5 != null)
                        {

                            if (ttrinhch5.NGAY_KTHUC.HasValue)
                            {

                                TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                {
                                    songayks = 1;
                                }

                                TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                {
                                    songaych5 = 1;
                                }

                                if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                    {
                                        data.TGIAN_KSAT = songaych5 + songayks - 1;
                                    }
                                    else
                                    {
                                        data.TGIAN_KSAT = songaych5 + songayks;
                                    }
                                }
                                else
                                {
                                    data.TGIAN_KSAT = songayks;
                                }


                            }
                        }

                    }
                    var ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN");
                    if (ttrinhbdn == null)
                    {
                        ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    }
                    if (ttrinhbdn != null)
                    {
                        data.NGAY_BDN = ttrinhbdn.NGAY_BDAU.ToString("dd/MM/yyyy");
                        if (ttrinhbdn.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan tgbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                            data.TGIAN_DNOI = Math.Ceiling(tgbdn.TotalDays) + 1;
                            if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                            {
                                data.TGIAN_DNOI = 1;
                            }

                        }
                    }



                    var ttrinhddn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");


                    if (ttrinhddn != null)
                    {
                        if (ttrinhddn.NGAY_KTHUC.HasValue)
                        {
                            data.NGAY_DT_TDN = ttrinhddn.NGAY_BDAU.ToString("dd/MM/yyyy");
                            data.NGAY_TTX_TDN = ttrinhddn.NGAY_KTHUC.Value.ToString("dd/MM/yyyy");

                        }
                    }


                    var ttrinhtnhskt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                    if (ttrinhtnhskt != null)
                    {
                        data.NGAY_TH_TVB = ttrinhtnhskt.NGAY_BDAU.ToString("dd/MM/yyyy");
                        data.NGAY_NV_TVB = ttrinhtnhskt.NGAY_BDAU.ToString("dd/MM/yyyy");
                    }
                    var ttrinhnt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");
                    if (ttrinhnt != null)
                    {

                        data.NGAY_NT_TVB = ttrinhnt.NGAY_BDAU.ToString("dd/MM/yyyy");

                        if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue && ttrinhnt.NGAY_KTHUC.HasValue)
                        {
                            data.NGAY_HTAT_YC = ttrinhnt.NGAY_BDAU.ToString("dd/MM/yyyy");
                            TimeSpan tgtnhs = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                            var songaytnhs = Math.Ceiling(tgtnhs.TotalDays) + 1;
                            TimeSpan tgnt = ttrinhnt.NGAY_KTHUC.Value.Date - ttrinhnt.NGAY_BDAU.Date;
                            var songaynt = Math.Ceiling(tgnt.TotalDays) + 1;
                            if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                            {
                                songaytnhs = 1;
                            }

                            if (ttrinhnt.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                            {
                                if (ttrinhnt.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                {
                                    data.TGIAN_NTHU = songaynt + songaytnhs - 1;
                                }
                                else
                                {
                                    data.TGIAN_NTHU = songaynt + songaytnhs;
                                }
                            }
                            else
                            {
                                data.TGIAN_NTHU = songaytnhs;
                            }
                        }
                    }
                }
                if (ttrinhs.Count() > 0)
                {
                    var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                    var ttrinhpk = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                    var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                    var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                    var ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN");
                    if (ttrinhbdn == null)
                    {
                        ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    }

                    var ttrinhddn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                    var ttrinhtnhskt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                    var ttrinhktr = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                    var ttrinhpc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                    var ttrinhtc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                    var ttrinhbtt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                    var ttrinhdhd = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                    var ttrinhnt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");


                    if (ttrinhtnhs != null)
                    {
                        if (ttrinhtnhs.NGAY_KTHUC.HasValue)
                        {

                            TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                            var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                            if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                            {
                                songaytnhs = 1;
                            }
                            data.TONG_TGIAN = data.TONG_TGIAN_GIAIQUYET = songaytnhs;

                            if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                                var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                                if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                                {
                                    songaypk = 1;
                                }
                                if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                    {
                                        data.TONG_TGIAN = data.TONG_TGIAN + songaypk - 1;
                                    }
                                    else
                                    {
                                        data.TONG_TGIAN = data.TONG_TGIAN + songaypk;
                                    }

                                    if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                                    {
                                        TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                        var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                        if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                        {
                                            songayks = 1;
                                        }
                                        if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhks.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                            {
                                                data.TONG_TGIAN = data.TONG_TGIAN + songayks - 1;
                                            }
                                            else
                                            {
                                                data.TONG_TGIAN = data.TONG_TGIAN + songayks;
                                            }
                                        }

                                        if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                        {
                                            TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                            var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                            if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                            {
                                                songaych5 = 1;
                                            }
                                            if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                                {
                                                    data.TONG_TGIAN = data.TONG_TGIAN + songaych5 - 1;
                                                }
                                                else
                                                {
                                                    data.TONG_TGIAN = data.TONG_TGIAN + songaych5;
                                                }

                                            }
                                            if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                            {
                                                TimeSpan varbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                                                var songaybdn = Math.Ceiling(varbdn.TotalDays) + 1;
                                                if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                {
                                                    songaybdn = 1;
                                                }
                                                if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                                {
                                                    if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                                    {
                                                        data.TONG_TGIAN = data.TONG_TGIAN + songaybdn - 1;
                                                    }
                                                    else
                                                    {
                                                        data.TONG_TGIAN = data.TONG_TGIAN + songaybdn;
                                                    }

                                                }
                                                if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                                {
                                                    TimeSpan varttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                                                    var songayttdn = Math.Ceiling(varttdn.TotalDays) + 1;
                                                    if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                                                    {
                                                        songayttdn = 1;
                                                    }
                                                    if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                                    {
                                                        if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                        {
                                                            data.TONG_TGIAN = data.TONG_TGIAN + songayttdn - 1;
                                                        }
                                                        else
                                                        {
                                                            data.TONG_TGIAN = data.TONG_TGIAN + songayttdn;
                                                        }

                                                    }
                                                    if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                                                    {
                                                        TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                                                        var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                                                        if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                        {
                                                            songaytnhskt = 1;
                                                        }
                                                        if (ttrinhtnhskt.NGAY_KTHUC.Value.Date != ttrinhddn.NGAY_KTHUC.Value.Date)
                                                        {
                                                            if (ttrinhtnhskt.NGAY_BDAU.Date == ttrinhddn.NGAY_BDAU.Date)
                                                            {
                                                                data.TONG_TGIAN = data.TONG_TGIAN + songaytnhskt - 1;
                                                            }
                                                            else
                                                            {
                                                                data.TONG_TGIAN = data.TONG_TGIAN + songaytnhskt;
                                                            }

                                                        }




                                                        if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                                                        {
                                                            TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                                                            var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                                                            if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                                                            {
                                                                songaytc = 1;
                                                            }
                                                            if (ttrinhtc.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                                            {
                                                                if (ttrinhtc.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                                {
                                                                    data.TONG_TGIAN = data.TONG_TGIAN + songaytc - 1;
                                                                }
                                                                else
                                                                {
                                                                    data.TONG_TGIAN = data.TONG_TGIAN + songaytc;
                                                                }

                                                            }



                                                            if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                                                            {
                                                                TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                                                var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                                                if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                                                {
                                                                    songaydhd = 1;
                                                                }
                                                                if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                                                {
                                                                    if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                                                    {
                                                                        data.TONG_TGIAN = data.TONG_TGIAN + songaydhd - 1;
                                                                    }
                                                                    else
                                                                    {
                                                                        data.TONG_TGIAN = data.TONG_TGIAN + songaydhd;
                                                                    }

                                                                }
                                                            }





                                                        }





                                                    }

                                                }


                                            }
                                        }

                                    }


                                }

                            }



                        }
                    }



                }






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
            var listbbntluyke = service.Query.Where(p => p.NgayLap < denNgay.AddDays(1) && p.NgayLap >= tuNgay && p.TrangThai > TrangThaiCongVan.MoiTao);
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


            var listDonVi = organizationService.GetAll();

            foreach (var dv in listDonVi)
            {
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
                    var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == bbnt.MaYeuCau);
                    if (ttrinhs.Count() > 0)
                    {
                        var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                        var ttrinhpk = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                        var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                        var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                        var ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN");
                        if (ttrinhbdn == null)
                        {
                            ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                        }

                        var ttrinhddn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                        var ttrinhtnhskt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                        var ttrinhktr = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                        var ttrinhpc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                        var ttrinhtc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                        var ttrinhbtt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                        var ttrinhdhd = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                        var ttrinhnt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");


                        if (ttrinhtnhs != null && ttrinhtnhs.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                            var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                            if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                            {
                                songaytnhs = 1;
                            }
                            tongTGTrongThang = tongTGTrongThang + songaytnhs;

                            if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                                var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                                if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                                {
                                    songaypk = 1;
                                }
                                if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                    {
                                        tongTGTrongThang = tongTGTrongThang + songaypk - 1;
                                    }
                                    else
                                    {
                                        tongTGTrongThang = tongTGTrongThang + songaypk;
                                    }
                                }

                                if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                    var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                    if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                    {
                                        songayks = 1;
                                    }
                                    if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhks.NGAY_BDAU.Date == ttrinhpk.NGAY_BDAU.Date)
                                        {
                                            tongTGTrongThang = tongTGTrongThang + songayks - 1;
                                        }
                                        else
                                        {
                                            tongTGTrongThang = tongTGTrongThang + songayks;
                                        }
                                    }

                                    if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                    {
                                        TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                        var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                        {
                                            songaych5 = 1;
                                        }
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                            {
                                                tongTGTrongThang = tongTGTrongThang + songaych5 - 1;
                                            }
                                            else
                                            {
                                                tongTGTrongThang = tongTGTrongThang + songaych5;
                                            }

                                        }
                                        if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                        {
                                            TimeSpan varbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                                            var songaybdn = Math.Ceiling(varbdn.TotalDays) + 1;
                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                                            {
                                                songaybdn = 1;
                                            }
                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                                {
                                                    tongTGTrongThang = tongTGTrongThang + songaybdn - 1;
                                                }
                                                else
                                                {
                                                    tongTGTrongThang = tongTGTrongThang + songaybdn;
                                                }

                                            }
                                            if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                            {
                                                TimeSpan varttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                                                var songayttdn = Math.Ceiling(varttdn.TotalDays) + 1;
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                                                {
                                                    songayttdn = 1;
                                                }
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                                {
                                                    if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                    {
                                                        tongTGTrongThang = tongTGTrongThang + songayttdn - 1;
                                                    }
                                                    else
                                                    {
                                                        tongTGTrongThang = tongTGTrongThang + songayttdn;
                                                    }

                                                }
                                                if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                                                {
                                                    TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                                                    var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                                                    if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                    {
                                                        songaytnhskt = 1;
                                                    }
                                                    if (ttrinhtnhskt.NGAY_KTHUC.Value.Date != ttrinhddn.NGAY_KTHUC.Value.Date)
                                                    {
                                                        if (ttrinhtnhskt.NGAY_BDAU.Date == ttrinhddn.NGAY_BDAU.Date)
                                                        {
                                                            tongTGTrongThang = tongTGTrongThang + songaytnhskt - 1;
                                                        }
                                                        else
                                                        {
                                                            tongTGTrongThang = tongTGTrongThang + songaytnhskt;
                                                        }

                                                    }

                                                    if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                                                    {
                                                        TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                                                        var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                                                        if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                                                        {
                                                            songaytc = 1;
                                                        }

                                                        if (ttrinhtc.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                                        {
                                                            if (ttrinhtc.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                            {
                                                                tongTGTrongThang = tongTGTrongThang + songaytc - 1;
                                                            }
                                                            else
                                                            {
                                                                tongTGTrongThang = tongTGTrongThang + songaytc;
                                                            }

                                                        }
                                                        if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                                                        {
                                                            TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                                            var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                                            if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                                            {
                                                                songaydhd = 1;
                                                            }

                                                            if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                                            {
                                                                if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                                                {
                                                                    tongTGTrongThang = tongTGTrongThang + songaydhd - 1;
                                                                }
                                                                else
                                                                {
                                                                    tongTGTrongThang = tongTGTrongThang + songaydhd;
                                                                }

                                                            }

                                                        }


                                                    }




                                                }

                                            }
                                        }


                                    }
                                }

                            }






                        }


                    }

                }
                double tongTGLuyKe = 0;
                foreach (var bbnt in listLuyKe)
                {
                    var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == bbnt.MaYeuCau);
                    if (ttrinhs.Count() > 0)
                    {
                        var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                        var ttrinhpk = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                        var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                        var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                        var ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN");
                        if (ttrinhbdn == null)
                        {
                            ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                        }

                        var ttrinhddn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                        var ttrinhtnhskt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                        var ttrinhktr = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                        var ttrinhpc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                        var ttrinhtc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                        var ttrinhbtt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                        var ttrinhdhd = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                        var ttrinhnt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");


                        if (ttrinhtnhs != null && ttrinhtnhs.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                            var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                            if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                            {
                                songaytnhs = 1;
                            }
                            tongTGLuyKe = tongTGLuyKe + songaytnhs;

                            if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                                var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                                if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                                {
                                    songaypk = 1;
                                }
                                if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                    {
                                        tongTGLuyKe = tongTGLuyKe + songaypk - 1;
                                    }
                                    else
                                    {
                                        tongTGLuyKe = tongTGLuyKe + songaypk;
                                    }
                                }

                                if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                    var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                    if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                    {
                                        songayks = 1;
                                    }
                                    if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhks.NGAY_BDAU.Date == ttrinhpk.NGAY_BDAU.Date)
                                        {
                                            tongTGLuyKe = tongTGLuyKe + songayks - 1;
                                        }
                                        else
                                        {
                                            tongTGLuyKe = tongTGLuyKe + songayks;
                                        }
                                    }

                                    if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                    {
                                        TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                        var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                        {
                                            songaych5 = 1;
                                        }
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                            {
                                                tongTGLuyKe = tongTGLuyKe + songaych5 - 1;
                                            }
                                            else
                                            {
                                                tongTGLuyKe = tongTGLuyKe + songaych5;
                                            }

                                        }
                                        if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                        {
                                            TimeSpan varbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                                            var songaybdn = Math.Ceiling(varbdn.TotalDays) + 1;
                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                                            {
                                                songaybdn = 1;
                                            }
                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                                {
                                                    tongTGLuyKe = tongTGLuyKe + songaybdn - 1;
                                                }
                                                else
                                                {
                                                    tongTGLuyKe = tongTGLuyKe + songaybdn;
                                                }

                                            }
                                            if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                            {
                                                TimeSpan varttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                                                var songayttdn = Math.Ceiling(varttdn.TotalDays) + 1;
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                                                {
                                                    songayttdn = 1;
                                                }
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                                {
                                                    if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                    {
                                                        tongTGLuyKe = tongTGLuyKe + songayttdn - 1;
                                                    }
                                                    else
                                                    {
                                                        tongTGLuyKe = tongTGLuyKe + songayttdn;
                                                    }

                                                }
                                                if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                                                {
                                                    TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                                                    var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                                                    if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                    {
                                                        songaytnhskt = 1;
                                                    }
                                                    if (ttrinhtnhskt.NGAY_KTHUC.Value.Date != ttrinhddn.NGAY_KTHUC.Value.Date)
                                                    {
                                                        if (ttrinhtnhskt.NGAY_BDAU.Date == ttrinhddn.NGAY_BDAU.Date)
                                                        {
                                                            tongTGLuyKe = tongTGLuyKe + songaytnhskt - 1;
                                                        }
                                                        else
                                                        {
                                                            tongTGLuyKe = tongTGLuyKe + songaytnhskt;
                                                        }

                                                    }

                                                    if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                                                    {
                                                        TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                                                        var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                                                        if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                                                        {
                                                            songaytc = 1;
                                                        }

                                                        if (ttrinhtc.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                                        {
                                                            if (ttrinhtc.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                            {
                                                                tongTGLuyKe = tongTGLuyKe + songaytc - 1;
                                                            }
                                                            else
                                                            {
                                                                tongTGLuyKe = tongTGLuyKe + songaytc;
                                                            }

                                                        }
                                                        if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                                                        {
                                                            TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                                            var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                                            if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                                            {
                                                                songaydhd = 1;
                                                            }

                                                            if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                                            {
                                                                if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                                                {
                                                                    tongTGLuyKe = tongTGLuyKe + songaydhd - 1;
                                                                }
                                                                else
                                                                {
                                                                    tongTGLuyKe = tongTGLuyKe + songaydhd;
                                                                }

                                                            }

                                                        }


                                                    }




                                                }

                                            }
                                        }


                                    }
                                }

                            }
                        }



                    }

                }
                if (listThang.Count() > 0)
                {
                    detail.TGTrongThang = Math.Round(tongTGTrongThang / (listThang.Count()), 1, MidpointRounding.AwayFromZero);
                }
                else
                {
                    detail.TGTrongThang = 0;
                }
                if (listLuyKe.Count() > 0)
                {
                    detail.TGLuyKe = Math.Round(tongTGLuyKe / (listLuyKe.Count()), 1, MidpointRounding.AwayFromZero);
                }
                else
                {
                    detail.TGLuyKe = 0;
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

            var thanghientai = new DateTime(denNgay.Year, denNgay.Month, 1);
            var thangdautien = new DateTime(denNgay.Year, 1, 1);


            var listbbnttrongthang = service.Query.Where(p => p.NgayLap <= denNgay && p.NgayLap >= thanghientai && p.TrangThai > TrangThaiCongVan.MoiTao);

            var listKT = ttrinhsrv.Query.Where(p => p.MA_CVIEC == "KT").Select(x => x.MA_YCAU_KNAI).ToList();

            if (isHoanTat)
            {
                listbbnttrongthang = listbbnttrongthang.Where(p => listKT.Contains(p.MaYeuCau));

            }
            else
            {
                listbbnttrongthang = listbbnttrongthang.Where(p => !listKT.Contains(p.MaYeuCau));

            }

            var listDonVi = organizationService.GetAll();

            foreach (var dv in listDonVi)
            {
                BaoCaoTongHopChiTietData detail = new BaoCaoTongHopChiTietData();
                detail.MaDonVi = dv.orgCode;
                detail.TenDonVi = dv.orgName;
                var listThang = listbbnttrongthang.Where(p => p.MaDViQLy == dv.orgCode);

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

                    var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == bbnt.MaYeuCau);
                    if (ttrinhs.Count() > 0)
                    {
                        var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                        var ttrinhpk = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                        var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                        var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                        var ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN");
                        if (ttrinhbdn == null)
                        {
                            ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                        }

                        var ttrinhddn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                        var ttrinhtnhskt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                        var ttrinhktr = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                        var ttrinhpc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                        var ttrinhtc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                        var ttrinhbtt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                        var ttrinhdhd = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                        var ttrinhnt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");

                        if (ttrinhtnhs != null && ttrinhtnhs.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                            var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                            if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                            {
                                songaytnhs = 1;
                            }
                            tongTGTNTrongThang = tongTGTNTrongThang + songaytnhs;
                            tongTGTrongThang = tongTGTrongThang + songaytnhs;

                            if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                                var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                                if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                                {
                                    songaypk = 1;
                                }
                                if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                    {
                                        tongTGTrongThang = tongTGTrongThang + songaypk - 1;
                                    }
                                    else
                                    {
                                        tongTGTrongThang = tongTGTrongThang + songaypk;
                                    }

                                }

                                if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                    var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                    if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                    {
                                        songayks = 1;
                                    }
                                    tongTGKSTrongThang = tongTGKSTrongThang + songayks;
                                    countKS = countKS + 1;
                                    if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhks.NGAY_BDAU.Date == ttrinhpk.NGAY_BDAU.Date)
                                        {
                                            tongTGTrongThang = tongTGTrongThang + songayks - 1;
                                        }
                                        else
                                        {
                                            tongTGTrongThang = tongTGTrongThang + songayks;
                                        }
                                    }

                                    if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                    {
                                        TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                        var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                        {
                                            songaych5 = 1;
                                        }
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                            {
                                                tongTGTrongThang = tongTGTrongThang + songaych5 - 1;
                                                tongTGKSTrongThang = tongTGKSTrongThang + songaych5 - 1;
                                            }
                                            else
                                            {
                                                tongTGTrongThang = tongTGTrongThang + songaych5;
                                                tongTGKSTrongThang = tongTGKSTrongThang + songaych5;
                                            }
                                        }

                                        if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                        {
                                            TimeSpan varbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                                            var songaybdn = Math.Ceiling(varbdn.TotalDays) + 1;
                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                                            {
                                                songaybdn = 1;
                                            }
                                            tongTGTTDNTrongThang = tongTGTTDNTrongThang + songaybdn;
                                            countTTDN = countTTDN + 1;
                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                                {
                                                    tongTGTrongThang = tongTGTrongThang + songaybdn - 1;
                                                }
                                                else
                                                {
                                                    tongTGTrongThang = tongTGTrongThang + songaybdn;
                                                }
                                            }
                                            if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                            {

                                                TimeSpan varttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                                                var songayttdn = Math.Ceiling(varttdn.TotalDays) + 1;
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                                                {
                                                    songayttdn = 1;
                                                }
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                                {
                                                    if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                    {
                                                        tongTGTrongThang = tongTGTrongThang + songayttdn - 1;
                                                        tongTGTTDNTrongThang = tongTGTTDNTrongThang + songayttdn - 1;
                                                    }
                                                    else
                                                    {
                                                        tongTGTrongThang = tongTGTrongThang + songayttdn;
                                                        tongTGTTDNTrongThang = tongTGTTDNTrongThang + songayttdn;
                                                    }
                                                }

                                                if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                                                {
                                                    TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                                                    var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                                                    if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                    {
                                                        songaytnhskt = 1;
                                                    }
                                                    tongTGNTTrongThang = tongTGNTTrongThang + songaytnhskt;
                                                    countNT = countNT + 1;
                                                    if (ttrinhtnhskt.NGAY_KTHUC.Value.Date != ttrinhddn.NGAY_KTHUC.Value.Date)
                                                    {
                                                        if (ttrinhtnhskt.NGAY_BDAU.Date == ttrinhddn.NGAY_BDAU.Date)
                                                        {
                                                            tongTGTrongThang = tongTGTrongThang + songaytnhskt - 1;
                                                        }
                                                        else
                                                        {
                                                            tongTGTrongThang = tongTGTrongThang + songaytnhskt;
                                                        }

                                                    }


                                                    if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                                                    {
                                                        TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                                                        var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                                                        if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                                                        {
                                                            songaytc = 1;
                                                        }

                                                        if (ttrinhtc.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                                        {
                                                            if (ttrinhtc.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                            {
                                                                tongTGTrongThang = tongTGTrongThang + songaytc - 1;
                                                                tongTGNTTrongThang = tongTGNTTrongThang + songaytc - 1;
                                                            }
                                                            else
                                                            {
                                                                tongTGTrongThang = tongTGTrongThang + songaytc;
                                                                tongTGNTTrongThang = tongTGNTTrongThang + songaytc;
                                                            }
                                                        }



                                                        if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                                                        {

                                                            TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                                            var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                                            if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                                            {
                                                                songaydhd = 1;
                                                            }

                                                            if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                                            {
                                                                if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                                                {
                                                                    tongTGTrongThang = tongTGTrongThang + songaydhd - 1;
                                                                    tongTGNTTrongThang = tongTGNTTrongThang + songaydhd - 1;
                                                                }
                                                                else
                                                                {
                                                                    tongTGTrongThang = tongTGTrongThang + songaydhd;
                                                                    tongTGNTTrongThang = tongTGNTTrongThang + songaydhd;
                                                                }
                                                            }

                                                        }



                                                    }




                                                }

                                            }


                                        }

                                    }

                                }

                            }

                        }

                    }

                }

                if (listThang.Count() > 0)
                {
                    detail.TGTrongThang = Math.Round(tongTGTrongThang / (listThang.Count()), 1, MidpointRounding.AwayFromZero);
                    detail.TGTiepNhanTrongThang = Math.Round(tongTGTNTrongThang / (listThang.Count()), 1, MidpointRounding.AwayFromZero);
                    detail.TGKhaoSatTrongThang = Math.Round(tongTGKSTrongThang / (countKS), 1, MidpointRounding.AwayFromZero);
                    detail.TGTTDNTrongThang = Math.Round(tongTGTTDNTrongThang / (countTTDN), 1, MidpointRounding.AwayFromZero);
                    detail.TGNTTrongThang = Math.Round(tongTGNTTrongThang / (countNT), 1, MidpointRounding.AwayFromZero);
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

            var thanghientai = new DateTime(denNgay.Year, denNgay.Month, 1);
            var thangdautien = new DateTime(denNgay.Year, 1, 1);


            var listbbntluyke = service.Query.Where(p => p.NgayLap < denNgay.AddDays(1) && p.NgayLap >= tuNgay.Date && p.TrangThai > TrangThaiCongVan.MoiTao);
            var listKT = ttrinhsrv.Query.Where(p => p.MA_CVIEC == "KT").Select(x => x.MA_YCAU_KNAI).ToList();

            if (isHoanTat)
            {

                listbbntluyke = listbbntluyke.Where(p => listKT.Contains(p.MaYeuCau));
            }
            else
            {

                listbbntluyke = listbbntluyke.Where(p => !listKT.Contains(p.MaYeuCau));
            }


            var listDonVi = organizationService.GetAll();

            foreach (var dv in listDonVi)
            {
                BaoCaoTongHopChiTietData detail = new BaoCaoTongHopChiTietData();
                detail.MaDonVi = dv.orgCode;
                detail.TenDonVi = dv.orgName;

                var listLuyKe = listbbntluyke.Where(p => p.MaDViQLy == dv.orgCode);

                detail.SoCTLuyKe = listLuyKe.Count();

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
                    var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == bbnt.MaYeuCau);
                    if (ttrinhs.Count() > 0)
                    {
                        var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                        var ttrinhpk = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                        var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                        var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                        var ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN");
                        if (ttrinhbdn == null)
                        {
                            ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                        }

                        var ttrinhddn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                        var ttrinhtnhskt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                        var ttrinhktr = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                        var ttrinhpc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                        var ttrinhtc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                        var ttrinhbtt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                        var ttrinhdhd = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                        var ttrinhnt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");


                        if (ttrinhtnhs != null && ttrinhtnhs.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                            var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                            if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                            {
                                songaytnhs = 1;
                            }
                            tongTGTNLuyKe = tongTGTNLuyKe + songaytnhs;
                            tongTGLuyKe = tongTGLuyKe + songaytnhs;

                            if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                                var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                                if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                                {
                                    songaypk = 1;
                                }
                                if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                    {
                                        tongTGLuyKe = tongTGLuyKe + songaypk - 1;
                                    }
                                    else
                                    {
                                        tongTGLuyKe = tongTGLuyKe + songaypk;
                                    }

                                }

                                if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                    var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                    if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                    {
                                        songayks = 1;
                                    }
                                    tongTGKSLuyKe = tongTGKSLuyKe + songayks;
                                    countKS = countKS + 1;
                                    if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhks.NGAY_BDAU.Date == ttrinhpk.NGAY_BDAU.Date)
                                        {
                                            tongTGLuyKe = tongTGLuyKe + songayks - 1;
                                        }
                                        else
                                        {
                                            tongTGLuyKe = tongTGLuyKe + songayks;
                                        }
                                    }

                                    if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                    {
                                        TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                        var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                        {
                                            songaych5 = 1;
                                        }
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                            {
                                                tongTGLuyKe = tongTGLuyKe + songaych5 - 1;
                                                tongTGKSLuyKe = tongTGKSLuyKe + songaych5 - 1;
                                            }
                                            else
                                            {
                                                tongTGLuyKe = tongTGLuyKe + songaych5;
                                                tongTGKSLuyKe = tongTGKSLuyKe + songaych5;
                                            }
                                        }

                                        if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                        {
                                            TimeSpan varbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                                            var songaybdn = Math.Ceiling(varbdn.TotalDays) + 1;
                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                                            {
                                                songaybdn = 1;
                                            }
                                            tongTGTTDNLuyKe = tongTGTTDNLuyKe + songaybdn;
                                            countTTDN = countTTDN + 1;
                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                                {
                                                    tongTGLuyKe = tongTGLuyKe + songaybdn - 1;
                                                }
                                                else
                                                {
                                                    tongTGLuyKe = tongTGLuyKe + songaybdn;
                                                }
                                            }
                                            if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                            {

                                                TimeSpan varttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                                                var songayttdn = Math.Ceiling(varttdn.TotalDays) + 1;
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                                                {
                                                    songayttdn = 1;
                                                }
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                                {
                                                    if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                    {
                                                        tongTGLuyKe = tongTGLuyKe + songayttdn - 1;
                                                        tongTGTTDNLuyKe = tongTGTTDNLuyKe + songayttdn - 1;
                                                    }
                                                    else
                                                    {
                                                        tongTGLuyKe = tongTGLuyKe + songayttdn;
                                                        tongTGTTDNLuyKe = tongTGTTDNLuyKe + songayttdn;
                                                    }
                                                }

                                                if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                                                {
                                                    TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                                                    var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                                                    if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                    {
                                                        songaytnhskt = 1;
                                                    }
                                                    tongTGNTLuyKe = tongTGNTLuyKe + songaytnhskt;
                                                    countNT = countNT + 1;
                                                    if (ttrinhtnhskt.NGAY_KTHUC.Value.Date != ttrinhddn.NGAY_KTHUC.Value.Date)
                                                    {
                                                        if (ttrinhtnhskt.NGAY_BDAU.Date == ttrinhddn.NGAY_BDAU.Date)
                                                        {
                                                            tongTGLuyKe = tongTGLuyKe + songaytnhskt - 1;
                                                        }
                                                        else
                                                        {
                                                            tongTGLuyKe = tongTGLuyKe + songaytnhskt;
                                                        }

                                                    }


                                                    if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                                                    {
                                                        TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                                                        var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                                                        if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                                                        {
                                                            songaytc = 1;
                                                        }

                                                        if (ttrinhtc.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                                        {
                                                            if (ttrinhtc.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                            {
                                                                tongTGLuyKe = tongTGLuyKe + songaytc - 1;
                                                                tongTGNTLuyKe = tongTGNTLuyKe + songaytc - 1;
                                                            }
                                                            else
                                                            {
                                                                tongTGLuyKe = tongTGLuyKe + songaytc;
                                                                tongTGNTLuyKe = tongTGNTLuyKe + songaytc;
                                                            }
                                                        }



                                                        if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                                                        {

                                                            TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                                            var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                                            if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                                            {
                                                                songaydhd = 1;
                                                            }

                                                            if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                                            {
                                                                if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                                                {
                                                                    tongTGLuyKe = tongTGLuyKe + songaydhd - 1;
                                                                    tongTGNTLuyKe = tongTGNTLuyKe + songaydhd - 1;
                                                                }
                                                                else
                                                                {
                                                                    tongTGLuyKe = tongTGLuyKe + songaydhd;
                                                                    tongTGNTLuyKe = tongTGNTLuyKe + songaydhd;
                                                                }
                                                            }

                                                        }



                                                    }




                                                }

                                            }


                                        }

                                    }

                                }

                            }

                        }
                    }
                }
                if (listLuyKe.Count() > 0)
                {
                    detail.TGLuyKe = Math.Round(tongTGLuyKe / (listLuyKe.Count()), 1, MidpointRounding.AwayFromZero);
                    detail.TGTiepNhanLuyKe = Math.Round(tongTGTNLuyKe / (listLuyKe.Count()), 1, MidpointRounding.AwayFromZero);
                    detail.TGKhaoSatLuyKe = Math.Round(tongTGKSLuyKe / (countKS), 1, MidpointRounding.AwayFromZero);
                    detail.TGTTDNLuyKe = Math.Round(tongTGTTDNLuyKe / (countTTDN), 1, MidpointRounding.AwayFromZero);
                    detail.TGNTLuyKe = Math.Round(tongTGNTLuyKe / (countNT), 1, MidpointRounding.AwayFromZero);
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

                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau);
                if (ttrinhs.Count() > 0)
                {
                    var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                    var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                    var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                    if (ttrinhtnhs != null)
                    {
                        if (ttrinhtnhs.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan tgtnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                            detail.ThoiGianTNHS = Math.Ceiling(tgtnhs.TotalDays) + 1;
                            detail.SoNgayTNHS = Math.Ceiling(tgtnhs.TotalDays) + 1;
                            if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                            {
                                detail.ThoiGianTNHS = 1;
                                detail.SoNgayTNHS = 1;
                            }
                        }

                    }
                    var ttrinhpk = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                    if (ttrinhpk != null)
                    {
                        if (ttrinhtnhs.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan tgpk = ttrinhpk.NGAY_BDAU.Date - ttrinhtnhs.NGAY_KTHUC.Value.Date;
                            detail.ThoiGianChuyenHSSangKS = Math.Ceiling(tgpk.TotalDays) + 1;
                            if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_KTHUC.Value.Date)
                            {
                                detail.ThoiGianChuyenHSSangKS = 1;
                            }
                        }
                    }

                    if (ttrinhpk == null && item.TrangThai == TrangThaiCongVan.Huy)
                    {
                        detail.TroNgaiTNHS = item.LyDoHuy;
                        var ttrinhhuy = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HU");
                        if (ttrinhhuy.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan tghu = ttrinhhuy.NGAY_BDAU.Date - ttrinhhuy.NGAY_KTHUC.Value.Date;
                            detail.ThoiGianTLHS = Math.Ceiling(tghu.TotalDays) + 1;
                            if (ttrinhhuy.NGAY_BDAU.Date == ttrinhhuy.NGAY_KTHUC.Value.Date)
                            {
                                detail.ThoiGianTLHS = 1;
                            }
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




                        if (ttrinhch5 != null)
                        {

                            if (ttrinhch5.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan tgtt = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                detail.ThoiGianKSThucTe = Math.Ceiling(tgtt.TotalDays) + 1;
                                detail.ThoiGianDuKienKS = Math.Ceiling(tgtt.TotalDays) + 1;
                                if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                {
                                    detail.ThoiGianKSThucTe = 1;
                                    detail.ThoiGianDuKienKS = 1;
                                }

                                TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                {
                                    songayks = 1;
                                }
                                detail.SoNgayThucHienKS = songayks;
                                TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                {
                                    songaych5 = 1;
                                }

                                if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                    {
                                        detail.SoNgayThucHienKS = songaych5 + detail.SoNgayThucHienKS - 1;
                                    }
                                    else
                                    {
                                        detail.SoNgayThucHienKS = songaych5 + detail.SoNgayThucHienKS;
                                    }
                                }



                            }
                        }

                    }
                    var ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN");
                    if (ttrinhbdn == null)
                    {
                        ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    }
                    if (ttrinhbdn != null)
                    {
                        if (ttrinhbdn.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan tgbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                            detail.SoNgayLapTTDN = Math.Ceiling(tgbdn.TotalDays) + 1;
                            if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                            {
                                detail.SoNgayLapTTDN = 1;
                            }
                            detail.SoNgayLapBBTTDN = detail.SoNgayLapTTDN;
                        }
                    }



                    var ttrinhddn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");


                    if (ttrinhddn != null && ttrinhbdn != null)
                    {
                        if (ttrinhddn.NGAY_KTHUC.HasValue && ttrinhbdn.NGAY_KTHUC.HasValue)
                        {


                            TimeSpan tglapbbttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                            detail.ThoiGianLapVaDuyetTTDN = Math.Ceiling(tglapbbttdn.TotalDays) + 1;
                            detail.ThoiGianChuyenBBTTDNDenKH = Math.Ceiling(tglapbbttdn.TotalDays) + 1;
                            if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                            {
                                detail.ThoiGianLapVaDuyetTTDN = 1;
                                detail.ThoiGianChuyenBBTTDNDenKH = 1;
                            }

                            if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                            {
                                if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                {
                                    detail.SoNgayLapBBTTDN = detail.ThoiGianLapVaDuyetTTDN + detail.SoNgayLapBBTTDN - 1;
                                }
                                else
                                {
                                    detail.SoNgayLapBBTTDN = detail.ThoiGianLapVaDuyetTTDN + detail.SoNgayLapBBTTDN;
                                }
                            }



                        }
                    }

                    if (ttrinhtnhs != null && ttrinhtnhs.NGAY_KTHUC.HasValue)
                    {

                        TimeSpan tgtnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_KTHUC.Value.Date;
                        var sntnhs = Math.Ceiling(tgtnhs.TotalDays) + 1;
                        if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_KTHUC.Value.Date)
                        {
                            sntnhs = 1;
                        }
                        detail.TongSoNgayTTDN = detail.SoNgayTNHS;

                        if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                            var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                            if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                            {
                                songaypk = 1;
                            }

                            if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                            {
                                if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                {
                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaypk - 1;
                                }
                                else
                                {
                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaypk;
                                }
                            }

                            if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                {
                                    songayks = 1;
                                }

                                if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhks.NGAY_BDAU.Date == ttrinhpk.NGAY_BDAU.Date)
                                    {
                                        detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songayks - 1;
                                    }
                                    else
                                    {
                                        detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songayks;
                                    }
                                }

                                if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                {
                                    if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                        {
                                            detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + detail.ThoiGianKSThucTe - 1;
                                        }
                                        else
                                        {
                                            detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + detail.ThoiGianKSThucTe;
                                        }
                                    }
                                    if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                    {
                                        if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                            {
                                                detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + detail.SoNgayLapTTDN - 1;
                                            }
                                            else
                                            {
                                                detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + detail.SoNgayLapTTDN;
                                            }
                                        }
                                        if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                        {
                                            if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                                {
                                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + detail.ThoiGianLapVaDuyetTTDN - 1;
                                                }
                                                else
                                                {
                                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + detail.ThoiGianLapVaDuyetTTDN;
                                                }
                                            }
                                        }
                                    }

                                }

                            }

                        }


                    }


                    result.Add(detail);

                }



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

                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau);
                if (ttrinhs.Count() > 0)
                {
                    var ttrinhtnhskt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                    var ttrinhktr = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                    if (ttrinhtnhskt != null)
                    {
                        if (ttrinhtnhskt.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan tgtnhs = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                            detail.ThoiGianTNHS = Math.Ceiling(tgtnhs.TotalDays) + 1;
                            detail.SoNgayTNHS = Math.Ceiling(tgtnhs.TotalDays) + 1;
                            if (ttrinhtnhskt.NGAY_KTHUC.Value == ttrinhtnhskt.NGAY_BDAU)
                            {
                                detail.ThoiGianTNHS = 1;
                                detail.SoNgayTNHS = 1;
                            }
                            if (ttrinhktr != null)
                            {
                                if (ttrinhktr.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan tgchuyenKT = ttrinhktr.NGAY_BDAU.Date - ttrinhtnhskt.NGAY_KTHUC.Value.Date;
                                    detail.ThoiGianChuyenHSSangKT = Math.Ceiling(tgchuyenKT.TotalDays) + 1;
                                    if (ttrinhktr.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                    {
                                        detail.ThoiGianChuyenHSSangKT = 1;
                                    }

                                }

                            }
                        }

                    }


                    if (ttrinhktr == null && item.TrangThai == TrangThaiNghiemThu.Huy)
                    {
                        //detail.TroNgaiTNHS = item.LyDoHuy;
                        var ttrinhhuy = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HU");
                        if (ttrinhhuy.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan tgHU = ttrinhhuy.NGAY_KTHUC.Value.Date - ttrinhhuy.NGAY_BDAU.Date;
                            detail.ThoiGianTLHS = Math.Ceiling(tgHU.TotalDays) + 1;
                            if (ttrinhhuy.NGAY_KTHUC.Value.Date == ttrinhhuy.NGAY_BDAU.Date)
                            {
                                detail.ThoiGianTLHS = 1;
                            }
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
                        var ttrinhkt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                        var ketquakt = ketQuaKTService.GetbyMaYCau(item.MaYeuCau);
                        if (ketquakt != null)
                        {
                            if (!string.IsNullOrWhiteSpace(ketquakt.MA_TNGAI))
                            {
                                var trongai = troNgaiService.Getbykey(ketquakt.MA_TNGAI);
                                if (ttrinhkt != null && ttrinhkt.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan tgktr = ttrinhkt.NGAY_KTHUC.Value.Date - ttrinhkt.NGAY_BDAU.Date;
                                    detail.ThoiGianYeuCauKHHoanThanhTonTai = Math.Ceiling(tgktr.TotalDays) + 1;
                                }
                                if (ttrinhkt.NGAY_KTHUC.Value.Date == ttrinhkt.NGAY_BDAU.Date)
                                {
                                    detail.ThoiGianYeuCauKHHoanThanhTonTai = 1;
                                }

                                if (trongai != null)
                                {
                                    detail.TroNgaiKT = trongai.TEN_TNGAI;
                                }
                            }

                        }

                        if (ttrinhkt != null)
                        {
                            if (ttrinhkt.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan tgktr = ttrinhkt.NGAY_KTHUC.Value.Date - ttrinhkt.NGAY_BDAU.Date;
                                detail.ThoiGianKTThucTe = Math.Ceiling(tgktr.TotalDays) + 1;
                                detail.ThoiGianLapVaDuyetBBKT = Math.Ceiling(tgktr.TotalDays) + 1;
                                detail.ThoiGianChuyenBBKTDenKH = Math.Ceiling(tgktr.TotalDays) + 1;


                                if (ttrinhkt.NGAY_KTHUC.Value.Date == ttrinhkt.NGAY_BDAU.Date)
                                {
                                    detail.ThoiGianKTThucTe = 1;
                                    detail.ThoiGianLapVaDuyetBBKT = 1;
                                    detail.ThoiGianChuyenBBKTDenKH = 1;

                                }

                            }

                        }
                        if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                            var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                            if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                            {
                                songaytnhskt = 1;
                            }
                            detail.SoNgayKTDK = songaytnhskt;

                            if (ttrinhktr != null && ttrinhktr.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varktr = ttrinhktr.NGAY_KTHUC.Value.Date - ttrinhktr.NGAY_BDAU.Date;
                                var songayktr = Math.Ceiling(varktr.TotalDays) + 1;
                                if (ttrinhktr.NGAY_KTHUC.Value.Date == ttrinhktr.NGAY_BDAU.Date)
                                {
                                    songayktr = 1;
                                }

                                if (ttrinhktr.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhktr.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                    {
                                        detail.SoNgayKTDK = detail.SoNgayKTDK + songayktr - 1;
                                    }
                                    else
                                    {
                                        detail.SoNgayKTDK = detail.SoNgayKTDK + songayktr;
                                    }
                                }
                            }
                        }
                    }


                    var ttrinhpc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                    var ttrinhtc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                    var ttrinhbtt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                    var ttrinhdhd = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                    var ttrinhnt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");


                    if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                    {
                        TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                        var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                        if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                        {
                            songaytc = 1;
                        }
                        detail.ThoiGianTNDNNT = songaytc;
                        detail.SoNgayNTVaKyHD = songaytc;




                        if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                        {

                            TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                            var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                            if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                            {
                                songaydhd = 1;
                            }
                            detail.ThoiGianNTDDVaKyHD = songaydhd;
                            if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                            {
                                if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                {
                                    detail.SoNgayNTVaKyHD = detail.SoNgayNTVaKyHD + songaydhd - 1;
                                }
                                else
                                {
                                    detail.SoNgayNTVaKyHD = detail.SoNgayNTVaKyHD + songaydhd;
                                }
                            }

                        }



                    }



                    if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                    {
                        TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                        var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                        if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                        {
                            songaytnhskt = 1;
                        }
                        detail.TongSoNgayYCNT = songaytnhskt;



                        if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                            var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                            if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                            {
                                songaytc = 1;
                            }

                            if (ttrinhtc.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                            {
                                if (ttrinhtc.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                {
                                    detail.TongSoNgayYCNT = detail.TongSoNgayYCNT + songaytc - 1;
                                }
                                else
                                {
                                    detail.TongSoNgayYCNT = detail.TongSoNgayYCNT + songaytc;
                                }
                            }



                            if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                            {

                                TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                {
                                    songaydhd = 1;
                                }

                                if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                    {
                                        detail.TongSoNgayYCNT = detail.TongSoNgayYCNT + songaydhd - 1;
                                    }
                                    else
                                    {
                                        detail.TongSoNgayYCNT = detail.TongSoNgayYCNT + songaydhd;
                                    }
                                }

                            }

                        }

                    }



                    result.Add(detail);
                }



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

                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau);

                if (ttrinhs.Count() > 0)
                {
                    var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                    var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                    var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                    var ttrinhpk = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                    var ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN");
                    if (ttrinhbdn == null)
                    {
                        ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    }
                    var ttrinhddn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");


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


                    if (ttrinhtnhs != null && ttrinhtnhs.NGAY_KTHUC.HasValue)
                    {

                        TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                        var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                        if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                        {
                            songaytnhs = 1;
                        }
                        detail.TongSoNgayTTDN = songaytnhs;

                        if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                            var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                            if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                            {
                                songaypk = 1;
                            }
                            if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                            {
                                if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                {
                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaypk - 1;
                                }
                                else
                                {
                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaypk;
                                }
                            }

                            if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                {
                                    songayks = 1;
                                }
                                if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhks.NGAY_BDAU.Date == ttrinhpk.NGAY_BDAU.Date)
                                    {
                                        detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songayks - 1;
                                    }
                                    else
                                    {
                                        detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songayks;
                                    }
                                }

                                if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                    var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                    if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                    {
                                        songaych5 = 1;
                                    }
                                    if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                        {
                                            detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaych5 - 1;
                                        }
                                        else
                                        {
                                            detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaych5;
                                        }
                                    }
                                    if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                    {
                                        TimeSpan varbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                                        var songaybdn = Math.Ceiling(varbdn.TotalDays) + 1;
                                        if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                                        {
                                            songaybdn = 1;
                                        }

                                        if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                            {
                                                detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaybdn - 1;
                                            }
                                            else
                                            {
                                                detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaybdn;
                                            }
                                        }
                                        if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                        {
                                            TimeSpan varttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                                            var songayttdn = Math.Ceiling(varttdn.TotalDays) + 1;
                                            if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                                            {
                                                songayttdn = 1;
                                            }
                                            if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                {
                                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songayttdn - 1;
                                                }
                                                else
                                                {
                                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songayttdn;
                                                }
                                            }
                                        }
                                    }

                                }
                            }

                        }


                    }
                    var ttrinhnt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");
                    var ttrinhtnhskt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                    var ttrinhpc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                    var ttrinhtc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                    var ttrinhbtt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                    var ttrinhdhd = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                    var ttrinhktr = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");

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
                                    TimeSpan tgktr = ttrinhktr.NGAY_KTHUC.Value - ttrinhktr.NGAY_BDAU;

                                    if (trongai != null)
                                    {
                                        detail.TroNgaiKTDK = trongai.TEN_TNGAI;
                                    }
                                }

                            }

                            if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                                var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                                if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                {
                                    songaytnhskt = 1;
                                }
                                detail.TongSoNgayKTDK = songaytnhskt;

                                if (ttrinhktr != null && ttrinhktr.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varktr = ttrinhktr.NGAY_KTHUC.Value.Date - ttrinhktr.NGAY_BDAU.Date;
                                    var songayktr = Math.Ceiling(varktr.TotalDays) + 1;
                                    if (ttrinhktr.NGAY_KTHUC.Value.Date == ttrinhktr.NGAY_BDAU.Date)
                                    {
                                        songayktr = 1;
                                    }

                                    if (ttrinhktr.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhktr.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                        {
                                            detail.TongSoNgayKTDK = detail.TongSoNgayKTDK + songayktr - 1;
                                        }
                                        else
                                        {
                                            detail.TongSoNgayKTDK = detail.TongSoNgayKTDK + songayktr;
                                        }
                                    }
                                }
                            }



                        }





                        if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                            var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                            if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                            {
                                songaytc = 1;
                            }
                            detail.TongSoNgayNT = songaytc;




                            if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                            {

                                TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                {
                                    songaydhd = 1;
                                }

                                if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                    {
                                        detail.TongSoNgayNT = detail.TongSoNgayNT + songaydhd - 1;
                                    }
                                    else
                                    {
                                        detail.TongSoNgayNT = detail.TongSoNgayNT + songaydhd;
                                    }
                                }

                            }



                        }


                    }

                    if (ttrinhtnhs != null && ttrinhtnhs.NGAY_KTHUC.HasValue)
                    {
                        TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                        var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                        if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                        {
                            songaytnhs = 1;
                        }
                        detail.TongSoNgayTCDN = songaytnhs;

                        if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                            var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                            if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                            {
                                songaypk = 1;
                            }
                            if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                            {
                                if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                {
                                    detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaypk - 1;
                                }
                                else
                                {
                                    detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaypk;
                                }

                            }

                            if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                {
                                    songayks = 1;
                                }
                                if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhks.NGAY_BDAU.Date == ttrinhpk.NGAY_BDAU.Date)
                                    {
                                        detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songayks - 1;
                                    }
                                    else
                                    {
                                        detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songayks;
                                    }
                                }

                                if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                    var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                    if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                    {
                                        songaych5 = 1;
                                    }
                                    if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                        {
                                            detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaych5 - 1;
                                        }
                                        else
                                        {
                                            detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaych5;
                                        }
                                    }

                                    if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                    {
                                        TimeSpan varbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                                        var songaybdn = Math.Ceiling(varbdn.TotalDays) + 1;
                                        if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                                        {
                                            songaybdn = 1;
                                        }
                                        if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                            {
                                                detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaybdn - 1;
                                            }
                                            else
                                            {
                                                detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaybdn;
                                            }
                                        }
                                        if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                        {

                                            TimeSpan varttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                                            var songayttdn = Math.Ceiling(varttdn.TotalDays) + 1;
                                            if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                                            {
                                                songayttdn = 1;
                                            }
                                            if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                {
                                                    detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songayttdn - 1;
                                                }
                                                else
                                                {
                                                    detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songayttdn;
                                                }
                                            }

                                            if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                                            {
                                                TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                                                var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                                                if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                {
                                                    songaytnhskt = 1;
                                                }
                                                if (ttrinhtnhskt.NGAY_KTHUC.Value.Date != ttrinhddn.NGAY_KTHUC.Value.Date)
                                                {
                                                    if (ttrinhtnhskt.NGAY_BDAU.Date == ttrinhddn.NGAY_BDAU.Date)
                                                    {
                                                        detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaytnhskt - 1;
                                                    }
                                                    else
                                                    {
                                                        detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaytnhskt;
                                                    }

                                                }


                                                if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                                                {
                                                    TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                                                    var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                                                    if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                                                    {
                                                        songaytc = 1;
                                                    }

                                                    if (ttrinhtc.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                                    {
                                                        if (ttrinhtc.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                        {
                                                            detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaytc - 1;
                                                        }
                                                        else
                                                        {
                                                            detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaytc;
                                                        }
                                                    }



                                                    if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                                                    {

                                                        TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                                        var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                                        if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                                        {
                                                            songaydhd = 1;
                                                        }

                                                        if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                                        {
                                                            if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                                            {
                                                                detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaydhd - 1;
                                                            }
                                                            else
                                                            {
                                                                detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaydhd;
                                                            }
                                                        }

                                                    }



                                                }



                                            }

                                        }

                                    }

                                }

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

                int countQuaHanTTDN=0;
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
                    var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau);
                    if (ttrinhs.Count() > 0)
                    {
                        var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                        var ttrinhpk = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                        var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                        var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                        var ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN");
                        if (ttrinhbdn == null)
                        {
                            ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                        }

                        var ttrinhddn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");
                        var ttrinhtnhskt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                        var ttrinhktr = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");
                        var ttrinhpc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                        var ttrinhtc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                        var ttrinhbtt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                        var ttrinhdhd = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                        var ttrinhnt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");
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

                        if (ttrinhtnhs != null && ttrinhtnhs.NGAY_KTHUC.HasValue)
                        {

                            TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                            var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                            if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                            {
                                songaytnhs = 1;
                            }
                            tongTGTTDN = tongTGTTDN + songaytnhs;
                            detailTGTTDN = detailTGTTDN + songaytnhs;

                            if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                                var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                                if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                                {
                                    songaypk = 1;
                                }
                                if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                    {
                                        tongTGTTDN = tongTGTTDN + songaypk - 1;
                                        detailTGTTDN = detailTGTTDN + songaypk - 1;
                                    }
                                    else
                                    {
                                        tongTGTTDN = tongTGTTDN + songaypk;
                                        detailTGTTDN = detailTGTTDN + songaypk;
                                    }
                                }

                                if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                    var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                    if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                    {
                                        songayks = 1;
                                    }
                                    if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhks.NGAY_BDAU.Date == ttrinhpk.NGAY_BDAU.Date)
                                        {
                                            tongTGTTDN = tongTGTTDN + songayks - 1;
                                            detailTGTTDN = detailTGTTDN + songayks - 1;
                                        }
                                        else
                                        {
                                            tongTGTTDN = tongTGTTDN + songayks;
                                            detailTGTTDN = detailTGTTDN + songayks;
                                        }
                                    }

                                    if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                    {
                                        TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                        var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                        {
                                            songaych5 = 1;
                                        }
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                            {
                                                tongTGTTDN = tongTGTTDN + songaych5 - 1;
                                                detailTGTTDN = detailTGTTDN + songaych5 - 1;
                                            }
                                            else
                                            {
                                                tongTGTTDN = tongTGTTDN + songaych5;
                                                detailTGTTDN = detailTGTTDN + songaych5;
                                            }
                                        }
                                        if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                        {
                                            TimeSpan varbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                                            var songaybdn = Math.Ceiling(varbdn.TotalDays) + 1;
                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                                            {
                                                songaybdn = 1;
                                            }

                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                                {
                                                    tongTGTTDN = tongTGTTDN + songaybdn - 1;
                                                    detailTGTTDN = detailTGTTDN + songaybdn - 1;
                                                }
                                                else
                                                {
                                                    tongTGTTDN = tongTGTTDN + songaybdn;
                                                    detailTGTTDN = detailTGTTDN + songaybdn;
                                                }
                                            }
                                            if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                            {
                                                TimeSpan varttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                                                var songayttdn = Math.Ceiling(varttdn.TotalDays) + 1;
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                                                {
                                                    songayttdn = 1;
                                                }
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                                {
                                                    if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                    {
                                                        tongTGTTDN = tongTGTTDN + songayttdn - 1;
                                                        detailTGTTDN = detailTGTTDN + songayttdn - 1;
                                                    }
                                                    else
                                                    {
                                                        tongTGTTDN = tongTGTTDN + songayttdn;
                                                        detailTGTTDN = detailTGTTDN + songayttdn;
                                                    }
                                                }
                                                countHT = countHT + 1;
                                            }
                                        }

                                    }
                                }

                            }


                        }


                        var ycnt = yCauNghiemThuService.GetbyMaYCau(item.MaYeuCau);
                        if (ycnt != null)
                        {
                            countTongKTDK = countTongKTDK + 1;

                            if (ycnt.TrangThai == TrangThaiNghiemThu.Huy)
                            {
                                countHUKTKD = countHUKTKD + 1;
                                if (ttrinhktr == null)
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

                            if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                                var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                                if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                {
                                    songaytnhskt = 1;
                                }
                                tongTGKTDK = tongTGKTDK + songaytnhskt;
                                detailKTDK = detailKTDK + songaytnhskt;
                                if (ttrinhktr != null && ttrinhktr.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varktr = ttrinhktr.NGAY_KTHUC.Value.Date - ttrinhktr.NGAY_BDAU.Date;
                                    var songayktr = Math.Ceiling(varktr.TotalDays) + 1;
                                    if (ttrinhktr.NGAY_KTHUC.Value.Date == ttrinhktr.NGAY_BDAU.Date)
                                    {
                                        songayktr = 1;
                                    }

                                    if (ttrinhktr.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhktr.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                        {
                                            tongTGKTDK = tongTGKTDK + songayktr - 1;
                                            detailKTDK = detailKTDK + songayktr - 1;
                                        }
                                        else
                                        {
                                            tongTGKTDK = tongTGKTDK + songayktr;
                                            detailKTDK = detailKTDK + songayktr;
                                        }
                                    }
                                    countHTKTDK = countHTKTDK + 1;
                                }
                            }



                            if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                                var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                                if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                                {
                                    songaytc = 1;
                                }
                                tongTGNT = tongTGNT + songaytc;
                                detailTGNT = detailTGNT + songaytc;
                                if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                                {

                                    TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                    var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                    if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                    {
                                        songaydhd = 1;
                                    }

                                    if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                        {
                                            tongTGNT = tongTGNT + songaydhd - 1;
                                            detailTGNT = detailTGNT + songaydhd - 1;
                                        }
                                        else
                                        {
                                            tongTGNT = tongTGNT + songaydhd;
                                            detailTGNT = detailTGNT + songaydhd;
                                        }
                                    }
                                    countHTNT = countHTNT + 1;

                                }



                            }

                        }

                        if (ttrinhtnhs != null && ttrinhtnhs.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                            var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                            if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                            {
                                songaytnhs = 1;
                            }
                            tongTGTCDN = songaytnhs;

                            if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                                var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                                if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                                {
                                    songaypk = 1;
                                }
                                if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                    {
                                        tongTGTCDN = tongTGTCDN + songaypk - 1;
                                    }
                                    else
                                    {
                                        tongTGTCDN = tongTGTCDN + songaypk;
                                    }

                                }

                                if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                    var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                    if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                    {
                                        songayks = 1;
                                    }
                                    if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhks.NGAY_BDAU.Date == ttrinhpk.NGAY_BDAU.Date)
                                        {
                                            tongTGTCDN = tongTGTCDN + songayks - 1;
                                        }
                                        else
                                        {
                                            tongTGTCDN = tongTGTCDN + songayks;
                                        }
                                    }

                                    if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                    {
                                        TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                        var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                        {
                                            songaych5 = 1;
                                        }
                                        if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                            {
                                                tongTGTCDN = tongTGTCDN + songaych5 - 1;
                                            }
                                            else
                                            {
                                                tongTGTCDN = tongTGTCDN + songaych5;
                                            }
                                        }

                                        if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                        {
                                            TimeSpan varbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                                            var songaybdn = Math.Ceiling(varbdn.TotalDays) + 1;
                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                                            {
                                                songaybdn = 1;
                                            }
                                            if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                                {
                                                    tongTGTCDN = tongTGTCDN + songaybdn - 1;
                                                }
                                                else
                                                {
                                                    tongTGTCDN = tongTGTCDN + songaybdn;
                                                }
                                            }
                                            if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                            {

                                                TimeSpan varttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                                                var songayttdn = Math.Ceiling(varttdn.TotalDays) + 1;
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                                                {
                                                    songayttdn = 1;
                                                }
                                                if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                                {
                                                    if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                    {
                                                        tongTGTCDN = tongTGTCDN + songayttdn - 1;
                                                    }
                                                    else
                                                    {
                                                        tongTGTCDN = tongTGTCDN + songayttdn;
                                                    }
                                                }

                                                if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                                                {
                                                    TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                                                    var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                                                    if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                    {
                                                        songaytnhskt = 1;
                                                    }
                                                    if (ttrinhtnhskt.NGAY_KTHUC.Value.Date != ttrinhddn.NGAY_KTHUC.Value.Date)
                                                    {
                                                        if (ttrinhtnhskt.NGAY_BDAU.Date == ttrinhddn.NGAY_BDAU.Date)
                                                        {
                                                            tongTGTCDN = tongTGTCDN + songaytnhskt - 1;
                                                        }
                                                        else
                                                        {
                                                            tongTGTCDN = tongTGTCDN + songaytnhskt;
                                                        }

                                                    }

                                                    if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                                                    {
                                                        TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                                                        var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                                                        if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                                                        {
                                                            songaytc = 1;
                                                        }

                                                        if (ttrinhtc.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                                        {
                                                            if (ttrinhtc.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                            {
                                                                tongTGTCDN = tongTGTCDN + songaytc - 1;
                                                            }
                                                            else
                                                            {
                                                                tongTGTCDN = tongTGTCDN + songaytc;
                                                            }
                                                        }

                                                        if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                                                        {

                                                            TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                                            var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                                            if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                                            {
                                                                songaydhd = 1;
                                                            }

                                                            if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                                            {
                                                                if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                                                {
                                                                    tongTGTCDN = tongTGTCDN + songaydhd - 1;
                                                                }
                                                                else
                                                                {
                                                                    tongTGTCDN = tongTGTCDN + songaydhd;
                                                                }
                                                            }

                                                        }



                                                    }


                                                }

                                            }

                                        }

                                    }

                                }

                            }

                        }

                        if (detailTGTTDN > 2 || detailTGNT > 1 || detailKTDK>4)
                        {
                            countQuaHan = countQuaHan + 1;
                            if (detailTGTTDN > 2)
                            {
                                countQuaHanTTDN = countQuaHanTTDN + 1;
                                ngayQuaHanTTDN = ngayQuaHanTTDN+(int)(detailTGTTDN - 2);
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

                var ttrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == item.MaYeuCau);

                if (ttrinhs.Count() > 0)
                {
                    var ttrinhtnhs = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                    var ttrinhks = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KS");
                    var ttrinhch5 = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "CH5");
                    var ttrinhpk = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PK");
                    var ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN");
                    if (ttrinhbdn == null)
                    {
                        ttrinhbdn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KDN");
                    }
                    var ttrinhddn = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DDN");


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


                    if (ttrinhtnhs != null && ttrinhtnhs.NGAY_KTHUC.HasValue)
                    {

                        TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                        var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                        if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                        {
                            songaytnhs = 1;
                        }
                        detail.TongSoNgayTTDN = songaytnhs;

                        if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                            var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                            if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                            {
                                songaypk = 1;
                            }
                            if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                            {
                                if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                {
                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaypk - 1;
                                }
                                else
                                {
                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaypk;
                                }
                            }

                            if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                {
                                    songayks = 1;
                                }
                                if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhks.NGAY_BDAU.Date == ttrinhpk.NGAY_BDAU.Date)
                                    {
                                        detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songayks - 1;
                                    }
                                    else
                                    {
                                        detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songayks;
                                    }
                                }

                                if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                    var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                    if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                    {
                                        songaych5 = 1;
                                    }
                                    if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                        {
                                            detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaych5 - 1;
                                        }
                                        else
                                        {
                                            detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaych5;
                                        }
                                    }
                                    if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                    {
                                        TimeSpan varbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                                        var songaybdn = Math.Ceiling(varbdn.TotalDays) + 1;
                                        if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                                        {
                                            songaybdn = 1;
                                        }

                                        if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                            {
                                                detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaybdn - 1;
                                            }
                                            else
                                            {
                                                detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songaybdn;
                                            }
                                        }
                                        if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                        {
                                            TimeSpan varttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                                            var songayttdn = Math.Ceiling(varttdn.TotalDays) + 1;
                                            if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                                            {
                                                songayttdn = 1;
                                            }
                                            if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                {
                                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songayttdn - 1;
                                                }
                                                else
                                                {
                                                    detail.TongSoNgayTTDN = detail.TongSoNgayTTDN + songayttdn;
                                                }
                                            }
                                        }
                                    }

                                }
                            }

                        }


                    }
                    var ttrinhnt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "HT");
                    var ttrinhtnhskt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TVB");
                    var ttrinhpc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "PC");
                    var ttrinhtc = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "TC");
                    var ttrinhbtt = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "BTT");
                    var ttrinhdhd = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "DHD");
                    var ttrinhktr = ttrinhs.FirstOrDefault(p => p.MA_CVIEC == "KTR");

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
                                    TimeSpan tgktr = ttrinhktr.NGAY_KTHUC.Value - ttrinhktr.NGAY_BDAU;

                                    if (trongai != null)
                                    {
                                        detail.TroNgaiKTDK = trongai.TEN_TNGAI;
                                    }
                                }

                            }

                            if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                                var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                                if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                {
                                    songaytnhskt = 1;
                                }
                                detail.TongSoNgayKTDK = songaytnhskt;

                                if (ttrinhktr != null && ttrinhktr.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varktr = ttrinhktr.NGAY_KTHUC.Value.Date - ttrinhktr.NGAY_BDAU.Date;
                                    var songayktr = Math.Ceiling(varktr.TotalDays) + 1;
                                    if (ttrinhktr.NGAY_KTHUC.Value.Date == ttrinhktr.NGAY_BDAU.Date)
                                    {
                                        songayktr = 1;
                                    }

                                    if (ttrinhktr.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhktr.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                        {
                                            detail.TongSoNgayKTDK = detail.TongSoNgayKTDK + songayktr - 1;
                                        }
                                        else
                                        {
                                            detail.TongSoNgayKTDK = detail.TongSoNgayKTDK + songayktr;
                                        }
                                    }
                                }
                            }



                        }





                        if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                            var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                            if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                            {
                                songaytc = 1;
                            }
                            detail.TongSoNgayNT = songaytc;




                            if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                            {

                                TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                {
                                    songaydhd = 1;
                                }

                                if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                    {
                                        detail.TongSoNgayNT = detail.TongSoNgayNT + songaydhd - 1;
                                    }
                                    else
                                    {
                                        detail.TongSoNgayNT = detail.TongSoNgayNT + songaydhd;
                                    }
                                }

                            }



                        }


                    }

                    if (ttrinhtnhs != null && ttrinhtnhs.NGAY_KTHUC.HasValue)
                    {
                        TimeSpan vartnhs = ttrinhtnhs.NGAY_KTHUC.Value.Date - ttrinhtnhs.NGAY_BDAU.Date;
                        var songaytnhs = Math.Ceiling(vartnhs.TotalDays) + 1;
                        if (ttrinhtnhs.NGAY_KTHUC.Value.Date == ttrinhtnhs.NGAY_BDAU.Date)
                        {
                            songaytnhs = 1;
                        }
                        detail.TongSoNgayTCDN = songaytnhs;

                        if (ttrinhpk != null && ttrinhpk.NGAY_KTHUC.HasValue)
                        {
                            TimeSpan varpk = ttrinhpk.NGAY_KTHUC.Value.Date - ttrinhpk.NGAY_BDAU.Date;
                            var songaypk = Math.Ceiling(varpk.TotalDays) + 1;
                            if (ttrinhpk.NGAY_KTHUC.Value.Date == ttrinhpk.NGAY_BDAU.Date)
                            {
                                songaypk = 1;
                            }
                            if (ttrinhpk.NGAY_KTHUC.Value.Date != ttrinhtnhs.NGAY_KTHUC.Value.Date)
                            {
                                if (ttrinhpk.NGAY_BDAU.Date == ttrinhtnhs.NGAY_BDAU.Date)
                                {
                                    detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaypk - 1;
                                }
                                else
                                {
                                    detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaypk;
                                }

                            }

                            if (ttrinhks != null && ttrinhks.NGAY_KTHUC.HasValue)
                            {
                                TimeSpan varks = ttrinhks.NGAY_KTHUC.Value.Date - ttrinhks.NGAY_BDAU.Date;
                                var songayks = Math.Ceiling(varks.TotalDays) + 1;
                                if (ttrinhks.NGAY_KTHUC.Value.Date == ttrinhks.NGAY_BDAU.Date)
                                {
                                    songayks = 1;
                                }
                                if (ttrinhks.NGAY_KTHUC.Value.Date != ttrinhpk.NGAY_KTHUC.Value.Date)
                                {
                                    if (ttrinhks.NGAY_BDAU.Date == ttrinhpk.NGAY_BDAU.Date)
                                    {
                                        detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songayks - 1;
                                    }
                                    else
                                    {
                                        detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songayks;
                                    }
                                }

                                if (ttrinhch5 != null && ttrinhch5.NGAY_KTHUC.HasValue)
                                {
                                    TimeSpan varch5 = ttrinhch5.NGAY_KTHUC.Value.Date - ttrinhch5.NGAY_BDAU.Date;
                                    var songaych5 = Math.Ceiling(varch5.TotalDays) + 1;
                                    if (ttrinhch5.NGAY_KTHUC.Value.Date == ttrinhch5.NGAY_BDAU.Date)
                                    {
                                        songaych5 = 1;
                                    }
                                    if (ttrinhch5.NGAY_KTHUC.Value.Date != ttrinhks.NGAY_KTHUC.Value.Date)
                                    {
                                        if (ttrinhch5.NGAY_BDAU.Date == ttrinhks.NGAY_BDAU.Date)
                                        {
                                            detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaych5 - 1;
                                        }
                                        else
                                        {
                                            detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaych5;
                                        }
                                    }

                                    if (ttrinhbdn != null && ttrinhbdn.NGAY_KTHUC.HasValue)
                                    {
                                        TimeSpan varbdn = ttrinhbdn.NGAY_KTHUC.Value.Date - ttrinhbdn.NGAY_BDAU.Date;
                                        var songaybdn = Math.Ceiling(varbdn.TotalDays) + 1;
                                        if (ttrinhbdn.NGAY_KTHUC.Value.Date == ttrinhbdn.NGAY_BDAU.Date)
                                        {
                                            songaybdn = 1;
                                        }
                                        if (ttrinhbdn.NGAY_KTHUC.Value.Date != ttrinhch5.NGAY_KTHUC.Value.Date)
                                        {
                                            if (ttrinhbdn.NGAY_BDAU.Date == ttrinhch5.NGAY_BDAU.Date)
                                            {
                                                detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaybdn - 1;
                                            }
                                            else
                                            {
                                                detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaybdn;
                                            }
                                        }
                                        if (ttrinhddn != null && ttrinhddn.NGAY_KTHUC.HasValue)
                                        {

                                            TimeSpan varttdn = ttrinhddn.NGAY_KTHUC.Value.Date - ttrinhddn.NGAY_BDAU.Date;
                                            var songayttdn = Math.Ceiling(varttdn.TotalDays) + 1;
                                            if (ttrinhddn.NGAY_KTHUC.Value.Date == ttrinhddn.NGAY_BDAU.Date)
                                            {
                                                songayttdn = 1;
                                            }
                                            if (ttrinhddn.NGAY_KTHUC.Value.Date != ttrinhbdn.NGAY_KTHUC.Value.Date)
                                            {
                                                if (ttrinhddn.NGAY_BDAU.Date == ttrinhbdn.NGAY_BDAU.Date)
                                                {
                                                    detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songayttdn - 1;
                                                }
                                                else
                                                {
                                                    detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songayttdn;
                                                }
                                            }

                                            if (ttrinhtnhskt != null && ttrinhtnhskt.NGAY_KTHUC.HasValue)
                                            {
                                                TimeSpan vartnhskt = ttrinhtnhskt.NGAY_KTHUC.Value.Date - ttrinhtnhskt.NGAY_BDAU.Date;
                                                var songaytnhskt = Math.Ceiling(vartnhskt.TotalDays) + 1;
                                                if (ttrinhtnhskt.NGAY_KTHUC.Value.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                {
                                                    songaytnhskt = 1;
                                                }
                                                if (ttrinhtnhskt.NGAY_KTHUC.Value.Date != ttrinhddn.NGAY_KTHUC.Value.Date)
                                                {
                                                    if (ttrinhtnhskt.NGAY_BDAU.Date == ttrinhddn.NGAY_BDAU.Date)
                                                    {
                                                        detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaytnhskt - 1;
                                                    }
                                                    else
                                                    {
                                                        detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaytnhskt;
                                                    }

                                                }


                                                if (ttrinhtc != null && ttrinhtc.NGAY_KTHUC.HasValue)
                                                {
                                                    TimeSpan vartc = ttrinhtc.NGAY_KTHUC.Value.Date - ttrinhtc.NGAY_BDAU.Date;
                                                    var songaytc = Math.Ceiling(vartc.TotalDays) + 1;
                                                    if (ttrinhtc.NGAY_KTHUC.Value.Date == ttrinhtc.NGAY_BDAU.Date)
                                                    {
                                                        songaytc = 1;
                                                    }

                                                    if (ttrinhtc.NGAY_KTHUC.Value.Date != ttrinhtnhskt.NGAY_KTHUC.Value.Date)
                                                    {
                                                        if (ttrinhtc.NGAY_BDAU.Date == ttrinhtnhskt.NGAY_BDAU.Date)
                                                        {
                                                            detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaytc - 1;
                                                        }
                                                        else
                                                        {
                                                            detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaytc;
                                                        }
                                                    }



                                                    if (ttrinhdhd != null && ttrinhdhd.NGAY_KTHUC.HasValue)
                                                    {

                                                        TimeSpan vardhd = ttrinhdhd.NGAY_KTHUC.Value.Date - ttrinhdhd.NGAY_BDAU.Date;
                                                        var songaydhd = Math.Ceiling(vardhd.TotalDays) + 1;
                                                        if (ttrinhdhd.NGAY_KTHUC.Value.Date == ttrinhdhd.NGAY_BDAU.Date)
                                                        {
                                                            songaydhd = 1;
                                                        }

                                                        if (ttrinhdhd.NGAY_KTHUC.Value.Date != ttrinhtc.NGAY_KTHUC.Value.Date)
                                                        {
                                                            if (ttrinhdhd.NGAY_BDAU.Date == ttrinhtc.NGAY_BDAU.Date)
                                                            {
                                                                detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaydhd - 1;
                                                            }
                                                            else
                                                            {
                                                                detail.TongSoNgayTCDN = detail.TongSoNgayTCDN + songaydhd;
                                                            }
                                                        }

                                                    }



                                                }



                                            }

                                        }

                                    }

                                }

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
    }
}
