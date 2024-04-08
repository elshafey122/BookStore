using Ecommerce.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repositories.IRepositories
{
    public interface IShoppingCartRepository:IGenericRepository<ShoppingCart>
    {
        public void Update(ShoppingCart shoppingCart);
    }
}
