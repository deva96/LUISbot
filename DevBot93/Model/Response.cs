using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisBot.Model
{
    public class Response
    {
        public int OrderId { get; set; }
        public String OrderItem { get; set; }
        public String ShippingAddress { get; set; }
        public String Status { get; set; }
    }
}