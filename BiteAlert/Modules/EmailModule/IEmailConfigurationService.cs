namespace BiteAlert.Modules.EmailModule;

public interface IEmailConfigurationService
{
    string FromEmail { get; }
    string FromName { get; }
    string SupportEmail { get; }
    string BaseUrl { get; }
}
