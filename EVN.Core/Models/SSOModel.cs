using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core
{
    //public class SSOResponse
    //{
    //    public string code { get; set; }
    //    public string paramCode { get; set; }
    //    public string message { get; set; }
    //    public string status { get; set; }
    //    public SSOUser_NEW data { get; set; }        
    //}
    //public class SSOUser
    //{
    //    public string serviceTicket { get; set; }
    //    public string expiresIn { get; set; }
    //    public Userdata identity { get; set; }
    //    public List<Role> listGroup { get; set; }
    //}

    public class SSOUser_NEW
    {
        public string serviceTicket { get; set; }
        public string expiresIn { get; set; }
        public Identity identity { get; set; }
        public List<Role> listGroup { get; set; }
    }


    public class SSOResponse
    {
        public string code { get; set; }
        public string paramCode { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public Data data { get; set; }
        public List<object> permissions { get; set; }
        public List<ListOrg> listOrg { get; set; }
    }


    public class ListOrg
    {
        public string username { get; set; }
        public int orgid { get; set; }
    }


    public class Data
    {
        public string serviceTicket { get; set; }
        public DateTime expiresIn { get; set; }
        public Identity identity { get; set; }
        public List<Group> listGroup { get; set; }
        public YourAppReturn yourAppReturn { get; set; }
        // Các thuộc tính khác nếu có
    }

    public class Group
    {
        public int groupId { get; set; }
        public string code { get; set; }
        public string groupName { get; set; }
        public object parentGroupId { get; set; }
        public int status { get; set; }
        public string description { get; set; }
    }

    public class YourAppReturn
    {
        public string code { get; set; }
        public string message { get; set; }
        public Body body { get; set; }
    }

    public class Body
    {
        public string type { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public string accessTokenHRMS { get; set; }
        public string refreshTokenHRMS { get; set; }
        public int accessTokenExpirationSecond { get; set; }
        public int refreshTokenExpirationSecond { get; set; }
        public int userId { get; set; }
        public string username { get; set; }
        public int orgId { get; set; }
        public string deptId { get; set; }
    }

    public class Identity
    {
        public string username { get; set; }
        public string usernameLocal { get; set; }
        public string fullName { get; set; }
        public int userId { get; set; }
        //public string appCode { get; set; }
        //public int appId { get; set; }
        public string email { get; set; }
        public string ns_id { get; set; }
        //public string deptId { get; set; }
        //public string deptName { get; set; }
        public string staffCode { get; set; }
        public object positionId { get; set; }
        //public object positionName { get; set; }
        public string phone { get; set; }
        //public bool authentication2Factor { get; set; }
        public string orgId { get; set; }
        //public string orgName { get; set; }
        public string orgEVNHES { get; set; }
        //public object password { get; set; }
        //public object role_congdoan { get; set; }
        //public object role_csnd { get; set; }
    }


}
