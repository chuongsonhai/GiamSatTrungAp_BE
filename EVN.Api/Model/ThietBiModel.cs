using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class DSachThietBiRequest
    {
        public string MaDViQLy { get; set; }
        public string MaYeuCau { get; set; }
        public IList<ThietBiModel> Items { get; set; }
    }

    public class ThietBiModel
    {
        public ThietBiModel()
        {

        }
        public ThietBiModel(ThietBi entity) : base()
        {
            ID = entity.ID;
            MaDViQLy = entity.MaDViQLy;
            MaYeuCau = entity.MaYeuCau;
            Ten = entity.Ten;
            CongSuat = entity.CongSuat;
            SoLuong = entity.SoLuong;
            TongCongSuat = entity.TongCongSuat;
            HeSoDongThoi = entity.HeSoDongThoi;

            SoGio = entity.SoGio;
            SoNgay = entity.SoNgay;
            DienNangSD = entity.DienNangSD;
            MucDichSD = entity.MucDichSD;
        }

        public virtual int ID { get; set; }
        public virtual string MaDViQLy { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string Ten { get; set; }
        public virtual decimal CongSuat { get; set; }
        public virtual decimal SoLuong { get; set; }
        public virtual decimal HeSoDongThoi { get; set; }
        public virtual decimal SoGio { get; set; }
        public virtual decimal SoNgay { get; set; }
        public virtual decimal TongCongSuat { get; set; }
        public virtual decimal DienNangSD { get; set; }
        public virtual string MucDichSD { get; set; }


        public ThietBi ToEntity(ThietBi entity)
        {
            entity.MaDViQLy = MaDViQLy;
            entity.MaYeuCau = MaYeuCau;
            entity.Ten = Ten;
            entity.CongSuat = CongSuat;
            entity.SoLuong = SoLuong;
            entity.TongCongSuat = TongCongSuat;
            entity.HeSoDongThoi = HeSoDongThoi;
            
            entity.SoGio = SoGio;
            entity.SoNgay = SoNgay;
            entity.TongCongSuat = TongCongSuat;
            entity.DienNangSD = DienNangSD;
            entity.MucDichSD = MucDichSD;
            return entity;
        }
    }
}