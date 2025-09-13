using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentManagement.PaginationUtility
{
    public interface IPaginationService<T>
    {
        Task<PaginationResult<T>> PaginateAsync(
            IQueryable<T> query,
            PaginationRequest request,
            CancellationToken cancellationToken = default);
    }
}
