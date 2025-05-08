using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Domain.Common
{
    public class PagedQueryOptions<T> : QueryOptions<T>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int Skip => (PageNumber - 1) * PageSize;

        public Dictionary<string, string> Filters { get; set; } = new();
        public string? SortBy { get; set; }    
        public bool SortDescending { get; set; } 
    }
}
