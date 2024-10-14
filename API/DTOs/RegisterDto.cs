using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public  string UserName { get; set; } = string.Empty;

        [Required]
        public  string? KnownAs { get; set; }

        [Required]
        public  string? Gender { get; set; }

        [Required]
        public  string? DateofBirth { get; set; }

        [Required]
        public  string? City { get; set; }

        [Required]
        public  string? Country { get; set; }

        [Required]
        public string password { get; set; } = string.Empty;
    }
}