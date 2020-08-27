﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Web_IT_HELPDESK
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Core.Objects.DataClasses;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    
    public partial class Web_IT_HELPDESKEntities : DbContext
    {
        public Web_IT_HELPDESKEntities()
            : base("name=Web_IT_HELPDESKEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Album> Albums { get; set; }
        public DbSet<AlbumType> AlbumTypes { get; set; }
        public DbSet<BIZ_TRIP> BIZ_TRIP { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CONTRACT_TYPE> CONTRACT_TYPE { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Drinking_Request> Drinking_Request { get; set; }
        public DbSet<EMP_ANSWER> EMP_ANSWER { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Employee_Screen> Employee_Screen { get; set; }
        public DbSet<EmployeeInfo> EmployeeInfoes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<hr_emp_infor> hr_emp_infor { get; set; }
        public DbSet<hr_emp_meal> hr_emp_meal { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Order_> Order_ { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PERIOD> PERIODs { get; set; }
        public DbSet<QUESTION> QUESTIONs { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Seal_Using> Seal_Using { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<UserLogon> UserLogons { get; set; }
        public DbSet<ORDER_TYPE_VIEW> ORDER_TYPE_VIEW { get; set; }
        public DbSet<CONTRACT> CONTRACTs { get; set; }
        public DbSet<CONTRACT_SUB> CONTRACT_SUB { get; set; }
        public DbSet<CONTRACT_TYPE_DETAIL> CONTRACT_TYPE_DETAIL { get; set; }
        public DbSet<EMP_ANSWER_bk20190826> EMP_ANSWER_bk20190826 { get; set; }
        public DbSet<employee_bk20190826> employee_bk20190826 { get; set; }
        public DbSet<EmployeeInfo_bk20190826> EmployeeInfo_bk20190826 { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
    
        [EdmFunction("Web_IT_HELPDESKEntities", "FUNC_ORDER")]
        public virtual IQueryable<FUNC_ORDER_Result> FUNC_ORDER(Nullable<System.DateTime> v_fromdate, Nullable<System.DateTime> v_todate)
        {
            var v_fromdateParameter = v_fromdate.HasValue ?
                new ObjectParameter("v_fromdate", v_fromdate) :
                new ObjectParameter("v_fromdate", typeof(System.DateTime));
    
            var v_todateParameter = v_todate.HasValue ?
                new ObjectParameter("v_todate", v_todate) :
                new ObjectParameter("v_todate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<FUNC_ORDER_Result>("[Web_IT_HELPDESKEntities].[FUNC_ORDER](@v_fromdate, @v_todate)", v_fromdateParameter, v_todateParameter);
        }
    
        public virtual int PRO_INSERT_HR_MEAL(string v_EMPNO, byte[] v_IMAGE, string v_Note)
        {
            var v_EMPNOParameter = v_EMPNO != null ?
                new ObjectParameter("V_EMPNO", v_EMPNO) :
                new ObjectParameter("V_EMPNO", typeof(string));
    
            var v_IMAGEParameter = v_IMAGE != null ?
                new ObjectParameter("V_IMAGE", v_IMAGE) :
                new ObjectParameter("V_IMAGE", typeof(byte[]));
    
            var v_NoteParameter = v_Note != null ?
                new ObjectParameter("V_Note", v_Note) :
                new ObjectParameter("V_Note", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PRO_INSERT_HR_MEAL", v_EMPNOParameter, v_IMAGEParameter, v_NoteParameter);
        }
    
        [EdmFunction("Web_IT_HELPDESKEntities", "func_report_detail")]
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
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<func_report_detail_Result>("[Web_IT_HELPDESKEntities].[func_report_detail](@v_fromdate, @v_todate, @v_plant)", v_fromdateParameter, v_todateParameter, v_plantParameter);
        }
    }
}
