using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Domain.Common
{
    public class PagedQueryOptions<T> : QueryOptions<T>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public int Skip => ((PageNumber ?? 1) - 1) * (PageSize ?? int.MaxValue);  

        public Dictionary<string, string>? Filters { get; set; }

        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }

}
