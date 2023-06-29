using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class ChuyenTiepModel
    {
        public ChuyenTiepModel()
        {

        }

        public ChuyenTiepModel(ChuyenTiep entity)
        {
            ID = entity.ID;
            MA_DVIQLY = entity.MA_DVIQLY;
            NDUNG_XLY = entity.NDUNG_XLY;
            MA_BPHAN_NHAN = entity.MA_BPHAN_NHAN;
            MA_NVIEN_NHAN = entity.MA_NVIEN_NHAN;

            NGAY_HEN = entity.NGAY_HEN.ToString("dd/MM/yyyy");
            MA_CVIEC = entity.MA_CVIEC;
            TRANG_THAI = entity.TRANG_THAI;
        }

        public virtual int ID { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual string NDUNG_XLY { get; set; }

        public virtual string MA_BPHAN_NHAN { get; set; }
        public virtual string MA_NVIEN_NHAN { get; set; }

        public virtual string NGAY_HEN { get; set; }
        public virtual string MA_CVIEC { get; set; }
        public virtual int TRANG_THAI { get; set; } = 0;
    }
}