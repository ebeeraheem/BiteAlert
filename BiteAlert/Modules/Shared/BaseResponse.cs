namespace BiteAlert.Modules.Shared;

public class BaseResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
}
