using AscentrikProjectWorkflow.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.ViewModel
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ListId { get; set; }
        [Required]
        public int Priority { get; set; }
        [Required]
        public string ProjectCode { get; set; }
        public string ProjectBrief { get; set; }
        public string LinkdinLink { get; set; }
        public string Requester { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string ApprovalDateText { get; set; }
        public Nullable<bool> DatesInformedToClient { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid value. eg. 0, 10, 100 etc. ")]
        public Nullable<int> NumberOfCompaniesApproved { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid value. eg. 0, 10, 100 etc. ")]
        public Nullable<int> JobTitlesPerCompany { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid value. eg. 0, 10, 100 etc. ")]
        public Nullable<int> RecordsToBeResearched { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid value. eg. 0, 10, 100 etc. ")]
        public Nullable<int> DuplicateFounds { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid value. eg. 0, 10, 100 etc. ")]
        public Nullable<int> NoOfRecordsDeliverd { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public DateTime? FTPUploadDate { get; set; }
        public string ExpectedDeliveryDateText { get; set; }
        public string ActualDeliveryDateText { get; set; }
        public string FTPUploadDateText { get; set; }
        public string FTPUploadRemarks { get; set; }
        [Required]
        public int ProjectStatus { get; set; }
        public string AllotedTo { get; set; }
        public string Names { get; set; }
        public string ResearchRemark { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> EditedBy { get; set; }
        public Nullable<System.DateTime> EditedOn { get; set; }
        public bool IsActive { get; set; }

        public Dictionary<int, string> PriorityList { get; set; }
        public Dictionary<int, string> StatusList { get; set; }
        public int ClientId { get; set; }
        public Dictionary<int, string> ClientList { get; set; }
        public Dictionary<int, string> ListReferenceList { get; set; }
        public Dictionary<bool, string> Choice { get; set; }
        public Dictionary<string, string> AllotedToList { get; set; }
        
        public virtual List List { get; set; }
        public virtual Priority Priority1 { get; set; }
        public virtual ProjectStatu ProjectStatu { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; }
        public virtual ICollection<ProjectCosting> ProjectCostings { get; set; }
    }
}