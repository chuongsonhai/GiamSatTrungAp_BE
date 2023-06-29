using EVN.Core.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace EVN.Core.Repository
{
    public class FileStoreRepository : IRepository
    {
        static ILog log = LogManager.GetLogger(typeof(FileStoreRepository));
        private string physicalSiteDataDirectory;
        public FileStoreRepository()
        {
            physicalSiteDataDirectory = ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"];
        }

        public byte[] GetData(string data)
        {
            string fullPath = physicalSiteDataDirectory + @"\" + data;
            if (!File.Exists(fullPath))
                return null;
            return File.ReadAllBytes(fullPath);
        }

        public string Store(string subfolder, byte[] data, string currentpath = "", string loaiFile = "PDF")
        {
            try
            {
                string subPath = string.Format("{0}\\{1}", subfolder, DateTime.Today.Year);
                if (!Directory.Exists(physicalSiteDataDirectory + @"\" + subPath))
                    Directory.CreateDirectory(physicalSiteDataDirectory + @"\" + subPath);
                string filepath = $"{subPath}\\{Guid.NewGuid().ToString("N")}.{loaiFile}";
                using (FileStream fs = new FileStream($"{physicalSiteDataDirectory}\\{filepath}", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    int length = Convert.ToInt32(data.Length);
                    fs.Write(data, 0, length);
                }
                if (!string.IsNullOrWhiteSpace(currentpath))
                    DeleteFile(physicalSiteDataDirectory + @"\" + currentpath);
                return filepath;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
      
        public string CombineToPdf(string subfolder, List<string> filePath)
        {

            try
            {

                string error = "";
                string subPath = string.Format("{0}\\{1}", subfolder, DateTime.Today.Year);
                if (!Directory.Exists(physicalSiteDataDirectory + @"\" + subPath))
                    Directory.CreateDirectory(physicalSiteDataDirectory + @"\" + subPath);
                string targetPath = $"{subPath}\\{Guid.NewGuid().ToString("N")}.PDF";
                string fullPath = $"{ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"]}/{targetPath}";
                PdfHelper.Instance.CombineImageToPdf(filePath, fullPath, out error);

                if (string.IsNullOrEmpty(error))
                {
                    return targetPath;
                }
                else
                {
                    log.Error(error);
                    return null;
                }
            }
            catch (Exception ex)
            {

                log.Error(ex);
                throw ex;
            }
        }

        void DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }            
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
