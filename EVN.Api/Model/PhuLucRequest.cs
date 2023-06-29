using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class PhuLucRequest
    {
        public string loaiHoSo { get; set; }
        public string xmlData { get; set; }
    }

    public class DocumentRequest
    {
        public string maYeuCau { get; set; }
        public IList<PhuLucDataRequest> Items { get; set; } = new List<PhuLucDataRequest>();
    }

    public class PhuLucDataRequest
    {
        public string maYeuCau { get; set; }
        public string loaiHoSo { get; set; }
        public string tenHoSo { get; set; }
        public string Base64Data { get; set; }
        public string loaiFile { get; set; } = "PDF"; //PDF, JPG, PNG, ZIP, RAR
    }
}