using EVN.Core.Domain;
using EVN.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface ICanhBaoService : FX.Data.IBaseService<CanhBao, int>
    {
        CanhBao GetbyNo(int idloai);
        CanhBao Getbyid(int id);
        IList<SoLuongGuiModel> GetSoLuongGui(string tungay, string denngay);
        IList<CanhBao> GetbyCanhbao(string tungay, string denngay);
        IList<CanhBao> GetAllCanhBao(out int total);
        IList<CanhBao> Filter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string maDonVi, int solangui, string maYeuCau, int pageindex, int pagesize, out int total);
        Task<bool> CheckExits(string maYeuCau, int loaicanhbaoid);
        bool CheckExits11(string maYeuCau, int loaicanhbaoid);
        // CanhBao CheckExitsid(int intl);
        CanhBao GetByMaYeuCautontai(string MaYeuCau, int loaicanhbaoid);
        bool CreateCanhBao(CanhBao canhbao, out string message);
        IList<CanhBao> GetbykhachhangFilter(string tungay, string denngay, int maLoaiCanhBao, string donViQuanLy, int pageindex, int pagesize, out int total);
        IList<CanhBao> FilterBytrangThaiAndDViQuanLy(string fromDate, string toDate, int trangThai, string DonViDienLuc);
        IList<CanhBao> FilterByMaYCauAndDViQuanLy(string fromDate, string toDate, string MaYeuCau, string DonViDienLuc);
        IList<BaocaoTienDoCanhBaoModel> GetBaoCaotonghoptiendo(string maDViQly, int maloaicanhbao, string fromdate, string todate);
        CanhBao GetByMaYeuCau(string MaYeuCau);
        IList<BaoCaoChiTietGiamSatTienDo> GetBaoCaoChiTietGiamSatTienDo(string maDViQly, string fromdate, string todate, int MaLoaiCanhBao);
        BaocaoTienDoCanhBaoModel GetBaoCaotonghoptiendoTong(string maDViQly, int maloaicanhbao, string fromdate, string todate);
    }
}
