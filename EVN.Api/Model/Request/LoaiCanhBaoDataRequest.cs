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
            ID = loaiCanhBao.ID; //maloaiCanhbao
            TENLOAICANHBAO = loaiCanhBao.TENLOAICANHBAO;
            CHUKYCANHBAO = loaiCanhBao.CHUKYCANHBAO;
            THOIGIANCHAYCUOI = loaiCanhBao.THOIGIANCHAYCUOI;
            TRANGTHAI = loaiCanhBao.TRANGTHAI;
        }
        public int ID { get; set; }
        public string TENLOAICANHBAO { get; set; }
        public int CHUKYCANHBAO { get; set; }
        public string THOIGIANCHAYCUOI { get; set; }
        public int TRANGTHAI { get; set; }

    }
}
