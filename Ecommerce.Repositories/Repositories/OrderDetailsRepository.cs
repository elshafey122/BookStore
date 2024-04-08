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
	public class OrderDetailsRepository : GenericRepository<OrderDetails>, IOrderDetails
	{
		private readonly ApplicationDbContext _context;
		public OrderDetailsRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(OrderDetails orderDetails)
		{
			_context.OrderDetails.Update(orderDetails);
		}
	}
}
