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
    
    public partial class ActivationUserInformation
    {
        public long ActivationUserInformationId { get; set; }
        public string SecondPhoneNumber { get; set; }
        public string FacebookId { get; set; }
        public int GenderId { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public bool IsBlocked { get; set; }
        public string ProfileImage { get; set; }
        public string NationalIdFrontImage { get; set; }
        public string NationalIdBackImage { get; set; }
        public string UserId { get; set; }
    
        public virtual City City { get; set; }
        public virtual Country Country { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual tbl_ClientUsers tbl_ClientUsers { get; set; }
    }
}