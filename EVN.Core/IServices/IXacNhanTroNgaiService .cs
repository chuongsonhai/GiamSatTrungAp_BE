using EVN.Core.Domain;
using EVN.Core.Models;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IXacNhanTroNgaiService : FX.Data.IBaseService<XacNhanTroNgai, int>
    {
        XacNhanTroNgai GetbyNo(int idloai);
        IList<XacNhanTroNgai> GetbyFilter(string tungay, string denngay, int trangThaiKhaoSat, string maYeuCau, string donViQuanLy
            , int pageindex, int pagesize, out int total);
        //IList<XacNhanTroNgai> GetbykhachhangFilter(string tungay, string denngay, int maLoaiCanhBao, string donViQuanLy
        //    , int pageindex, int pagesize, out int total);
        IList<XacNhanTroNgai> GetbyKhaoSat(string tungay, string denngay);
        XacNhanTroNgai GetKhaoSat(int id);
        bool Save(XacNhanTroNgai loaiCanhBao, out string message);
        IList<XacNhanTroNgai> khaosatfilter(string tungay, string denngay, int trangThaiKhaoSat, string donViQuanLy
        , int pageindex, int pagesize, out int total);
        SoLuongKhaoSatModel GetSoLuongKhaoSat(string tungay, string denngay );
        IList<XacNhanTroNgai> FilterByCanhBaoIDAndTrangThai(int ID, int TrangThaiKhaoSat);
        IList<XacNhanTroNgai> FilterByCanhBaoID(int ID);
        XacNhanTroNgai UpdateKhaoid(int id);
    }
}