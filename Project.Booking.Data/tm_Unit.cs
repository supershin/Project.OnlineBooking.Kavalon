
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
    
public partial class tm_Unit
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public tm_Unit()
    {

        this.tr_UnitSpecialPromotion = new HashSet<tr_UnitSpecialPromotion>();

        this.ts_Booking = new HashSet<ts_Booking>();

        this.tr_UnitVIP = new HashSet<tr_UnitVIP>();

        this.ts_Unitbooking_History = new HashSet<ts_Unitbooking_History>();

    }


    public System.Guid ID { get; set; }

    public Nullable<System.Guid> ProjectID { get; set; }

    public string UnitCode { get; set; }

    public Nullable<int> BuildID { get; set; }

    public Nullable<int> FloorID { get; set; }

    public Nullable<int> Room { get; set; }

    public Nullable<int> UnitTypeID { get; set; }

    public Nullable<int> ModelTypeID { get; set; }

    public Nullable<decimal> Area { get; set; }

    public Nullable<decimal> AreaIncrease { get; set; }

    public Nullable<decimal> SellingPrice { get; set; }

    public Nullable<decimal> SpecialPrice { get; set; }

    public Nullable<decimal> Discount { get; set; }

    public Nullable<decimal> BookingAmount { get; set; }

    public Nullable<decimal> ContractAmount { get; set; }

    public Nullable<int> UnitStatusID { get; set; }

    public Nullable<System.Guid> AnnotationID { get; set; }

    public Nullable<bool> FlagActive { get; set; }

    public Nullable<System.DateTime> CreateDate { get; set; }

    public Nullable<System.Guid> CreateBy { get; set; }

    public Nullable<System.DateTime> UpdateDate { get; set; }

    public Nullable<System.Guid> UpdateBy { get; set; }

    public Nullable<System.DateTime> UserUpdateDate { get; set; }

    public Nullable<System.Guid> UserUpdateByID { get; set; }



    public virtual tm_Annotation tm_Annotation { get; set; }

    public virtual tm_Build tm_Build { get; set; }

    public virtual tm_Floor tm_Floor { get; set; }

    public virtual tm_ModelType tm_ModelType { get; set; }

    public virtual tm_Project tm_Project { get; set; }

    public virtual tm_UnitStatus tm_UnitStatus { get; set; }

    public virtual tm_UnitType tm_UnitType { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tr_UnitSpecialPromotion> tr_UnitSpecialPromotion { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ts_Booking> ts_Booking { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tr_UnitVIP> tr_UnitVIP { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ts_Unitbooking_History> ts_Unitbooking_History { get; set; }

}

}
