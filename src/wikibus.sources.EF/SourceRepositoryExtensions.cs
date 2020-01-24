using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Wikibus.Sources.EF
{
    public static class SourceRepositoryExtensions
    {
        public static async Task<TCollection> GetCollectionPage<T, TEntity, TCollection>(
            this IQueryable<TEntity> dbSet,
            Uri identifier,
            Expression<Func<TEntity, object>> ordering,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> applyFilters,
            int page,
            int pageSize,
            Func<EntityWrapper<TEntity>, T> resourceFactory)
            where TEntity : class, IHasImage
            where TCollection : SearchableCollection<T>, new()
        {
            var entireCollection = applyFilters(dbSet.AsNoTracking()).OrderBy(ordering);
            var pageOfBrochures = entireCollection.Skip((page - 1) * pageSize).Take(pageSize);
            var entityWrappers = pageOfBrochures.Select(entity => new EntityWrapper<TEntity>
            {
                Entity = entity,
                HasLegacyImage = entity.Image.Bytes != null
            });
            var books = await entityWrappers.ToListAsync();

            return new TCollection
            {
                Id = identifier,
                Members = books.ToList().Select(resourceFactory).ToArray(),
                TotalItems = await entireCollection.CountAsync()
            };
        }

        public static async Task<TCollection> GetCollectionPage<T, TEntity, TCollection>(
            this IQueryable<TEntity> dbSet,
            Uri identifier,
            Expression<Func<TEntity, object>> ordering,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> applyFilters,
            int page,
            int pageSize,
            Func<TEntity, T> resourceFactory)
            where TEntity : class
            where TCollection : SearchableCollection<T>, new()
        {
            var entireCollection = applyFilters(dbSet.AsNoTracking()).OrderBy(ordering);
            var pageOfBrochures = entireCollection.Skip((page - 1) * pageSize).Take(pageSize);
            var books = await pageOfBrochures.ToListAsync();

            return new TCollection
            {
                Id = identifier,
                Members = books.ToList().Select(resourceFactory).ToArray(),
                TotalItems = await entireCollection.CountAsync()
            };
        }
    }
}
