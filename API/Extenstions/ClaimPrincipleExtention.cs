using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Claims;

namespace API.Extenstions
{
    public static class ClaimPrincipleExtention
    {

      public static string GetUserName(this ClaimsPrincipal user)
      {
              var username = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Cannot get username from token");

              return username;
      }
    }
}