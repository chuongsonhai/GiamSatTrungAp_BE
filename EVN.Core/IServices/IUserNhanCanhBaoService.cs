using EVN.Core.Domain;
using EVN.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IUserNhanCanhBaoService : FX.Data.IBaseService<UserNhanCanhBao, int>
    {
        IList<UserNhanCanhBao> GetbyMaDviQly(string MaDviQly);
        UserNhanCanhBao GetbyNo(int idloai);
        UserNhanCanhBao GetMaDviQly(string MaDviQly);
        IList<UserNhanCanhBao> Getid(string MaDviQly);
        //IList<UserNhanCanhBao> GetMadvi(string MaDviQly);
    }
}
