using BiteAlert.Modules.Shared;

namespace BiteAlert.Modules.CustomerModule;

public interface ICustomerService
{
    Task<BaseResponse> RegisterCustomerAsync(string userId);
    Task<BaseResponse> GetCustomerById(string userId);
}