using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IUserdataService : FX.Data.IBaseService<Userdata, int>
    {
        Userdata Getid(int userid);
        IList<Userdata> GetMadvi(string MaDviQly);
        Userdata GetMaDviQly(string MaDviQly);
        IList<Userdata> Getbyusernhan(string maDViQLy);

        Userdata GetbyTicket(string ticket);
        Userdata Authenticate(string username, string password);

        bool SaveRolesToUser(Userdata userdata, string[] roles);
        IList<Userdata> GetbyMaDviQly(string MaDviQly);
        Userdata GetbyName(string username);

        bool ChangePassword(Userdata userdata, string password, string newpassword, out string message);

        IList<Userdata> GetbyFilter(string maDViQLy, string maBPhan, string keyword, int pageindex, int pagesize, out int total);
        IList<Userdata> GetByMaCV(string maDViQLy, string maCV);
        Userdata Getbysdt(string sdt);
    }
}
