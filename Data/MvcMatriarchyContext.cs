using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Matriarchy.Models;

namespace Matriarchy.Data
{
    public class MvcMatriarchyContext : DbContext
    {
        public MvcMatriarchyContext(DbContextOptions<MvcMatriarchyContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<CompanyPlans> Company_Plans { get; set; }
        public DbSet<CountyPlans> County_Plans { get; set; }
        public DbSet<PlanDetail> PlanDetails { get; set; }
        public DbSet<Issue> Issues { get; set; }

        public DbSet<ReportServiceRecommendations> ReportServiceRecommendations { get; set; }

        public DbSet<ReportUserFavorites> ReportFavoriteMovies { get; set; }

        public DbSet<ReportFailedLoginAttempts> ReportFailedLoginAttempts { get; set; }

        public DbSet<ReportServiceOfferingCount> ReportServiceOfferingCount { get; set; }

        public DbSet<GetAllIssues> GetAllIssues { get; set; }
    }
}

