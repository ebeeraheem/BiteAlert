namespace BiteAlert.Modules.EmailModule;

public class MailerSendService(IHttpClientFactory httpClientFactory,
                               ILogger<MailerSendService> logger)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("mailerSend");

    public async Task<HttpResponseMessage> SendEmailAsync(IEmailRequestBase emailRequest)
    {
		try
		{
            logger.LogInformation("Attempting to send email to {@Email}", emailRequest.To);
            return await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress, emailRequest);
        }
		catch (Exception ex)
		{
            logger.LogError(ex, "An unexpected error occurred while sending email to {@Email}",
                        emailRequest.To);
			throw;
		}
    }
}
