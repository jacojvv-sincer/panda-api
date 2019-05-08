using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using Panda.API.Data.Models;
using Panda.API.Mappings;
using System;

namespace Panda.API.Tests
{
    public class BaseTest
    {
        public static Guid _userId = Guid.NewGuid();
        public Mock<IHttpContextAccessor> _httpContextAccessor;

        public virtual void Setup()
        {
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.Items["ApplicationUser"] = new User()
            {
                Id = _userId
            };
            _httpContextAccessor.Setup(a => a.HttpContext).Returns(context);

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
        }
    }
}