using Microsoft.EntityFrameworkCore;

namespace CRUD.BLL.Filters
{
    public abstract class FilterBase<T>
    {
        public int Skip { get; set; }

        public int? Take { get; set; }

        protected IQueryable<T> Query { get; set; }

        public abstract IQueryable<T> CreateQuery(IQueryable<T> query);

        public async Task<int> CountAsync(IQueryable<T> query)
        {
            query = CreateQuery(query);
            return await query.CountAsync();
        }

        public int Count(IQueryable<T> query)
        {
            query = CreateQuery(query);
            return query.Count();
        }

        public virtual IQueryable<T> FilterObjects(IQueryable<T> query)
        {
            if (Query == null)
                Query = query;
            if (Take.HasValue)
                query = query.Skip((Skip - 1) * Take.Value).Take(Take.Value);

            return query;
        }
    }
}
