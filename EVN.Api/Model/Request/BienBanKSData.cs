using EVN.Api.Model.Response;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class BienBanKSData
    {
        public bool LapBienBan { get; set; } = false;
        public KetQuaKSModel KetQuaKS { get; set; }
        public BienBanKSModel BienBanKS { get; set; }        
    }

    public class BienBanKTData
    {
        public KetQuaKTModel KetQuaKT { get; set; }
        public BienBanKTModel BienBanKT { get; set; }
    }

    public class BienBanDauNoiData
    {
        public YeuCauDauNoiData CongVanYeuCau { get; set; }
        public BienBanKSModel BienBanKS { get; set; }
        public BienBanDNModel BienBanDN { get; set; }    
    }

    public class BienBanDNData
    {
        public YeuCauDauNoiData CongVanYeuCau { get; set; }
        public BienBanDNModel BienBanDN { get; set; }
    }

    public class BienBanTTData
    {
        public bool LapBienBan { get; set; } = false;
        public KetQuaTCModel KetQuaTC { get; set; }
        public BienBanTTModel BienBanTT { get; set; }
    }
}