using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Matriarchy.Models
{
    public class Service
    {
        public int ID { get; set; }

        [Display(Name = "Service Provider")]
        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        public decimal Price { get; set; }

    }

    public class ServiceMovie
    {
        public int ID { get; set; }
        public int MovieCD { get; set; }
        public int ServiceCD { get; set; }

        [Display(Name = "Movie Title")]
        public string Description { get; set; }
        public int? Rating { get; set; }
    }

     
    public class ServiceNetwork
    {
        public int ID { get; set; }
        public int NetworkCD { get; set; }
        public int ServiceCD { get; set; }
        public string Description { get; set; }
        public int? Rating { get; set; }
    }

     public class ServiceSerie
    {
        public int ID { get; set; }
        public int SerieCD { get; set; }
        public int ServiceCD { get; set; }
        public string Description { get; set; }
        public string Seasons { get; set; }
        public int? Rating { get; set; }
    }

    public class ServiceSeriesRightJoinServices
    {
        public int ID { get; set; }
        public int? SerieCD { get; set; }
        public string Description { get; set; }
        public int? SerieServiceID { get; set; }
        public string Seasons { get; set; }
    }


    public class ServiceSeriesRightJoinSeries
    {
        public int ID { get; set; }

        public int? ServiceCD { get; set; }

        public string Description { get; set; }

        public int? SerieServiceID { get; set; }
        
        public string Seasons { get; set; }

    }

    public class ServiceNetworksRightJoinServices
    {
        public int ID { get; set; }

        public int? NetworkCD { get; set; }

        public string Description { get; set; }

        public int? NetworkServiceID { get; set; }

    }

    public class ServiceNetworksRightJoinNetworks
    {
        public int ID { get; set; }

        public int? ServiceCD { get; set; }

        public string Description { get; set; }

        public int? NetworkServiceID { get; set; }

    }

    public class ServiceMoviesRightJoinMovies
    {
        public int ID { get; set; }

        public int? ServiceCD { get; set; }

        [Display(Name = "Movie Title")]
        public string Description { get; set; }

        public int? MovieServiceID { get; set; }
    }

    public class ServiceMoviesRightJoinServices
    {
        public int ID { get; set; }

        public int? MovieCD { get; set; }

        [Display(Name = "Movie Title")]
        public string Description { get; set; }

        public int? MovieServiceID { get; set; }

    }
    public class Movie
    {
        public int ID { get; set; }
        [Display(Name = "Movie Title")]
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
    }

    public class Network
    {
        public int ID { get; set; }
        [Display(Name = "Network")]
        [Required]
        [StringLength(500)]

        public string Description { get; set; }
    }

    public class Serie
    {
        public int ID { get; set; }

        [Display(Name = "Serie Name")]
        [Required]
        [StringLength(500)]

        public string Description { get; set; }

    }

    public class MovieRating
    {
        public int ID { get; set; }
        [Display(Name = "Movie Title")]
        public string Description { get; set; }
        public int? Rating { get; set; }
    }

    public class NetworkRating
    {
        public int ID { get; set; }
        [Display(Name = "Network")]
        public string Description { get; set; }
        public int? Rating { get; set; }
    }

    public class SerieRating 
    {
        public int ID { get; set; }

        [Display(Name = "Serie Name")]
        public string Description { get; set; }

        public int? Rating { get; set; }

    }

    public class SerieServiceSeason
    {
        public int ID { get; set; }
        public int SerieCD { get; set; }
        public int ServiceCD { get; set; }
        [Display(Name = "Seasons")]
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
    }


    public class SeasonEdit : SerieServiceSeason
    {
        public int pageNumber { get; set; }
        public string Title { get; set; }

        public string SeriesName { get; set; }
    }

    public class MovieEdit : Movie
    {
        public int ServiceCD { get; set; }
        public int pageNumber { get; set; }
        public string Title { get; set; }
    }

    public class NetworkEdit: Network
    {
        public int ServiceCD { get; set; }
        public int pageNumber { get; set; }
        public string Title { get; set; }
    }
    public class SerieEdit

    {
        public int ID { get; set; }

        [Display(Name = "Serie Name")]
        public string Description { get; set; }
        public int ServiceCD { get; set; }
        public int pageNumber { get; set; }
        public string Title { get; set; }

    }

    public class ServiceEdit : Service
    {
        public int pageNumber { get; set; }
    }

    public class CompareServices
    {
        public int ID { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }

        public int? MovieSum { get; set; }
        public int? MovieCount { get; set; }
        public int? NetworkSum { get; set; }
        public int? NetworkCount { get; set; }
        public int? SerieCount { get; set; }
        public int? SerieSum { get; set; }
        public int? ServiceCount { get; set; }
        public int? ServiceSum { get; set; }

        public float? UserTotals { get; set; }

        public float? ServiceScore { get; set; }

    }

    public class FunctionCall
    {
        public int ServiceCD { get; set; }
        public int PageNumber { get; set; }
        public int MovieCD { get; set; }
        public int Rating { get; set; }
        public string Clear { get; set; }
        public string Title { get; set; }
    }


}
