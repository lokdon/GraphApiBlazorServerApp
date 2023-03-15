using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace GraphApiBlazorServerApp.GraphBrokers.EmailBrokers
{
    public class EmailBroker : IEmailBroker
    {

        private readonly GraphServiceClient _graphServiceClient;
        MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;

        private readonly IConfiguration _configuration;
        public EmailBroker(GraphServiceClient graphServiceClient,
                    MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler,
                    IConfiguration configuration)
        {
            _graphServiceClient = graphServiceClient;
            _consentHandler = consentHandler;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string subject, string content, string toAddress, string fromAddress)
        {
            Message message = new()
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = content
                },
                ToRecipients = new List<Recipient>()
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = toAddress
                        }
                    }
                }
            };

            bool saveToSentItems = true;

            try
            {
                //await _graphServiceClient.Users["b6edd00b-a8e4-4c94-aa8c-428cdb0b690a"]
                //                        .SendMail(message, saveToSentItems)
                //                        .Request()
                //                        .PostAsync();

                await _graphServiceClient.Me
                                   .SendMail(message, saveToSentItems)
                                   .Request()
                                   .PostAsync();
            }
            catch (Exception ex)
            {
                _consentHandler.HandleException(ex);
            }
        }

        public async Task SendEmailAsync(string subject, string content, List<string> toAddress, string fromAddress)
        {
            
            var recipents = new List<Recipient>();

            if (toAddress.Count != 0)
            {
                foreach (var recipient in toAddress)
                {
                    var model = new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = recipient
                        }
                    };

                    recipents.Add(model);
                }
            }


            Message message = new()
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = content
                },
                ToRecipients = recipents
            };

            bool saveToSentItems = true;

            try
            {
                //await _graphServiceClient.Users["b6edd00b-a8e4-4c94-aa8c-428cdb0b690a"]
                //                        .SendMail(message, saveToSentItems)
                //                        .Request()
                //                        .PostAsync();

                await _graphServiceClient.Me
                                   .SendMail(message, saveToSentItems)
                                   .Request()
                                   .PostAsync();
            }
            catch (Exception ex)
            {
                _consentHandler.HandleException(ex);
            }
        }
    }
}
