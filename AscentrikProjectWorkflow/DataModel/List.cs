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
    
    public partial class List
    {
        public List()
        {
            this.Projects = new HashSet<Project>();
        }
    
        public int Id { get; set; }
        public string Code { get; set; }
        public string Reference { get; set; }
        public int Type { get; set; }
        public int ClientId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> EditedBy { get; set; }
        public Nullable<System.DateTime> EditedOn { get; set; }
        public bool IsActive { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual ListType ListType { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
