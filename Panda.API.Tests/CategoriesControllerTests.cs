using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Panda.API.Controllers;
using Panda.API.Data.Models;
using Panda.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panda.API.Tests
{
    [TestClass]
    public class CategoriesControllerTests : BaseTest
    {
        private Mock<ICategoryService> _categoryService;
        private Mock<IRelationAnalyticsService> _relationAnalyticsService;
        private CategoriesController _controller;

        private static int _categoryId = 1;
        private static string _categoryName = "CategoryName";
        private static DateTime _categoryTimeStamp = DateTime.Now;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _categoryService = new Mock<ICategoryService>();
            _categoryService.Setup(s => s.GetCategoriesForUserTransactions(_userId))
                            .ReturnsAsync(new List<Category>() {
                                new Category() {
                                    Id = _categoryId,
                                    Name = _categoryName,
                                    CreatedAt = _categoryTimeStamp,
                                    UpdatedAt = _categoryTimeStamp
                                }
                            });
            _relationAnalyticsService = new Mock<IRelationAnalyticsService>();

            _controller = new CategoriesController(_httpContextAccessor.Object, _categoryService.Object, _relationAnalyticsService.Object);
        }

        [TestMethod]
        public async Task Get_Returns_Categories()
        {
            var result = (await _controller.Get()).Value;

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(_categoryId, result.First().Id);
            Assert.AreEqual(_categoryName, result.First().Name);
        }
    }
}