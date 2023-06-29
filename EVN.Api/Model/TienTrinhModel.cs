using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class TienTrinhModel
    {
        public TienTrinhModel(DvTienTrinh entity)
        {
            KQ_ID_BUOC = entity.KQ_ID_BUOC;
            MA_BPHAN_GIAO = entity.MA_BPHAN_GIAO;
            MA_BPHAN_NHAN = entity.MA_BPHAN_NHAN;
            MA_CVIEC = entity.MA_CVIEC;
            MA_CVIECTIEP = entity.MA_CVIECTIEP;
            MA_DDO_DDIEN = entity.MA_DDO_DDIEN;
            MA_DVIQLY = entity.MA_DVIQLY;
            MA_NVIEN_GIAO = entity.MA_NVIEN_GIAO;
            MA_NVIEN_NHAN = entity.MA_NVIEN_NHAN;

            MA_TNGAI = entity.MA_TNGAI;
            MA_YCAU_KNAI = entity.MA_YCAU_KNAI;

            NDUNG_XLY = entity.NDUNG_XLY;
            NGAY_BDAU = entity.NGAY_BDAU.ToString("dd/MM/yyyy");
            NGAY_HEN = entity.NGAY_HEN.ToString("dd/MM/yyyy");
            NGAY_KTHUC = entity.NGAY_KTHUC.HasValue ? entity.NGAY_KTHUC.Value.ToString("dd/MM/yyyy") : "";
            NGUYEN_NHAN = entity.NGUYEN_NHAN;
            SO_LAN = entity.SO_LAN;
            if (entity.NGAY_KTHUC.HasValue)
            {
                var songay = CommonUtils.TotalDate(entity.NGAY_BDAU.Date, entity.NGAY_KTHUC.Value.Date);                
                SO_NGAY_LVIEC = songay.ToString();
            }
            TRANG_THAI = entity.TRANG_THAI;
        }
        public virtual long KQ_ID_BUOC { get; set; }
        public virtual string MA_BPHAN_GIAO { get; set; }
        public virtual string MA_BPHAN_NHAN { get; set; }
        public virtual string MA_CNANG { get; set; } = "WEB_TBAC_D";
        public virtual string MA_CVIEC { get; set; }
        public virtual string MA_CVIECTIEP { get; set; }
        public virtual string MA_DDO_DDIEN { get; set; } //Mã KH
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_NVIEN_GIAO { get; set; }
        public virtual string MA_NVIEN_NHAN { get; set; }
        public virtual string MA_TNGAI { get; set; }
        public virtual string MA_YCAU_KNAI { get; set; }
        public virtual string NDUNG_XLY { get; set; }

        public virtual string NGAY_BDAU { get; set; }
        public virtual string NGAY_HEN { get; set; }
        public virtual string NGAY_KTHUC { get; set; }
        public virtual string NGUYEN_NHAN { get; set; }

        public virtual int SO_LAN { get; set; } = 1;
        public virtual string SO_NGAY_LVIEC { get; set; }

        public virtual int TRANG_THAI { get; set; } = 0;//0: Chưa gửi, 1: Đã gửi, 2: Gửi lỗi
    }
}