﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Web_IT_HELPDESK
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ServiceDeskEntities : DbContext
    {
        public ServiceDeskEntities()
            : base("name=ServiceDeskEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<AlbumType> AlbumTypes { get; set; }
        public virtual DbSet<BIZ_TRIP> BIZ_TRIP { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CONTRACT> CONTRACTs { get; set; }
        public virtual DbSet<CONTRACT_SUB> CONTRACT_SUB { get; set; }
        public virtual DbSet<CONTRACT_TYPE> CONTRACT_TYPE { get; set; }
        public virtual DbSet<CONTRACT_TYPE_DETAIL> CONTRACT_TYPE_DETAIL { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Device_Type> Device_Type { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<DocumentType> DocumentTypes { get; set; }
        public virtual DbSet<Drinking_Request> Drinking_Request { get; set; }
        public virtual DbSet<EMP_ANSWER> EMP_ANSWER { get; set; }
        public virtual DbSet<EmployeeInfo> EmployeeInfoes { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<hr_emp_infor> hr_emp_infor { get; set; }
        public virtual DbSet<hr_emp_meal> hr_emp_meal { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<Order_> Order_ { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<PERIOD> PERIODs { get; set; }
        public virtual DbSet<QUESTION> QUESTIONs { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Screen> Screens { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<UserLogon> UserLogons { get; set; }
        public virtual DbSet<Authorization> Authorizations { get; set; }
        public virtual DbSet<Plant> Plants { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Rule> Rules { get; set; }
        public virtual DbSet<Allocation> Allocations { get; set; }
        public virtual DbSet<Incident> Incidents { get; set; }
        public virtual DbSet<Seal_Using> Seal_Using { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Mail> Mails { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
    
        [DbFunction("ServiceDeskEntities", "func_report_detail")]
        public virtual IQueryable<func_report_detail_Result> func_report_detail(Nullable<System.DateTime> v_fromdate, Nullable<System.DateTime> v_todate, string v_plant)
        {
            var v_fromdateParameter = v_fromdate.HasValue ?
                new ObjectParameter("v_fromdate", v_fromdate) :
                new ObjectParameter("v_fromdate", typeof(System.DateTime));
    
            var v_todateParameter = v_todate.HasValue ?
                new ObjectParameter("v_todate", v_todate) :
                new ObjectParameter("v_todate", typeof(System.DateTime));
    
            var v_plantParameter = v_plant != null ?
                new ObjectParameter("v_plant", v_plant) :
                new ObjectParameter("v_plant", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<func_report_detail_Result>("[ServiceDeskEntities].[func_report_detail](@v_fromdate, @v_todate, @v_plant)", v_fromdateParameter, v_todateParameter, v_plantParameter);
        }
    
        [DbFunction("ServiceDeskEntities", "GetLastAllocationOfDevice")]
        public virtual IQueryable<Allocation> GetLastAllocationOfDevice()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<Allocation>("[ServiceDeskEntities].[GetLastAllocationOfDevice]()");
        }
    }
}
