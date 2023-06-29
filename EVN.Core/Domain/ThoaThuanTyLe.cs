using EVN.Core.IServices;
using EVN.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace EVN.Core.Domain
{
    public class ThoaThuanTyLe
    {
        public virtual int ID { get; set; }
        public virtual int CongVanID { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string MaDViQLy { get; set; }
        public virtual string Data { get; set; }
        public virtual DateTime NgayLap { get; set; } = DateTime.Now;
        public virtual string DiaDiem { get; set; }
        public virtual string DiaChiGiaoDich { get; set; }
        public virtual string DonVi { get; set; }
        public virtual string MaSoThue { get; set; }
        public virtual string SoTaiKhoan { get; set; }
        public virtual string NganHang { get; set; }
        public virtual string DienThoai { get; set; }
        public virtual string Fax { get; set; }
        public virtual string Email { get; set; }
        public virtual string DienThoaiCSKH { get; set; }
        public virtual string Website { get; set; }
        public virtual string DaiDien { get; set; }
        public virtual string ChucVu { get; set; }
        public virtual string SoGiayTo { get; set; }
        public virtual DateTime NgayCap { get; set; } = DateTime.Now;
        public virtual string NoiCap { get; set; }
        public virtual string VanBanUQ { get; set; }
        public virtual DateTime NgayUQ { get; set; } = DateTime.Now;
        public virtual string NguoiKyUQ { get; set; }
        public virtual DateTime NgayKyUQ { get; set; } = DateTime.Now;
        public virtual string ChucVuUQ { get; set; }
        public virtual string KHTen { get; set; }
        public virtual string KHMa { get; set; }
        public virtual string KHDiaChiGiaoDich { get; set; }
        public virtual string KHDiaChiDungDien { get; set; }
        public virtual string KHDangKyKD { get; set; }
        public virtual string KHNoiCapDangKyKD { get; set; }
        public virtual DateTime KHNgayCaoDangKyKD { get; set; } = DateTime.Now;
        public virtual string KHMaSoThue { get; set; }
        public virtual string KHSoTK { get; set; }
        public virtual string KHNganHang { get; set; }
        public virtual string KHDienThoai { get; set; }
        public virtual string KHFax { get; set; }
        public virtual string KHEmail { get; set; }
        public virtual string KHDaiDien { get; set; }
        public virtual string KHChucVu { get; set; }
        public virtual string KHSoGiayTo { get; set; }
        public virtual DateTime KHNgayCap { get; set; } = DateTime.Now;
        public virtual string KHNoiCap { get; set; }
        public virtual string KHVanBanUQ { get; set; }
        public virtual DateTime KHNgayUQ { get; set; } = DateTime.Now;
        public virtual string KHNguoiKyUQ { get; set; }
        public virtual DateTime KHNgayKyUQ { get; set; } = DateTime.Now;
        public virtual string UngDung { get; set; }
        public virtual int TrangThai { get; set; }
        public virtual IList<MucDichThucTeSDD> MucDichThucTeSDD { get; set; } = new List<MucDichThucTeSDD>();
        public virtual IList<GiaDienTheoMucDich> GiaDienTheoMucDich { get; set; } = new List<GiaDienTheoMucDich>();

        public virtual string GetPdf(bool replace = false)
        {
            if (!string.IsNullOrWhiteSpace(Data) && !replace)
                return Data;

            string xmldata = GetXMLData();
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.XmlResolver = null;
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(xmldata);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var temp = TemplateManagement.GetTemplate(LoaiHSoCode.PL_HD_MB);
            StringWriter sw = new StringWriter();
            XslCompiledTransform xct = new XslCompiledTransform();
            xct.Load(new XmlTextReader(new StringReader(temp.XsltData)));
            xct.Transform(xmlDoc, null, sw);
            string html = sw.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");

            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            htmlToPdf.Zoom = 1.6f;
            htmlToPdf.Margins.Top = 5;
            htmlToPdf.Margins.Bottom = 5;
            htmlToPdf.Size = NReco.PdfGenerator.PageSize.A4;
            htmlToPdf.CustomWkHtmlPageArgs = "--enable-smart-shrinking";

            string folder = $"{MaDViQLy}/{MaYeuCau}";
            var pdf = htmlToPdf.GeneratePdf(html);
            IRepository repository = new FileStoreRepository();
            return repository.Store(folder, pdf, Data);
        }
        private string GetXMLData()
        {
            StringBuilder str = new StringBuilder("<ThoaThuanTyLe>");
            str.AppendFormat("<NgayLap><![CDATA[{0}]]></NgayLap>", NgayLap.ToString("dd/MM/yyyy"));
            str.AppendFormat("<DiaDiem><![CDATA[{0}]]></DiaDiem>", DiaDiem);
            str.AppendFormat("<DiaChiGiaoDich><![CDATA[{0}]]></DiaChiGiaoDich>", DiaChiGiaoDich);
            str.AppendFormat("<DonVi><![CDATA[{0}]]></DonVi>", DonVi);
            str.AppendFormat("<MaSoThue><![CDATA[{0}]]></MaSoThue>", MaSoThue);
            str.AppendFormat("<SoTaiKhoan><![CDATA[{0}]]></SoTaiKhoan>", SoTaiKhoan);
            str.AppendFormat("<NganHang><![CDATA[{0}]]></NganHang>", NganHang);
            str.AppendFormat("<DienThoai><![CDATA[{0}]]></DienThoai>", DienThoai);
            str.AppendFormat("<Fax><![CDATA[{0}]]></Fax>", Fax);
            str.AppendFormat("<Email><![CDATA[{0}]]></Email>", Email);
            str.AppendFormat("<DienThoaiCSKH><![CDATA[{0}]]></DienThoaiCSKH>", DienThoaiCSKH);
            str.AppendFormat("<Website><![CDATA[{0}]]></Website>", Website);
            str.AppendFormat("<DaiDien><![CDATA[{0}]]></DaiDien>", DaiDien);
            str.AppendFormat("<ChucVu><![CDATA[{0}]]></ChucVu>", ChucVu);
            str.AppendFormat("<SoGiayTo><![CDATA[{0}]]></SoGiayTo>", SoGiayTo);
            str.AppendFormat("<NgayCap><![CDATA[{0}]]></NgayCap>", NgayCap.ToString("dd/MM/yyyy"));
            str.AppendFormat("<NoiCap><![CDATA[{0}]]></NoiCap>", NoiCap);
            str.AppendFormat("<VanBanUQ><![CDATA[{0}]]></VanBanUQ>", VanBanUQ);
            str.AppendFormat("<NgayUQ><![CDATA[{0}]]></NgayUQ>", NgayUQ.ToString("dd/MM/yyyy"));
            str.AppendFormat("<NguoiKyUQ><![CDATA[{0}]]></NguoiKyUQ>", NguoiKyUQ);
            str.AppendFormat("<NgayKyUQ><![CDATA[{0}]]></NgayKyUQ>", NgayKyUQ.ToString("dd/MM/yyyy"));
            str.AppendFormat("<ChucVuUQ><![CDATA[{0}]]></ChucVuUQ>", ChucVuUQ);

            str.AppendFormat("<KHTen><![CDATA[{0}]]></KHTen>", KHTen);
            str.AppendFormat("<KHMa><![CDATA[{0}]]></KHMa>", KHMa);
            str.AppendFormat("<KHDiaChiGiaoDich><![CDATA[{0}]]></KHDiaChiGiaoDich>", KHDiaChiGiaoDich);

            str.AppendFormat("<KHDiaChiDungDien><![CDATA[{0}]]></KHDiaChiDungDien>", KHDiaChiDungDien);
            str.AppendFormat("<KHDangKyKD><![CDATA[{0}]]></KHDangKyKD>", KHDangKyKD);
            str.AppendFormat("<KHNoiCapDangKyKD><![CDATA[{0}]]></KHNoiCapDangKyKD>", KHNoiCapDangKyKD);
            str.AppendFormat("<KHNgayCaoDangKyKD><![CDATA[{0}]]></KHNgayCaoDangKyKD>", KHNgayCaoDangKyKD.ToString("dd/MM/yyyy"));
            str.AppendFormat("<KHMaSoThue><![CDATA[{0}]]></KHMaSoThue>", KHMaSoThue);
            str.AppendFormat("<KHSoTK><![CDATA[{0}]]></KHSoTK>", KHSoTK);
            str.AppendFormat("<KHNganHang><![CDATA[{0}]]></KHNganHang>", KHNganHang);
            str.AppendFormat("<KHDienThoai><![CDATA[{0}]]></KHDienThoai>", KHDienThoai);
            str.AppendFormat("<KHFax><![CDATA[{0}]]></KHFax>", KHFax);
            str.AppendFormat("<KHEmail><![CDATA[{0}]]></KHEmail>", KHEmail);
            str.AppendFormat("<KHDaiDien><![CDATA[{0}]]></KHDaiDien>", KHDaiDien);
            str.AppendFormat("<KHChucVu><![CDATA[{0}]]></KHChucVu>", KHChucVu);
            str.AppendFormat("<KHSoGiayTo><![CDATA[{0}]]></KHSoGiayTo>", KHSoGiayTo);
            str.AppendFormat("<KHNgayCap><![CDATA[{0}]]></KHNgayCap>", KHNgayCap.ToString("dd/MM/yyyy"));
            str.AppendFormat("<KHNoiCap><![CDATA[{0}]]></KHNoiCap>", KHNoiCap);
            str.AppendFormat("<KHVanBanUQ><![CDATA[{0}]]></KHVanBanUQ>", KHVanBanUQ);
            str.AppendFormat("<KHNgayUQ><![CDATA[{0}]]></KHNgayUQ>", KHNgayUQ.ToString("dd/MM/yyyy"));
            str.AppendFormat("<KHNguoiKyUQ><![CDATA[{0}]]></KHNguoiKyUQ>", KHNguoiKyUQ);
            str.AppendFormat("<KHNgayKyUQ><![CDATA[{0}]]></KHNgayKyUQ>", KHNgayKyUQ.ToString("dd/MM/yyyy"));
            str.AppendFormat("<UngDung><![CDATA[{0}]]></UngDung>", UngDung);
            str.AppendFormat("<TrangThai><![CDATA[{0}]]></TrangThai>", TrangThai);

            str.Append("<MucDichThucTeSDD>");
            int i = 0;
            decimal tongdiennang = 0;
            decimal tongcongsuat = 0;
            foreach (var chitiet in MucDichThucTeSDD)
            {
                i++;
                str.Append("<ChiTietMucDich>");
                str.AppendFormat("<STT><![CDATA[{0}]]></STT>", i);
                str.AppendFormat("<TenThietBi><![CDATA[{0}]]></TenThietBi>", chitiet.TenThietBi);
                str.AppendFormat("<CongSuat><![CDATA[{0}]]></CongSuat>", chitiet.CongSuat);

                str.AppendFormat("<SoLuong><![CDATA[{0}]]></SoLuong>", chitiet.SoLuong);
                str.AppendFormat("<HeSoDongThoi><![CDATA[{0}]]></HeSoDongThoi>", chitiet.HeSoDongThoi);
                str.AppendFormat("<SoGio><![CDATA[{0}]]></SoGio>", chitiet.SoGio);
                str.AppendFormat("<SoNgay><![CDATA[{0}]]></SoNgay>", chitiet.SoNgay);
                str.AppendFormat("<TongCongSuatSuDung><![CDATA[{0}]]></TongCongSuatSuDung>", chitiet.TongCongSuatSuDung);
                str.AppendFormat("<DienNangSuDung><![CDATA[{0}]]></DienNangSuDung>", chitiet.DienNangSuDung);
                str.AppendFormat("<MucDichSDD><![CDATA[{0}]]></MucDichSDD>", chitiet.MucDichSDD);
                str.Append("</ChiTietMucDich>");
                tongcongsuat = tongcongsuat + chitiet.TongCongSuatSuDung;
                tongdiennang = tongdiennang + chitiet.DienNangSuDung;
            }
            str.Append("</MucDichThucTeSDD>");
            str.AppendFormat("<TongCS><![CDATA[{0}]]></TongCS>", Math.Round(tongcongsuat,2,MidpointRounding.AwayFromZero));
            str.AppendFormat("<TongDN><![CDATA[{0}]]></TongDN>", Math.Round(tongdiennang,2, MidpointRounding.AwayFromZero));
            str.Append("<GiaDienTheoMucDich>");

            foreach (var chitiet in GiaDienTheoMucDich)
            {

                str.Append("<ChiTietGiaDien>");
                str.AppendFormat("<SoCongTo><![CDATA[{0}]]></SoCongTo>", chitiet.SoCongTo);
                str.AppendFormat("<MaGhiChiSo><![CDATA[{0}]]></MaGhiChiSo>", chitiet.MaGhiChiSo);

                str.AppendFormat("<ApDungTuChiSo><![CDATA[{0}]]></ApDungTuChiSo>", chitiet.ApDungTuChiSo);
                str.AppendFormat("<MucDichSuDung><![CDATA[{0}]]></MucDichSuDung>", chitiet.MucDichSuDung);
                str.AppendFormat("<TyLe><![CDATA[{0}]]></TyLe>", chitiet.TyLe);
                str.AppendFormat("<GDKhongTheoTG><![CDATA[{0}]]></GDKhongTheoTG>", chitiet.GDKhongTheoTG);
                str.AppendFormat("<GDGioBT><![CDATA[{0}]]></GDGioBT>", chitiet.GDGioBT);
                str.AppendFormat("<GDGioCD><![CDATA[{0}]]></GDGioCD>", chitiet.GDGioCD);
                str.AppendFormat("<GDGioTD><![CDATA[{0}]]></GDGioTD>", chitiet.GDGioTD);
                str.Append("</ChiTietGiaDien>");
            }
            str.Append("</GiaDienTheoMucDich>");

            str.Append("</ThoaThuanTyLe>");
            return str.ToString();
        }
    }
}