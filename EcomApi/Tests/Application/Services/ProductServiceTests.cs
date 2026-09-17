using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using Xunit;
using Moq;
using FluentAssertions;


public class ProductServiceTests
{

    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepo.Object);
    }
    [Fact]
    public async Task GetAllProducts_Should_Return_Product_List()
    {
        //Arrange
        var products = new List<Product> { 
            new Product
             {
                Id = 1,
                Name = "Mouse"
            }
        };

       _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);
        //Act
        var result = await _service.GetAllAsync();   

        //Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.ToList()[0].Name.Should().Be("Mouse");
    }
    [Fact]
    public async Task AddProductAsync_Should_Call_AddAsync_Method()
    {

        //Arrange
        var newProduct = new Product
        {
            Id = 2,
            Name = "ProductTest",
            Description = "Descrição Produto",
            BarCode = "1234567890123",
            Price = 12.99m
        };

        //Mockclear
        _mockRepo.Setup(repo => repo.IsCodeBarUniqueAsync(newProduct.BarCode)).ReturnsAsync(true);
        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Product>()));

        //Act
        await _service.AddAsync(newProduct);
        
        //Assert
        _mockRepo.Verify(repo => repo.AddAsync(It.Is<Product>(p => p == newProduct)), Times.Once);
    }   
    [Fact]
    public async Task GetProductByIdAsync_Should_Return_Product_By_Id()
    {
        //Arranje
        var productById = new Product
        {
            Id = 1
        };

        _mockRepo.Setup(repo => repo.GetByIdAsync(productById.Id)).ReturnsAsync(productById);

        //Act
        var result = await _service.GetByIdAsync(productById.Id);   

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productById.Id);
    }
}

