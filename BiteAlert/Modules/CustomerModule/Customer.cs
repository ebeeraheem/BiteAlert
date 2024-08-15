using BiteAlert.Modules.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BiteAlert.Modules.CustomerModule;

public class Customer
{
    [ForeignKey(nameof(User))]
    public Guid Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual ApplicationUser User { get; set; } = null!;
    //public ICollection<Vendor>? FollowedVendors { get; set; }
    //public ICollection<Review>? Reviews { get; set; }
}
