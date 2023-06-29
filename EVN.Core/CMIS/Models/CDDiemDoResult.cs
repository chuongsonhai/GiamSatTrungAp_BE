using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.CMIS
{
    public class CDDiemDoResult
    {
        public string chuoiGia { get; set; }
        public int csuat { get; set; }
        public string diaChi { get; set; }

        public int kimuaCspk { get; set; } = 0;
        public int loaiDdo { get; set; } = 1;
        public string maCapda { get; set; } = "1";
        public string maDdo { get; set; }
        public string maDdoDdien { get; set; }
        public string maDviqly { get; set; }
        public string maHdong { get; set; }
        public string maTo { get; set; }
        public string maTram { get; set; }
        public string mucDich { get; set; }


        public string pha { get; set; } = "A";
        public int soHo { get; set; } = 1;
        public int soPha { get; set; } = 1;
        public int sohuuLuoi { get; set; } = 1;
        public int ttrangTreothao { get; set; } = 0;
    }
}
