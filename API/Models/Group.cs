using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Group
    {
         public Group()
        {
        }
        public Group(string name)
        {
            Name = name;
        }

        [Key]
        public required string Name { get; set; }

        public ICollection<Connections> Connections { get; set; } = new List<Connections>();
    }
}