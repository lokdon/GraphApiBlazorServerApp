namespace GraphApiBlazorServerApp.GraphBrokers.EmailBrokers
{
    public interface IEmailBroker
    {
        public Task SendEmailAsync(string subject, string content, string toAddress, string fromAddress);
        public Task SendEmailAsync(string subject, string content, List<string> toAddress, string fromAddress);
    }
}
