using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class UserdataService : FX.Data.BaseService<Userdata, int>, IUserdataService
    {
        private ILog log = LogManager.GetLogger(typeof(UserdataService)); 
        public UserdataService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<Userdata> GetbyMaDviQly(string MaDviQly)
        {
          
            if (MaDviQly != "-1")
            {
                return Query.Where(p => p.maDViQLy == MaDviQly || p.maDViQLy == "X0206" || p.maDViQLy == "PD").ToList();
            }
            else
            {
                return Query.ToList();
            }
        }

        public Userdata Getbysdt(string sdt)
        {
            return Get(p => p.phoneNumber == sdt);
        }
        public Userdata Authenticate(string username, string password)
        {
            var user = Get(p => p.username.ToUpper() == username.ToUpper());
            //if (user == null || !user.isactive) return null;
            //string passHash = GeneratorPassword.EncodePassword(password, user.passwordsalt);
            //if (user.password != passHash)
            //    return null;
            return user;
        }

        public bool SaveRolesToUser(Userdata userdata, string[] roles)
        {
            try
            {
                IRoleService service = IoC.Resolve<IRoleService>();
                var listRole = service.Query.Where(p => roles.Contains(p.groupName)).ToList();
                userdata.Roles = listRole;
                Save(userdata);
                CommitChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Userdata GetbyName(string username)
        {
            return Get(p => p.username.ToUpper() == username.ToUpper());
        }

        public bool ChangePassword(Userdata userdata, string password, string newpassword, out string message)
        {
            message = "";
            return true;
        }

        public IList<Userdata> GetbyFilter(string maDViQLy, string maBPhan, string keyword, int pageindex, int pagesize, out int total)
        {
            var query = Query;
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.maDViQLy == maDViQLy);
            if (!string.IsNullOrWhiteSpace(maBPhan))
                query = query.Where(p => p.maBPhan == maBPhan);
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.username.Contains(keyword) || p.fullName.Contains(keyword) || p.maNVien.Contains(keyword));
            total = query.Count();
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

        public IList<Userdata> GetByMaCV(string maDViQLy, string maCV)
        {
            var query = Query;
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.maDViQLy == maDViQLy);
            IBPhanCongViecService bPhanCongViecService = IoC.Resolve<IBPhanCongViecService>();
            var listbpcv = bPhanCongViecService.GetbyTienTrinh(maDViQLy, "TBAC_D", new[] { maCV });
            var listMaBP = listbpcv.Select(p => p.MA_BPHAN).ToArray();
            if (listMaBP != null && listMaBP.Count() > 0)
                query = query.Where(p => listMaBP.Contains(p.maBPhan));
            return query.ToList();

        }

        public Userdata GetbyTicket(string ticket)
        {
            try
            {
                var result = GetInfo(ticket);
                if (result == null) return null;
                var userdata = result.data.identity;
                if (!Query.Any(p => p.orgId == userdata.orgId && p.maBPhan == userdata.maBPhan && p.username == userdata.username))
                {
                    userdata.userId = 0;
                    userdata.isactive = true;
                    userdata.passwordsalt = GeneratorPassword.GenerateSalt();
                    userdata.password = GeneratorPassword.EncodePassword("098765@a", userdata.passwordsalt);
                    CreateNew(userdata);                    
                    CommitChanges();
                    string[] roles = new string[] { "NhanVien" };
                    if (userdata.orgId == "281")
                        roles = new string[] { "Admin" };
                    SaveRolesToUser(userdata, roles);                    
                }
                return userdata;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        SSOResponse GetInfo(string ticket)
        {
            try
            {
                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
                var ssoConfigs = cfgservice.GetDictionary("SSO_URL_GETINFO");

                string syncUrl = ssoConfigs["SSO_URL_GETINFO"];
                var client = new RestClient($"{syncUrl}?ticket={ticket}&appCode=DAUNOI_TRUNGAP");
                var request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("Content-Type", "application/json");

                IRestResponse response = client.Execute(request);
                log.Error(response.Content);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;
                SSOResponse result = JsonConvert.DeserializeObject<SSOResponse>(response.Content);
                if (result.status != "SUCCESS") return null;
                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }
    }
}
