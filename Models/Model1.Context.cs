﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class datap : DbContext
    {
        public datap()
            : base("name=datap")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tbl_Awards> tbl_Awards { get; set; }
        public virtual DbSet<tbl_Doctors> tbl_Doctors { get; set; }
        public virtual DbSet<tbl_Patient> tbl_Patient { get; set; }
        public virtual DbSet<tbl_Research> tbl_Research { get; set; }
        public virtual DbSet<tbl_skills> tbl_skills { get; set; }
        public virtual DbSet<tbl_Visit> tbl_Visit { get; set; }
        public virtual DbSet<tbl_VisitPerDoctors> tbl_VisitPerDoctors { get; set; }
        public virtual DbSet<tbl_VisitStatus> tbl_VisitStatus { get; set; }
        public virtual DbSet<tbl_VisitType> tbl_VisitType { get; set; }
        public virtual DbSet<View_Visit> View_Visit { get; set; }
        public virtual DbSet<View_VisitPerDoctors> View_VisitPerDoctors { get; set; }
        public virtual DbSet<tbl_comm> tbl_comm { get; set; }
        public virtual DbSet<tbl_Language> tbl_Language { get; set; }
        public virtual DbSet<View_Comm> View_Comm { get; set; }
    }
}
