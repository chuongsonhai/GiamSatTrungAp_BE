using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Models
{
    public class Thoigiancapdien
    {
        public Thoigiancapdien(IList<ThoiGianCapDienModel> baoCaoTHTCDNs)
        {
            ListItem = baoCaoTHTCDNs;
            TongSoCTTiepNhanTTDN = baoCaoTHTCDNs.Sum(x => x.TongSoCTTiepNhanTTDN);

            var countTongSoNgayTB = 0;


            foreach (var item in baoCaoTHTCDNs)
            {


                if (item.TongSoNgayTB > 0)
                {
                    countTongSoNgayTB = countTongSoNgayTB + 1;
                }

            }



            if (countTongSoNgayTB > 0)
            {
                TongSoNgayTB = Math.Round(baoCaoTHTCDNs.Sum(x => x.TongSoNgayTB) / countTongSoNgayTB, 1, MidpointRounding.AwayFromZero);
            }

        }
        public IList<ThoiGianCapDienModel> ListItem { get; set; } = new List<ThoiGianCapDienModel>();
        public int TongSoCTTiepNhanTTDN { get; set; }


        public decimal TongSoNgayTB { get; set; } = 0;

    }
}
