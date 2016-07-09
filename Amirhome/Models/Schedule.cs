//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Amirhome.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Schedule
    {
        public Schedule()
        {
            this.ScheduleHistories = new HashSet<ScheduleHistory>();
            this.ScheduleItemSettings = new HashSet<ScheduleItemSetting>();
        }
    
        public int ScheduleID { get; set; }
        public string TypeFullName { get; set; }
        public int TimeLapse { get; set; }
        public string TimeLapseMeasurement { get; set; }
        public int RetryTimeLapse { get; set; }
        public string RetryTimeLapseMeasurement { get; set; }
        public int RetainHistoryNum { get; set; }
        public string AttachToEvent { get; set; }
        public bool CatchUpEnabled { get; set; }
        public bool Enabled { get; set; }
        public string ObjectDependencies { get; set; }
        public string Servers { get; set; }
        public Nullable<int> CreatedByUserID { get; set; }
        public Nullable<System.DateTime> CreatedOnDate { get; set; }
        public Nullable<int> LastModifiedByUserID { get; set; }
        public Nullable<System.DateTime> LastModifiedOnDate { get; set; }
        public string FriendlyName { get; set; }
    
        public virtual ICollection<ScheduleHistory> ScheduleHistories { get; set; }
        public virtual ICollection<ScheduleItemSetting> ScheduleItemSettings { get; set; }
    }
}
