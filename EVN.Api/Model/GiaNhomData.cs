using EVN.Core.CMIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class GiaNhomData
    {
        public IList<Select2DataResult> ListKT { get; set; } = new List<Select2DataResult>();
        public IList<Select2DataResult> ListBT { get; set; } = new List<Select2DataResult>();
        public IList<Select2DataResult> ListCD { get; set; } = new List<Select2DataResult>();
        public IList<Select2DataResult> ListTD { get; set; } = new List<Select2DataResult>();
    }
}