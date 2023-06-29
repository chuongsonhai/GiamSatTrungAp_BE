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
    public class YCauNghiemThu
    {
        public virtual int ID { get; set; }
        public virtual string MaKHang { get; set; }
        public virtual string MaDDoDDien { get; set; }

        public virtual string SoCongVan { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual DateTime NgayYeuCau { get; set; } = DateTime.Now;
        public virtual string MaLoaiYeuCau { get; set; }

        public virtual string MaDViQLy { get; set; }
        public virtual string BenNhan { get; set; }

        public virtual string NguoiYeuCau { get; set; }
        public virtual string DiaChi { get; set; }

        public virtual string CoQuanChuQuan { get; set; }
        public virtual string DiaChiCoQuan { get; set; }
        public virtual string MaSoThue { get; set; }
        public virtual string DienThoai { get; set; }
        public virtual string Email { get; set; }

        public virtual string SoThoaThuanDN { get; set; }
        public virtual DateTime NgayThoaThuan { get; set; }

        public virtual DateTime? NgayHen { get; set; } = DateTime.Now;
        public virtual DateTime? NgayHenKhaoSat { get; set; } = DateTime.Now;

        public virtual string NoiDungYeuCau { get; set; }

        public virtual string DiaChiDungDien { get; set; }
        public virtual string DuAnDien { get; set; }

        public virtual TrangThaiNghiemThu TrangThai { get; set; } = TrangThaiNghiemThu.MoiTao;

        public virtual DateTime NgayLap { get; set; } = DateTime.Now;
        public virtual string NguoiLap { get; set; }
        public virtual string NguoiDuyet { get; set; }
        public virtual DateTime NgayDuyet { get; set; } = DateTime.Now;
        public virtual string Fkey { get; set; } = Guid.NewGuid().ToString();
        public virtual string Data { get; set; }
        public virtual string MaCViec { get; set; } = "DDN";
        public virtual string MaTram { get; set; }

        public virtual string LyDoHuy { get; set; }

        public virtual bool DienSinhHoat { get; set; } = false;        

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

            var temp = TemplateManagement.GetTemplate(DocumentCode.CV_NT);
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
            StringBuilder str = new StringBuilder("<CongVan>");
            str.AppendFormat("<ChuDauTu><![CDATA[{0}]]></ChuDauTu>", CoQuanChuQuan);
            str.AppendFormat("<SoCongVan><![CDATA[{0}]]></SoCongVan>", SoCongVan);

            str.AppendFormat("<SoThoaThuan><![CDATA[{0}]]></SoThoaThuan>", SoThoaThuanDN);
            str.AppendFormat("<NgayThoaThuan>{0}</NgayThoaThuan>", NgayThoaThuan.ToString("dd/MM/yyyy"));
            str.AppendFormat("<BenNhan><![CDATA[{0}]]></BenNhan>", BenNhan);
            
            str.AppendFormat("<NgayYeuCau>{0}</NgayYeuCau>", NgayYeuCau.ToString("dd/MM/yyyy"));
            
            str.AppendFormat("<DuAnDien><![CDATA[{0}]]></DuAnDien>", DuAnDien);
            str.AppendFormat("<DiaChiDungDien><![CDATA[{0}]]></DiaChiDungDien>", DiaChiDungDien);
            str.AppendFormat("<NhuCau><![CDATA[{0}]]></NhuCau>", NoiDungYeuCau);
            str.Append("</CongVan>");
            return str.ToString();
        }
    }
}