using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class ResponseResult
    {
        public bool success { get; set; } = false;
        public string error { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public int total { get; set; }
    }

    public class BienBanResult
    {
        public bool success { get; set; } = false;
        public bool sign { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    public class ResponseFileResult
    {
        public bool success { get; set; } = false;
        public string message { get; set; }        
        public FileDataResult data { get; set; }
    }

    public class FileDataResult
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string Base64Data { get; set; }
    }
    public class Select2DataResult
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class PdfDataResult
    {
        public bool success { get; set; } = false;
        public string message { get; set; }
        public string data { get; set; }
    }
    public class ChartResult
    {

        public List<int> series { get; set; } = new List<int>();
        public List<string> labels { get; set; } = new List<string>();
    }
}
