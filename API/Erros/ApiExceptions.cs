using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Erros
{
    public class ApiExceptions(int statuscode, string message, string? details)
    {
        public int StatusCode { get; set; } = statuscode;

        public string Message { get; set; } = message;

        public string? Details { get; set; } = details;
    }
}