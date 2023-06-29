
using EVN.Core.Domain;


namespace EVN.Api.Model
{
    public class HoSoKemTheoModel
    {

        public HoSoKemTheoModel()
        {

        }
        public HoSoKemTheoModel(HSKemTheo entity) : base()
        {
            ID = entity.ID;
            MaHoSo = entity.MaHoSo;
            MaDViQLy = entity.MaDViQLy;
            MaYeuCau = entity.MaYeuCau;
            LoaiHoSo = entity.LoaiHoSo;
            TenHoSo = entity.TenHoSo;
            Data = entity.Data;
            TrangThai = entity.TrangThai;
            Type = entity.Type;

        }
        public int ID { get; set; }
        public string MaHoSo { get; set; }
        public string MaDViQLy { get; set; }
        public string MaYeuCau { get; set; }
        public string LoaiHoSo { get; set; }
        public string TenHoSo { get; set; }
        public string Data { get; set; }
        public int TrangThai { get; set; } = 0;//0: Mới tạo, 1: KH ký, 2: EVN ký 
        public int Type { get; set; } = 0;//0: TTDN,1:BBKT,2:BBNT, 3: Phụ lục hợp đồng, 4 : TLKS
        public string Base64 { get; set; }
    }
}