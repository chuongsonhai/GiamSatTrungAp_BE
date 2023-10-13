using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IBienBanKSService : FX.Data.IBaseService<BienBanKS, int>
    {
        BienBanKS GetbyYeuCau(string maYCau);

        IList<BienBanKS> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);

        bool Sign(BienBanKS item, string maCViec, string maPBanNhan, string nVienNhan, DateTime ngayHen, string noiDung);

        bool SignRemote(BienBanKS item, byte[] pdfdata, string maCViec, string maPBanNhan, string nVienNhan, DateTime ngayHen, string noiDung);

        bool Approve(BienBanKS item, KetQuaKS ketqua);

        BienBanKS Update(BienBanKS bienban, IList<ThanhPhanKS> thanhPhans, out string message);

        bool HuyHoSo(BienBanKS bienban, KetQuaKS ketqua);

        bool HuyHoSo2(BienBanKS bienban, KetQuaKS ketqua);

        bool Cancel(CongVanYeuCau yeucau, BienBanKS item, string noiDung);

        bool Confirm(BienBanKS item, byte[] pdfdata);

        bool KhaoSatLai(BienBanKS item, out string message);
    }
}
