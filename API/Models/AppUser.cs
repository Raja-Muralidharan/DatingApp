using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class AppUser
    {

        public int ID { get; set; }

        [Required]
        public required string UserName { get; set; }
        
    }
}