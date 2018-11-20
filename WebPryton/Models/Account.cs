using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPryton.Models
{
    public class Account
    {
        public string UserId { get; set; }
        public string Login { get; set; }
        public int Password { get; set; }
        public string Status { get; set; }
    }
}
