//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace hospital.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_comm
    {
        public int pkID { get; set; }
        public string Name { get; set; }
        public string valuee { get; set; }
        public string dis { get; set; }
        public int typee { get; set; }
        public int fkLangID { get; set; }
    
        public virtual tbl_Language tbl_Language { get; set; }
    }
}
