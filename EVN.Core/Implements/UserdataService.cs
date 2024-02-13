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
                //return Query.Where(p => p.maDViQLy == MaDviQly || p.maDViQLy == "X0206" || p.maDViQLy == "PD" ).ToList();
                var madviqlies = new List<string> { MaDviQly, "X0206", "PD" };
                return Query.Where(u => u.phoneNumber != null && madviqlies.Contains(u.maDViQLy)).ToList();
            }
            else
            {
                return Query.ToList();
            }
        }

        public IList<Userdata> GetMadvi(string MaDviQly)
        {

            if (MaDviQly != "-1")
            {
                return Query.Where(p => p.maDViQLy == MaDviQly && p.maDViQLy != null).ToList();
            }
            else
            {
                return Query.ToList();
            }
        }

        public Userdata GetMaDviQly(string MaDviQly)
        {
            return Get(p => p.maDViQLy == MaDviQly);
        }

        public Userdata Getid(int userid)
        {
            return Get(p => p.userId == userid);
        }

        public Userdata Getbysdt(string sdt)
        {
            return Get(p => p.phoneNumber == sdt);
        }

        public Userdata AuthenticateTrungap(string username, string password)
        {
            //code check mk iso
            var user = Get(p => p.username.ToUpper() == username.ToUpper());
            if (user == null || !user.isactive) return null;

            // Kiểm tra mật khẩu theo yêu cầu mới
            if (!IsPasswordValid(password)) return null;

            string passHash = GeneratorPassword.EncodePassword(password, user.passwordsalt);
            if (user.password != passHash)
                return null;

            return user;
        }

        public Userdata Authenticate(string username, string password)
        {
            //code cũ
            var user = Get(p => p.username.ToUpper() == username.ToUpper());
            //if (user == null || !user.isactive) return null;
            //string passHash = GeneratorPassword.EncodePassword(password, user.passwordsalt);
            //if (user.password != passHash)
            //    return null;
            return user;
        }
        private bool IsPasswordValid(string password)
        {
            // Kiểm tra độ dài tối thiểu
            if (password.Length < 10)
                return false;

            // Kiểm tra đủ 4 loại ký tự
            bool hasUpperCase = false;
            bool hasLowerCase = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUpperCase = true;
                else if (char.IsLower(c))
                    hasLowerCase = true;
                else if (char.IsDigit(c))
                    hasDigit = true;
                else if (!char.IsLetterOrDigit(c))
                    hasSpecialChar = true;
            }

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
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

        public IList<Userdata> Getbyusernhan(string maDViQLy)
        {
            var query = Query;

            query = query.Where(p => "-1" == maDViQLy);

            return query.ToList();
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

        public async Task<Userdata> GetbyTicket(string ticket)
        {
            //try
            //{
            //    var result = GetInfo(ticket);
            //    if (result == null) return null;
            //    var userdata = result.data.identity;
            //    if (!Query.Any(p => p.orgId == userdata.orgId && p.username == userdata.username))
            //    {
            //        userdata.userId = 0;
            //        //userdata.isactive = true;
            //        userdata. = GeneratorPassword.GenerateSalt();
            //        userdata.password = GeneratorPassword.EncodePassword("098765@a", userdata.passwordsalt);
            //        CreateNew(userdata);                    
            //        CommitChanges();
            //        string[] roles = new string[] { "NhanVien" };
            //        if (userdata.orgId == "281")
            //            roles = new string[] { "Admin" };
            //        SaveRolesToUser(userdata, roles);                    
            //    }
            //    return userdata;
            //}
            //catch (Exception ex)
            //{
            //    log.Error(ex);
            //    return null;
            //}

            var newUser = new Userdata();
            try
            {
                var result = await GetInfo(ticket);
                if (result == null) return null;
                var userdata = result.data.identity;
                var query = Query.Where(p => p.username == userdata.usernameLocal || p.username == userdata.username).FirstOrDefault();
                if (query == null)
                {
                    //newUser.userId = userdata.userId;
                    newUser.isactive = true;
                    newUser.staffCode = userdata.staffCode;
                    newUser.orgId = userdata.orgId;
                    newUser.username = userdata.username;
                    newUser.fullName = userdata.fullName;
                    newUser.maDViQLy = userdata.orgEVNHES;
                    newUser.passwordsalt = GeneratorPassword.GenerateSalt();
                    newUser.password = GeneratorPassword.EncodePassword("098765@a", GeneratorPassword.GenerateSalt());
                    //userdata.userId = 0;
                    //userdata.isactive = true;
                    //userdata.passwordsalt = GeneratorPassword.GenerateSalt();
                    //userdata.password = GeneratorPassword.EncodePassword("098765@a", userdata.passwordsalt);
                    CreateNew(newUser);
                    CommitChanges();
                    string[] roles = new string[] { "NhanVien" };
                    if (userdata.orgId == "281") 
                    { 
                        roles = new string[] { "Admin" };
                    }
                    if (userdata.orgId == "276")
                    {
                        roles = new string[] { "Admin" };
                    }
                    SaveRolesToUser(newUser, roles);
                    return newUser;
                }

                else
                {
                    newUser.userId = query.userId;
                    newUser.fullName = query.fullName;
                    newUser.username = query.username;
                    newUser.email = query.email;
                    newUser.phoneNumber = query.phoneNumber;
                    newUser.orgId = query.orgId;
                    newUser.staffCode = query.staffCode;
                    newUser.maDViQLy = query.maDViQLy;
                    return newUser;
                }
               
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        async Task<SSOResponse> GetInfo(string ticket)
        {
            try
            {
                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
                var ssoConfigs = cfgservice.GetDictionary("SSO_URL_NEW");

                string syncUrl = ssoConfigs["SSO_URL_NEW"];
                var client = new RestClient($"{syncUrl}?ticket={ticket}&appCode=DAUNOI_TRUNGAP");
                var request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("Content-Type", "application/json");

                IRestResponse response = await client.ExecuteTaskAsync(request);
                log.Error(response.Content);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;

                SSOResponse result = JsonConvert.DeserializeObject<SSOResponse>(response.Content);
                if (result.status != "SUCCESS")
                    return null;

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
