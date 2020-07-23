using System.Net;
using System.Net.Mail;
using BusinessLayer.Helpers;

namespace BusinessLayer.Services
{
	public static class SmtpManager
	{
		public static void CreateAndSendEmail(string body, string subject, string emailTo, Attachment at = null)
		{
			SendEmail(CreateEmail(body, subject, emailTo, null, at));
		}

		public static void CreateAndSendEmail(string body, string subject, string emailTo, string emailCopyTo, Attachment at = null)
		{
			SendEmail(CreateEmail(body, subject, emailTo, emailCopyTo, at));
		}

		public static MailMessage CreateEmail(string body, string subject, string emailTo, string emailCopyTo = null,
			Attachment at = null)
		{
			var result = new MailMessage
			{
				From = new MailAddress(ConfigData.EmailSendFrom, ConfigData.EmailSenderName),
				IsBodyHtml = true,
				Subject = subject,
				Body = body
			};
			if (at != null)
			{
				result.Attachments.Add(at);
			}

			result.To.Add(new MailAddress(emailTo));
			if (!string.IsNullOrEmpty(emailCopyTo))
			{
				result.CC.Add(new MailAddress(emailCopyTo));
			}

			return result;
		}

		private static void SendEmail(MailMessage email)
		{
			using var smtp = GetSmtp();
			smtp.Send(email);
		}

		private static SmtpClient GetSmtp()
		{
			var client = new SmtpClient(ConfigData.EmailSmtp, ConfigData.EmailSmtpPort);
			client.UseDefaultCredentials = false;
			client.Credentials = new NetworkCredential(ConfigData.EmailLogin, ConfigData.EmailPassword);
			client.EnableSsl = true;

			return client;
		}
	}
}