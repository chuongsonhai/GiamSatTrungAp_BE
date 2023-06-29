using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface INotificationService
    {
        void PushMessage(string apikey, string appid, string content, params string[] userids);
    }
}
