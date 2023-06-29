using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class Template
    {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string XmlData { get; set; }
        public virtual string XsltData { get; set; }
        public virtual string ChucVuKy { get; set; }
        public virtual int TrangThai { get; set; } = 0; //0: Sử dụng, 1: Khóa
    }
}