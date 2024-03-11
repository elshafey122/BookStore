using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repositories.IRepositories
{
    public interface IGenericRepository<T> where T:class 
    {
        IEnumerable<T> GetAll();
        T Get(Expression<Func<T,bool>> expression);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);



    }
}
