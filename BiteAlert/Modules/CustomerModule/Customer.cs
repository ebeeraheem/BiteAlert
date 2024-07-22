using BiteAlert.Modules.Authentication;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteAlert.Modules.CustomerModule;

public class Customer
{
    [ForeignKey(nameof(User))]
    public Guid Id { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    //public ICollection<Vendor>? FollowedVendors { get; set; }
    //public ICollection<Review>? Reviews { get; set; }
}
