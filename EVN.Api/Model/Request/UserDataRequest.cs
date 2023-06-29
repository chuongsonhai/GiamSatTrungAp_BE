using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class UserDataRequest
    {
        public UserDataRequest()
        {
            Roles = new List<string>();
        }
        public UserDataRequest(Userdata user) : base()
        {
            userId = user.userId;
            username = user.username;
            fullName = user.fullName;
            email = user.email;
            maDViQLy = user.maDViQLy;
            maBPhan = user.maBPhan;
            maNVien = user.maNVien;
            password = user.password;
            NotifyId = user.NotifyId;
            isactive = user.isactive;
            Roles = user.Roles.Select(p => p.groupName).ToList();            
        }
        public virtual int userId { get; set; }
        public virtual string username { get; set; }
        public virtual string fullName { get; set; }
        public virtual string email { get; set; }

        public virtual string maDViQLy { get; set; }
        public virtual string maBPhan { get; set; }
        public virtual string maNVien { get; set; }

        public virtual string NotifyId { get; set; }
        public virtual string password { get; set; }
        public virtual bool isactive { get; set; }
        public List<string> Roles { get; set; } = new List<string>();        

        public Userdata ToEntity(Userdata entity)
        {
            entity.email = email;
            entity.fullName = fullName;
            entity.staffCode = maNVien;
            entity.maNVien = maNVien;
            entity.maBPhan = maBPhan;
            entity.NotifyId = NotifyId;
            entity.isactive = isactive;
            return entity;
        }
    }
}
