using System;
using System.Net;
using System.Net.Mail;
using BusinessLayer.Helpers;

namespace BusinessLayer.Services
{
	public static class SmtpManager
	{
		public static bool CreateAndSendEmail(string body, string subject, string emailTo, Attachment at = null)
		{
			return SendEmail(CreateEmail(body, subject, emailTo, at));
		}

		public static MailMessage CreateEmail(string body, string subject, string emailTo, Attachment at = null)
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

			return result;
		}

		private static bool SendEmail(MailMessage email)
		{
			var result = false;
			try
			{
				using var smtp = GetSmtp();
				smtp.Send(email);
				result = true;
			}
			catch (Exception)
			{
				// ignored
			}

			return result;
		}

		private static SmtpClient GetSmtp()
		{
			return new SmtpClient(ConfigData.EmailSmtp, ConfigData.EmailSmtpPort)
			{
				EnableSsl = true,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(ConfigData.EmailLogin, ConfigData.EmailPassword)
			};
		}
	}
}