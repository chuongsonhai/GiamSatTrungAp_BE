using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class CanhBaoId
    {
        public CanhBaoRequestId listcanhbaoid { get; set; }

    }
    public class CanhBaoRequestId
    {
        public List<Canhbaoid> listcanhbao { get; set; }

        public List<Yeucauid> listyeucau { get; set; }

        public List<phanhoiid> listphanhoi { get; set; }

    }
    public class Canhbaoid
    {
        public int idCanhBao { get; set; }
        public int maLoaiCanhBao { get; set; }
        public string noiDungCanhBao { get; set; }
        public DateTime thoiGianGui { get; set; }
        public int trangThai { get; set; }
    }
    public class Yeucauid
    {
        public int idYeuCau { get; set; }
        public string maYeuCau { get; set; }
        public string tenKhachHang { get; set; }
        public string soDienThoai { get; set; }
        public TrangThaiCongVan trangThaiYeuCau { get; set; }
    }

    public class phanhoiid
    {
        public int idPhanHoi { get; set; }
        public string noiDungPhanHoi { get; set; }
        public string nguoiGui { get; set; }
    }
}
