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
    
    public partial class EventLog
    {
        public string LogGUID { get; set; }
        public string LogTypeKey { get; set; }
        public Nullable<int> LogConfigID { get; set; }
        public Nullable<int> LogUserID { get; set; }
        public string LogUserName { get; set; }
        public Nullable<int> LogPortalID { get; set; }
        public string LogPortalName { get; set; }
        public System.DateTime LogCreateDate { get; set; }
        public string LogServerName { get; set; }
        public string LogProperties { get; set; }
        public Nullable<bool> LogNotificationPending { get; set; }
        public long LogEventID { get; set; }
    
        public virtual EventLogConfig EventLogConfig { get; set; }
        public virtual EventLogType EventLogType { get; set; }
    }
}
