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
    
    public partial class PackageDependency
    {
        public int PackageDependencyID { get; set; }
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public string Version { get; set; }
    
        public virtual Package Package { get; set; }
    }
}
