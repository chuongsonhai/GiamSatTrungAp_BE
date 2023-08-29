using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class XacNhanTroNgaiRequest
    {
        public XacNhanTroNgaiRequest()
        {
        }
        public XacNhanTroNgaiRequest(XacNhanTroNgai lKhaoSat) : base()
        {
            ID = lKhaoSat.ID;
            CANHBAO_ID = lKhaoSat.CANHBAO_ID;
            NOIDUNG_CAUHOI = lKhaoSat.NOIDUNG_CAUHOI;
            PHANHOI_KH = lKhaoSat.PHANHOI_KH;
            THOIGIAN_KHAOSAT = lKhaoSat.THOIGIAN_KHAOSAT;
            NGUOI_KS = lKhaoSat.NGUOI_KS;
            KETQUA = lKhaoSat.KETQUA;
            TINHTRANG_KT_CB = lKhaoSat.TINHTRANG_KT_CB;
            TRANGTHAI = lKhaoSat.TRANGTHAI;
            DONVI_QLY = lKhaoSat.DONVI_QLY;

        }

     
        public int ID { get; set; }
        public int CANHBAO_ID { get; set; }
        public string NOIDUNG_CAUHOI { get; set; }
        public string PHANHOI_KH { get; set; }
        public DateTime THOIGIAN_KHAOSAT { get; set; }
        public string NGUOI_KS { get; set; }
        public string KETQUA { get; set; }
        public string TINHTRANG_KT_CB { get; set; }
        public int TRANGTHAI { get; set; }
        public string DONVI_QLY { get; set; }

    }
    public class XacNhanTroNgaikhaosatadd
    {
        public XacNhanTroNgaikhaosatadd()
        {
        }
        public XacNhanTroNgaikhaosatadd(XacNhanTroNgai lKhaoSat) : base()
        {
            CANHBAO_ID = lKhaoSat.CANHBAO_ID;
            NOIDUNG_CAUHOI = lKhaoSat.NOIDUNG_CAUHOI;
            PHANHOI_KH = lKhaoSat.PHANHOI_KH;
            PHANHOI_DV = lKhaoSat.PHANHOI_DV;
            KETQUA = lKhaoSat.KETQUA;
            NGUOI_KS = lKhaoSat.NGUOI_KS;
            ID = lKhaoSat.ID;
            MA_YC = lKhaoSat.MA_YC;
        }

        public int ID { get; set; }
        public string NOIDUNG_CAUHOI { get; set; }
        public int CANHBAO_ID { get; set; }
        public string PHANHOI_KH { get; set; }
        public string PHANHOI_DV { get; set; }
        public string KETQUA { get; set; }
        public string NGUOI_KS { get; set; }
        public string MA_YC { get; set; }

    }

    public class XacNhanTroNgakhaosatid
    {
        public XacNhanTroNgakhaosatid()
        {
        }
        public XacNhanTroNgakhaosatid(XacNhanTroNgai lKhaoSat) : base()
        {
            idKhaoSat = lKhaoSat.ID;
            noiDungKhaoSat = lKhaoSat.NOIDUNG_CAUHOI;
            khachHangPhanHoi = lKhaoSat.PHANHOI_KH;
            ketQuaKhaoSat = lKhaoSat.KETQUA;
            nguoiKhaoSat = lKhaoSat.NGUOI_KS;
            trangThaiKhaoSat = lKhaoSat.TRANGTHAI;

        }


        public int idKhaoSat { get; set; }
        public string noiDungKhaoSat { get; set; }
        public string khachHangPhanHoi { get; set; }
        public string nguoiKhaoSat { get; set; }
        public string ketQuaKhaoSat { get; set; }
        public int trangThaiKhaoSat { get; set; }


    }



}
