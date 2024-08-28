using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class KetQuaKTModel
    {
        public KetQuaKTModel() { }
        public KetQuaKTModel(KetQuaKT entity) : base()
        {
            ID = entity.ID;
            MA_DVIQLY = entity.MA_DVIQLY;
            MA_YCAU_KNAI = entity.MA_YCAU_KNAI;
            MA_DDO_DDIEN = entity.MA_DDO_DDIEN;

            MA_TNGAI = entity.MA_TNGAI;
            MA_BPHAN_GIAO = entity.MA_BPHAN_GIAO;
            MA_NVIEN_GIAO = entity.MA_NVIEN_GIAO;

            MA_BPHAN_NHAN = entity.MA_BPHAN_NHAN;
            MA_NVIEN_NHAN = entity.MA_NVIEN_NHAN;

            NDUNG_XLY = entity.NDUNG_XLY;
            NGAY_HEN = entity.NGAY_HEN.ToString("dd/MM/yyyy");
            NGAY_BDAU = entity.NGAY_BDAU.ToString("dd/MM/yyyy");
            NGAY_KTHUC = entity.NGAY_KTHUC.HasValue ? entity.NGAY_KTHUC.Value.ToString("dd/MM/yyyy") : "";

            MA_CVIEC_TRUOC = entity.MA_CVIEC_TRUOC;
            MA_CVIEC = entity.MA_CVIEC;
            MA_LOAI_YCAU = entity.MA_LOAI_YCAU;

            TRANG_THAI = entity.TRANG_THAI;
            THUAN_LOI = entity.THUAN_LOI ? 1 : 0;
            NGUYEN_NHAN = entity.NGUYEN_NHAN;
        }
        public virtual int ID { get; set; } = 0;
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_YCAU_KNAI { get; set; }
        public virtual string MA_DDO_DDIEN { get; set; }
        public virtual string NDUNG_XLY { get; set; }
        public virtual string MA_TNGAI { get; set; }
        public virtual string MA_BPHAN_GIAO { get; set; }
        public virtual string MA_NVIEN_GIAO { get; set; }

        public virtual string MA_BPHAN_NHAN { get; set; }
        public virtual string MA_NVIEN_NHAN { get; set; }

        public virtual string NGAY_HEN { get; set; }
        public virtual string NGAY_BDAU { get; set; }
        public virtual string NGAY_KTHUC { get; set; }

        public virtual string MA_CVIEC_TRUOC { get; set; }
        public virtual string MA_CVIEC { get; set; }
        public virtual string MA_LOAI_YCAU { get; set; }
        public int TRANG_THAI { get; set; } = 0;
        public int THUAN_LOI { get; set; } = 1;
        public virtual string NGUYEN_NHAN { get; set; }
        public KetQuaKT ToEntity(KetQuaKT entity)
        {
            entity.MA_DVIQLY = MA_DVIQLY;
            entity.MA_YCAU_KNAI = MA_YCAU_KNAI;
            entity.MA_DDO_DDIEN = MA_DDO_DDIEN;
            entity.NDUNG_XLY = NDUNG_XLY;
            entity.MA_TNGAI = MA_TNGAI;

            entity.MA_BPHAN_GIAO = MA_BPHAN_GIAO;
            entity.MA_NVIEN_GIAO = MA_NVIEN_GIAO;

            entity.MA_BPHAN_NHAN = MA_BPHAN_NHAN;
            entity.MA_NVIEN_NHAN = MA_NVIEN_NHAN;

            if (!string.IsNullOrWhiteSpace(NGAY_HEN))
                entity.NGAY_HEN = DateTime.ParseExact(NGAY_HEN, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            if (!string.IsNullOrWhiteSpace(NGAY_BDAU))
                entity.NGAY_BDAU = DateTime.ParseExact(NGAY_BDAU, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            if (!string.IsNullOrWhiteSpace(NGAY_KTHUC))
                entity.NGAY_KTHUC = DateTime.ParseExact(NGAY_KTHUC, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

            entity.MA_CVIEC_TRUOC = MA_CVIEC_TRUOC;
            entity.MA_CVIEC = MA_CVIEC;
            entity.MA_LOAI_YCAU = entity.MA_LOAI_YCAU;

            entity.TRANG_THAI = TRANG_THAI;
            entity.THUAN_LOI = THUAN_LOI == 1 ? true : false;
            return entity;
        }
    }

    public class KetQuaTCModel
    {
        public KetQuaTCModel() { }
        public KetQuaTCModel(KetQuaTC entity) : base()
        {
            ID = entity.ID;
            MA_DVIQLY = entity.MA_DVIQLY;
            MA_YCAU_KNAI = entity.MA_YCAU_KNAI;
            MA_DDO_DDIEN = entity.MA_DDO_DDIEN;

            MA_BPHAN_GIAO = entity.MA_BPHAN_GIAO;
            MA_NVIEN_GIAO = entity.MA_NVIEN_GIAO;

            MA_BPHAN_NHAN = entity.MA_BPHAN_NHAN;
            MA_NVIEN_NHAN = entity.MA_NVIEN_NHAN;

            NDUNG_XLY = entity.NDUNG_XLY;
            NGAY_HEN = entity.NGAY_HEN.ToString("dd/MM/yyyy");
            NGAY_BDAU = entity.NGAY_BDAU.ToString("dd/MM/yyyy");
            NGAY_KTHUC = entity.NGAY_KTHUC.HasValue ? entity.NGAY_KTHUC.Value.ToString("dd/MM/yyyy") : "";

            MA_CVIEC_TRUOC = entity.MA_CVIEC_TRUOC;
            MA_CVIEC = entity.MA_CVIEC;
            MA_LOAI_YCAU = entity.MA_LOAI_YCAU;

            TRANG_THAI = entity.TRANG_THAI;
            MA_TNGAI = entity.MA_TNGAI;
            NGUYEN_NHAN = entity.NGUYEN_NHAN;
            THUAN_LOI = entity.THUAN_LOI ? 1 : 0;

        

        }
        public virtual int ID { get; set; } = 0;
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_YCAU_KNAI { get; set; }
        public virtual string MA_DDO_DDIEN { get; set; }
        public virtual string NDUNG_XLY { get; set; }
        
        public virtual string MA_BPHAN_GIAO { get; set; }
        public virtual string MA_NVIEN_GIAO { get; set; }

        public virtual string MA_BPHAN_NHAN { get; set; }
        public virtual string MA_NVIEN_NHAN { get; set; }

        public virtual string NGAY_HEN { get; set; }
        public virtual string NGAY_BDAU { get; set; }
        public virtual string NGAY_KTHUC { get; set; }

        public virtual string MA_CVIEC_TRUOC { get; set; }
        public virtual string MA_CVIEC { get; set; }
        public virtual string MA_LOAI_YCAU { get; set; }
        public int TRANG_THAI { get; set; } = 0;
        public virtual int THUAN_LOI { get; set; } = 1;
        public virtual string MA_TNGAI { get; set; }

        public virtual string NGUYEN_NHAN { get; set; }

        public KetQuaTC ToEntity(KetQuaTC entity)
        {
            entity.MA_DVIQLY = MA_DVIQLY;
            entity.MA_YCAU_KNAI = MA_YCAU_KNAI;
            entity.MA_DDO_DDIEN = MA_DDO_DDIEN;
            entity.NDUNG_XLY = NDUNG_XLY;

            entity.MA_BPHAN_GIAO = MA_BPHAN_GIAO;
            entity.MA_NVIEN_GIAO = MA_NVIEN_GIAO;

            entity.MA_BPHAN_NHAN = MA_BPHAN_NHAN;
            entity.MA_NVIEN_NHAN = MA_NVIEN_NHAN;

            if (!string.IsNullOrWhiteSpace(NGAY_HEN))
                entity.NGAY_HEN = DateTime.ParseExact(NGAY_HEN, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            if (!string.IsNullOrWhiteSpace(NGAY_BDAU))
                entity.NGAY_BDAU = DateTime.ParseExact(NGAY_BDAU, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            if (!string.IsNullOrWhiteSpace(NGAY_KTHUC))
                entity.NGAY_KTHUC = DateTime.ParseExact(NGAY_KTHUC, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

            entity.MA_CVIEC_TRUOC = MA_CVIEC_TRUOC;
            entity.MA_CVIEC = MA_CVIEC;
            entity.MA_LOAI_YCAU = entity.MA_LOAI_YCAU;

            entity.TRANG_THAI = TRANG_THAI;
            entity.THUAN_LOI = THUAN_LOI == 1 ? true : false;
            entity.MA_TNGAI = MA_TNGAI;
            return entity;
        }
    }
}