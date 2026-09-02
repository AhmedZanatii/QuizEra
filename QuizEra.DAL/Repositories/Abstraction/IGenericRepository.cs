using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QuizEra.DAL.Repositories.Abstraction
{
    public interface IGenericRepository<TEntity>
    {
        public Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>>? filter = null,
              List<Expression<Func<TEntity, object>>>? includeProperties = null, bool noTrack = false,
              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
              int? skip = null,
              int? take = null);
        public Task<TEntity> GetBy(Expression<Func<TEntity, bool>> filter,
            List<Expression<Func<TEntity, object>>>? includeProperties = null, bool noTrack = false);
        public Task Create(TEntity entity);
        public Task AddRangeAsync(IEnumerable<TEntity> entities);
        public void Update(TEntity entity);
        public void Delete(TEntity entity);
        void HardDelete(TEntity entity);
        public Task SaveAsync();



    }
}
