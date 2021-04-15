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

}