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
    public class ChamDutHopDong
    {
        public virtual int ID { get; set; }
        public virtual int CongVanID { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string CanCu { get; set; }
        public virtual DateTime NgayLap { get; set; } = DateTime.Now;
        public virtual string DiaDiem { get; set; }
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
        public virtual DateTime NgayChamDut { get; set; } = DateTime.Now;
        public virtual string SoHopDong { get; set; }
        public virtual DateTime NgayKyhopDong { get; set; } = DateTime.Now;
        public virtual string UngDung { get; set; }
        public virtual int HopDongID { get; set; }
        public virtual int TrangThai { get; set; }
        public virtual string MaDViQLy { get; set; }
        public virtual string Data { get; set; }
        public virtual string DiaChiGiaoDich { get; set; }
        public virtual string KHTen { get; set; }
        public virtual string KHMa { get; set; }
        public virtual IList<HeThongDDChamDut> HeThongDDChamDut { get; set; } = new List<HeThongDDChamDut>();
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

            var temp = TemplateManagement.GetTemplate(LoaiHSoCode.PL_HD_CD);
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
            StringBuilder str = new StringBuilder("<ChamDutHopDong>");
            str.AppendFormat("<NgayLap><![CDATA[{0}]]></NgayLap>", NgayLap.ToString("dd/MM/yyyy"));
            str.AppendFormat("<CanCu><![CDATA[{0}]]></CanCu>", CanCu);
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
            str.AppendFormat("<NgayChamDut><![CDATA[{0}]]></NgayChamDut>", NgayChamDut.ToString("dd/MM/yyyy"));
            str.AppendFormat("<SoHopDong><![CDATA[{0}]]></SoHopDong>", SoHopDong);
            str.AppendFormat("<NgayKyhopDong><![CDATA[{0}]]></NgayKyhopDong>", NgayKyhopDong.ToString("dd/MM/yyyy"));

            str.AppendFormat("<UngDung><![CDATA[{0}]]></UngDung>", UngDung);
            str.AppendFormat("<TrangThai><![CDATA[{0}]]></TrangThai>", TrangThai);

            str.Append("<HeThongDDChamDut>");

            foreach (var chitiet in HeThongDDChamDut)
            {

                str.Append("<ChiTietDDem>");
                str.AppendFormat("<DiemDo><![CDATA[{0}]]></DiemDo>", chitiet.DiemDo);
                str.AppendFormat("<SoCongTo><![CDATA[{0}]]></SoCongTo>", chitiet.SoCongTo);
                str.AppendFormat("<Loai><![CDATA[{0}]]></Loai>", chitiet.Loai);

                str.AppendFormat("<TI><![CDATA[{0}]]></TI>", chitiet.TI);
                str.AppendFormat("<TU><![CDATA[{0}]]></TU>", chitiet.TU);
                str.AppendFormat("<HeSoNhan><![CDATA[{0}]]></HeSoNhan>", chitiet.HeSoNhan);
                str.AppendFormat("<ChiSoChot><![CDATA[{0}]]></ChiSoChot>", chitiet.ChiSoChot);


                str.Append("</ChiTietDDem>");
            }
            str.Append("</HeThongDDChamDut>");


            str.Append("</ChamDutHopDong>");
            return str.ToString();
        }
    }
}