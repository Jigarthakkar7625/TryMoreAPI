//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TryMoreAPI
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblSMSPlan
    {
        public int SMSPlanID { get; set; }
        public string PlanName { get; set; }
        public long NoOfSMS { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public int IsActive { get; set; }
    }
}
