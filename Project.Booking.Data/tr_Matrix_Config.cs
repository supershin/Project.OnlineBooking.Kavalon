
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
    
public partial class tr_Matrix_Config
{

    public int ID { get; set; }

    public Nullable<System.Guid> ProjectID { get; set; }

    public Nullable<int> BuildID { get; set; }

    public string Text { get; set; }

    public Nullable<int> RowNo { get; set; }

    public Nullable<int> ColSpan { get; set; }

    public Nullable<int> LineOrder { get; set; }

    public string Style { get; set; }

    public Nullable<bool> FlagActive { get; set; }

    public Nullable<System.DateTime> CreateDate { get; set; }

    public Nullable<System.Guid> CreateBy { get; set; }



    public virtual tm_Build tm_Build { get; set; }

    public virtual tm_Project tm_Project { get; set; }

}

}
