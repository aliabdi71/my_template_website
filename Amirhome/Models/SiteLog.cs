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
    
    public partial class SiteLog
    {
        public int SiteLogId { get; set; }
        public System.DateTime DateTime { get; set; }
        public int PortalId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Referrer { get; set; }
        public string Url { get; set; }
        public string UserAgent { get; set; }
        public string UserHostAddress { get; set; }
        public string UserHostName { get; set; }
        public Nullable<int> TabId { get; set; }
        public Nullable<int> AffiliateId { get; set; }
    
        public virtual Portal Portal { get; set; }
    }
}
