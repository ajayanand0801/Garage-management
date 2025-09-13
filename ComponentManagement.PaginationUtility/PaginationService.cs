using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentManagement.PaginationUtility
{
    public class PaginationService<T> : IPaginationService<T>
    {
        public async Task<PaginationResult<T>> PaginateAsync(
    IQueryable<T> query,
    PaginationRequest request,
    CancellationToken cancellationToken = default)
        {
            // Filtering
            var predicate = ExpressionBuilder.BuildPredicate<T>(request.Filters ?? new List<FilterField>());
            query = query.Where(predicate);

            // Sorting
            query = query.ApplySorting(request.Sorts ?? new List<SortField>());

            // Total count before skip/take
            var total = await Task.Run(() => query.Count(), cancellationToken);

            // Pagination
            var items = await Task.Run(() => query
                .Skip(request.Skip)
                .Take(request.Take)
                .ToList(), cancellationToken);

            return new PaginationResult<T>
            {
                Items = items,
                TotalCount = total
            };
        }

    }

}
