namespace BiteAlert.Modules.EmailModule;

public interface IEmailRequestBase
{
    From From { get; set; }
    List<To> To { get; set; }
}
