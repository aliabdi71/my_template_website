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
    
    public partial class CoreMessaging_NotificationTypeActions
    {
        public int NotificationTypeActionID { get; set; }
        public int NotificationTypeID { get; set; }
        public string NameResourceKey { get; set; }
        public string DescriptionResourceKey { get; set; }
        public string ConfirmResourceKey { get; set; }
        public int Order { get; set; }
        public string APICall { get; set; }
        public Nullable<int> CreatedByUserID { get; set; }
        public Nullable<System.DateTime> CreatedOnDate { get; set; }
        public Nullable<int> LastModifiedByUserID { get; set; }
        public Nullable<System.DateTime> LastModifiedOnDate { get; set; }
    
        public virtual CoreMessaging_NotificationTypes CoreMessaging_NotificationTypes { get; set; }
    }
}
