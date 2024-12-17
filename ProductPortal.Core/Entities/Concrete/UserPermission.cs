using ProductPortal.Core.Entities.Aggregates;
using ProductPortal.Core.Enums;

namespace ProductPortal.Core.Entities.Concrete
{
    public class UserPermission:BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int PermissionId { get; set; }
    }
}