
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Project.Booking.Data
{

using System;
    using System.Collections.Generic;
    
public partial class ts_Payment
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public ts_Payment()
    {

        this.ts_Payment_Credit = new HashSet<ts_Payment_Credit>();

    }


    public System.Guid ID { get; set; }

    public Nullable<System.Guid> BookingID { get; set; }

    public Nullable<int> PaymentTypeID { get; set; }

    public string PaymentNo { get; set; }

    public Nullable<System.DateTime> PaymentDate { get; set; }

    public Nullable<bool> FlagActive { get; set; }

    public Nullable<System.DateTime> CreateDate { get; set; }

    public Nullable<System.Guid> CreateBy { get; set; }

    public Nullable<System.DateTime> UpdateDate { get; set; }

    public Nullable<System.Guid> UpdateBy { get; set; }



    public virtual tm_Ext tm_Ext { get; set; }

    public virtual ts_Booking ts_Booking { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ts_Payment_Credit> ts_Payment_Credit { get; set; }

}

}
