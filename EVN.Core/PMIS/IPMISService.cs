using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core
{
    public interface IPMISService
    {
        bool GhiDuLieuPMIS(string fileName, decimal filesize, string fileType, byte[] fileData);
    }

}
