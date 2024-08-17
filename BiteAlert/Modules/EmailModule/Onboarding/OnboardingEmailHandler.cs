// Ignore Spelling: Onboarding

using BiteAlert.Modules.NotificationModule;
using MediatR;

namespace BiteAlert.Modules.EmailModule.Onboarding;

public class OnboardingEmailHandler(
    IHttpClientFactory httpClientFactory,
    IConfiguration config,
    ILogger<OnboardingEmailHandler> logger) : INotificationHandler<UserRegisteredEvent>
{
    private readonly string _fromEmail = config.GetSection("MailerSend:From:Email").Value ??
        throw new ArgumentException("Failed to get MailerSend sender email from the configurations.");

    private readonly string _fromName = config.GetSection("MailerSend:From:Name").Value ??
        throw new ArgumentException("Failed to get MailerSend sender name from the configurations.");

    private readonly string _templateId = config.GetSection("OnboardingEmail:TemplateId").Value ??
        throw new ArgumentException("Failed to get Onboarding Template ID from the configurations.");

    private readonly string _supportEmail = config.GetSection("OnboardingEmail:SupportEmail").Value ??
        throw new ArgumentException("Failed to get Onboarding Support Email from the configurations.");

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("mailerSend");

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var onboardingEmail = CreateOnboardingEmail(notification);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = _httpClient.BaseAddress,
            Content = JsonContent.Create(onboardingEmail)
        };

        logger.LogInformation("Attempting to send confirmation email to {Email}", notification.Email);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Confirmation email successfully sent to {Email}", notification.Email);
        }
        else
        {
            logger.LogWarning("Failed to send confirmation email to {Email}. Errors: {Errors}",
                        notification.Email,
                        response.Content.ReadAsStringAsync(cancellationToken).Result);
        }
    }

    private OnboardingEmailRequest CreateOnboardingEmail(UserRegisteredEvent notification)
    {
        var onboardingEmail = new OnboardingEmailRequest
        {
            From = new From { Email = _fromEmail, Name = _fromName },
            To = [new() { Email = notification.Email, Name = notification.UserName }],
            Personalization = [new Personalization()
            {
                Email = notification.Email,
                Data = new Data()
                {
                    UserName = notification.UserName,
                    SupportEmail = _supportEmail,
                    EmailConfirmationToken = notification.EmailConfirmationToken
                }
            }],
            TemplateId = _templateId
        };

        return onboardingEmail;
    }
}
