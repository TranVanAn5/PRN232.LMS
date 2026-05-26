using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using System.Reflection;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly LmsDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(LmsDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            string search = null,
            string sort = null,
            int page = 1,
            int pageSize = 10,
            string fields = null,
            string expand = null)
        {
            var (data, _) = await GetAllWithCountAsync(search, sort, page, pageSize, fields, expand);
            return data;
        }

        public async Task<(IEnumerable<T> data, int totalCount)> GetAllWithCountAsync(
            string search = null,
            string sort = null,
            int page = 1,
            int pageSize = 10,
            string fields = null,
            string expand = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            // Apply expansion (Include related entities)
            query = ApplyExpand(query, expand);

            // Apply filtering (Search)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = ApplySearch(query, search);
            }

            // Get total count before paging
            int totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = ApplySort(query, sort);
            }
            else
            {
                query = query.OrderBy(x => x);
            }

            // Apply paging
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            // Apply field selection
            if (!string.IsNullOrWhiteSpace(fields))
            {
                var data = await query.ToListAsync();
                return (ApplyFieldSelection(data, fields), totalCount);
            }

            var result = await query.ToListAsync();
            return (result, totalCount);
        }

        public async Task<T> GetByIdAsync(int id, string expand = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            // Apply expansion
            query = ApplyExpand(query, expand);

            var idProperty = typeof(T).GetProperty("Id", BindingFlags.IgnoreCase | BindingFlags.Public);
            if (idProperty == null)
            {
                idProperty = typeof(T).GetProperties().FirstOrDefault(p =>
                    p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) && p.PropertyType == typeof(int));
            }

            if (idProperty == null)
                return null;

            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
            var property = System.Linq.Expressions.Expression.Property(parameter, idProperty.Name);
            var constant = System.Linq.Expressions.Expression.Constant(id);
            var equal = System.Linq.Expressions.Expression.Equal(property, constant);
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(equal, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return true;
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // Helper Methods

        private IQueryable<T> ApplyExpand(IQueryable<T> query, string expand)
        {
            if (string.IsNullOrWhiteSpace(expand))
                return query;

            var expandProperties = expand.Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            foreach (var property in expandProperties)
            {
                // tìm property không phân biệt hoa thường
                var navigationProperty = typeof(T)
                    .GetProperty(
                        property,
                        BindingFlags.IgnoreCase |
                        BindingFlags.Public |
                        BindingFlags.Instance);

                if (navigationProperty != null)
                {
                    query = query.Include(navigationProperty.Name);
                }
            }

            return query;
        }

        private IQueryable<T> ApplySearch(IQueryable<T> query, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            var searchLower = search.ToLower();
            var stringProperties = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanRead)
                .ToList();

            if (!stringProperties.Any())
                return query;

            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
            System.Linq.Expressions.Expression expression = null;

            foreach (var property in stringProperties)
            {
                var propertyExpr = System.Linq.Expressions.Expression.Property(parameter, property.Name);
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var searchExpr = System.Linq.Expressions.Expression.Constant(searchLower);
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var toLowerExpr = System.Linq.Expressions.Expression.Call(propertyExpr, toLowerMethod);
                var callExpr = System.Linq.Expressions.Expression.Call(toLowerExpr, method, searchExpr);

                expression = expression == null
                    ? callExpr
                    : System.Linq.Expressions.Expression.Or(expression, callExpr);
            }

            if (expression != null)
            {
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(expression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        private IQueryable<T> ApplySort(IQueryable<T> query, string sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
                return query;

            var sortFields = sort.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            bool isFirstSort = true;

            foreach (var sortField in sortFields)
            {
                var isDescending = sortField.StartsWith("-");
                var fieldName = isDescending ? sortField.Substring(1) : sortField;

                var property = typeof(T).GetProperty(fieldName,
                    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public);

                if (property != null)
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
                    var propertyExpr = System.Linq.Expressions.Expression.Property(parameter, property.Name);
                    var lambda = System.Linq.Expressions.Expression.Lambda(propertyExpr, parameter);

                    var methodName = isFirstSort
                        ? (isDescending ? "OrderByDescending" : "OrderBy")
                        : (isDescending ? "ThenByDescending" : "ThenBy");

                    var method = typeof(System.Linq.Queryable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(T), property.PropertyType);

                    query = (IQueryable<T>)method.Invoke(null, new object[] { query, lambda });
                    isFirstSort = false;
                }
            }

            return query;
        }

        private IEnumerable<T> ApplyFieldSelection(IEnumerable<T> data, string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
                return data;

            var fieldList = fields.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToLower())
                .ToList();

            return data; // Full implementation would filter properties, keeping simple version
        }
    }
}
