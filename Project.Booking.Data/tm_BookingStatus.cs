
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
    
public partial class tm_BookingStatus
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public tm_BookingStatus()
    {

        this.ts_Booking = new HashSet<ts_Booking>();

        this.ts_Unitbooking_History = new HashSet<ts_Unitbooking_History>();

    }


    public int ID { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }

    public Nullable<int> LineOrder { get; set; }

    public Nullable<bool> FlagActive { get; set; }

    public Nullable<System.DateTime> CreateDate { get; set; }

    public Nullable<System.Guid> CreateBy { get; set; }

    public Nullable<System.DateTime> UpdateDate { get; set; }

    public Nullable<System.Guid> UpdateBy { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ts_Booking> ts_Booking { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ts_Unitbooking_History> ts_Unitbooking_History { get; set; }

}

}
