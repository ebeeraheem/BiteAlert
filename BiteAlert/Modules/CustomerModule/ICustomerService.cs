
namespace BiteAlert.Modules.CustomerModule;

public interface ICustomerService
{
    Task<UpsertCustomerResponse> RegisterCustomerAsync(string userId);
}