using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.RequestParams
{
    public class QueryParameters
    {
        public Dictionary<string, string>? Filters { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
