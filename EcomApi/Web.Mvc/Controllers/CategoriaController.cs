using Domain.Entities; 
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    // [Authorize] 
    public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpPost]
    // [Authorize] 
    public async Task<ActionResult<Category>> AddAsync(Category category)
    {
        if(category == null){
            return BadRequest("Categoria invalida. ");
        }

      await _categoryService.AddAsync(category);
      return Ok("Categoria criada com sucesso.");
    }
}
