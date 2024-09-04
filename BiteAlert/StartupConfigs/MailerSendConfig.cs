using System.Net.Http.Headers;

namespace BiteAlert.StartupConfigs;

public static class MailerSendConfig
{
    public static void AddMailerSendConfig(this IServiceCollection services, IConfiguration config)
    {
        var apiKey = config["MailerSend:ApiKey"] ??
            throw new ArgumentException("Failed to get MailerSend API key.");

        var sendEmailUrl = config["MailerSend:SendEmailUrl"] ??
            throw new ArgumentException("Failed to get MailerSend email sending url.");

        services.AddHttpClient("mailerSend", client =>
        {
            client.BaseAddress = new Uri(sendEmailUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        });
    }
}
