using BiteAlert.Modules.EmailModule.Onboarding;

namespace BiteAlert.Modules.EmailModule;

public class Personalization
{
    public string Email { get; set; } = string.Empty;
    public object Data { get; set; } = null!;
}
