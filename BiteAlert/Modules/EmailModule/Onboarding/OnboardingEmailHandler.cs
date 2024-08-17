// Ignore Spelling: Onboarding

using BiteAlert.Modules.NotificationModule;

namespace BiteAlert.Modules.EmailModule.Onboarding;

public class OnboardingEmailHandler(MailerSendService mailerSendService,
                                    IConfiguration config,
                                    ILogger<OnboardingEmailHandler> logger) : IEventSubscriber<UserRegisteredEvent>
{
    private readonly string _fromEmail = config.GetSection("MailerSend:From:Email").Value ??
        throw new ArgumentException("Failed to get MailerSend sender email from the configurations.");

    private readonly string _fromName = config.GetSection("MailerSend:From:Name").Value ??
        throw new ArgumentException("Failed to get MailerSend sender name from the configurations.");

    private readonly string _templateId = config.GetSection("OnboardingEmail:TemplateId").Value ??
        throw new ArgumentException("Failed to get Onboarding Template ID from the configurations.");

    private readonly string _supportEmail = config.GetSection("OnboardingEmail:SupportEmail").Value ??
        throw new ArgumentException("Failed to get Onboarding Support Email from the configurations.");

    public async void Handle(UserRegisteredEvent eventToHandle)
    {
        var onboardingEmail = new OnboardingEmailRequest
        {
            From = new From { Email = _fromEmail, Name = _fromName },
            To = [new() { Email = eventToHandle.Email, Name = eventToHandle.UserName }],
            Personalisation = new Personalisation()
            {
                Email = eventToHandle.Email,
                Data = new Data()
                {
                    UserName = eventToHandle.UserName,
                    SupportEmail = _supportEmail
                }
            },
            TemplateId = _templateId
        };

        logger.LogInformation("Attempting to send confirmation email to {Email}", eventToHandle.Email);

        var response = await mailerSendService.SendEmailAsync(onboardingEmail);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Confirmation email successfully sent to {Email}", eventToHandle.Email);
        }
        else
        {
            logger.LogWarning("Failed to send confirmation email to {Email}. Error: {@Error}",
                        eventToHandle.Email,
                        response.Content);
        }
    }
}
