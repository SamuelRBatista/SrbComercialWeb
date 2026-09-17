using Domain.Entities;
using Xunit;

public class ProductTests
{
  [Fact]
  public void CreateProduct_WithValidData_ShouldSetProprietiesCorrectly()
  {
    //Arrange
    var product = new Product
    {
        Id = 1,
        Name = "Notebook",
        Description = "Produto Test",
        Price = 5000m,
        Sku = "E35",
        BarCode = "1256454",
        ImageUrl ="test.png",
        CategoryId = 2       

    };

    //Assert
    Assert.Equal(1, product.Id);
    Assert.Equal("Notebook", product.Name);
    Assert.Equal("Produto Test", product.Description);
    Assert.Equal("E35", product.Sku);
    Assert.Equal("1256454", product.BarCode);
    Assert.Equal("test.png", product.ImageUrl);
    Assert.Equal(2, product.CategoryId);
  }





}
