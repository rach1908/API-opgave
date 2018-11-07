using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime UserSince { get; set; }
    }
}
