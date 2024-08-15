namespace BiteAlert.Modules.Shared;

public class FluentValidationError
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
