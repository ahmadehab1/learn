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
    
    public partial class UserIntrest
    {
        public int UserIntrestsId { get; set; }
        public string UserId { get; set; }
        public int IntrestId { get; set; }
    
        public virtual Intrest Intrest { get; set; }
        public virtual tbl_ClientUsers tbl_ClientUsers { get; set; }
    }
}
