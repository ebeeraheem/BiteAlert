// Ignore Spelling: Upsert

using BiteAlert.Modules.Shared;
using System.Text.Json.Serialization;

namespace BiteAlert.Modules.ProductModule;

public class UpsertProductResponse : BaseResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<FluentValidationError>? FluentValidationErrors { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Product? Product { get; set; }
}
