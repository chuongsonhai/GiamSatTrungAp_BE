using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class CanhbaoModel
    {
        public CanhbaoModel()
        {
        }
        public CanhbaoModel(CanhBao CanhBao) : base()
        {
            maLoaiCanhBao = CanhBao.LOAI_CANHBAO_ID;
            soLuongDaGui = CanhBao.LOAI_SOLANGUI;


        }
        public int maLoaiCanhBao { get; set; }
        public int soLuongDaGui { get; set; }
        public int soLuongThanhCong { get; set; }
        public int soLuongThatBai { get; set; }

    }
}
