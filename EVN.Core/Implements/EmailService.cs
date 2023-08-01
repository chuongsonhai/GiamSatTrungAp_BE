using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Core.Implements
{
    public class EmailService : FX.Data.BaseService<Email, int>, IEmailService
    {
        private ILog log = LogManager.GetLogger(typeof(DvTienTrinhService));
        public EmailService(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
        {
        }
    }
}
