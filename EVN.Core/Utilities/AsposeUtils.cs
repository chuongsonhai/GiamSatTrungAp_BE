using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core
{
    public class AsposeUtils
    {
        ILog log = LogManager.GetLogger(typeof(AsposeUtils));

        public const string Key = "PExpY2Vuc2U+CjxEYXRhPgo8TGljZW5zZWRUbz5GbHVvciBGZWRlcmFsIFNlcnZpY2VzPC9MaWNlbnNlZFRvPgo8RW1haWxUbz50LmoudHVtbGluQGZsdW9yLmNvbTwvRW1haWxUbz4KPExpY2Vuc2VUeXBlPlNpdGUgU21hbGwgQnVzaW5lc3M8L0xpY2Vuc2VUeXBlPgo8TGljZW5zZU5vdGU+TGltaXRlZCB0byAxMCBwaHlzaWNhbCBsb2NhdGlvbnMsIG5vdCB0byBleGNlZWQgMTAgZGV2ZWxvcGVyczwvTGljZW5zZU5vdGU+CjxPcmRlcklEPjE5MDUwMTA5MDUxMzwvT3JkZXJJRD4KPFVzZXJJRD40MTI1Nzc8L1VzZXJJRD4KPE9FTT5UaGlzIGlzIG5vdCBhIHJlZGlzdHJpYnV0YWJsZSBsaWNlbnNlPC9PRU0+CjxQcm9kdWN0cz4KPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0Pgo8L1Byb2R1Y3RzPgo8RWRpdGlvblR5cGU+RW50ZXJwcmlzZTwvRWRpdGlvblR5cGU+CjxTZXJpYWxOdW1iZXI+NTdjYmRjYjUtZTY1Ny00ZWQxLWFlZDItYjI2MTNiZGQzNTE3PC9TZXJpYWxOdW1iZXI+CjxTdWJzY3JpcHRpb25FeHBpcnk+MjAyMTAxMDY8L1N1YnNjcmlwdGlvbkV4cGlyeT4KPExpY2Vuc2VWZXJzaW9uPjMuMDwvTGljZW5zZVZlcnNpb24+CjxMaWNlbnNlSW5zdHJ1Y3Rpb25zPmh0dHBzOi8vcHVyY2hhc2UuYXNwb3NlLmNvbS9wb2xpY2llcy91c2UtbGljZW5zZTwvTGljZW5zZUluc3RydWN0aW9ucz4KPC9EYXRhPgo8U2lnbmF0dXJlPkdOY3pidW9Ld0VFS0NRSmFRbFR1Z0Z0MzBwQndnQUVmUEFmSUNaNnY2K0NFK0FCZ202Y2JsUDhJL0tNcUppV0ZBTVRmMS9qUlhSNjFTcUtSRnBwRmwrVzFydm5kMjZZWDlmUWtJM2IvNFZxMkpIRHIxNWNaYkZOeEhtUkFBalc2Vy9iR1JjZlZCc25JRzhYc0Q3eXJwODE0Nkc4emJ5WDJCSjA1SlRDVDJZYz08L1NpZ25hdHVyZT4KPC9MaWNlbnNlPg==";
        public Stream LStreamWord = new MemoryStream(Convert.FromBase64String(Key));
        public bool Setted = false;
        private void SetAsposeLisence()
        {
            if (!Setted)
            {
                new Aspose.Words.License().SetLicense(LStreamWord);
                Setted = true;
            }
        }

        public string ConvertWordToPDF(string subfolder, string pathWord)
        {
            try
            {
                string physicalSiteDataDirectory = ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"];
                string subPath = string.Format("{0}\\{1}", subfolder, DateTime.Today.Year);
                if (!Directory.Exists(physicalSiteDataDirectory + @"\" + subPath))
                    Directory.CreateDirectory(physicalSiteDataDirectory + @"\" + subPath);
                SetAsposeLisence();
                string filepath = $"{subPath}\\{Guid.NewGuid().ToString("N")}.PDF";

                Aspose.Words.Document doc = new Aspose.Words.Document($"{physicalSiteDataDirectory}\\{pathWord}");
                doc.Save($"{physicalSiteDataDirectory}\\{filepath}");
                return filepath;
            }
            catch (Exception ex)
            {
                log.Error("AsposeUtils:" + ex);
                return String.Empty;
            }
        }
    }
}
