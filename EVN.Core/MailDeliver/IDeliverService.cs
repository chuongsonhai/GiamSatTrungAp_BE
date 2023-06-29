using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core
{
    public interface IDeliverService
    {
        void PushTienTrinh(string mamaDViQLy, string maYeuCau);
        void PushTienTrinh(IList<DvTienTrinh> tientrinhs);
        void Deliver(string maYCau, params string[] emails);
    }
}
