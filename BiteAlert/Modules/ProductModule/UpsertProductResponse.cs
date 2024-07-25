// Ignore Spelling: Upsert

using System.Text.Json.Serialization;

namespace BiteAlert.Modules.ProductModule;

public class UpsertProductResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Product? Product { get; set; }
}
