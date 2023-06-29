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
    public class ThoaThuanDamBao
    {
        public virtual int ID { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string MaDViQLy { get; set; }

        public virtual int CongVanID { get; set; }
        public virtual int Gio { get; set; }
        public virtual int Phut { get; set; }
        public virtual DateTime NgayLap { get; set; } = DateTime.Now;
        public virtual string DiaDiem { get; set; }
        
        public virtual string DonVi { get; set; }
        public virtual string MaSoThue { get; set; }
        public virtual string DaiDien { get; set; }
        public virtual string ChucVu { get; set; }
        public virtual string VanBanUQ { get; set; }
        public virtual DateTime NgayUQ { get; set; } = DateTime.Now;
        public virtual string NguoiKyUQ { get; set; }
        public virtual DateTime NgayKyUQ { get; set; } = DateTime.Now;
        public virtual string ChucVuUQ { get; set; }
        public virtual string DiaChi { get; set; }
        public virtual string DienThoai { get; set; }
        public virtual string Email { get; set; }
        public virtual string DienThoaiCSKH { get; set; }
        public virtual string SoTaiKhoan { get; set; }
        public virtual string NganHang { get; set; }

        public virtual string KHMa { get; set; }
        public virtual string KHTen { get; set; }
        public virtual string KHDaiDien { get; set; }
        public virtual string KHChucVu { get; set; }
        public virtual string KHMaSoThue { get; set; }
        public virtual string KHDangKyKD { get; set; }
        public virtual string KHDiaChi { get; set; }
        public virtual string KHDienThoai { get; set; }
        public virtual string KHEmail { get; set; }
        public virtual string KHSoGiayTo { get; set; }

        public virtual DateTime NgayCap { get; set; }=DateTime.Now;
        public virtual string NoiCap { get; set; }
        public virtual string KHSoTK { get; set; }
        public virtual string KHNganHang { get; set; }
        public virtual string KHVanBanUQ { get; set; }
        public virtual string KHNguoiUQ { get; set; }
        public virtual DateTime KHNgayUQ { get; set; } = DateTime.Now;

        public virtual decimal GiaTriTien { get; set; }
        public virtual string TienBangChu { get; set; }

        public virtual DateTime TuNgay { get; set; } = DateTime.Now;
        public virtual DateTime DenNgay { get; set; } = DateTime.Now;

        public virtual string HinhThuc { get; set; }
        public virtual string GhiChu { get; set; }
        public virtual int TrangThai { get; set; }

        public virtual string Data { get; set; }

        public virtual IList<ChiTietDamBao> GiaTriDamBao { get; set; } = new List<ChiTietDamBao>();
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
            var temp = TemplateManagement.GetTemplate(LoaiHSoCode.PL_HD_DB);
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
            StringBuilder str = new StringBuilder("<ThoaThuanDamBao>");
            str.AppendFormat("<Gio><![CDATA[{0}]]></Gio>", Gio);
            str.AppendFormat("<Phut><![CDATA[{0}]]></Phut>", Phut);
            str.AppendFormat("<NgayLap><![CDATA[{0}]]></NgayLap>", NgayLap.ToString("dd/MM/yyyy"));
            str.AppendFormat("<DiaDiem><![CDATA[{0}]]></DiaDiem>", DiaDiem);
            str.AppendFormat("<DonVi><![CDATA[{0}]]></DonVi>", DonVi);
            str.AppendFormat("<MaSoThue><![CDATA[{0}]]></MaSoThue>", MaSoThue);
            str.AppendFormat("<DaiDien><![CDATA[{0}]]></DaiDien>", DaiDien);
            str.AppendFormat("<ChucVu><![CDATA[{0}]]></ChucVu>", ChucVu);
            str.AppendFormat("<VanBanUQ><![CDATA[{0}]]></VanBanUQ>", VanBanUQ);
            str.AppendFormat("<NgayUQ><![CDATA[{0}]]></NgayUQ>", NgayUQ.ToString("dd/MM/yyyy"));
            str.AppendFormat("<NguoiKyUQ><![CDATA[{0}]]></NguoiKyUQ>", NguoiKyUQ);
            str.AppendFormat("<NgayKyUQ><![CDATA[{0}]]></NgayKyUQ>", NgayKyUQ.ToString("dd/MM/yyyy"));
            str.AppendFormat("<DiaChi><![CDATA[{0}]]></DiaChi>", DiaChi);
            str.AppendFormat("<DienThoai><![CDATA[{0}]]></DienThoai>", DienThoai);
            str.AppendFormat("<Email><![CDATA[{0}]]></Email>", Email);
            str.AppendFormat("<DienThoaiCSKH><![CDATA[{0}]]></DienThoaiCSKH>", DienThoaiCSKH);
            str.AppendFormat("<SoTaiKhoan><![CDATA[{0}]]></SoTaiKhoan>", SoTaiKhoan);
            str.AppendFormat("<NganHang><![CDATA[{0}]]></NganHang>", NganHang);

            str.AppendFormat("<KHMa><![CDATA[{0}]]></KHMa>", KHMa);
            str.AppendFormat("<KHTen><![CDATA[{0}]]></KHTen>", KHTen);

            str.AppendFormat("<KHMaSoThue><![CDATA[{0}]]></KHMaSoThue>", KHMaSoThue);
            str.AppendFormat("<KHDangKyKD><![CDATA[{0}]]></KHDangKyKD>", KHDangKyKD);
            str.AppendFormat("<KHSoGiayTo><![CDATA[{0}]]></KHSoGiayTo>", KHSoGiayTo);
            str.AppendFormat("<NgayCap><![CDATA[{0}]]></NgayCap>", NgayCap.ToString("dd/MM/yyyy"));
            str.AppendFormat("<NoiCap><![CDATA[{0}]]></NoiCap>", NoiCap);

            str.AppendFormat("<KHDaiDien><![CDATA[{0}]]></KHDaiDien>", KHDaiDien);
            str.AppendFormat("<KHChucVu><![CDATA[{0}]]></KHChucVu>", KHChucVu);
            str.AppendFormat("<KHDiaChi><![CDATA[{0}]]></KHDiaChi>", KHDiaChi);
            str.AppendFormat("<KHDienThoai><![CDATA[{0}]]></KHDienThoai>", KHDienThoai);
            str.AppendFormat("<KHEmail><![CDATA[{0}]]></KHEmail>", KHEmail);
            str.AppendFormat("<KHSoTK><![CDATA[{0}]]></KHSoTK>", KHSoTK);
            str.AppendFormat("<KHNganHang><![CDATA[{0}]]></KHNganHang>", KHNganHang);
            str.AppendFormat("<KHVanBanUQ><![CDATA[{0}]]></KHVanBanUQ>", KHVanBanUQ);
            str.AppendFormat("<KHNguoiUQ><![CDATA[{0}]]></KHNguoiUQ>", KHNguoiUQ);
            str.AppendFormat("<KHNgayUQ><![CDATA[{0}]]></KHNgayUQ>", KHNgayUQ.ToString("dd/MM/yyyy"));

            str.AppendFormat("<GiaTriTien><![CDATA[{0}]]></GiaTriTien>", GiaTriTien);
            str.AppendFormat("<TienBangChu><![CDATA[{0}]]></TienBangChu>", TienBangChu);
            str.AppendFormat("<TuNgay><![CDATA[{0}]]></TuNgay>", TuNgay.ToString("dd/MM/yyyy"));
            str.AppendFormat("<DenNgay><![CDATA[{0}]]></DenNgay>", DenNgay.ToString("dd/MM/yyyy"));
            str.AppendFormat("<HinhThuc><![CDATA[{0}]]></HinhThuc>", HinhThuc);
            str.AppendFormat("<GhiChu><![CDATA[{0}]]></GhiChu>", GhiChu);
            str.AppendFormat("<TrangThai><![CDATA[{0}]]></TrangThai>", TrangThai);
            str.Append("<GiaTriDamBao>");
            foreach (var chitiet in GiaTriDamBao)
            {
                str.Append("<ChiTietDamBao>");
                str.AppendFormat("<MucDich><![CDATA[{0}]]></MucDich>", chitiet.MucDich);
                str.AppendFormat("<SLTrungBinh><![CDATA[{0}]]></SLTrungBinh>", chitiet.SLTrungBinh);
                str.AppendFormat("<SoNgayDamBao><![CDATA[{0}]]></SoNgayDamBao>", chitiet.SoNgayDamBao);

                str.AppendFormat("<GiaBanDien><![CDATA[{0}]]></GiaBanDien>", chitiet.GiaBanDien);
                str.AppendFormat("<ThanhTien><![CDATA[{0}]]></ThanhTien>", chitiet.ThanhTien);

                str.Append("</ChiTietDamBao>");
            }
            str.Append("</GiaTriDamBao>");

            str.Append("</ThoaThuanDamBao>");
            return str.ToString();
        }
    }

}
