using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Api.Model
{
    public class BaoCaoTongHopModel
    {
        public IList<BaoCaoTongHopChiTietDataModel> baoCaoTongHopChiTietDataModels = new List<BaoCaoTongHopChiTietDataModel>();
        public double TongSoCTTrongThang { get; set; } = 0;
        public double TongTGTrongThang { get; set; } = 0;
        public double TongSoCTLuyKe { get; set; } = 0;
        public double TongTGLuyKe { get; set; } = 0;

        public double TongTGTiepNhanTrongThang { get; set; } = 0;
        public double TongTGKhaoSatTrongThang { get; set; } = 0;
        public double TongTGTTDNTrongThang { get; set; } = 0;
        public double TongTGNTTrongThang { get; set; } = 0;

        public double TongTGTiepNhanLuyKe { get; set; } = 0;
        public double TongTGKhaoSatLuyKe { get; set; } = 0;
        public double TongTGTTDNLuyKe { get; set; } = 0;
        public double TongTGNTLuyKe { get; set; } = 0;

        public BaoCaoTongHopModel(IList<BaoCaoTongHopChiTietData> baoCaoTongHopChiTietDatas)
        {
            double count = 0;
            double countluyke = 0;
            foreach (var item in baoCaoTongHopChiTietDatas)
            {
                BaoCaoTongHopChiTietDataModel detail = new BaoCaoTongHopChiTietDataModel();
                detail.MaDonVi = item.MaDonVi;
                detail.TenDonVi = item.TenDonVi;
                detail.SoCTTrongThang = item.SoCTTrongThang;
                detail.TGTrongThang = item.TGTrongThang;
                detail.SoNgayTrongThang = item.SoNgayTrongThang;

                detail.SoCTLuyKe = item.SoCTLuyKe;
                detail.TGLuyKe = item.TGLuyKe;
                detail.SoNgayLuyKe = item.SoNgayLuyKe;

                detail.TGTiepNhanTrongThang = item.TGTiepNhanTrongThang;                
                detail.TGKhaoSatTrongThang = item.TGKhaoSatTrongThang;
                detail.SLKSatTrongThang = item.SLKSatTrongThang;

                detail.TGTTDNTrongThang = item.TGTTDNTrongThang;
                detail.SLTTDNTrongThang = item.SLTTDNTrongThang;

                detail.TGNTTrongThang = item.TGNTTrongThang;
                detail.SLNTTrongThang = item.SLNTTrongThang;


                detail.TGTiepNhanLuyKe = item.TGTiepNhanLuyKe;
                detail.TGKhaoSatLuyKe = item.TGKhaoSatLuyKe;
                detail.SLKSatLuyKe = item.SLKSatLuyKe;

                detail.TGTTDNLuyKe = item.TGTTDNLuyKe;
                detail.SLTTDNLuyKe = item.SLTTDNLuyKe;

                detail.TGNTLuyKe = item.TGNTLuyKe;
                detail.SLNTLuyKe = item.SLNTLuyKe;

                detail.SoNgayTiepNhanTrongThang = item.SoNgayTiepNhanTrongThang;
                detail.SoNgayKhaoSatTrongThang = item.SoNgayKhaoSatTrongThang;
                detail.SoNgayTTDNTrongThang = item.SoNgayTTDNTrongThang;
                detail.SoNgayNTTrongThang = item.SoNgayNTTrongThang;

                detail.SoNgayTiepNhanLuyKe = item.SoNgayTiepNhanLuyKe;
                detail.SoNgayKhaoSatLuyKe = item.SoNgayKhaoSatLuyKe;
                detail.SoNgayTTDNLuyKe = item.SoNgayTTDNLuyKe;
                detail.SoNgayNTLuyKe = item.SoNgayNTLuyKe;

                baoCaoTongHopChiTietDataModels.Add(detail);
                if (detail.SoCTTrongThang > 0)
                {
                    count++;
                }
                if (detail.SoCTLuyKe > 0)
                {
                    countluyke++;
                }
            }

            if (count > 0)
            {
                TongSoCTTrongThang = baoCaoTongHopChiTietDatas.Sum(x => x.SoCTTrongThang);

                if (TongSoCTTrongThang > 0)
                {
                    TongTGTrongThang = Math.Round(baoCaoTongHopChiTietDatas.Sum(x => x.SoNgayTrongThang) / TongSoCTTrongThang, 2, MidpointRounding.AwayFromZero);                
                    TongTGTiepNhanTrongThang = Math.Round(baoCaoTongHopChiTietDatas.Sum(x => x.SoNgayTiepNhanTrongThang)/ TongSoCTTrongThang, 2, MidpointRounding.AwayFromZero);
                }
                if (baoCaoTongHopChiTietDatas.Sum(x => x.SLKSatTrongThang) > 0)
                {
                    TongTGKhaoSatTrongThang = Math.Round(baoCaoTongHopChiTietDatas.Sum(x => x.SoNgayKhaoSatTrongThang) / baoCaoTongHopChiTietDatas.Sum(x => x.SLKSatTrongThang), 2, MidpointRounding.AwayFromZero);
                }
                if (baoCaoTongHopChiTietDatas.Sum(x => x.SLTTDNTrongThang) > 0)
                {
                    TongTGTTDNTrongThang = Math.Round(baoCaoTongHopChiTietDatas.Sum(x => x.SoNgayTTDNTrongThang) / baoCaoTongHopChiTietDatas.Sum(x => x.SLTTDNTrongThang), 2, MidpointRounding.AwayFromZero);
                }
                if (baoCaoTongHopChiTietDatas.Sum(x => x.SLNTTrongThang) > 0)
                {
                    TongTGNTTrongThang = Math.Round(baoCaoTongHopChiTietDatas.Sum(x => x.SoNgayNTTrongThang) / baoCaoTongHopChiTietDatas.Sum(x => x.SLNTTrongThang), 2, MidpointRounding.AwayFromZero);
                }
            }
            if (countluyke > 0)
            {
                TongSoCTLuyKe = baoCaoTongHopChiTietDatas.Sum(x => x.SoCTLuyKe);

                if (TongSoCTLuyKe > 0)
                {
                    TongTGLuyKe = Math.Round(baoCaoTongHopChiTietDatas.Sum(x => x.SoNgayLuyKe) / TongSoCTLuyKe, 2, MidpointRounding.AwayFromZero);
                    TongTGTiepNhanLuyKe = Math.Round(baoCaoTongHopChiTietDatas.Sum(x => x.SoNgayTiepNhanLuyKe) / TongSoCTLuyKe, 2, MidpointRounding.AwayFromZero);
                }
                if (baoCaoTongHopChiTietDatas.Sum(x => x.SLKSatLuyKe) > 0)
                {
                    TongTGKhaoSatLuyKe = Math.Round(baoCaoTongHopChiTietDatas.Sum(x => x.SoNgayKhaoSatLuyKe) / baoCaoTongHopChiTietDatas.Sum(x => x.SLKSatLuyKe), 2, MidpointRounding.AwayFromZero);
                }
                if (baoCaoTongHopChiTietDatas.Sum(x => x.SLTTDNLuyKe) > 0)
                {
                    TongTGTTDNLuyKe = Math.Round(baoCaoTongHopChiTietDatas.Sum(x => x.SoNgayTTDNLuyKe) / baoCaoTongHopChiTietDatas.Sum(x => x.SLTTDNLuyKe), 2, MidpointRounding.AwayFromZero);
                }
                if (baoCaoTongHopChiTietDatas.Sum(x => x.SLNTLuyKe) > 0)
                {
                    TongTGNTLuyKe = Math.Round(baoCaoTongHopChiTietDatas.Sum(x => x.SoNgayNTLuyKe) / baoCaoTongHopChiTietDatas.Sum(x => x.SLNTLuyKe), 2, MidpointRounding.AwayFromZero);
                }
            }
        }

    }
    public class BaoCaoTongHopChiTietDataModel
    {
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double SoCTTrongThang { get; set; } = 0;
        public double TGTrongThang { get; set; } = 0;
        public double SoNgayTrongThang { get; set; } = 0;
        public double SoCTLuyKe { get; set; } = 0;
        public double TGLuyKe { get; set; } = 0;
        public double SoNgayLuyKe { get; set; } = 0;

        public double TGTiepNhanTrongThang { get; set; } = 0;
        public double TGKhaoSatTrongThang { get; set; } = 0;
        public int SLKSatTrongThang { get; set; } = 0;

        public double TGTTDNTrongThang { get; set; } = 0;
        public int SLTTDNTrongThang { get; set; } = 0;

        public double TGNTTrongThang { get; set; } = 0;
        public int SLNTTrongThang { get; set; } = 0;

        public double TGTiepNhanLuyKe { get; set; } = 0;
        public double TGKhaoSatLuyKe { get; set; } = 0;
        public int SLKSatLuyKe { get; set; } = 0;

        public double TGTTDNLuyKe { get; set; } = 0;
        public int SLTTDNLuyKe { get; set; } = 0;

        public double TGNTLuyKe { get; set; } = 0;
        public int SLNTLuyKe { get; set; } = 0;

        public double SoNgayTiepNhanTrongThang { get; set; } = 0;
        public double SoNgayKhaoSatTrongThang { get; set; } = 0;
        public double SoNgayTTDNTrongThang { get; set; } = 0;
        public double SoNgayNTTrongThang { get; set; } = 0;

        public double SoNgayTiepNhanLuyKe { get; set; } = 0;
        public double SoNgayKhaoSatLuyKe { get; set; } = 0;
        public double SoNgayTTDNLuyKe { get; set; } = 0;
        public double SoNgayNTLuyKe { get; set; } = 0;
    }
}