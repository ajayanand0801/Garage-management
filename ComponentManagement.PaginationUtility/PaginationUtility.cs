using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentManagement.PaginationUtility
{
    public class FilterField
    {
        public string Field { get; set; } = string.Empty;
        public string Operation { get; set; } = "eq";  // eq, ne, gt, lt, gte, lte, contains, in, notin, range
        public object? Value { get; set; }
        public string LogicalOperator { get; set; } = "and";  // "and" or "or"
    }

    public class SortField
    {
        public string Field { get; set; } = string.Empty;
        public string Direction { get; set; } = "asc";  // "asc" or "desc"
    }

    public class PaginationRequest
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
        public List<FilterField> Filters { get; set; } = new();
        public List<SortField> Sorts { get; set; } = new();
    }

    public class PaginationResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
