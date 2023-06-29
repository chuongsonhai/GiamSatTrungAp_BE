using EVN.Core.CMIS;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class MayBienDongModel
    {
        public MayBienDongModel() { }
        public MayBienDongModel(MayBienDong entity)
        {
            ID = entity.ID;
            BBAN_ID = entity.BBAN_ID;
            SO_TBI = entity.SO_TBI;
            NAM_SX = entity.NAM_SX;
            NGAY_KDINH = entity.NGAY_KDINH;
            LOAI = entity.LOAI;
            TYSO_TI = entity.TYSO_TI;
            CHIHOP_VIEN = entity.CHIHOP_VIEN;
            TEM_KD_VIEN = entity.TEM_KD_VIEN;
            TI_THAO = entity.TI_THAO;
        }
        public int ID { get; set; }
        public int BBAN_ID { get; set; }
        public string SO_TBI { get; set; }
        public string NAM_SX { get; set; }
        public string NGAY_KDINH { get; set; }
        public string LOAI { get; set; }
        public string TYSO_TI { get; set; }
        public string CHIHOP_VIEN { get; set; }
        public string TEM_KD_VIEN { get; set; }
        public bool TI_THAO { get; set; } = true;

        public MayBienDong ToEntity(MayBienDong entity, string maDViQLy)
        {
            entity.SO_TBI = SO_TBI;
            entity.NAM_SX = NAM_SX;
            entity.NGAY_KDINH = NGAY_KDINH;
            entity.LOAI = LOAI;
            entity.TYSO_TI = TYSO_TI;
            entity.CHIHOP_VIEN = CHIHOP_VIEN;
            entity.TEM_KD_VIEN = TEM_KD_VIEN;
            if (!string.IsNullOrWhiteSpace(SO_TBI) && string.IsNullOrWhiteSpace(TEM_KD_VIEN))
            {
                ICmisProcessService service = new CmisProcessService();
                var list = service.GetTI(maDViQLy, SO_TBI);
                if (list.Count > 0)
                {
                    entity.TEM_KD_VIEN = list[0].MTEM_KD;
                }
            }
            entity.TI_THAO = TI_THAO;
            return entity;
        }
    }
}
