using EVN.Core.Domain;
using EVN.Core.Models;
using System;
using System.Collections.Generic;
namespace EVN.Core.IServices
{
    public interface IXacMinhTroNgaiNewService : FX.Data.IBaseService<XacMinhTroNgaiNew, int>
    {
        // lấy báo cáo có trạng thái = kết thúc chuyển khai thác 
        IList<BaoCaoTongHopDanhGiaMucDo> GetBaoCaoTongHopDanhGiaMucDo(string fromdate, string todate);

        //lấy báo cáo có trạng thái = trở ngại hoặc hết hạn
        IList<BaoCaoTongHopDanhGiaMucDo> GetBaoCaoTongHopDanhGiaMucDo1(string fromdate, string todate);

        ChuyenKhaiThacTotal GetListChuyenKhaiThacTotal(string fromdate, string todate);

        ChuyenKhaiThacTotal GetListTroNgaiTotal(string fromdate, string todate);

        IList<XacMinhTroNgaiNew> GetBaoCaoChiTietMucDoHaiLong(string maDViQly, string fromdate, string todate);
   

    }
}
