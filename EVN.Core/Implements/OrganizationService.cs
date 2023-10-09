using EVN.Core.Domain;
using EVN.Core.IServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Core.Implements
{
    public class OrganizationService : FX.Data.BaseService<Organization, long>, IOrganizationService
    {
        public OrganizationService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public Organization GetbyCode(string code)
        {
            return Get(p => p.orgCode == code);
        }

        public IList<Organization> Getbymadvi()
        {
            return Query.Where(p => p.orgId != 281 && p.orgId != 276).ToList();
        }

        public IList<Organization> GetbyParent(string code)
        {
            return Query.Where(p => p.parentCode == code).ToList();
        }

        public IList<Organization> Sync()
        {
            var result = ApiHelper.SyncApi("/api/donviqly/sync");
            if (result == null)
            {
                return Query.ToList();
            }
            var list = JsonConvert.DeserializeObject<IList<Organization>>(result);
            var orgs = new List<Organization>();
            foreach (var item in list)
            {
                var org = GetbyCode(item.orgCode);
                if (org == null) continue;
                org.orgName = item.orgName;
                if(!string.IsNullOrWhiteSpace(item.address))
                    org.address = item.address;
                org.capDvi = item.capDvi;
                if (!string.IsNullOrWhiteSpace(item.chucVu))
                    org.chucVu = item.chucVu;
                if (!string.IsNullOrWhiteSpace(item.daiDien))
                    org.daiDien = item.daiDien;
                org.email = item.email;
                org.fax = item.fax;
                org.idDiaChinh = item.idDiaChinh;                
                org.parentCode = item.parentCode;
                if (!string.IsNullOrWhiteSpace(item.phone))
                    org.phone = item.phone;
                if (!string.IsNullOrWhiteSpace(item.taxCode))
                    org.taxCode = item.taxCode;
                org.updatetime = DateTime.Now;
                Save(org);
                orgs.Add(org);
            }
            CommitChanges();
            return orgs;
        }
    }
}
