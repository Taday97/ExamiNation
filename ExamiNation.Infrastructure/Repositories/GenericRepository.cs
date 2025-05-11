using ExamiNation.Domain.Common;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using ExamiNation.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExamiNation.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (asNoTracking)
                query = query.AsNoTracking();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? options = null)
        {
            IQueryable<T> query = _dbSet;

            if (options?.AsNoTracking ?? true)
                query = query.AsNoTracking();

            if (options?.Includes != null)
            {
                foreach (var include in options.Includes)
                {
                    query = query.Include(include);
                }
            }

            if (options?.Filter != null)
                query = query.Where(options.Filter);

            if (options?.OrderBy != null)
                query = options.OrderBy(query);

            return await query.ToListAsync();
        }
        public async Task<(IEnumerable<T>, int)> GetPagedWithCountAsync(PagedQueryOptions<T> options)
        {
            IQueryable<T> query = _dbSet;

            if (options.AsNoTracking)
                query = query.AsNoTracking();

            if (options?.Includes != null)
            {
                foreach (var include in options.Includes)
                {
                    query = query.Include(include);
                }
            }

            if (options?.Filters != null && options.Filters.Any())
            {
                query = query.ApplyFilters(options.Filters); 
            }

            if (!string.IsNullOrEmpty(options?.SortBy))
            {
                query = query.ApplyOrdering(options.SortBy, options.SortDescending);
            }

            var totalCount = await query.CountAsync();

           
            query = query
                .Skip(((options.PageNumber ?? 1) - 1) * (options.PageSize ?? int.MaxValue)) 
                .Take(options.PageSize ?? int.MaxValue); 

            return (await query.ToListAsync(), totalCount);
        }





        public async Task<T?> FindFirstAsync(Expression<Func<T, bool>> filter, bool asNoTracking = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (asNoTracking)
                query = query.AsNoTracking();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T?> UpdateAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Entity must have an 'Id' property.");

            var id = (Guid?)idProperty.GetValue(entity);
            if (id == null || id == Guid.Empty)
                throw new InvalidOperationException("Entity Id is missing or invalid.");

            var existing = await _dbSet.FindAsync(id.Value);

            if (existing == null)
                return null;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<T?> DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return null;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }

}
