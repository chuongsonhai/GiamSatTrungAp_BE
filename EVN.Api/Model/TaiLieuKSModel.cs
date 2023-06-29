using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class TaiLieuKSModel
    {
        public int ID { get; set; }
        public int CongVanID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string Createby { get; set; }
        public int Status { get; set; }

        public TaiLieuKS ToEntity()
        {
            TaiLieuKS taiLieuKS = new TaiLieuKS();
            taiLieuKS.CongVanID = CongVanID;
            taiLieuKS.Name = Name;
            taiLieuKS.Description = Description;
            return taiLieuKS;
        }
    }
}