using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TourSite.Core.Specification
{
    public class BaseSpecifications<TEntity> : ISpecifications<TEntity> where TEntity : class
    {
        public Expression<Func<TEntity, bool>> Criteria { get; set; } = null!;
        public List<Expression<Func<TEntity, object>>> Includes { get; set; } = new();
        public List<string> IncludeStrings { get; set; } = new();
        public Expression<Func<TEntity, object>> OrderBy { get; set; } = null!;
        public Expression<Func<TEntity, object>> OrderByDesc { get; set; } = null!;
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPagEnable { get; set; }
        public bool AsNoTracking { get; set; } = true;

        public BaseSpecifications(Expression<Func<TEntity, bool>> expression)
        {
            Criteria = expression;
        }

        public BaseSpecifications()
        {
        }

        public void AddInclude(Expression<Func<TEntity, object>> include)
            => Includes.Add(include);

        public void AddInclude(string include)
            => IncludeStrings.Add(include);

        public void AddOrderBy(Expression<Func<TEntity, object>> expression)
        {
            OrderBy = expression;
        }

        public void OrderByDescend(Expression<Func<TEntity, object>> expression)
        {
            OrderByDesc = expression;
        }

        public void ApplyPag(int pageSize, int pageIndex)
        {
            IsPagEnable = true;
            Take = pageSize;
            Skip = pageSize * (pageIndex - 1);
        }
    }
}
