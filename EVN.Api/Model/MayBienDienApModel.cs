using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class MayBienDienApModel
    {
        public MayBienDienApModel() { }
        public MayBienDienApModel(MayBienDienAp entity)
        {
            ID = entity.ID;
            BBAN_ID = entity.BBAN_ID;
            SO_TBI = entity.SO_TBI;
            NAM_SX = entity.NAM_SX;
            NGAY_KDINH = entity.NGAY_KDINH;
            LOAI = entity.LOAI;
            TYSO_TU = entity.TYSO_TU;
            CHIHOP_VIEN = entity.CHIHOP_VIEN;
            TEM_KD_VIEN = entity.TEM_KD_VIEN;
            TU_THAO = entity.TU_THAO;
        }
        public int ID { get; set; }
        public int BBAN_ID { get; set; }
        public string SO_TBI { get; set; }
        public string NAM_SX { get; set; }
        public string NGAY_KDINH { get; set; }
        public string LOAI { get; set; }
        public string TYSO_TU { get; set; }
        public string CHIHOP_VIEN { get; set; }
        public string TEM_KD_VIEN { get; set; }
        public bool TU_THAO { get; set; } = true;

        public MayBienDienAp ToEntity(MayBienDienAp entity)
        {
            
            
            entity.SO_TBI = SO_TBI;
            entity.NAM_SX = NAM_SX;
            entity.NGAY_KDINH = NGAY_KDINH;
            entity.LOAI = LOAI;
            entity.TYSO_TU = TYSO_TU;
            entity.CHIHOP_VIEN = CHIHOP_VIEN;
            entity.TEM_KD_VIEN = TEM_KD_VIEN;
            entity.TU_THAO = TU_THAO;
            return entity;
        }
    }
}
