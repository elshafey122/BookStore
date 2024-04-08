using Ecommerce.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model.ViewModels
{
    public class ShoppingCartViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
    }
}
