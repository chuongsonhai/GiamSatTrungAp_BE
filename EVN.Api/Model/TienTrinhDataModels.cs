using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class TienTrinhDataModels
    {
        public IList<BoPhan> boPhans { get; set; }
        public List<NhanVien> nhanViens { get; set; }
        public IList<CauHinhCViec> congViecs { get; set; }
        public string staffCode { get; set; }
        public string deptId { get; set; }
        public string maCViec { get; set; }
    }

}