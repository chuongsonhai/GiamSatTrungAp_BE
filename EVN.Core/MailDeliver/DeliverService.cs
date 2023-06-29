using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace EVN.Core
{
    public class DeliverService : IDeliverService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DeliverService));

        public void PushTienTrinh(string maDViQLy, string maYeuCau)
        {
            IDvTienTrinhService service = IoC.Resolve<IDvTienTrinhService>();
            var items = service.ListNew(maDViQLy, maYeuCau, new int[] { 0, 2 });
            PushTienTrinh(items);
        }

        public void PushTienTrinh(IList<DvTienTrinh> tientrinhs)
        {
            pushTienTrinh(tientrinhs);
        }

        public void Deliver(string maYCau, params string[] emails)
        {
            Thread thread = new Thread(delegate ()
            {
                sendMail(maYCau, emails);
            });

            thread.Start();
            if (thread.ThreadState != ThreadState.Running)
                thread.Abort();
        }

        private void pushTienTrinh(IList<DvTienTrinh> tientrinhs)
        {
            try
            {
                IDvTienTrinhService service = IoC.Resolve<IDvTienTrinhService>();
                ICongViecService cviecsrv = IoC.Resolve<ICongViecService>();
                CMISAction action = new CMISAction();

                IList<TienTrinhRequest> datareq = new List<TienTrinhRequest>();
                foreach (var item in tientrinhs.OrderBy(p => p.NGAY_BDAU).ThenBy(p => p.STT).ToList())
                {
                    if (item.NGAY_KTHUC.HasValue)
                    {
                        TimeSpan variable = item.NGAY_KTHUC.Value.Date - item.NGAY_BDAU.Date;
                        var songay = Math.Round(variable.TotalDays, 1, MidpointRounding.AwayFromZero) + 1;
                        item.SO_NGAY_LVIEC = songay.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(item.NDUNG_XLY))
                    {
                        var congviec = cviecsrv.Getbykey(item.MA_CVIEC);
                        item.NDUNG_XLY = congviec.TEN_CVIEC;
                    }
                    datareq.Add(new TienTrinhRequest(item));
                }
                int tthai = 3;
                string data = JsonConvert.SerializeObject(datareq);
                ApiService apiservice = IoC.Resolve<ApiService>();
                var result = apiservice.PostData(action.themDvTienTrinh, data);
                if (result == null) tthai = 3; //3 - Đồng bộ CMIS lỗi
                var response = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (response != null && (response.TYPE == "OK" || response.TYPE == "SUCCESS"))
                    tthai = 1;
                for (int i = 0; i < tientrinhs.Count(); i++)
                {
                    var item = tientrinhs[i];
                    item.TRANG_THAI = tthai;
                    service.Save(item);
                }
                service.CommitChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void sendMail(string maYCau, params string[] emails)
        {
            MailConfig config = new MailConfig();

            BootstrapperPool.InitializeContainer();
            ISendMailService tranSrv = IoC.Resolve<ISendMailService>();
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                var items = tranSrv.Query.Where(p => (p.MA_YCAU_KNAI == maYCau && p.TRANG_THAI == 0) || p.TRANG_THAI == 3).Take(50).ToList();
                foreach (var item in items)
                {
                    if (string.IsNullOrWhiteSpace(item.EMAIL))
                    {
                        item.TRANG_THAI = 1;
                        tranSrv.Save(item);
                        continue;
                    }
                    try
                    {
                        bool useSsl = false;
                        if (!string.IsNullOrWhiteSpace(config.enableSsl))
                        {
                            if (!bool.TryParse(config.enableSsl, out useSsl))
                                useSsl = false;
                        }

                        MailMessage mailMessage = new MailMessage(config.mailSender, item.EMAIL, item.TIEUDE, item.NOIDUNG);
                        mailMessage.BodyEncoding = Encoding.UTF8;
                        mailMessage.From = new MailAddress(config.mailSender, config.mailSender);
                        if (emails != null && emails.Count() > 1)
                        {
                            for (int i = 1; i < emails.Count(); i++)
                            {
                                mailMessage.To.Add(emails[i]);
                            }
                        }

                        mailMessage.IsBodyHtml = true;
                        SmtpClient smtpClient = new SmtpClient(config.host, int.Parse(config.port));
                        smtpClient.Credentials = new NetworkCredential(config.mailSender, config.mailPassword);

                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.EnableSsl = useSsl;
                        smtpClient.Send(mailMessage);

                        log.Error("Send mail OK");
                        item.TRANG_THAI = 1;
                        tranSrv.Save(item);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        item.TRANG_THAI = 2;
                        tranSrv.Save(item);
                    }
                }
                tranSrv.CommitChanges();
            }
            catch (Exception ex)
            {
                log.Error("err service " + ex);
            }
        }
    }
}