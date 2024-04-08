using Ecommerce.Model.Models;
using Ecommerce.Repositories.Data;
using Ecommerce.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repositories.Repositories
{
    public class CompanyRepository : GenericRepository<Company>,ICompanyRepository
    {
        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
            _context=context;
        }

        public void Update(Company entity)
        {
            _dbSet.Update(entity);
        }
    }
}
