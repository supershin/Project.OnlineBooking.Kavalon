﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WisePayEntities : DbContext
    {
        public WisePayEntities()
            : base("name=WisePayEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ts_OrderChagre> ts_OrderChagre { get; set; }
        public virtual DbSet<ts_OrderPayment> ts_OrderPayment { get; set; }
        public virtual DbSet<ts_Order> ts_Order { get; set; }
        public virtual DbSet<ts_OrderPaymentMethod> ts_OrderPaymentMethod { get; set; }
        public virtual DbSet<tm_Company> tm_Company { get; set; }
        public virtual DbSet<tm_Project> tm_Project { get; set; }
        public virtual DbSet<tr_PaymentGateway> tr_PaymentGateway { get; set; }
    }
}
