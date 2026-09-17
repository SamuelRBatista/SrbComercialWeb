using Domain.Entities;
using Xunit;


public class CategoryTests
{

    [Fact]
    public void CreateCategory_WithValidData_ShouldSetPropertiesCorrectly()
    {

        //Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Nome Categoria Teste"
        };
        
        //Assets
        Assert.Equal(1, category.Id);
        Assert.Equal("Nome Categoria Teste", category.Name);




    }


}