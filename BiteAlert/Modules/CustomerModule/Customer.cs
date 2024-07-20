using BiteAlert.Modules.Authentication;
using BiteAlert.Modules.LikeModule;
using BiteAlert.Modules.NotificationModule;
using BiteAlert.Modules.ReviewModule;
using BiteAlert.Modules.VendorModule;

namespace BiteAlert.Modules.CustomerModule;

public class Customer : ApplicationUser
{
    public ICollection<Vendor> FollowedVendors { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<Like> Likes { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
