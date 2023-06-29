using EVN.Core.Domain;
using System.Collections.Generic;

namespace EVN.Api.Model
{
    public class UserModel
    {
        public int userId { get; set; }
        public string username { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string maDViQLy { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }

        public IList<RoleModel> Roles { get; set; } = new List<RoleModel>();
    }    
}
