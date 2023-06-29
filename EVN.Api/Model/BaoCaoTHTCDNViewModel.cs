using EVN.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class BaoCaoTHTCDNViewModel
    {
        public BaoCaoTHTCDNViewModel(IList<BaoCaoTHTCDN> baoCaoTHTCDNs)
        {
            ListItem = baoCaoTHTCDNs;
            TongSoCTTiepNhanTTDN = baoCaoTHTCDNs.Sum(x => x.TongSoCTTiepNhanTTDN);
            TongSoCTCoTroNgaiTTDN = baoCaoTHTCDNs.Sum(x => x.TongSoCTCoTroNgaiTTDN);
            TongSoCTDaHoanThanhTTDN = baoCaoTHTCDNs.Sum(x => x.TongSoCTDaHoanThanhTTDN);
            TongSoCTChuaHoanThanhTTDN = baoCaoTHTCDNs.Sum(x => x.TongSoCTChuaHoanThanhTTDN);
            TongSoCTQuaHanTTDN = baoCaoTHTCDNs.Sum(x => x.TongSoCTQuaHanTTDN);
            SoNgayQuaHanTTDN = baoCaoTHTCDNs.Sum(x => x.SoNgayQuaHanTTDN);

            TongSoCTTiepNhanKTDK = baoCaoTHTCDNs.Sum(x => x.TongSoCTTiepNhanKTDK);
            TongSoCTCoTroNgaiKTDK = baoCaoTHTCDNs.Sum(x => x.TongSoCTCoTroNgaiKTDK);
            TongSoCTDaHoanThanhKTDK = baoCaoTHTCDNs.Sum(x => x.TongSoCTDaHoanThanhKTDK);
            TongSoCTChuaHoanThanhKTDK = baoCaoTHTCDNs.Sum(x => x.TongSoCTChuaHoanThanhKTDK);
            TongSoCTQuaHanKTDK = baoCaoTHTCDNs.Sum(x => x.TongSoCTQuaHanKTDK);
            SoNgayQuaHanKTDK = baoCaoTHTCDNs.Sum(x => x.SoNgayQuaHanKTDK);

            TongSoCTTiepNhanNT = baoCaoTHTCDNs.Sum(x => x.TongSoCTTiepNhanNT);
            TongSoCTCoTroNgaiNT = baoCaoTHTCDNs.Sum(x => x.TongSoCTCoTroNgaiNT);
            TongSoCTDaHoanThanhNT = baoCaoTHTCDNs.Sum(x => x.TongSoCTDaHoanThanhNT);
            TongSoCTChuaHoanThanhNT = baoCaoTHTCDNs.Sum(x => x.TongSoCTChuaHoanThanhNT);
            TongSoCTQuaHanNT = baoCaoTHTCDNs.Sum(x => x.TongSoCTQuaHanNT);
            SoNgayQuaHanNT = baoCaoTHTCDNs.Sum(x => x.SoNgayQuaHanNT);
            TongSoCTQuaHan = baoCaoTHTCDNs.Sum(x => x.TongSoCTQuaHan);
            SoNgayQuaHan = baoCaoTHTCDNs.Sum(x => x.SoNgayQuaHan);


            var countSoNgayThucHienTBTTDN = 0;

       
            var countSoNgayThucHienTBKTDK = 0;

       
            var countSoNgayThucHienTBNT = 0;

            var countTongSoNgayTB = 0; 


            foreach(var item in baoCaoTHTCDNs)
            {
                
                if (item.SoNgayThucHienTBTTDN > 0)
                {
                    countSoNgayThucHienTBTTDN = countSoNgayThucHienTBTTDN + 1;
                }
               
                if (item.SoNgayThucHienTBKTDK > 0)
                {
                    countSoNgayThucHienTBKTDK = countSoNgayThucHienTBKTDK + 1;
                }
               
                if (item.SoNgayThucHienTBNT > 0)
                {
                    countSoNgayThucHienTBNT = countSoNgayThucHienTBNT + 1;
                }
                if (item.TongSoNgayTB > 0)
                {
                    countTongSoNgayTB = countTongSoNgayTB + 1;
                }
                
            }

           
            if(countSoNgayThucHienTBTTDN>0){
                SoNgayThucHienTBTTDN= Math.Round(baoCaoTHTCDNs.Sum(x => x.SoNgayThucHienTBTTDN) / countSoNgayThucHienTBTTDN, 1, MidpointRounding.AwayFromZero) ; 
            }
            
            if(countSoNgayThucHienTBKTDK>0){
                SoNgayThucHienTBKTDK = Math.Round(baoCaoTHTCDNs.Sum(x => x.SoNgayThucHienTBKTDK) / countSoNgayThucHienTBKTDK, 1, MidpointRounding.AwayFromZero);
            }
          
            if (countSoNgayThucHienTBNT>0){
                SoNgayThucHienTBNT = Math.Round(baoCaoTHTCDNs.Sum(x => x.SoNgayThucHienTBNT) / countSoNgayThucHienTBNT, 1, MidpointRounding.AwayFromZero);
            }
            if(countTongSoNgayTB>0){
                TongSoNgayTB = Math.Round(baoCaoTHTCDNs.Sum(x => x.TongSoNgayTB) / countTongSoNgayTB, 1, MidpointRounding.AwayFromZero);
            }

        }
        public IList<BaoCaoTHTCDN> ListItem { get; set; }=new List<BaoCaoTHTCDN>();
        public  int TongSoCTTiepNhanTTDN { get; set; }
        public  int TongSoCTCoTroNgaiTTDN { get; set; }
        public  int TongSoCTDaHoanThanhTTDN { get; set; }
        public  int TongSoCTChuaHoanThanhTTDN { get; set; }
        public  decimal SoNgayThucHienTBTTDN { get; set; } = 0;
        public virtual int TongSoCTQuaHanTTDN { get; set; }
        public virtual decimal SoNgayQuaHanTTDN { get; set; } = 0;

        public  int TongSoCTTiepNhanKTDK { get; set; }
        public  int TongSoCTCoTroNgaiKTDK { get; set; }
        public  int TongSoCTDaHoanThanhKTDK { get; set; }
        public  int TongSoCTChuaHoanThanhKTDK { get; set; }
        public  decimal SoNgayThucHienTBKTDK { get; set; } = 0;
        public virtual int TongSoCTQuaHanKTDK { get; set; }
        public virtual decimal SoNgayQuaHanKTDK { get; set; } = 0;

        public  int TongSoCTTiepNhanNT { get; set; }
        public  int TongSoCTCoTroNgaiNT { get; set; }
        public  int TongSoCTDaHoanThanhNT { get; set; }
        public  int TongSoCTChuaHoanThanhNT { get; set; }
        public  decimal SoNgayThucHienTBNT { get; set; } = 0;
        public virtual int TongSoCTQuaHanNT { get; set; }
        public virtual decimal SoNgayQuaHanNT { get; set; } = 0;

        public  decimal TongSoNgayTB { get; set; } = 0;
        public virtual int TongSoCTQuaHan { get; set; }
        public virtual decimal SoNgayQuaHan { get; set; } = 0;
    }
}