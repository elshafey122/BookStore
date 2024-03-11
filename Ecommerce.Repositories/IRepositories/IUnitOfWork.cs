using Ecommerce.Repositories.Data;
using Ecommerce.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repositories.IRepositories
{
    public interface IUnitOfWork:IDisposable
    {
        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }

        int Complete();
    }
}
