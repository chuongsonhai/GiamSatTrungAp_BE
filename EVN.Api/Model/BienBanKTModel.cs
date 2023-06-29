using EVN.Core;
using EVN.Core.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model
{

    public class BienBanKTModel
    {
        public BienBanKTModel()
        {

        }
        public BienBanKTModel(BienBanKT entity) : base()
        {
            ID = entity.ID;
            ThoaThuanID = entity.ThoaThuanID;
            SoThoaThuan = entity.SoThoaThuan;
            ThoaThuanDauNoi = entity.ThoaThuanDauNoi;

            MaYeuCau = entity.MaYeuCau;
            MaDViQLy = entity.MaDViQLy;
            MaCViec = entity.MaCViec;

            SoBienBan = entity.SoBienBan;
            TenCongTrinh = entity.TenCongTrinh;
            DiaDiemXayDung = entity.DiaDiemXayDung;

            DonVi = entity.DonVi;
            MaSoThue = entity.MaSoThue;
            DaiDien = entity.DaiDien;
            ChucVu = entity.ChucVu;

            KHMa = entity.KHMa;
            KHTen = entity.KHTen;
            KHMaSoThue = entity.KHMaSoThue;
            KHDaiDien = entity.KHDaiDien;
            KHChucVu = entity.KHChucVu;
            KHDiaChi = entity.KHDiaChi;
            KHDienThoai = entity.KHDienThoai;
            KHEmail = entity.KHEmail;

            NgayLap = entity.NgayLap.ToString("dd/MM/yyyy");
            NgayThoaThuan = entity.NgayThoaThuan.ToString("dd/MM/yyyy");

            QuyMo = entity.QuyMo;
            HoSoKemTheo = entity.HoSoKemTheo;
            KetQuaKiemTra = entity.KetQuaKiemTra;
            TonTai = entity.TonTai;
            KienNghi = entity.KienNghi;
            YKienKhac = entity.YKienKhac;
            KetLuan = entity.KetLuan;
            ThoiHanDongDien = entity.ThoiHanDongDien;
            TrangThai = entity.TrangThai;

            MaTroNgai = entity.MaTroNgai;
            ThuanLoi = entity.ThuanLoi;
            TroNgai = entity.TroNgai;
            CongSuat = entity.ListCongSuat;

            Data = entity.Data;
            foreach (var tp in entity.ThanhPhans)
            {
                foreach (var dd in tp.DaiDiens)
                {
                    ThanhPhanKTModel thanhPhanKTModel = new ThanhPhanKTModel();
                    thanhPhanKTModel.ChucVu = dd.ChucVu;
                    thanhPhanKTModel.DaiDien = dd.DaiDien;
                    if (tp.Loai == 0)
                    {
                        thanhPhanKTModel.Loai = 0;
                    }
                    if (tp.Loai == 1)
                    {
                        thanhPhanKTModel.Loai = 1;
                    }
                    ThanhPhans.Add(thanhPhanKTModel);
                }
            }
        }

        public virtual int ID { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string MaDViQLy { get; set; }

        public virtual int CongVanID { get; set; }

        public virtual int ThoaThuanID { get; set; }
        public virtual string SoThoaThuan { get; set; }
        public virtual string NgayThoaThuan { get; set; }
        public string ThoaThuanDauNoi { get; set; }

        public virtual string SoBienBan { get; set; }
        public virtual string NgayLap { get; set; }

        public virtual string DonVi { get; set; }
        public virtual string MaSoThue { get; set; }
        public virtual string DaiDien { get; set; }
        public virtual string ChucVu { get; set; }

        public virtual string KHMa { get; set; }
        public virtual string KHTen { get; set; }
        public virtual string KHMaSoThue { get; set; }
        public virtual string KHDaiDien { get; set; }
        public virtual string KHChucVu { get; set; }
        public virtual string KHDiaChi { get; set; }
        public virtual string KHDienThoai { get; set; }
        public virtual string KHEmail { get; set; }

        public virtual string TenCongTrinh { get; set; }
        public virtual string DiaDiemXayDung { get; set; }

        public virtual string QuyMo { get; set; }
        public virtual string HoSoKemTheo { get; set; }
        public virtual string KetQuaKiemTra { get; set; }
        public virtual string TonTai { get; set; }
        public virtual string KienNghi { get; set; }
        public virtual string YKienKhac { get; set; }
        public virtual string KetLuan { get; set; }
        public virtual string ThoiHanDongDien { get; set; }
        public virtual int TrangThai { get; set; }
        public virtual string MaCViec { get; set; }
        public virtual string Data { get; set; }

        public string MaTroNgai { get; set; }
        public bool ThuanLoi { get; set; } = true;
        public string TroNgai { get; set; }
        public bool LapBienBan { get; set; }
        public IList<string> CongSuat { get; set; }

        public virtual IList<ThanhPhanKTModel> ThanhPhans { get; set; } = new List<ThanhPhanKTModel>();
        public BienBanKT ToEntity(BienBanKT entity)
        {
            entity.ThoaThuanID = ThoaThuanID;
            entity.SoThoaThuan = SoThoaThuan;
            entity.ThoaThuanDauNoi = ThoaThuanDauNoi;

            entity.MaYeuCau = MaYeuCau;
            entity.MaDViQLy = MaDViQLy;
            entity.MaCViec = MaCViec;

            entity.SoBienBan = SoBienBan;
            entity.TenCongTrinh = TenCongTrinh;
            entity.DiaDiemXayDung = DiaDiemXayDung;

            entity.DonVi = DonVi;
            entity.MaSoThue = MaSoThue;
            entity.DaiDien = DaiDien;
            entity.ChucVu = ChucVu;

            entity.KHMa = KHMa;
            entity.KHTen = KHTen;
            entity.KHMaSoThue = KHMaSoThue;
            entity.KHDaiDien = KHDaiDien;
            entity.KHChucVu = KHChucVu;
            entity.KHDiaChi = KHDiaChi;
            entity.KHDienThoai = KHDienThoai;
            entity.KHEmail = KHEmail;

            entity.QuyMo = QuyMo;
            entity.HoSoKemTheo = HoSoKemTheo;
            entity.KetQuaKiemTra = KetQuaKiemTra;
            entity.TonTai = TonTai;
            entity.KienNghi = KienNghi;
            entity.YKienKhac = YKienKhac;
            entity.KetLuan = KetLuan;
            entity.ThoiHanDongDien = ThoiHanDongDien;

            entity.MaTroNgai = MaTroNgai;
            entity.TroNgai = TroNgai;
            entity.ThuanLoi = ThuanLoi;
            entity.CongSuat = JsonConvert.SerializeObject(CongSuat);

            if (!string.IsNullOrWhiteSpace(NgayLap))
                entity.NgayLap = DateTime.ParseExact(NgayLap, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

            if (!string.IsNullOrWhiteSpace(NgayThoaThuan))
                entity.NgayThoaThuan = DateTime.ParseExact(NgayThoaThuan, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

            return entity;
        }
    }
    public class ThanhPhanKTModel
    {

        public virtual string ChucVu { get; set; }
        public virtual string DaiDien { get; set; }
        public virtual int Loai { get; set; } = 0;

    }
}
