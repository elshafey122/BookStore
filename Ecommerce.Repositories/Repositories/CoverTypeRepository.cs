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
    public class CoverTypeRepository : GenericRepository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(CoverType obj)
        {
            _context.coverTypes.Update(obj);
        }
    }
}
