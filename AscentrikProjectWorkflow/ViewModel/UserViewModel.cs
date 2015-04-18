using AscentrikProjectWorkflow.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.ViewModel
{
    public class UserViewModel
    {

        public int Id { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        [Required]
        public int Role { get; set; }
        public Dictionary<int, string> Roles { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<Client> Clients1 { get; set; }
        public virtual ICollection<List> Lists { get; set; }
        public virtual ICollection<List> Lists1 { get; set; }
        public virtual ICollection<ListType> ListTypes { get; set; }
        public virtual ICollection<ListType> ListTypes1 { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Project> Projects1 { get; set; }
        public virtual ICollection<ProjectCosting> ProjectCostings { get; set; }
        public virtual ICollection<ProjectCosting> ProjectCostings1 { get; set; }
        public virtual Role Role1 { get; set; }
    }
}