using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model
{


    public class UserDataNHANdata
    {
        public UserDataNHANdata(Userdata entity) : base()
        {
            userId = entity.userId;
            username = entity.username;
            fullName = entity.fullName;
            maDViQLy = entity.maDViQLy;
        }
        public virtual int userId { get; set; }
        public virtual string username { get; set; }
        public virtual string fullName { get; set; }
        public virtual string maDViQLy { get; set; }

     

    }
}
