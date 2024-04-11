using Ecommerce.Repositories.Data;
using Ecommerce.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repositories.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }
        public ICompanyRepository Companies { get; private set; }
        public IShoppingCartRepository ShoppingCarts { get; private set; }
        public IApplicationUserRepository ApplicationUsers { get; private set; }

		public IOrderHeader OrderHeaders { get; private set; }

		public IOrderDetails OrderDetails { get; private set; }

        public ICoverTypeRepository CoverType { get; set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context=context;
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            Companies=new CompanyRepository(_context);
            ShoppingCarts = new ShoppingCartRepository(_context);
            ApplicationUsers = new ApplicationUserRepository(_context);
            OrderHeaders = new OrderHeaderRepository(_context);
            OrderDetails = new OrderDetailsRepository(_context);
            CoverType = new CoverTypeRepository(_context);
        }
        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
