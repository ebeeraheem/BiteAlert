﻿// Ignore Spelling: Onboarding

using BiteAlert.Modules.NotificationModule;
using MediatR;
using System.Web;

namespace BiteAlert.Modules.EmailModule.Onboarding;

public class OnboardingEmailHandler(
    IHttpClientFactory httpClientFactory,
    IEmailConfigurationService emailConfig,
    IConfiguration config,
    ILogger<OnboardingEmailHandler> logger) : INotificationHandler<UserRegisteredEvent>
{
    private readonly string _templateId = config.GetSection("OnboardingEmail:TemplateId").Value ??
        throw new ArgumentException("Failed to get Onboarding Template ID from the configurations.");

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

    private EmailTemplate CreateOnboardingEmail(UserRegisteredEvent notification)
    {
        var onboardingEmail = new EmailTemplate
        {
            From = new From { Email = emailConfig.FromEmail, Name = emailConfig.FromName },
            To = [new() { Email = notification.Email, Name = notification.UserName }],
            Personalization = [new Personalization()
            {
                Email = notification.Email,
                Data = new OnboardingEmailData()
                {
                    UserName = notification.UserName,
                    SupportEmail = emailConfig.SupportEmail,
                    EmailConfirmationLink = GenerateEmailConfirmationLink(notification.UserId,
                                                                          notification.EmailConfirmationToken),
                }
            }],
            TemplateId = _templateId
        };

        return onboardingEmail;
    }

    private string GenerateEmailConfirmationLink(Guid userId, string token)
    {
        var encodedToken = HttpUtility.UrlEncode(token);

        return $"{emailConfig.BaseUrl}/api/v1/auth/verify-email?userId={userId}&token={encodedToken}";
    }
}
