
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
    
public partial class tr_UnitVIP
{

    public int ID { get; set; }

    public Nullable<System.Guid> UnitID { get; set; }

    public string BookName { get; set; }

    public Nullable<bool> FlagActive { get; set; }

    public Nullable<System.DateTime> CreateDate { get; set; }

    public Nullable<System.Guid> CreateBy { get; set; }

    public Nullable<System.DateTime> UpdateDate { get; set; }

    public Nullable<System.Guid> UpdateBy { get; set; }



    public virtual tm_Unit tm_Unit { get; set; }

}

}
