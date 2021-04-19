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
        public DbSet<Service> Services { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Network> Networks { get; set; }
        public DbSet<Serie> Series { get; set; }
        public DbSet<SerieServiceSeason> SerieServiceSeasons { get; set; }

        public DbSet<SerieRating> GetSerieFavorites { get; set; }
        public DbSet<MovieRating> GetMovieFavorites { get; set; }
        public DbSet<NetworkRating> GetNetworkFavorites { get; set; }

        public DbSet<ServiceSerie> GetServiceSeries { get; set; }
        public DbSet<ServiceMovie> GetServiceMovies { get; set; }
        public DbSet<ServiceNetwork> GetServiceNetworks { get; set; }

        public DbSet<ServiceMoviesRightJoinMovies> ServiceMoviesRightJoinMovies { get; set; }
                                                   
        public DbSet<ServiceNetworksRightJoinNetworks> ServiceNetworksRightJoinNetworks { get; set; }

         public DbSet<ServiceSeriesRightJoinSeries> ServiceSeriesRightJoinSeries { get; set; }


        public DbSet<ServiceMoviesRightJoinServices> ServiceMoviesRightJoinServices { get; set; }

        public DbSet<ServiceNetworksRightJoinServices> ServiceNetworksRightJoinServices { get; set; }

        public DbSet<ServiceSeriesRightJoinServices> ServiceSeriesRightJoinServices { get; set; }

        public DbSet<CompareServices> CompareServices { get; set; }

        public DbSet<ReportServiceRecommendations> ReportServiceRecommendations { get; set; }

        public DbSet<ReportUserFavorites> ReportFavoriteMovies { get; set; }

        public DbSet<ReportFailedLoginAttempts> ReportFailedLoginAttempts { get; set; }

        public DbSet<ReportServiceOfferingCount> ReportServiceOfferingCount { get; set; }

        public DbSet<GetAllIssues> GetAllIssues { get; set; }
    }
}

