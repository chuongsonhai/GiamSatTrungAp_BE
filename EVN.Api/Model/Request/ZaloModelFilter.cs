using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class CanhBaoRequest
    {
        public CanhBaoRequest()
        {
        }
        public CanhBaoRequest(CanhBao CanhBao) : base()
        {
            id = CanhBao.ID;
            maLoaiCanhBao = CanhBao.LOAI_CANHBAO_ID;
            noiDungCanhBao = CanhBao.NOIDUNG;
            thoiGianGui = CanhBao.THOIGIANGUI;
            donViQuanLy = CanhBao.DONVI_DIENLUC;
            trangThai = CanhBao.TRANGTHAI_CANHBAO;
            lanGui = CanhBao.LOAI_SOLANGUI;
            maYeuCau = CanhBao.MA_YC;
        }
        public int id { get; set; }
        public int maLoaiCanhBao { get; set; }
        public string noiDungCanhBao { get; set; }
        public DateTime thoiGianGui { get; set; }
        public string donViQuanLy { get; set; }
        public string maYeuCau { get; set; }
        public int trangThai { get; set; }
        public int lanGui { get; set; }
      

    }
}
