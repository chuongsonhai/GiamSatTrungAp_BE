using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IOrganizationService : FX.Data.IBaseService<Organization, long>
    {
        IList<Organization> GetbyParent(string code);
        Organization GetbyCode(string code);
        IList<Organization> Sync();
    }
}
