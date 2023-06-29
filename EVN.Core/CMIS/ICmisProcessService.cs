using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.CMIS
{
    public interface ICmisProcessService
    {
        bool TiepNhanYeuCau(CongVanYeuCau congvan, DvTienTrinh tienTrinh);

        bool ChuyenTiep(CongVanYeuCau congvan, string maDViTNhan);

        bool LapBBanTrTh(BienBanTT bienban, DvTienTrinh tienTrinh);

        HSoGToResult GetlistHSoGTo(string maDViQly, string maYCau);

        bool UploadPdf(string maDViQly, string maYCau, byte[] pdfdata, string maLoaiHSo = "53");

        byte[] GetData(string maDViQly, string maYCau, string maLoaiHSo);

        IList<CongToResult> GetCongTo(string maDViQLy, string soCTo);

        IList<TIResult> GetTI(string maDViQLy, string soTI);

        IList<TUResult> GetTU(string maDViQLy, string soTU);

        IList<TTRamResult> GetTTTram(string maDViQLy, string maTram);

        IList<TienTrinh> GetTienDo(string maDViQLy, string maYCau, string maDDoDDien);

        IList<ThongTinNhomGia> GetGiaNhomNNHieuluc(string maCapDA, string ngayHieuLuc);
        IList<ThongTinUQ> GetDanhMucUQ(string maDViQLy, string tenDanhMuc = "D_PCAP_UQUYEN_DVI_MAHSO", string PARAM  = "HDNSH");

        IList<BoPhan> GetBoPhans(string maDViQLy);

        IList<BoPhan> GetDSTo(string maDViQLy);
        IList<NhanVien> GetNhanViens(string maDViQLy);

        bool SignNhanVien(string maDViQLy, string maYeuCau, string maNVien, string maLHSo, string maDTuongKy, string idKey = "-1");        
    }
}
