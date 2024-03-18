using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class UpdateStatusModel
    {
        public virtual int ID { get; set; }
        public virtual int NGUYENHHAN_CANHBAO { get; set; }
        public virtual string KETQUA_GIAMSAT { get; set; }
    
    }

}
