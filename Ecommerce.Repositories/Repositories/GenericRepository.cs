using Ecommerce.Repositories.Data;
using Ecommerce.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repositories.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public DbSet<T> _dbSet;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? IncludeProperites = null)
        {
           
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(IncludeProperites))
            {
                foreach (var includeitem in IncludeProperites.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeitem);
                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? IncluseProperites = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(IncluseProperites))
            {
                foreach (var includeitem in IncluseProperites.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeitem);
                }
            }
            return query.ToList();
        }

        public IEnumerable<T> GetAll( string? IncludeProperites = null)
        {
            IQueryable<T> query = _dbSet;
            if (!string.IsNullOrEmpty(IncludeProperites))
            {
                foreach (var includeitem in IncludeProperites.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeitem);
                }
            }
            return query.ToList();
        }

       

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            _dbSet.RemoveRange(entity);
        }
    }
}
