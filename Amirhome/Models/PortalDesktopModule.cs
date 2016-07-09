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
    
    public partial class PortalDesktopModule
    {
        public PortalDesktopModule()
        {
            this.DesktopModulePermissions = new HashSet<DesktopModulePermission>();
        }
    
        public int PortalDesktopModuleID { get; set; }
        public int PortalID { get; set; }
        public int DesktopModuleID { get; set; }
        public Nullable<int> CreatedByUserID { get; set; }
        public Nullable<System.DateTime> CreatedOnDate { get; set; }
        public Nullable<int> LastModifiedByUserID { get; set; }
        public Nullable<System.DateTime> LastModifiedOnDate { get; set; }
    
        public virtual ICollection<DesktopModulePermission> DesktopModulePermissions { get; set; }
        public virtual DesktopModule DesktopModule { get; set; }
        public virtual Portal Portal { get; set; }
    }
}
