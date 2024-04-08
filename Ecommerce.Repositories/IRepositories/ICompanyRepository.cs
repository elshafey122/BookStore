using Ecommerce.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repositories.IRepositories
{
    public interface ICompanyRepository:IGenericRepository<Company>
    {
        void Update(Company entity);
    }
}
