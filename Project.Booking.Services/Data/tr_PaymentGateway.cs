//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project.Booking.Services.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class tr_PaymentGateway
    {
        public int ID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public Nullable<int> BankID { get; set; }
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        public Nullable<bool> FlagActive { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<System.Guid> UpdateBy { get; set; }
    
        public virtual tm_Company tm_Company { get; set; }
    }
}
