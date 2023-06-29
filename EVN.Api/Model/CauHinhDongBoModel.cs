using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class CauHinhDongBoModel
    {
        public CauHinhDongBoModel()
        {

        }
        public CauHinhDongBoModel(CauHinhDongBo entity) : base()
        {
            MA = entity.MA;
            MO_TA = entity.MO_TA;
            NGAY_KTHUC = entity.NGAY_KTHUC.ToString("dd/MM/yyyy");
        }
        public virtual string MA { get; set; }
        public virtual string MO_TA { get; set; }
        public virtual string NGAY_KTHUC { get; set; }
        public CauHinhDongBo ToEntity(CauHinhDongBo entity)
        {
            entity.MO_TA = MO_TA;
            if (!string.IsNullOrWhiteSpace(NGAY_KTHUC))
                entity.NGAY_KTHUC = DateTime.ParseExact(NGAY_KTHUC, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
          
            return entity;
        }
    }
}