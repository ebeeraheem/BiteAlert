using BiteAlert.Modules.NotificationModule;
using MediatR;
using System.Web;

namespace BiteAlert.Modules.EmailModule.PasswordReset;

public class PasswordResetEmailHandler(
    IHttpClientFactory httpClientFactory,
    IEmailConfigurationService emailConfig,
    IConfiguration config,
    ILogger<PasswordResetEmailHandler> logger) : INotificationHandler<PasswordResetEvent>
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("mailerSend");
    private readonly string _templateId = config.GetSection("PasswordReset:TemplateId").Value ??
        throw new ArgumentException("Failed to get Password Reset Template ID from the configurations.");

    public async Task Handle(PasswordResetEvent notification, CancellationToken cancellationToken)
    {
        var passwordResetEmail = CreatePasswordResetEmail(notification);

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = _httpClient.BaseAddress,
            Content = JsonContent.Create(passwordResetEmail)
        };

        logger.LogInformation("Attempting to send password reset email to {Email}", notification.Email);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Password reset email successfully sent to {Email}", notification.Email);
        }
        else
        {
            logger.LogWarning("Failed to send password reset email to {Email}. Errors: {Errors}",
                        notification.Email,
                        response.Content.ReadAsStringAsync(cancellationToken).Result);
        }
    }

    private EmailTemplate CreatePasswordResetEmail(PasswordResetEvent notification)
    {
        var passwordResetEmail = new EmailTemplate()
        {
            From = new From { Email = emailConfig.FromEmail, Name = emailConfig.FromName },
            To = [new To { Email = notification.Email, Name = notification.UserName }],
            Personalization = [new Personalization()
            {
                Email = notification.Email,
                Data = new PasswordResetData()
                {
                    UserName = notification.UserName,
                    SupportEmail = emailConfig.SupportEmail,
                    PasswordResetLink = GeneratePasswordResetLink(notification.UserId,
                                                                  notification.PasswordResetToken)
                }
            }],
            TemplateId = _templateId
        };

        return passwordResetEmail;
    }

    private string GeneratePasswordResetLink(Guid userId, string token)
    {
        var encodedToken = HttpUtility.UrlEncode(token);

        return $"{emailConfig.BaseUrl}/api/v1/auth/reset-password?userId={userId}&token={encodedToken}";
    }
}
