using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class TramBienAp
    {
        public virtual int ID { get; set; }
        public virtual int BienBanID { get; set; }
        public virtual string TenTram { get; set; }
        public virtual string KieuTram { get; set; }
        public virtual string ViTriXayLap { get; set; }
        public virtual string MayBienAp { get; set; }
        public virtual string CongSuat { get; set; }
        public virtual string CapDienAp { get; set; }
        public virtual string ToDauDayCap { get; set; }
        public virtual string TuTrungThe { get; set; }
        public virtual string ThietBiHaThe { get; set; }
    }
}
