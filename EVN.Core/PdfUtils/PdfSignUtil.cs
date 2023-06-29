using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace EVN.Core
{
    public class RequestData
    {
        public string HoTenNguoiKy { get; set; }
        public string MaDonVi { get; set; }
        public string ChucVuNguoiKy { get; set; }
        public string Base64File { get; set; }
    }

    public class PdfSignUtil
    {
        static ILog log = LogManager.GetLogger(typeof(PdfSignUtil));

        public static SignResponseData SignPdf(string signName, string maDonVi, byte[] pdfdata, string signPosition = "Giám đốc")
        {
            RequestData data = new RequestData();
            data.HoTenNguoiKy = signName.Trim();
            data.MaDonVi = maDonVi.Trim();
            data.ChucVuNguoiKy = signPosition;
            data.Base64File = Convert.ToBase64String(pdfdata);

            var jsondata = JsonConvert.SerializeObject(data);
            string message = "";
            var response = ApiHelper.SignApi(jsondata, out message);
            return response;
        }

        public static int NamesSigned(byte[] pdfdata)
        {
            PdfReader reader = new PdfReader(pdfdata);
            try
            {
                AcroFields fields = reader.AcroFields;
                return fields.GetSignatureNames().Count;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return 0;
            }
            finally
            {
                reader.Close();
            }
        }

        public static int CustomerSigned(byte[] pdfdata)
        {
            int total = 0;
            PdfReader pdfReader = new PdfReader(pdfdata);
            try
            {

                for (int page = pdfReader.NumberOfPages; page >= 1; page--)
                {
                    PdfDictionary pageDict = pdfReader.GetPageN(page);

                    PdfArray annotArray = pageDict.GetAsArray(PdfName.ANNOTS);
                    if (annotArray == null || annotArray.ArrayList.Count() == 0) continue;
                    total++;
                }
                return total;
            }
            finally
            {
                pdfReader.Close();
            }
        }

        public static int HasSigned(byte[] pdfdata, string searchText)
        {
            log.Error(searchText);
            PdfReader pdfReader = new PdfReader(pdfdata);
            try
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                string currentPageText = PdfTextExtractor.GetTextFromPage(pdfReader, pdfReader.NumberOfPages, strategy);
                string[] arrayStr = currentPageText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrayStr.Contains(searchText) && arrayStr.Contains("Đã Ký"))
                {
                    return 1;
                }
                return 0;
            }
            finally
            {
                pdfReader.Close();
            }
        }
    }
}
