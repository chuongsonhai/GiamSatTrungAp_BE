﻿using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class Cauhinhcanhbao
    {
        public Cauhinhcanhbao()
        {
        }
        public Cauhinhcanhbao(DanhMucLoaiCanhBao loaiCanhBao) : base()
        {
            maLoaiCanhBao = loaiCanhBao.ID;
            tenLoaiCanhbao = loaiCanhBao.TENLOAICANHBAO;
            chuKy = loaiCanhBao.CHUKYCANHBAO;
            thoiGianChayCuoi = loaiCanhBao.THOIGIANCHAYCUOI;
            trangThai = loaiCanhBao.TRANGTHAI;
        }

        public int maLoaiCanhBao { get; set; }
        public string tenLoaiCanhbao { get; set; }
        public int chuKy { get; set; }
        public DateTime thoiGianChayCuoi { get; set; }
        public int trangThai { get; set; }

    }
}
