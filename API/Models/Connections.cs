using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Connections
    {
        [Key]
        public required string ConnectionId { get; set; }

        public required string UserName { get; set; }


    }
}