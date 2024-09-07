// Ignore Spelling: Upsert

using BiteAlert.Modules.Shared;
using System.Text.Json.Serialization;

namespace BiteAlert.Modules.VendorModule;

public class UpsertVendorResponse : BaseResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<FluentValidationError>? FluentValidationErrors { get; set; }
}
