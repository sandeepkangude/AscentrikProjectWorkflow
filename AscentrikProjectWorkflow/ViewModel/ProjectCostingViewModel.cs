using AscentrikProjectWorkflow.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.ViewModel
{
    public class ProjectCostingViewModel
    {
        public int Id { get; set; }
        public int ProjectCostingId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid value. eg. 0, 10.10, 100 etc. ")]
        public Nullable<decimal> CostPerRecord { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid value. eg. 0, 10.10, 100 etc. ")]
        public Nullable<decimal> ProjectInvoice { get; set; }
        public Nullable<int> Currency { get; set; }
        public string InvoiceCode { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceDateText { get; set; }
        public Nullable<int> PaymentStatus { get; set; }
        public string InvoiceRemark { get; set; }
        public Nullable<int> EditedBy { get; set; }
        public Nullable<System.DateTime> EditedOn { get; set; }
        public Dictionary<int, string> CurrencyList { get; set; }
        public Dictionary<int, string> PaymentStatusList { get; set; }

        public virtual Currency Currency1 { get; set; }
        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}