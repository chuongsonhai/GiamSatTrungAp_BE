using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class LoaiCanhBaoDataRequest
    {
        public LoaiCanhBaoDataRequest()
        {
        }
        public LoaiCanhBaoDataRequest(DanhMucLoaiCanhBao loaiCanhBao) : base()
        {
            ID = loaiCanhBao.Id;
            TenLoaiCanhBao = loaiCanhBao.TenLoaiCanhBao;
            ChuKyGui = loaiCanhBao.ChuKyGui;
            PhanLoai = loaiCanhBao.PhanLoai;

        }
        public int ID { get; set; }
        public string TenLoaiCanhBao { get; set; }
        public int ChuKyGui { get; set; }
        public int PhanLoai { get; set; }
    }
}
