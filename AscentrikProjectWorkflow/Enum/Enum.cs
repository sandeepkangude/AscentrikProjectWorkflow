using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow
{
    public class Enum
    {
        public enum ProjectStatus
        {
            New = 1,
            ResearchOngoing = 2,
            OnHold = 3,
            Query = 4,
            ApprovalPending = 5,
            ReseachHalted = 6,
            LinkedInApprovalPending = 7,
            PartlyDeliveredAndResearchHalted = 8,
            PartlyDeliveredAndInvoiceRaised = 9,
            InvoiceRaised = 10
        };

        public enum PaymentStatus
        {
            Pending = 1,
            Recieved = 2
        }
    }
}