using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class UserNhanCanhBaoid
    {
        public UserNhanCanhBaoid()
        {
        }
        public UserNhanCanhBaoid(UserNhanCanhBao loaiCanhBao) : base()
        {
            ID = loaiCanhBao.ID;
            MA_DVIQLY = loaiCanhBao.MA_DVIQLY;
            USER_ID = loaiCanhBao.USER_ID;
  
        }
        public string MA_DVIQLY { get; set; }
        public int USER_ID { get; set; }

        public int ID { get; set; }
    }
}
