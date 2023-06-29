using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core
{
    public enum TrangThaiCongVan
    {
        MoiTao = 0,
        TiepNhan = 1,
        PhanCongKS = 2,
        GhiNhanKS = 3,
        BienBanKS = 4,
        DuThaoTTDN = 5,
        KHKy = 6,
        DuChuKy = 7,
        HoanThanh = 8,
        ChuyenTiep = 9,
        Huy = 13
    }

    public enum TrangThaiNghiemThu
    {
        MoiTao = 0,
        TiepNhan = 1,
        PhanCongKT = 2,
        GhiNhanKT = 3,
        BienBanKT = 4,
        DuThaoHD = 5,
        PhanCongTC = 6,
        KetQuaTC = 7,
        BienBanTC = 8,
        NghiemThu = 9,
        EVNKyHD = 10,
        HoanThanh = 11,
        Huy = 13
    }

    public static class LoaiHSoCode
    {
        public static string CV_DN = "55";
        public static string BB_KS = "56";
        public static string BB_DN = "57";
        public static string CV_NT = "58";
        public static string HD_MB = "59";
        public static string BB_KT = "60";
        public static string BB_NT = "61";
        public static string DN_NT = "62";
        public static string HD_SH = "HDSH";
        public static string HD_NSH = "HDNSH";
        public static string HD_NH = "HDNH";
        public static string BB_TT = "BBAN_TTHAO";
        public static string PL_HD = "PLuc_hdong";

        public static string PL_HD_DB = "PL_TTDBao";
        public static string PL_HD_MB = "PL_TTMBan";
        public static string PL_HD_CD = "PL_TTCDut";
        public static string PL_BDPT = "PL_BDPT";
        public static string PL_TB = "PL_TB";

        public static string TL_TKKT = "TL_TKKT";
        public static string TL_HDVH = "TL_HDVH";
        public static string TL_BBNT = "TL_BBNT";
        public static string TL_DKDD = "TL_DKDD";
    }

    public static class DocumentCode
    {
        public static string CV_DN = "CV_DN";
        public static string BB_KS = "BB_KS";
        public static string CV_NT = "CV_NT";
        public static string BB_KT = "BB_KT";

        public static string PL_HD_DB = "PL_TTDBao";
        public static string PL_HD_MB = "PL_TTMBan";
        public static string PL_HD_CD = "PL_TTCDut";
    }

    public static class DateTimeParse
    {
        public static string[] Format = new string[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy" };
    }

    public enum TrangThaiBienBan
    {
        DuThao = -1,
        MoiTao = 0,
        DaDuyet = 1,
        KhachHangKy = 2,
        HoanThanh = 3,
        TuChoi = 7
    }
}
