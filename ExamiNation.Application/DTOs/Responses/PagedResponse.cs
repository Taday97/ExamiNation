using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.Responses
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public string? SortBy { get; set; }     // Campo por el que ordenar
        public bool SortDescending { get; set; }
        public Dictionary<string, string> Filters { get;  set; }
    }
}
