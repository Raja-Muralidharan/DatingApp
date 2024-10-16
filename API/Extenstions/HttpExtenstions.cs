using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Extenstions
{
    public static class HttpExtenstions
    {
        public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
        {
            var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));

            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        }
    }
}