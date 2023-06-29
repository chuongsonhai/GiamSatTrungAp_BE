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
    public class CongVanYeuCau
    {
        public virtual int ID { get; set; }
        public virtual string MaKHang { get; set; }
        public virtual string MaDDoDDien { get; set; }

        public virtual string SoCongVan { get; set; }        
        public virtual string MaYeuCau { get; set; }
        public virtual DateTime NgayYeuCau { get; set; } = DateTime.Now;
        public virtual string MaLoaiYeuCau { get; set; }

        public virtual string MaDViTNhan { get; set; }
        public virtual string MaDViQLy { get; set; }  
        public virtual string BenNhan { get; set; }

        public virtual string NguoiYeuCau { get; set; }
        public virtual string DChiNguoiYeuCau { get; set; }        
        public virtual string DuongPho { get; set; }
        public virtual string SoNha { get; set; }
        public virtual string DiaChinhID { get; set; } = "0";

        public virtual string TenKhachHang { get; set; }
        public virtual string CoQuanChuQuan { get; set; }
        public virtual string DiaChiCoQuan { get; set; }
        public virtual string MST { get; set; }
        public virtual string DienThoai { get; set; }
        public virtual string Email { get; set; }
        public virtual string Fax { get; set; }
        public virtual string SoTaiKhoan { get; set; }
        public virtual string SoPha { get; set; } = "1";
        public virtual bool DienSinhHoat { get; set; }

        public virtual DateTime? NgayHen { get; set; } = DateTime.Now;
        public virtual DateTime? NgayHenKhaoSat { get; set; } = DateTime.Now;

        public virtual int TinhTrang { get; set; }
        public virtual string MaHinhThuc { get; set; }
        
        public virtual string NoiDungYeuCau { get; set; }

        public virtual string DiaChiDungDien { get; set; }
        public virtual string DuAnDien { get; set; }

        public virtual int LoaiHinh { get; set; }
        public virtual TrangThaiCongVan TrangThai { get; set; } = TrangThaiCongVan.MoiTao;
        
        public virtual DateTime NgayLap { get; set; } = DateTime.Now;
        public virtual string NguoiLap { get; set; }
        public virtual string NguoiDuyet { get; set; }
        public virtual DateTime NgayDuyet { get; set; } = DateTime.Now;
        public virtual string Fkey { get; set; } = Guid.NewGuid().ToString();
        public virtual string Data { get; set; }
        public virtual string MaCViec { get; set; } = "YCM";

        public virtual string LyDoHuy { get; set; }
        public virtual string TenKhachHangUQ { get; set; }
        public virtual string SoDienThoaiKHUQ { get; set; }

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

            var temp = TemplateManagement.GetTemplate(DocumentCode.CV_DN);
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
            str.AppendFormat("<SoCongVan><![CDATA[{0}]]></SoCongVan>", SoCongVan);            
            str.AppendFormat("<VeViec><![CDATA[{0}]]></VeViec>", NoiDungYeuCau);
            str.AppendFormat("<ChuDauTu><![CDATA[{0}]]></ChuDauTu>", !string.IsNullOrWhiteSpace(CoQuanChuQuan) ? CoQuanChuQuan: TenKhachHang);
            str.AppendFormat("<NgayYeuCau>{0}</NgayYeuCau>", NgayYeuCau.ToString("dd/MM/yyyy"));
            str.AppendFormat("<BenNhan><![CDATA[{0}]]></BenNhan>", BenNhan);
            str.AppendFormat("<DuAnDien><![CDATA[{0}]]></DuAnDien>", DuAnDien);
            str.AppendFormat("<DiaChiDungDien><![CDATA[{0}]]></DiaChiDungDien>", DiaChiDungDien);
            str.AppendFormat("<NhuCau><![CDATA[{0}]]></NhuCau>", NoiDungYeuCau);
            str.Append("</CongVan>");
            return str.ToString();
        }
    }    
}