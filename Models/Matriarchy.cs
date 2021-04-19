using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Matriarchy.Models
{

    public class AccessLevel
    {
        public int AccessLevelID { get; set; }
        public string AccessLevelDescription { get; set; }
    }



    public class IssueType
    {
        public long ID { get; set; }
        public string Description { get; set; }
    }



    public class UsersShowsMoviesFavorite
    {
        public long ID { get; set; }
        public long UserCD { get; set; }
        public long ShowMovieCD { get; set; }
        public int Rating { get; set; }
    }

    public class Company
    {
        public int ID { get; set; }
        public string CompanyName { get; set; }

    }
    public class County
    {
        public int ID { get; set; }
        public string CountyName { get; set; }

    }

    public class CountyPlans
    {
        public int ID { get; set; }
        public string CompanyName { get; set; }
        public string PlanName { get; set; }
        public int CompanyID { get; set; }
        public int CountyID { get; set; }
    }
    public class CompanyPlans
    {
        public int ID { get; set; }
        public int CompanyCD { get; set; }
        public string PlanName { get; set; }
        public decimal MonthlyPremium { get; set; }
        public decimal Copay { get; set; }
        public decimal Deductible { get; set; }
        public Boolean EmergencyCare { get; set; }
        public Boolean PreventativeCare { get; set; }
        public Boolean PreexistingCondition { get; set; }
        public Boolean Prescriptions { get; set; }
        public Boolean Vision { get; set; }
        public Boolean Dental { get; set; }
    }

    public class PlanDetail
    {
        public int ID { get; set; }
        public int CompanyCD { get; set; }
        public string PlanName { get; set; }
        public decimal MonthlyPremium { get; set; }
        public decimal Copay { get; set; }
        public decimal Deductible { get; set; }
        public Boolean EmergencyCare { get; set; }
        public Boolean PreventativeCare { get; set; }
        public Boolean PreexistingCondition { get; set; }
        public Boolean Prescriptions { get; set; }
        public Boolean Vision { get; set; }
        public Boolean Dental { get; set; }
        public string CompanyName { get; set; }
    }
}