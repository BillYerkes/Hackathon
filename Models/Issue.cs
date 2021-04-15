using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Matriarchy.Models
{
    public class Issue
    {
        
        public int ID { get; set; }
        public int UserCD { get; set; }

        [DataType(DataType.Date)]
        public DateTime IssueDateTime { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
    }

    public class GetAllIssues 
    {

        public int ID { get; set; }
        public int UserCD { get; set; }

        [DataType(DataType.Date)]
        public DateTime IssueDateTime { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }

        public string Email { get; set; }
    }

}
