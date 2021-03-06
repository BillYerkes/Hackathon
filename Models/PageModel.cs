using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Matriarchy.Models
{
    public class PageModel
    {

        public class PaginationModel : PageModel

        {

            [BindProperty(SupportsGet = true)]

            public int CurrentPage { get; set; } = 1;

            public int Count { get; set; }

            public int PageSize { get; set; } = 10;

            public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        }

    }
}
