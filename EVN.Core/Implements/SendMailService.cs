using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class SendMailService : FX.Data.BaseService<SendMail, int>, ISendMailService
    {
        public SendMailService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<SendMail> GetbyFilter(string maYCau, string keyword, int status, DateTime fromtime, DateTime totime, int pageindex, int pagesize, out int total)
        {
            var query = Query;
            if (status > -1)
                query = query.Where(p => p.TRANG_THAI == status);
            if (!string.IsNullOrWhiteSpace(maYCau))
                query = query.Where(p => p.MA_YCAU_KNAI == maYCau);
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.TIEUDE.Contains(keyword) || p.EMAIL.Contains(keyword));
            if (fromtime != DateTime.MinValue)
                query = query.Where(p => p.NGAY_TAO >= fromtime.Date);
            if (totime != DateTime.MaxValue)
                query = query.Where(p => p.NGAY_TAO < totime.AddDays(1));

            query = query.OrderByDescending(p => p.NGAY_TAO);
            int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
            var ret = query.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
            total = pageindex * pagesize + ret.Count;
            return ret.Take(pagesize).ToList();
        }

        public void Process(SendMail item, string templateName, Dictionary<string, string> bodyParams)
        {
            try
            {
                string tempPath = DetermineTemplatePath(templateName);
                item.NOIDUNG = GetBody(tempPath, bodyParams);
                CreateNew(item);
                CommitChanges();
            }
            catch (Exception ex)
            {

            }
        }

        string DetermineTemplatePath(string templateName)
        {
            string filePath = $"/MailTemplates/{templateName}.txt";
            return AppDomain.CurrentDomain.BaseDirectory + filePath;
        }

        string GetBody(string tempPath, Dictionary<string, string> bodyParams)
        {
            string templateContent = File.ReadAllText(tempPath);
            string emailTemplateContent = "";
            if (!string.IsNullOrEmpty(templateContent))
            {
                emailTemplateContent = templateContent;
            }

            Regex bodyRegex = new Regex(@"\[body\](.*)\[/body\]"
                , RegexOptions.Compiled | RegexOptions.Singleline);
            string body = bodyRegex.Match(emailTemplateContent).Groups[1].Value;

            body = ReplacePlaceholdersWithValues(bodyParams, body);
            return body;
        }

        string ReplacePlaceholdersWithValues(Dictionary<string, string> parameters, string textWithPlaceholders)
        {
            string processedText = textWithPlaceholders;
            foreach (KeyValuePair<string, string> param in parameters)
            {
                processedText = processedText.Replace(param.Key, param.Value);
            }
            return processedText;
        }
    }
}
