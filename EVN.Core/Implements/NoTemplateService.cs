using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class NoTemplateService : FX.Data.BaseService<NoTemplate, int>, INoTemplateService
    {
        private static Hashtable LockTable = new Hashtable();
        public NoTemplateService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public NoTemplate GetbyType(NoType type)
        {
            if (!LockTable.Contains(type))
            {
                object lockobj = new object();
                LockTable.Add(type, lockobj);
            }
            lock (LockTable[type])
            {
                var sql = $"SELECT c.* FROM NoTemplate c WHERE c.Type={(int)type} and c.CurrentYear={DateTime.Today.Year}";
                var item = GetbySQLQuery(sql);
                if (item == null || item.Count() == 0)
                {                    
                    var noTemp = new NoTemplate();
                    noTemp.Type = type;
                    noTemp.CurrentYear = DateTime.Today.Year;
                    noTemp.CurrentNo = 0;
                    noTemp.Format = "{0}-{1}/{2}";
                    CreateNew(noTemp);
                    CommitChanges();
                    return noTemp;
                }
                return item.FirstOrDefault();
            }
        }
    }
}