using System.Collections.Generic;

namespace EVNService
{
	public interface IDeliverService
	{
		void SendMail(IList<SendMail> mails);
	}
}
