using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class ThanhPhanKS
    {
        public virtual int ID { get; set; }
        
        public virtual string DonVi { get; set; }
        public virtual string ThanhPhan { get; set; }
        public virtual int Loai { get; set; } = 0; //0: Công ty điện, 1: Chủ đầu tư
        public virtual int BienBanID { get; set; }

        public virtual IList<ThanhPhanDaiDien> DaiDiens
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ThanhPhan)) return new List<ThanhPhanDaiDien>();
                return JsonConvert.DeserializeObject<IList<ThanhPhanDaiDien>>(ThanhPhan);
            }
        }
    }    
}
