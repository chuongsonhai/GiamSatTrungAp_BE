using EVN.Core.Domain;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IDepartmentService : FX.Data.IBaseService<Department, long>
    {
        IList<Department> GetbyOrgId(long orgId);
        IList<Department> ListbyParentID(string keyword, long parentID);
        IList<Department> GetbyParentID(string keyword, int status, long parentID);
        Department GetbyCode(string code);

        IList<Department> Sync();
    }
}