using EVN.Core.IServices;
using EVN.Core.Repository;
using Newtonsoft.Json;
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
    public class BienBanKT
    {
        public virtual int ID { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string MaDViQLy { get; set; }

        public virtual int ThoaThuanID { get; set; }
        public virtual string SoThoaThuan { get; set; }
        public virtual DateTime NgayThoaThuan { get; set; }
        public virtual string ThoaThuanDauNoi { get; set; }

        public virtual string SoBienBan { get; set; }
        public virtual DateTime NgayLap { get; set; } = DateTime.Now;
        public virtual string DonVi { get; set; }
        public virtual string MaSoThue { get; set; }
        public virtual string DaiDien { get; set; }
        public virtual string ChucVu { get; set; }

        public virtual string KHMa { get; set; }
        public virtual string KHTen { get; set; }
        public virtual string KHMaSoThue { get; set; }
        public virtual string KHDaiDien { get; set; }
        public virtual string KHChucVu { get; set; }
        public virtual string KHDiaChi { get; set; }
        public virtual string KHDienThoai { get; set; }
        public virtual string KHEmail { get; set; }

        public virtual string TenCongTrinh { get; set; }
        public virtual string DiaDiemXayDung { get; set; }

        public virtual string QuyMo { get; set; }
        public virtual string HoSoKemTheo { get; set; }
        public virtual string KetQuaKiemTra { get; set; }
        public virtual string TonTai { get; set; }
        public virtual string KienNghi { get; set; }
        public virtual string YKienKhac { get; set; }
        public virtual string KetLuan { get; set; }
        public virtual string ThoiHanDongDien { get; set; }
        public virtual int TrangThai { get; set; } = 0; //0: Mới tạo, 1: Gửi KH ký, 2: Đã ký KH, 3: Đồng bộ lên CMIS
        public virtual string MaCViec { get; set; }
        public virtual string Data { get; set; }

        public virtual string NguoiLap { get; set; }
        public virtual bool ThuanLoi { get; set; } = true;
        public virtual string MaTroNgai { get; set; }
        public virtual string TroNgai { get; set; }
        public virtual string CongSuat { get; set; }
        public virtual IList<string> ListCongSuat
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CongSuat)) return new List<string>();
                return JsonConvert.DeserializeObject<IList<string>>(CongSuat);
            }
        }

        public virtual IList<ThanhPhanKT> ThanhPhans { get; set; } = new List<ThanhPhanKT>();

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

            var temp = TemplateManagement.GetTemplate(DocumentCode.BB_KT);
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
            StringBuilder str = new StringBuilder("<BienBan>");
            str.AppendFormat("<SoThoaThuan><![CDATA[{0}]]></SoThoaThuan>", SoThoaThuan);
            str.AppendFormat("<NgayThoaThuan><![CDATA[{0}]]></NgayThoaThuan>", NgayThoaThuan.ToString("dd/MM/yyyy"));
            str.AppendFormat("<ThoaThuanDauNoi><![CDATA[{0}]]></ThoaThuanDauNoi>", ThoaThuanDauNoi);

            str.AppendFormat("<SoBienBan><![CDATA[{0}]]></SoBienBan>", SoBienBan);
            str.AppendFormat("<NgayLap>{0}</NgayLap>", NgayLap.ToString("dd/MM/yyyy"));

            str.AppendFormat("<DonVi><![CDATA[{0}]]></DonVi>", DonVi);
            str.AppendFormat("<MaSoThue><![CDATA[{0}]]></MaSoThue>", MaSoThue);

            str.Append("<ThanhPhans>");
            var dddonvi = ThanhPhans.Where(p => p.Loai == 0).FirstOrDefault();
            str.Append("<DonVi>");
            var listtpdv = dddonvi != null? dddonvi.DaiDiens : new List<ThanhPhanDaiDien>();
            foreach(var tphan in listtpdv)
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
            if (ListCongSuat.Count == 0)
            {
                str.AppendFormat("<ShowCS><![CDATA[{0}]]></ShowCS>", 0);
            }
            else
            {
                str.AppendFormat("<ShowCS><![CDATA[{0}]]></ShowCS>", 1);
            }

            str.Append("<DSCongSuat>");
            foreach(var cs in ListCongSuat)
            {
                str.Append("<CongSuat>");
                str.AppendFormat("<SoCongSuat><![CDATA[{0}]]></SoCongSuat>", cs);
                str.Append("</CongSuat>");
               
            }
            str.Append("</DSCongSuat>");

            str.AppendFormat("<KHTen><![CDATA[{0}]]></KHTen>", KHTen);
            str.AppendFormat("<KHDaiDien><![CDATA[{0}]]></KHDaiDien>", KHDaiDien);
            str.AppendFormat("<KHChucVu><![CDATA[{0}]]></KHChucVu>", KHChucVu);
            str.AppendFormat("<KHDiaChi><![CDATA[{0}]]></KHDiaChi>", KHDiaChi);
            str.AppendFormat("<KHDienThoai><![CDATA[{0}]]></KHDienThoai>", KHDienThoai);
            str.AppendFormat("<KHMaSoThue><![CDATA[{0}]]></KHMaSoThue>", KHMaSoThue);

            str.AppendFormat("<TenCongTrinh><![CDATA[{0}]]></TenCongTrinh>", TenCongTrinh);
            str.AppendFormat("<DiaDiemXayDung><![CDATA[{0}]]></DiaDiemXayDung>", DiaDiemXayDung);
            str.AppendFormat("<QuyMo><![CDATA[{0}]]></QuyMo>", QuyMo);

            //str.AppendFormat("<HoSoKemTheo><![CDATA[{0}]]></HoSoKemTheo>", HoSoKemTheo);
            str.AppendFormat("<KetQuaKiemTra><![CDATA[{0}]]></KetQuaKiemTra>", KetQuaKiemTra);
            str.AppendFormat("<TonTai><![CDATA[{0}]]></TonTai>", TonTai);

            str.AppendFormat("<KienNghi><![CDATA[{0}]]></KienNghi>", KienNghi);
            str.AppendFormat("<YKienKhac><![CDATA[{0}]]></YKienKhac>", YKienKhac);
            str.AppendFormat("<KetLuan><![CDATA[{0}]]></KetLuan>", KetLuan);
            str.AppendFormat("<ThoiHanDongDien><![CDATA[{0}]]></ThoiHanDongDien>", ThoiHanDongDien);
            str.Append("</BienBan>");
            return str.ToString();
        }
    }
}
