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
    
    public partial class Classification
    {
        public Classification()
        {
            this.Classification1 = new HashSet<Classification>();
            this.VendorClassifications = new HashSet<VendorClassification>();
        }
    
        public int ClassificationId { get; set; }
        public string ClassificationName { get; set; }
        public Nullable<int> ParentId { get; set; }
    
        public virtual ICollection<Classification> Classification1 { get; set; }
        public virtual Classification Classification2 { get; set; }
        public virtual ICollection<VendorClassification> VendorClassifications { get; set; }
    }
}
