using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EVN.Core.CMIS
{
    public class YeuCauRequest
    {
        public DvYeuCau DV_YEU_CAU { get; set; }
        public IList<TienTrinh> DV_TIEN_TRINH { get; set; } = new List<TienTrinh>();
        public IList<DvTienTNhan> DV_TIEN_TNHAN { get; set; } = new List<DvTienTNhan>();
        public IList<DDoDDien> CD_DDO_DDIEN { get; set; }
        public IList<HsoGto> DV_HSO_GTO { get; set; } = new List<HsoGto>();
        public IList<KHangLienHe> CD_KHANG_LIENHE { get; set; }
        public IList<HoDChung> CD_HO_DCHUNG { get; set; } = new List<HoDChung>();
    }
}
