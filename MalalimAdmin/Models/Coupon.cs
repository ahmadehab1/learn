//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MalalimAdmin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Coupon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Coupon()
        {
            this.OrderCoupons = new HashSet<OrderCoupon>();
        }
    
        public long CouponId { get; set; }
        public string Code { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public decimal Price { get; set; }
        public bool IsWin { get; set; }
        public bool IsDrawed { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> LockedTill { get; set; }
        public string DrawedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderCoupon> OrderCoupons { get; set; }
        public virtual Product Product { get; set; }
    }
}
