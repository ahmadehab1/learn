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
    
    public partial class OrderCoupon
    {
        public long OrderCouponsId { get; set; }
        public long CouponId { get; set; }
        public long OrderId { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual Coupon Coupon { get; set; }
    }
}
