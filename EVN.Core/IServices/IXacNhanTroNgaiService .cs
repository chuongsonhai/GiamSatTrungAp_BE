﻿using EVN.Core.Domain;
using EVN.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        IList<XacNhanTroNgai> Getnotikhaosat(string maycau, string madvi);
        Task<bool> CheckExits(string maYeuCau);
        bool Save(XacNhanTroNgai loaiCanhBao, out string message);
        IList<XacNhanTroNgai> khaosatfilter(string tungay, string denngay, int trangThaiKhaoSat, string donViQuanLy
        , int pageindex, int pagesize, out int total);
        SoLuongKhaoSatModel GetSoLuongKhaoSat(string madvi);
        IList<XacNhanTroNgai> FilterByCanhBaoIDAndTrangThai(string MA_YCAU);
        IList<XacNhanTroNgai> FilterByCanhBaoIDAndTrangThai2(string MA_YCAU, int trangthai_khaosat, int mucdo_hailong);
        XacNhanTroNgai FilterByMaYeuCau(string ID);
        XacNhanTroNgai UpdateKhaoid(int id);

        // lấy báo cáo có trạng thái = kết thúc chuyển khai thác 
        IList<BaoCaoTongHopDanhGiaMucDo> GetBaoCaoTongHopDanhGiaMucDo(string madvi, string fromdate, string todate);

        //lấy báo cáo có trạng thái = trở ngại hoặc hết hạn
        IList<BaoCaoTongHopDanhGiaMucDo> GetBaoCaoTongHopDanhGiaMucDo1(string madvi, string fromdate, string todate);

        ChuyenKhaiThacTotal GetListChuyenKhaiThacTotal(string madvi, string fromdate, string todate);

        ChuyenKhaiThacTotal GetListTroNgaiTotal(string fromdate, string todate);

        IList<XacNhanTroNgai> GetBaoCaoChiTietMucDoHaiLong(string maDViQly, string fromdate, string todat);

        
    }
}