using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class LogCanhBaoRequest
    {
        public LogCanhBaoRequest()
        {
        }
        public LogCanhBaoRequest(LogCanhBao logCanhbao) : base()
        {
            ID = logCanhbao.ID;
            CANHBAO_ID = logCanhbao.CANHBAO_ID;
            TRANGTHAI = logCanhbao.TRANGTHAI;
            DATA_CU = logCanhbao.DATA_CU;
            DATA_MOI = logCanhbao.DATA_MOI;
            THOIGIAN = logCanhbao.THOIGIAN;
            NGUOITHUCHIEN = logCanhbao.NGUOITHUCHIEN;
        }
        public int ID { get; set; }
        public int CANHBAO_ID { get; set; }
        public int TRANGTHAI { get; set; }
        public string DATA_CU { get; set; }
        public string DATA_MOI { get; set; }
        public DateTime THOIGIAN { get; set; } = DateTime.Now;
        public string NGUOITHUCHIEN { get; set; }

    }
}
