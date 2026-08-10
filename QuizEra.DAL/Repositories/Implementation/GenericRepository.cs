using Microsoft.EntityFrameworkCore;
using QuizEra.DAL.DataBase;
using QuizEra.DAL.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QuizEra.DAL.Repositories.Implementation
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, new()
    {
        QuizEraDBContext _db;
        IQueryable<TEntity> _dbSet;

        public GenericRepository(QuizEraDBContext db)
        {
            _db = db;
            _dbSet = _db.Set<TEntity>();
        }


        public async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>>? filter = null,
            List<Expression<Func<TEntity, object>>>? includeProperties = null, bool noTrack = false,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
              int? skip = null,
              int? take = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            if (noTrack)
            {
                query = query.AsNoTracking();
            }

            if (orderBy != null)
                query = orderBy(query);


            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);


            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetBy(Expression<Func<TEntity, bool>> filter,
            List<Expression<Func<TEntity, object>>>? includeProperties = null, bool noTrack = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task Create(TEntity entity)
        {
            await _db.Set<TEntity>().AddAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            //_db.Set<TEntity>().Remove(entity);
            //soft delete
            _db.Set<TEntity>().Update(entity);
        }

        public void Update(TEntity entity)
        {
            _db.Set<TEntity>().Update(entity);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
