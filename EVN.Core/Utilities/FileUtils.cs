using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EVN.Core.Utilities
{
    public static class FileUtils
    {
        static string physicalSiteDataDirectory = ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"];
        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        public static byte[] StreamToByteArray(Stream input)
        {
            MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }

        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        public static bool CheckIfFileImage(this HttpPostedFile file)
        {
            var extension = (file.FileName.Split('.')[file.FileName.Split('.').Length - 1]).ToLower();
            List<string> lst = new List<string>() { "jpg", "jpeg", "jpe", "jfif", "bmp", "dib", "rle", "gif", "tif", "tiff", "png" };
            return lst.Any(c => c.ToLower().Contains(extension)); // Change the extension based on your need
        }
        public static bool CheckIfFileExcel(this HttpPostedFile file)
        {
            var extension = (file.FileName.Split('.')[file.FileName.Split('.').Length - 1]).ToLower();
            List<string> lst = new List<string>() { "xls", "xlsx" };
            return lst.Any(c => c.ToLower().Contains(extension)); // Change the extension based on your need
        }

        public static bool CheckIfFilePDF(this HttpPostedFile file)
        {
            var extension = (file.FileName.Split('.')[file.FileName.Split('.').Length - 1]).ToLower();
            List<string> lst = new List<string>() { "pdf" };
            return lst.Any(c => c.ToLower().Contains(extension)); // Change the extension based on your need
        }

        public static async Task<string> SaveFileAsync(HttpPostedFile file, string fileFolder, string fileName)
        {
            try
            {
                if (file.ContentLength <= 0 || !CheckIfFileImage(file)) return null;
                var fullFolder = physicalSiteDataDirectory + @"/" + fileFolder;
                if (!Directory.Exists(fullFolder)) Directory.CreateDirectory(fullFolder);
                var imagePath = $"{fullFolder}//{fileName}";
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await file.InputStream.CopyToAsync(stream);
                }
                return imagePath;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string  SaveFilePDFAsync(HttpPostedFile file, string fileFolder, string fileName)
        {
            try
            {
                if (file.ContentLength <= 0 || !CheckIfFilePDF(file)) return null;
                var fullFolder = physicalSiteDataDirectory + @"/" + fileFolder;
                if (!Directory.Exists(fullFolder)) Directory.CreateDirectory(fullFolder);
                var imagePath = $"{fullFolder}/{fileName}";
                file.SaveAs(imagePath);
               
                return imagePath;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string SaveFile(HttpPostedFile file, string fileFolder, string fileName)
        {
            try
            {
                if (file.ContentLength <= 0) return null;
                var fullFolder = physicalSiteDataDirectory + @"/" + fileFolder;
                if (!Directory.Exists(fullFolder)) Directory.CreateDirectory(fullFolder);
                var imagePath = $"{fullFolder}/{fileName}";
                file.SaveAs(imagePath);

                return imagePath;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
