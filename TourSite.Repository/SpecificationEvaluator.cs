using Microsoft.EntityFrameworkCore;
using TourSite.Core.Specification;

namespace TourSite.Repository
{
    public static class SpecificationEvaluator<Entity> where Entity : class
    {
        public static IQueryable<Entity> GetQuery(IQueryable<Entity> inputQuery, ISpecifications<Entity> spec)
        {
            var query = inputQuery;

            if (spec.AsNoTracking)
                query = query.AsNoTracking();

            if (spec.Criteria is not null)
                query = query.Where(spec.Criteria);

            if (spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);

            if (spec.OrderByDesc is not null)
                query = query.OrderByDescending(spec.OrderByDesc);

            if (spec.IsPagEnable)
                query = query.Skip(spec.Skip).Take(spec.Take);

            query = spec.Includes.Aggregate(query, (currentInclude, nextInclude) => currentInclude.Include(nextInclude));

            if (spec.IncludeStrings?.Count > 0)
            {
                foreach (var path in spec.IncludeStrings)
                    query = query.Include(path);
            }

            return query;
        }
    }
}
