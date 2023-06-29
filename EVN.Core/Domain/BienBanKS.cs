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
    public class BienBanKS
    {
        public virtual int ID { get; set; }
        public virtual string MaDViQLy { get; set; }
        public virtual string MaDViTNhan { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string SoCongVan { get; set; }
        public virtual DateTime NgayCongVan { get; set; } = DateTime.Now;

        public virtual string MaKH { get; set; }
        public virtual string SoBienBan { get; set; }
        public virtual string TenCongTrinh { get; set; }
        public virtual string DiaDiemXayDung { get; set; }
        public virtual string KHTen { get; set; }
        public virtual string KHDienThoai { get; set; }
        public virtual string KHDaiDien { get; set; }
        public virtual string KHChucDanh { get; set; }
        public virtual string EVNDonVi { get; set; }
        public virtual string EVNDaiDien { get; set; }
        public virtual string EVNChucDanh { get; set; }

        public virtual DateTime NgayDuocGiao { get; set; } = DateTime.Now;

        public virtual DateTime NgayKhaoSat { get; set; } = DateTime.Now;
        public virtual string CapDienAp { get; set; }
        public virtual string TenLoDuongDay { get; set; }
        public virtual string DiemDauDuKien { get; set; }
        public virtual string DayDan { get; set; }
        public virtual int SoTramBienAp { get; set; } = 1;
        public virtual int SoMayBienAp { get; set; } = 1;
        public virtual decimal TongCongSuat { get; set; } = 1000;
        public virtual string ThoaThuanKyThuat { get; set; }
        public virtual DateTime NgayLap { get; set; } = DateTime.Now;
        public virtual string NguoiLap { get; set; }
        public virtual int TrangThai { get; set; } = 0;//0: Mới tạo, 1: Gửi KH ký, 2: Đã ký KH, 3: Đồng bộ lên CMIS
        public virtual string Data { get; set; }
        public virtual string MaCViec { get; set; }

        public virtual bool ThuanLoi { get; set; } = true;
        public virtual string MaTroNgai { get; set; }
        public virtual string TroNgai { get; set; }

        public virtual IList<ThanhPhanKS> ThanhPhans { get; set; } = new List<ThanhPhanKS>();

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

            var temp = TemplateManagement.GetTemplate(DocumentCode.BB_KS);
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
            StringBuilder str = new StringBuilder("<BienBanKS>");
            str.AppendFormat("<EVNDonVi><![CDATA[{0}]]></EVNDonVi>", EVNDonVi);
            str.AppendFormat("<NgayLap><![CDATA[{0}]]></NgayLap>", NgayLap.ToString("dd/MM/yyyy"));
            str.AppendFormat("<SoBienBan><![CDATA[{0}]]></SoBienBan>", SoBienBan);
            str.AppendFormat("<ChuDauTu><![CDATA[{0}]]></ChuDauTu>", KHTen);
            str.AppendFormat("<SoCongVan><![CDATA[{0}]]></SoCongVan>", SoCongVan);
            str.AppendFormat("<NgayCongVan>{0}</NgayCongVan>", NgayCongVan.ToString("dd/MM/yyyy"));
            str.AppendFormat("<TenCongTrinh><![CDATA[{0}]]></TenCongTrinh>", TenCongTrinh);
            str.AppendFormat("<DiaDiemXayDung><![CDATA[{0}]]></DiaDiemXayDung>", DiaDiemXayDung);
            str.AppendFormat("<CDTDaiDien><![CDATA[{0}]]></CDTDaiDien>", KHDaiDien);
            str.AppendFormat("<CDTChucDanh><![CDATA[{0}]]></CDTChucDanh>", KHChucDanh);
            str.AppendFormat("<EVNDaiDien><![CDATA[{0}]]></EVNDaiDien>", EVNDaiDien);
            str.AppendFormat("<EVNChucDanh><![CDATA[{0}]]></EVNChucDanh>", EVNChucDanh);
            str.AppendFormat("<TroNgai><![CDATA[{0}]]></TroNgai>", ThuanLoi ? "Thuận lợi" : TroNgai);
            str.AppendFormat("<NgayKhaoSat><![CDATA[{0}]]></NgayKhaoSat>", NgayKhaoSat.ToString("dd/MM/yyyy"));
            str.AppendFormat("<CapDienApDauNoi><![CDATA[{0}]]></CapDienApDauNoi>", CapDienAp);
            str.AppendFormat("<TenLoDuongDay><![CDATA[{0}]]></TenLoDuongDay>", TenLoDuongDay);
            str.AppendFormat("<DiemDauDuKien><![CDATA[{0}]]></DiemDauDuKien>", DiemDauDuKien);
            str.AppendFormat("<DayDan><![CDATA[{0}]]></DayDan>", DayDan);
            str.AppendFormat("<SoTramBienAp><![CDATA[{0}]]></SoTramBienAp>", SoTramBienAp);
            str.AppendFormat("<SoMayBienAp><![CDATA[{0}]]></SoMayBienAp>", SoMayBienAp);
            str.AppendFormat("<TongCongSuat><![CDATA[{0}]]></TongCongSuat>", TongCongSuat);
            str.AppendFormat("<ThoaThuanKyThuat><![CDATA[{0}]]></ThoaThuanKyThuat>", ThoaThuanKyThuat);

            str.Append("<ThanhPhans>");
            var dddonvi = ThanhPhans.Where(p => p.Loai == 0).FirstOrDefault();
            str.Append("<DonVi>");
            var listtpdv = dddonvi != null ? dddonvi.DaiDiens : new List<ThanhPhanDaiDien>();
            foreach (var tphan in listtpdv)
            {
                str.AppendFormat("<DaiDien><![CDATA[{0}]]></DaiDien>", tphan.DaiDien);
                str.AppendFormat("<ChucVu><![CDATA[{0}]]></ChucVu>", tphan.ChucVu);
            }
            str.Append("</DonVi>");

            var ddchudt = ThanhPhans.Where(p => p.Loai == 1).FirstOrDefault();
            str.Append("<ChuDauTu>");
            var listtpcdt = ddchudt != null ? ddchudt.DaiDiens : new List<ThanhPhanDaiDien>();
            foreach (var tphan in listtpcdt)
            {
                str.AppendFormat("<DaiDien><![CDATA[{0}]]></DaiDien>", tphan.DaiDien);
                str.AppendFormat("<ChucVu><![CDATA[{0}]]></ChucVu>", tphan.ChucVu);
            }
            str.Append("</ChuDauTu>");
            str.Append("</ThanhPhans>");

            str.Append("</BienBanKS>");
            return str.ToString();
        }
    }
}
