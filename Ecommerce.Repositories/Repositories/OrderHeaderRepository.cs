using Ecommerce.Model.Models;
using Ecommerce.Repositories.Data;
using Ecommerce.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repositories.Repositories
{
	public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeader
	{
		private readonly ApplicationDbContext _context;
		public OrderHeaderRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(OrderHeader orderHeader)
		{
			_context.OrderHeaders.Update(orderHeader);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderfromdb = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if (orderfromdb != null)
			{
				orderfromdb.OrderStatus=orderStatus;
				if(paymentStatus != null)
				{
					orderfromdb.PaymentStatus=paymentStatus;
				}
			}
		}

		public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
			var orderfromdb = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
			orderfromdb.SessionId = sessionId;
			orderfromdb.PaymentIntentId=paymentIntentId;
		}
	}
}
