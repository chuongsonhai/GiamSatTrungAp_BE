using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model.Response
{
    public class HistoryData
    {
        public HistoryData(ThoaThuanDNChiTiet entity)
        {            
            MaYeuCau = entity.MaYeuCau;
            DuAnDien = entity.DuAnDien;
            DiaChiDungDien = entity.DiaChiDungDien;
            TrangThai = entity.TrangThai;
            Type = (int)entity.Type;
            MoTa = entity.MoTa;
            NgayYeuCau = entity.NgayYeuCau.ToString("dd/MM/yyyy");
            NgayThucHien = entity.NgayThucHien.ToString("dd/MM/yyyy");

            TimeSpan variable = entity.NgayThucHien - entity.NgayYeuCau;
            SoNgayThucHien = (decimal)Math.Round(variable.TotalDays, 1, MidpointRounding.AwayFromZero) + 1;
        }
        
        public string MaYeuCau { get; set; }

        public string DuAnDien { get; set; }
        public string DiaChiDungDien { get; set; }
        public int TrangThai { get; set; } = 0;//0: Mới, 1: Đang xử lý, 2: Hoàn thành
        public int Type { get; set; }
        public string MoTa { get; set; }
        public string NgayYeuCau { get; set; }
        public string NgayThucHien { get; set; }
        public decimal SoNgayThucHien { get; set; }
    }
}