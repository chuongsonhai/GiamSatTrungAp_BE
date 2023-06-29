using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core
{
    public class NoTransaction
    {
        private readonly INoTemplateService service;
        private NoTemplate CurrentTemp;
        decimal currentNo;

        public NoTransaction(INoTemplateService nosrv, NoType type = NoType.BBKhaoSat)
        {
            service = nosrv;
            CurrentTemp = service.GetbyType(type);

            if (CurrentTemp == null)
                throw new Exception("Chưa cấu hình dải số cho biên bản/công văn.");
            
            currentNo = CurrentTemp.CurrentNo;
        }

        public decimal GetNextNo()
        {
            currentNo = currentNo + 1;
            return currentNo;
        }

        public string GetCode(string compCode, decimal no)
        {
            var currentYear = DateTime.Today.Year;
            return string.Format(CurrentTemp.Format, currentYear, no,  compCode);
        }

        public string GetCodePMIS(string compCode, decimal no)
        {
            var currentYear = DateTime.Today.Year;
            return string.Format(CurrentTemp.Format, compCode, no);
        }

        public void CommitTran()
        {
            CurrentTemp.CurrentNo = currentNo;
            service.Save(CurrentTemp);
        }
    }
}
