using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Panda.API.Data;
using Panda.API.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Panda.API.Middleware
{
    public class SetApplicationUserMiddleware
    {
        private readonly RequestDelegate _next;

        public SetApplicationUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            if (context.User.Claims.Count() > 0)
            {
                Guid userId = new Guid(context.User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").First().Value);
                var email = context.User.Claims.Where(c => c.Type == "emails").First().Value;
                var name = context.User.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").First().Value;
                var surname = context.User.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname").First().Value;

                User user = await dbContext.Users.Where(u => u.Id == new Guid(userId.ToString())).FirstOrDefaultAsync();
                if (user == null)
                {
                    user = new User();
                    user.Id = userId;
                    user.Name = name;
                    user.Surname = surname;
                    user.Email = email;
                    dbContext.Users.Add(user);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    user.Name = name;
                    user.Surname = surname;
                    user.Email = email;

                    if (dbContext.Entry(user).State == EntityState.Modified)
                    {
                        await dbContext.SaveChangesAsync();
                    }
                }

                context.Items["ApplicationUser"] = user;
            }

            await _next(context);
        }
    }

    public static class SetApplicationUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseSetApplicationUser(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SetApplicationUserMiddleware>();
        }
    }
}