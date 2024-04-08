using Ecommerce.Model.Models;
using Ecommerce.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repositories.IRepositories
{
	public interface IOrderHeader:IGenericRepository<OrderHeader>
	{
		void Update(OrderHeader orderHeader);
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
		void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
	}
}
