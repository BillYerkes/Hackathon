using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Matriarchy.Models
{

    public class ReportServiceRecommendations
    {
        public int ID { get; set; }

        public string Description { get; set; }

        public int Recommendations { get; set; }

    }

    public class ReportUserFavorites
    {
        public int ID { get; set; }

        public string Description { get; set; }
        public double AvgRating { get; set; }

        public int Count { get; set; }

    }

    public class ReportFailedLoginAttempts
    {
        [Key]
        public string UserID { get; set; }
        public string IPAddress { get; set; }

        public int Count { get; set; }

    }


    public class ReportServiceOfferingCount
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int? MovieCount { get; set; }
        public int? NetworkCount { get; set; }
        public int? SerieCount { get; set; }

    }




}
