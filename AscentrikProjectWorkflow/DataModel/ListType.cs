//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AscentrikProjectWorkflow.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class ListType
    {
        public ListType()
        {
            this.Lists = new HashSet<List>();
        }
    
        public int Id { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> EditedBy { get; set; }
        public Nullable<System.DateTime> EditedOn { get; set; }
    
        public virtual ICollection<List> Lists { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
