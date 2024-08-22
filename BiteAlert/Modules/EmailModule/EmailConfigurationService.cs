namespace BiteAlert.Modules.EmailModule;

public class EmailConfigurationService(IConfiguration config) : IEmailConfigurationService
{
    public string FromEmail { get; } = config.GetSection(
        "MailerSend:From:Email").Value ?? throw new ArgumentException(
                "Failed to get MailerSend sender email from the configurations.");

    public string FromName { get; } = config.GetSection(
        "MailerSend:From:Name").Value ?? throw new ArgumentException(
                "Failed to get MailerSend sender name from the configurations.");

    public string SupportEmail { get; } = config.GetSection(
        "MailerSend:SupportEmail").Value ?? throw new ArgumentException(
                "Failed to get MailerSend Support Email from the configurations.");

    public string BaseUrl { get; } = config.GetSection(
        "BiteAlert:BaseUrl").Value ?? throw new ArgumentException(
                "Failed to get Bite Alert base url from the configurations.");
}
