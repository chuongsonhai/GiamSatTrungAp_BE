using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Repository
{
    public interface IRepository
    {
        byte[] GetData(string data);
        string Store(string subfolder, byte[] data, string currentpath = "", string loaiFile = "PDF");
        string CombineToPdf(string subfolder, List<string> filePath);
    }
}
