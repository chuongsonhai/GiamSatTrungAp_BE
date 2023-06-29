using EVN.Core;
using EVN.Core.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class BienBanKSModel
    {
        public BienBanKSModel()
        {

        }
        public BienBanKSModel(BienBanKS entity) : base()
        {
            ID = entity.ID;
            SoCongVan = entity.SoCongVan;
            NgayCongVan = entity.NgayCongVan.ToString("dd/MM/yyyy");

            MaYeuCau = entity.MaYeuCau;
            MaDViQLy = entity.MaDViQLy;

            MaKH = entity.MaKH;
            SoBienBan = entity.SoBienBan;
            TenCongTrinh = entity.TenCongTrinh;
            DiaDiemXayDung = entity.DiaDiemXayDung;
            KHTen = entity.KHTen;
            KHDaiDien = entity.KHDaiDien;
            KHDienThoai = entity.KHDienThoai;
            KHChucDanh = entity.KHChucDanh;
            EVNDonVi = entity.EVNDonVi;
            EVNDaiDien = entity.EVNDaiDien;
            EVNChucDanh = entity.EVNChucDanh;
            NgayDuocGiao = entity.NgayDuocGiao.ToString("dd/MM/yyyy");

            MaTroNgai = entity.MaTroNgai;            
            NgayKhaoSat = entity.NgayKhaoSat.ToString("dd/MM/yyyy");
            CapDienAp = entity.CapDienAp;
            TenLoDuongDay = entity.TenLoDuongDay;
            DiemDauDuKien = entity.DiemDauDuKien;
            DayDan = entity.DayDan;
            SoTramBienAp = entity.SoTramBienAp;
            SoMayBienAp = entity.SoMayBienAp;
            TongCongSuat = entity.TongCongSuat;
            ThoaThuanKyThuat = entity.ThoaThuanKyThuat;
            TrangThai = entity.TrangThai;

            ThuanLoi = entity.ThuanLoi;
            TroNgai = entity.TroNgai;

            Data = entity.Data;

            foreach (var tp in entity.ThanhPhans)
            {
                foreach (var dd in tp.DaiDiens)
                {
                    ThanhPhanKSModel thanhPhanKSModel = new ThanhPhanKSModel();
                    thanhPhanKSModel.ChucVu = dd.ChucVu;
                    thanhPhanKSModel.DaiDien = dd.DaiDien;
                    if (tp.Loai == 0)
                    {
                        thanhPhanKSModel.Loai = 0;
                    }
                    if (tp.Loai == 1)
                    {
                        thanhPhanKSModel.Loai = 1;
                    }
                    ThanhPhans.Add(thanhPhanKSModel);
                }
            }
        }

        public int ID { get; set; }
        public int CongVanID { get; set; }

        public string SoCongVan { get; set; }
        public string NgayCongVan { get; set; }

        public string MaYeuCau { get; set; }
        public string MaDViQLy { get; set; }
        public string MaKH { get; set; }
        public string SoBienBan { get; set; }
        public string TenCongTrinh { get; set; }
        public string DiaDiemXayDung { get; set; }
        public string KHTen { get; set; }
        public string KHDaiDien { get; set; }
        public string KHDienThoai { get; set; }
        public string KHChucDanh { get; set; }
        public string EVNDonVi { get; set; }
        public string EVNDaiDien { get; set; }
        public string EVNChucDanh { get; set; }
        public string NgayDuocGiao { get; set; }

        public string MaTroNgai { get; set; }
        public string TroNgai { get; set; }

        public string NgayKhaoSat { get; set; }
        public string CapDienAp { get; set; }
        public string TenLoDuongDay { get; set; }
        public string DiemDauDuKien { get; set; }
        public string DayDan { get; set; }
        public int SoTramBienAp { get; set; }
        public int SoMayBienAp { get; set; }
        public decimal TongCongSuat { get; set; }
        public string ThoaThuanKyThuat { get; set; }
        public int TrangThai { get; set; } = 0;
        public string Data { get; set; }
        public bool ThuanLoi { get; set; } = true;
        public virtual IList<ThanhPhanKSModel> ThanhPhans { get; set; } = new List<ThanhPhanKSModel>();

        public BienBanKS ToEntity(BienBanKS entity)
        {
            entity.MaKH = MaKH;
            entity.SoCongVan = SoCongVan;
            if (!string.IsNullOrWhiteSpace(NgayCongVan))
                entity.NgayCongVan = DateTime.ParseExact(NgayCongVan, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.SoBienBan = SoBienBan;
            entity.TenCongTrinh = TenCongTrinh;
            entity.DiaDiemXayDung = DiaDiemXayDung;
            entity.KHTen = KHTen;
            entity.KHDaiDien = KHDaiDien;
            entity.KHChucDanh = KHChucDanh;
            entity.EVNDonVi = EVNDonVi;
            entity.EVNDaiDien = EVNDaiDien;
            entity.EVNChucDanh = EVNChucDanh;
            if (!string.IsNullOrWhiteSpace(NgayDuocGiao))
                entity.NgayDuocGiao = DateTime.ParseExact(NgayDuocGiao, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

            if (!string.IsNullOrWhiteSpace(NgayKhaoSat))
                entity.NgayKhaoSat = DateTime.ParseExact(NgayKhaoSat, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.CapDienAp = CapDienAp;

            entity.MaTroNgai = MaTroNgai;
            entity.ThuanLoi = ThuanLoi;

            entity.TenLoDuongDay = TenLoDuongDay;
            if (!string.IsNullOrWhiteSpace(DiemDauDuKien))
            {
                ThoaThuanKyThuat = ThoaThuanKyThuat.Replace("\n", "");
                entity.DiemDauDuKien = DiemDauDuKien;
            }
            entity.DayDan = DayDan;
            entity.SoTramBienAp = SoTramBienAp;
            entity.SoMayBienAp = SoMayBienAp;
            entity.TongCongSuat = TongCongSuat;
            if (!string.IsNullOrWhiteSpace(ThoaThuanKyThuat))
            {
                ThoaThuanKyThuat = ThoaThuanKyThuat.Replace("\n", "");
                entity.ThoaThuanKyThuat = ThoaThuanKyThuat;
            }
            return entity;
        }
    }

    public class ThanhPhanKSModel
    {
        public virtual string ChucVu { get; set; }
        public virtual string DaiDien { get; set; }
        public virtual int Loai { get; set; } = 0;

    }
}
