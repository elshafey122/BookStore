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
        public ICompanyRepository Companies { get; }
        public IShoppingCartRepository ShoppingCarts { get; }
        public IApplicationUserRepository ApplicationUsers { get; }
        public IOrderHeader OrderHeaders { get; }
		public IOrderDetails OrderDetails { get; }


		int Complete();
    }
}
