using AscentrikProjectWorkflow.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.ViewModel
{
    public class ListTypeViewModel
    {
        public int Id { get; set; }
        [Display(Name = "List Type")]
        [Required]
        public string Type { get; set; }
        [Display(Name = "Is Active")]
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