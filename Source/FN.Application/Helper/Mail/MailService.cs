using FN.Application.Systems.Redis;
using FN.ViewModel.Helper;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace FN.Application.Helper.Mail
{
    public class MailService : IMailService
    {
        private readonly MailSetting _smtpSettings;
        private readonly IRedisService _redisService;
        public MailService(IOptions<MailSetting> smtpSettings, IRedisService redisService)
        {
            _smtpSettings = smtpSettings.Value;
            _redisService = redisService;
        }
        public async Task<bool> SendMail(string toMail, string subject, string templateId, JObject variables)
        {
            MailjetClient client = new MailjetClient(_smtpSettings.ApiKey, _smtpSettings.SecretKey);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.Vars, variables)
            .Property(Send.FromEmail, _smtpSettings.SenderEmail)
            .Property(Send.FromName, _smtpSettings.SenderName)
            .Property(Send.Subject, subject)
            .Property(Send.MjTemplateID, templateId)
            .Property(Send.MjTemplateLanguage, true)
            .Property(Send.To, toMail);

            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
                return true;
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}", response.StatusCode));
                Console.WriteLine(string.Format("ErrorInfo: {0}", response.GetErrorInfo()));
                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}", response.GetErrorMessage()));
                return false;
            }
        }
        public async Task<bool> SendMail(string toMail, string subject, string templateId, Dictionary<string, object> variables)
        {
            MailjetClient client = new MailjetClient(_smtpSettings.ApiKey, _smtpSettings.SecretKey);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.FromEmail, _smtpSettings.SenderEmail)
            .Property(Send.FromName, _smtpSettings.SenderName)
            .Property(Send.Subject, subject)
            .Property(Send.MjTemplateID, templateId)
            .Property(Send.MjTemplateLanguage, true)
            .Property(Send.Vars, JObject.FromObject(variables))
            .Property(Send.To, toMail);
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
                return true;
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}", response.StatusCode));

                Console.WriteLine(string.Format("ErrorInfo: {0}", response.GetErrorInfo()));

                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}", response.GetErrorMessage()));
                return false;
            }
        }

        public async Task<bool> SendMail(string toEmail, string subject, string body)
        {
            MailjetClient client = new MailjetClient(_smtpSettings.ApiKey, _smtpSettings.SecretKey);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }.Property(Send.FromEmail, _smtpSettings.SenderEmail)
            .Property(Send.FromName, _smtpSettings.SenderName)
            .Property(Send.Subject, subject)
            .Property(Send.TextPart, body)
            .Property(Send.To, toEmail);
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
                return true;
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}", response.StatusCode));

                Console.WriteLine(string.Format("ErrorInfo: {0}", response.GetErrorInfo()));

                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}", response.GetErrorMessage()));
                return false;
            }
        }
        public async Task<bool> IsJustSendMail(int userId)
        {
            var key = $"auth:{userId}:just_send_mail";
            return await _redisService.KeyExist(key);
        }
    }
}
