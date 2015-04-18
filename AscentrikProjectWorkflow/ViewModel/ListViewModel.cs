using AscentrikProjectWorkflow.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.ViewModel
{
    public class ListViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name="List Code")]
        public string Code { get; set; }
        [Required]
        [Display(Name = "List Reference")]
        public string Reference { get; set; }
        [Required]
        [Display(Name = "List Type")]
        public int Type { get; set; }
        public Dictionary<int, string> ListTypes { get; set; }
        [Required]
        [Display(Name = "Client Name")]
        public int ClientId { get; set; }
        public Dictionary<int, string> Clients { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> EditedBy { get; set; }
        public Nullable<System.DateTime> EditedOn { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public virtual Client Client { get; set; }
        public virtual ListType ListType { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}