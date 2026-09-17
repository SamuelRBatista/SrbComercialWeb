using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Mvc.Controllers;
using Application.Services;
using Domain.Interfaces;
using Domain.Entities;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;


public class ProductsControllerTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepo.Object);
        _controller = new ProductsController(_service);
    }

    [Fact]
    public async Task GetAllProducts_Should_Return_Ok_With_ProductList()
    {
        //Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Mouse", BarCode = "1234567891234", Price = 100.00m },
            new Product { Id = 2, Name = "Keyboard", BarCode = "9876543210987", Price = 200.0m }
        };

        _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);
        //Act
        var result = await _controller.GetProducts();  

        //Assert        
           var okResult = Assert.IsType<OkObjectResult>(result.Result);
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(products);
    }

    // [Fact]
    // public async Task GetProductById_Should_Return_Ok_With_Product()
    // {
    //     // Arrange
    //     var product = new Product { Id = 1, Name = "Mouse", BarCode = "1234567890123", Price = 100.0m };

    //     _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

    //     // Act
    //     var result = await _controller.GetById(1);

    //     // Assert
    //     var okResult = result as OkObjectResult;
    //     okResult.Should().NotBeNull();
    //     okResult!.StatusCode.Should().Be(200);
    //     okResult.Value.Should().BeEquivalentTo(product);
    // }

    // [Fact]
    // public async Task GetProductById_Should_Return_NotFound_If_Product_Does_Not_Exist()
    // {
    //     // Arrange
    //     _mockRepo.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Product)null);

    //     // Act
    //     var result = await _controller.GetById(99);

    //     // Assert
    //     var notFoundResult = result as NotFoundResult;
    //     notFoundResult.Should().NotBeNull();
    //     notFoundResult!.StatusCode.Should().Be(404);
    // }

    // [Fact]
    // public async Task AddProduct_Should_Return_CreatedAtAction()
    // {
    //     // Arrange
    //     var newProduct = new Product { Id = 1, Name = "Mouse", BarCode = "1234567890123", Price = 100.0m };

    //     _mockRepo.Setup(repo => repo.IsCodeBarUniqueAsync(newProduct.BarCode)).ReturnsAsync(true);
    //     _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Product>())).ReturnsAsync(newProduct);

    //     // Act
    //     var result = await _controller.AddProductAsync(newProduct);

    //     // Assert
    //     var createdResult =  Assert.IsType<OkObjectResult>(result.Result);
    //     createdResult.Should().NotBeNull();
    //     createdResult!.StatusCode.Should().Be(201);
    //     createdResult.ActionName.Should().Be(nameof(_controller.GetProducts));
    //     createdResult.RouteValues["id"].Should().Be(newProduct.Id);
    // }

    // [Fact]
    // public async Task DeleteProduct_Should_Return_NoContent()
    // {
    //     // Arrange
    //     _mockRepo.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

    //     // Act
    //     var result = await _controller.Delete(1);

    //     // Assert
    //     var noContentResult = result as NoContentResult;
    //     noContentResult.Should().NotBeNull();
    //     noContentResult!.StatusCode.Should().Be(204);
    // }
}

